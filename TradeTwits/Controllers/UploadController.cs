using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using TradeTwits.Data;
using TradeTwits.Data.Models;
using TradeTwits.Data.Services;
using TradeTwits.Hubs;
using TradeTwits.Utils;

namespace TradeTwits.Controllers
{
    [Authorize]
    public class UploadController : ApiControllerWithHub<TwitHub>
    {
        private readonly ITwitsService twitsService;
        private readonly IUserService userService;

        private CloudQueue imageRequestQueue;
        private static CloudBlobContainer imagesBlobContainer;

        public UploadController(
            ITwitsService twitsService, 
            IUserService userService)
        {
            this.twitsService = twitsService;
            this.userService = userService;
            InitializeStorage();
        }

        private void InitializeStorage()
        {
            // Open storage account using credentials from .cscfg file.
            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["AzureWebJobsStorage"].ToString());

            // Get context object for working with blobs, and 
            // set a default retry policy appropriate for a web user interface.
            var blobClient = storageAccount.CreateCloudBlobClient();
            //blobClient.DefaultRequestOptions.RetryPolicy = new LinearRetry(TimeSpan.FromSeconds(3), 3);

            // Get a reference to the blob container.
            imagesBlobContainer = blobClient.GetContainerReference("images");

            // Get context object for working with queues, and 
            // set a default retry policy appropriate for a web user interface.
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            //queueClient.DefaultRequestOptions.RetryPolicy = new LinearRetry(TimeSpan.FromSeconds(3), 3);

            // Get a reference to the queue.
            imageRequestQueue = queueClient.GetQueueReference("imagerequest");
        }

