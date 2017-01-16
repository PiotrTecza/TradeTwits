using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using TradeTwits.Data;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using TradeTwits.Data.Models;
using System.Linq;
using TradeTwits.Data.Services;

namespace UploadImageWebJob
{
    public class Functions
    {
        private readonly IQueryable<TwitModel> twits;
        private readonly ITwitsService twitsService;

        public Functions(IQueryable<TwitModel> twits, ITwitsService twitsService)
        {
            this.twits = twits;
            this.twitsService = twitsService;
        }

        public void GenerateImage(
        [QueueTrigger("imagerequest")] BlobInformation blobInfo,
        [Blob("images/{BlobName}", FileAccess.Read)] Stream input,
        [Blob("images/{BlobNameWithoutExtension}_medium.jpg")] CloudBlockBlob outputBlobMedium,
        [Blob("images/{BlobNameWithoutExtension}_big.jpg")] CloudBlockBlob outputBlobBig)
        {
            using (Stream output = outputBlobMedium.OpenWrite())
            {
                ConvertImageToJPG(input, output, 580);
                outputBlobMedium.Properties.ContentType = "image/jpeg";
            }

            using (Stream output = outputBlobBig.OpenWrite())
            {
                ConvertImageToJPG(input, output, 650);
                outputBlobBig.Properties.ContentType = "image/jpeg";
            }


            var id = blobInfo.TwitId;
            TwitModel twit = twits.Where(x => x.Id == id).FirstOrDefault();
            if (twit == null)
            {
                throw new Exception(String.Format("TwitId {0} not found, can't create images", id.ToString()));
            }
            twit.MediumImageURL = outputBlobMedium.Uri.ToString();
            twit.BigImageURL = outputBlobBig.Uri.ToString();

            twitsService.Replace(twit);

        }

        public static void ConvertImageToJPG(Stream input, Stream output, int maxWidth)
        {
            //562x315 - st size - ours 580
            int maxHeight = int.MaxValue;
            var originalImage = new Bitmap(input);
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

                newImage.Save(output, ImageFormat.Jpeg);
            }
            finally
            {
                if (newImage != null)
                {
                    newImage.Dispose();
                }
            }
        }

        public static void ConvertImageToBigJPG(Stream input, Stream output)
        {
            //620 max width, no max height
            int thumbnailsize = 80;
            int width;
            int height;
            var originalImage = new Bitmap(input);

            if (originalImage.Width > originalImage.Height)
            {
                width = thumbnailsize;
                height = thumbnailsize * originalImage.Height / originalImage.Width;
            }
            else
            {
                height = thumbnailsize;
                width = thumbnailsize * originalImage.Width / originalImage.Height;
            }

            Bitmap thumbnailImage = null;
            try
            {
                thumbnailImage = new Bitmap(width, height);

                using (Graphics graphics = Graphics.FromImage(thumbnailImage))
                {
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    graphics.DrawImage(originalImage, 0, 0, width, height);
                }

                thumbnailImage.Save(output, ImageFormat.Jpeg);
            }
            finally
            {
                if (thumbnailImage != null)
                {
                    thumbnailImage.Dispose();
                }
            }
        }
    }
}
