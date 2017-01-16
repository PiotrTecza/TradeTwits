using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using TradeTwits.Models;
using System.Linq.Expressions;
using TradeTwits.Data.Models;
using TradeTwits.Service;
using TradeTwits.Data.Services;

namespace TradeTwits.Controllers
{
    [Authorize]
    public class TwitsController : ApiController
    {
        private readonly IUserFacade userFacade;
        private readonly ITwitsService twitsService;
        private readonly IUserService userService;
        private readonly IQueryable<TwitModel> twits;

        public TwitsController(
            IUserFacade userFacade, 
            ITwitsService twitsService, 
            IUserService userService,
            IQueryable<TwitModel> twits)
        {
            this.userFacade = userFacade;
            this.twitsService = twitsService;
            this.userService = userService;
            this.twits = twits;
        }

        public dynamic Get(DateTime? date = null)
        {
            ApplicationUser user = userService.GetUser(User.Identity.Name);
            List<string> followedIds = user.FollowedIds;
            followedIds.Add(User.Identity.Name);
            var twits = GetTwits(t => t.Tags.Any(x => followedIds.Contains(x)) && !user.BlockedIds.Contains(t.UserName));

            if (date != null)
            {
                twits = GetTwits(t => !user.BlockedIds.Contains(t.UserName) && t.Tags.Any(x => followedIds.Contains(x)) && t.CreatedAt < date.Value);
            }

            return FormatTwits(twits);
        }

        public dynamic GetForUser(string username, DateTime? date = null)
        {
            IQueryable<TwitModel> twits = GetTwits(t => t.Tags.Contains(username));
            int count = twits.Count();

            if (date != null)
            {
                twits = GetTwits(t => t.Tags.Contains(username) && t.CreatedAt < date.Value);
            }

            return new { data = FormatTwits(twits), count = count };
        }

        public void Vote(Vote vote)
        {
            if (String.IsNullOrEmpty(vote.CommentId))
                twitsService.VoteForTwit(vote.TwitId, vote.Up, User.Identity.Name);
            else
                twitsService.VoteForComment(vote.TwitId, vote.CommentId, vote.Up, User.Identity.Name);

            userFacade.UpdateRanking(vote);
        }

        private IQueryable<TwitModel> GetTwits(Expression<Func<TwitModel, bool>> whereCriteria)
        {
            return twits.Where(whereCriteria).OrderByDescending(t => t.CreatedAt);
        }

        private object FormatTwits(IQueryable<TwitModel> twits)
        {
            return twits.Take(10).ToList().Select(t => new
            {
                Id = t.Id,
                Message = t.Message,
                CreatedAt = t.CreatedAt,
                UserName = t.UserName,
                GravatarHash = t.GravatarHash,
                Comments = FormatComments(t.Comments),
                LikesCount = t.Likes.Count,
                IsLiked = t.Likes.Contains(User.Identity.Name),
                ImageURL = t.ImageURL,
                MediumImageURL = t.MediumImageURL,
                BigImageURL = t.BigImageURL
            });
        }

        private object FormatComments(IList<CommentModel> comments)
        {
            return comments.Select(c => new
            {
                Id = c.Id,
                Message = c.Message,
                CreatedAt = c.CreatedAt,
                UserName = c.UserName,
                GravatarHash = c.GravatarHash,
                LikesCount = c.Likes.Count,
                IsLiked = c.Likes.Contains(User.Identity.Name),
            });
        }
    }
}