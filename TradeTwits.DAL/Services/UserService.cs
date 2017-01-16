using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using TradeTwits.Data.Models;

namespace TradeTwits.Data.Services
{
    public class UserService : IUserService
    {
        private readonly IMongoCollection<ApplicationUser> Users;
        private readonly IMongoCollection<TwitModel> Twits;

        public UserService(IMongoCollection<ApplicationUser> users, IMongoCollection<TwitModel> twits)
        {
            this.Users = users;
            this.Twits = twits;
        }

        public ApplicationUser GetUser(string userName)
        {
            return Users.AsQueryable().Single(u => u.UserName == userName);
        }

        public IEnumerable<ApplicationUser> GetByNameMatches(string query)
        {
            var builder = Builders<ApplicationUser>.Filter;
            var filter = builder.Regex(u => u.Name, new BsonRegularExpression(new Regex(query, RegexOptions.IgnoreCase)))
                | builder.Regex(u => u.FullName, new BsonRegularExpression(new Regex(query, RegexOptions.IgnoreCase)))
                | builder.Regex(u => u.UserName, new BsonRegularExpression(new Regex(query, RegexOptions.IgnoreCase)));

            return Users.Find(filter).ToList();
        }

        public IEnumerable<ApplicationUser> GetNextUsers(string lastName, Expression<Func<ApplicationUser, bool>> whereCriteria)
        {
            var builder = Builders<ApplicationUser>.Filter;
            var filter = builder.Where(whereCriteria) & builder.Gt(u => u.UserName, lastName);
            return Users.Find(filter).SortBy(u => u.Name).Limit(10).ToList();
        }

        public ApplicationUser GetUserByTwitId(string id)
        {
            var twit = Twits.AsQueryable().Where(t => t.Id == id).First();
            return Users.AsQueryable().Where(u => u.UserName == twit.UserName).First();
        }

        public ApplicationUser GetUserByCommentId(string id)
        {
            var twit = Twits.AsQueryable().Where(t => t.Comments.Any(c => c.Id == id)).Single();
            var comment = twit.Comments.Single(c => c.Id == id);
            return Users.AsQueryable().Where(u => u.UserName == comment.UserName).First();
        }

        public void Save(ApplicationUser user)
        {
            Users.ReplaceOne(u => u.Id == user.Id, user, new UpdateOptions() { IsUpsert = true });
        }

        public void Follow(string followedUserId, string currentUser)
        {
                var query = Builders<ApplicationUser>.Filter.Eq(u => u.UserName, currentUser);
                var update = Builders<ApplicationUser>.Update.Push(u => u.FollowedIds, followedUserId);
                var result = Users.UpdateOne(query, update);
        }

        public void Unfollow(string followedUserId, string currentUser)
        {
            var query = Builders<ApplicationUser>.Filter.Eq(u => u.UserName, currentUser);
            var update = Builders<ApplicationUser>.Update.Pull(u => u.FollowedIds, followedUserId);
            var result = Users.UpdateOne(query, update);
        }

        public void Block(string blockedUserId, string currentUser)
        {
            var query = Builders<ApplicationUser>.Filter.Eq(u => u.UserName, currentUser);
            var update = Builders<ApplicationUser>.Update.Push(u => u.BlockedIds, blockedUserId);
            var result = Users.UpdateOne(query, update);
        }

        public void Unblock(string blockedUserId, string currentUser)
        {
            var query = Builders<ApplicationUser>.Filter.Eq(u => u.UserName, currentUser);
            var update = Builders<ApplicationUser>.Update.Pull(u => u.BlockedIds, blockedUserId);
            var result = Users.UpdateOne(query, update);
        }
    }
}