        [HttpPost]
        public async Task<IHttpActionResult> Send()
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent("form-data"))
            {
                return BadRequest("Unsupported media type");
            }

            try
            {
                if (HttpContext.Current.Request.Files.AllKeys.Any())
                {
                    HttpPostedFile httpPostedFile = HttpContext.Current.Request.Files["file"] as HttpPostedFile;
                    string message = HttpContext.Current.Request.Form["message"] as string;
                    var originalImage = new Bitmap(httpPostedFile.InputStream);
                    ApplicationUser user = userService.GetUser(User.Identity.Name);
                    string gravatarHash = GravatarGenerator.GenerateHash(user.EmailAddress);
                    List<string> tags = ParseTagsFromMessage(message, user.UserName);
                    string parsedMessage = UrlShortener.ShortenUrl(message);
                    TwitModel twit = new TwitModel(message, gravatarHash, user.UserName, tags);
                    CloudBlockBlob imageBlob = null;
                    CloudBlockBlob outputBlobMedium = null;
                    CloudBlockBlob outputBlobBig = null;

                    if (httpPostedFile != null && httpPostedFile.ContentLength != 0)
                    {
                        imageBlob = await UploadAndSaveBlobAsync(httpPostedFile);
                        twit.ImageURL = imageBlob.Uri.ToString();

                        outputBlobMedium = UploadAndSaveBlobConvertedImage(originalImage, httpPostedFile.FileName, 580, true);
                        twit.MediumImageURL = outputBlobMedium.Uri.ToString();

                        outputBlobBig = UploadAndSaveBlobConvertedImage(originalImage, httpPostedFile.FileName, 600, false);
                        twit.BigImageURL = outputBlobBig.Uri.ToString();
                    }
                    twit.CreatedAt = DateTime.Now;
                    twitsService.Insert(twit);
                    Trace.TraceInformation("Created twitID {0} in database", twit.Id);


                    // NOT using the WebJob at this pont due to big lag - changing the size of image on the fly
                    //if (imageBlob != null)
                    //{
                    //    BlobInformation blobInfo = new BlobInformation() { TwitId = twit.Id, BlobUri = new Uri(twit.ImageURL) };
                    //    var queueMessage = new CloudQueueMessage(JsonConvert.SerializeObject(blobInfo));
                    //    await imageRequestQueue.AddMessageAsync(queueMessage);
                    //    Trace.TraceInformation("Created queue message for twitID {0}", twit.Id);
                    //}

                    Hub.Clients.Groups(tags).newMessage(FormatTwit(twit));
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.GetBaseException().Message);
            }
        }

        private async Task<CloudBlockBlob> UploadAndSaveBlobAsync(HttpPostedFile imageFile, int? maxWidth = null)
        {
            Trace.TraceInformation("Uploading image file {0}", imageFile.FileName);

            string blobName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            // Retrieve reference to a blob. 
            CloudBlockBlob imageBlob = imagesBlobContainer.GetBlockBlobReference(blobName);
            // Create the blob by uploading a local file.
            using (Stream fileStream = imageFile.InputStream)
            {
                await imageBlob.UploadFromStreamAsync(fileStream);
            }

            Trace.TraceInformation("Uploaded image file to {0}", imageBlob.Uri.ToString());

            return imageBlob;
        }

        private CloudBlockBlob UploadAndSaveBlobConvertedImage(Bitmap imageFile, string fileName, int maxWidth, bool toBeCropped)
        {
            Trace.TraceInformation("Uploading image file {0}", fileName);
            string blobName;
            if (maxWidth > 580)
            {
                blobName = Guid.NewGuid().ToString() + "_big" + Path.GetExtension(fileName);
            }
            else
            {
                blobName = Guid.NewGuid().ToString() + "_medium" + Path.GetExtension(fileName);
            }
            // Retrieve reference to a blob. 
            CloudBlockBlob imageBlob = imagesBlobContainer.GetBlockBlobReference(blobName);
            // Create the blob by uploading a local file.
            using (var output = imageBlob.OpenWrite())
            {
                ConvertImageToJPG(imageFile, output, maxWidth, toBeCropped);
            }

            Trace.TraceInformation("Uploaded image file to {0}", imageBlob.Uri.ToString());

            return imageBlob;
        }

        private List<string> ParseTagsFromMessage(string message, string twitUserName)
        {
            List<string> tags = new List<string>() { twitUserName, User.Identity.Name };
            Regex userRegex = new Regex("(@|\\$)([a-zA-Z0-9_]+)");
            var mc = userRegex.Matches(message);

            foreach (Match match in mc)
            {
                tags.Add(match.Groups.Cast<Group>().Last().Value);
            }

            tags = tags.Distinct().ToList();
            return tags;
        }

        private object FormatTwit(TwitModel twit)
        {
            return new
            {
                Id = twit.Id,
                Message = twit.Message,
                CreatedAt = twit.CreatedAt,
                UserName = twit.UserName,
                GravatarHash = twit.GravatarHash,
                Comments = twit.Comments,
                LikesCount = 0,
                IsLiked = false,
                ImageURL = twit.ImageURL,
                MediumImageURL = twit.MediumImageURL,
                BigImageURL = twit.BigImageURL
            };
        }

        public static void ConvertImageToJPG(Bitmap originalImage, Stream output, int maxWidth, bool toBeCropped)
        {
            //562x315 - st size - ours 580
            int maxHeight = int.MaxValue;

            //var originalImage = new Bitmap(image);
            int originalWidth = originalImage.Width;
            int originalHeight = originalImage.Height;

            // To preserve the aspect ratio
            float ratioX = (float)maxWidth / (float)originalWidth;
            float ratioY = (float)maxHeight / (float)originalHeight;
            float ratio = Math.Min(ratioX, ratioY);

            // New width and height based on aspect ratio
            int newWidth = (int)(originalWidth * ratio);
            int newHeight = (int)(originalHeight * ratio);

            Bitmap newImage = null;
            Bitmap croppedImage = null;
            try
            {

                // Convert other formats (including CMYK) to RGB.
                newImage = new Bitmap(newWidth, newHeight, PixelFormat.Format24bppRgb);

                // Draws the image in the specified size with quality mode set to HighQuality
                using (Graphics graphics = Graphics.FromImage(newImage))
                {
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.DrawImage(originalImage, 0, 0, newWidth, newHeight);
                }
                if (toBeCropped)
                {
                    Rectangle cropRect = new Rectangle(0, 0, newWidth, 340);
                    croppedImage = new Bitmap(cropRect.Width, cropRect.Height);

                    using (Graphics g = Graphics.FromImage(croppedImage))
                    {
                        g.DrawImage(newImage, new Rectangle(0, 0, croppedImage.Width, croppedImage.Height),
                                        cropRect,
                                        GraphicsUnit.Pixel);
                    }

                    croppedImage.Save(output, ImageFormat.Jpeg);
                }
                else
                {
                    newImage.Save(output, ImageFormat.Jpeg);
                }
            }
            finally
            {
                if (newImage != null)
                {
                    newImage.Dispose();
                }
                if(croppedImage != null)
                {
                    croppedImage.Dispose();
                }
            }
        }
    }
}
