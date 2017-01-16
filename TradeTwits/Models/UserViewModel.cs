using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TradeTwits.Models
{
    public class UserViewModel
    {
        public string UserName { get; set; }
        public string EmailAddress { get; set; }
        public bool IsFollowed { get; set; }
        public int FollowedUsers { get; set; }
        public int FollowedStocks { get; set; }
        public int Followers { get; set; }
        public int TwitsCount { get; set; }
        public bool IsBlocked { get; set; }
        public int ReputationPoits { get; set; }

        public UserViewModel(string userName, string email, bool isFollowed, bool isBlocked, int followedUsers, int followedStocks, int followers, int twitCount, int reputationPoints)
        {
            UserName = userName;
            EmailAddress = email;
            IsFollowed = isFollowed;
            FollowedUsers = followedUsers;
            FollowedStocks = followedStocks;
            Followers = followers;
            TwitsCount = twitCount;
            IsBlocked = isBlocked;
            ReputationPoits = reputationPoints;
        }
    }
}