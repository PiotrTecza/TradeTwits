using TradeTwits.Data.Services;
using TradeTwits.Models;

namespace TradeTwits.Service
{
    public class UserFacade : IUserFacade
    {
        private readonly IUserService userService;

        public UserFacade(IUserService userService)
        {
            this.userService = userService;
        }

        public void UpdateRanking(Vote vote)
        {
            var user = string.IsNullOrEmpty(vote.CommentId) ? 
                userService.GetUserByTwitId(vote.TwitId) : 
                userService.GetUserByCommentId(vote.CommentId);

            if (vote.Up)
            {
                user.ReputationPoints++;
            }
            else
            {
                user.ReputationPoints--;
            }

            userService.Save(user);
        }
    }
}