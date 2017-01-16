using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using TradeTwits.Data.Models;

namespace TradeTwits.Data.Services
{
    public interface IUserService
    {
        IEnumerable<ApplicationUser> GetByNameMatches(string query);
        IEnumerable<ApplicationUser> GetNextUsers(string lastName, Expression<Func<ApplicationUser, bool>> whereCriteria);
        ApplicationUser GetUser(string userName);
        ApplicationUser GetUserByTwitId(string id);
        ApplicationUser GetUserByCommentId(string id);
        void Save(ApplicationUser user);
        void Follow(string followedUser, string currentUser);
        void Unfollow(string followedUser, string currentUser);
        void Block(string blockedUserId, string currentUser);
        void Unblock(string blockedUserId, string currentUser);
    }
}
