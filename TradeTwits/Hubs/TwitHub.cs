using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using TradeTwits.Utils;
using System.Text.RegularExpressions;
using TradeTwits.Data.Models;
using TradeTwits.Data.Services;
using TradeTwits.Data.Helpers;

namespace TradeTwits.Hubs
{
    [Authorize]
    public class TwitHub : Hub
    {
        private readonly IUserService userService;
        private readonly ITwitsService twitsService;
        private readonly IQueryable<TwitModel> twits;
        private readonly IStockDataService stockDataService;
        private readonly IDocumentHelper documentHelper;

        public TwitHub(
            IUserService userService, 
            ITwitsService twitsService, 
            IQueryable<TwitModel> twits,
            IStockDataService stockDataService,
            IDocumentHelper documentHelper)
        {
            this.userService = userService;
            this.twitsService = twitsService;
            this.twits = twits;
            this.stockDataService = stockDataService;
            this.documentHelper = documentHelper;
        }

        public override Task OnConnected()
        {
            IEnumerable<string> followedIds = userService.GetUser(Context.User.Identity.Name).FollowedIds;
            foreach (string id in followedIds)
            {
                Groups.Add(Context.ConnectionId, id);
            }
            Groups.Add(Context.ConnectionId, Context.User.Identity.Name);
            return base.OnConnected();
        }

        public void Send(string message)
        {
            ApplicationUser user = userService.GetUser(Context.User.Identity.Name);
            string gravatarHash = GravatarGenerator.GenerateHash(user.EmailAddress);
            List<string> tags = ParseTagsFromMessage(message, user.UserName);
            string parsedMessage = UrlShortener.ShortenUrl(message);
            TwitModel twitModel = new TwitModel(parsedMessage, gravatarHash, user.UserName, tags);
            twitsService.Insert(twitModel);
            Clients.Groups(tags).newMessage(FormatTwit(twitModel));
        }

        public void Follow(string value)
        {
            if (!String.IsNullOrEmpty(value))
            {
                userService.Follow(value, Context.User.Identity.Name);
                Groups.Add(Context.ConnectionId, value);
            }
        }

        public void Unfollow(string value)
        {
            userService.Unfollow(value, Context.User.Identity.Name);
            Groups.Remove(Context.ConnectionId, value);
        }

        public void Block(string value)
        {
            if (!String.IsNullOrEmpty(value))
            {
                userService.Block(value, Context.User.Identity.Name);
            }
        }

        public void Unblock(string value)
        {
            userService.Unblock(value, Context.User.Identity.Name);
        }

        public void AddComment(string comment, string twitId, string twitUserName)
        {
            ApplicationUser user = userService.GetUser(Context.User.Identity.Name);
            string gravatarHash = GravatarGenerator.GenerateHash(user.EmailAddress);
            List<string> tags = ParseTagsFromMessage(comment, user.UserName);
            CommentModel model = new CommentModel(comment, user.UserName, gravatarHash, documentHelper.NewDocumentId());
            twitsService.AddComment(twitId, model, tags);
            tags = twits.Where(t => t.Id == twitId).Single().Tags;
            Clients.Groups(tags).newComment(model, twitId);
        }

        public StockDataModel GetStockData(string stockName)
        {
            StockDataModel stockData = stockDataService.GetStockData(stockName);
            return stockData;
        }

        private List<string> ParseTagsFromMessage(string message, string twitUserName)
        {
            List<string> tags = new List<string>() { twitUserName, Context.User.Identity.Name };
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

        private object FormatComment(CommentModel comment)
        {
            return new
            {
                Id = comment.Id,
                Message = comment.Message,
                CreatedAt = comment.CreatedAt,
                UserName = comment.UserName,
                GravatarHash = comment.GravatarHash,
                LikesCount = 0,
                IsLiked = false
            };
        }
    }
}