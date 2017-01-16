using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using TradeTwits.Data.Models;

namespace TradeTwits.Data.Services
{
    public class TwitsService : ITwitsService
    {
        private readonly IMongoCollection<TwitModel> twits;

        public TwitsService(IMongoCollection<TwitModel> twits)
        {
            this.twits = twits;
        }

        public void Replace(TwitModel twit)
        {
            twits.ReplaceOne(t => t.Id == twit.Id, twit);
        }

        public void Insert(TwitModel twit)
        {
            twits.InsertOne(twit);
        }

        public void VoteForTwit(string twitId, bool vote, string userName)
        {
            var filter = Builders<TwitModel>.Filter.Eq(t => t.Id, twitId);
            UpdateDefinition<TwitModel> update = null;

            if (vote)
            {
                update = Builders<TwitModel>.Update.AddToSet(t => t.Likes, userName);
            }
            else
            {
                update = Builders<TwitModel>.Update.Pull(t => t.Likes, userName);
            }

            twits.UpdateOne(filter, update);
        }

        public void VoteForComment(string twitId, string commentId, bool vote, string userName)
        {
            var twitFilterBuilder = Builders<TwitModel>.Filter;
            var commentFilterBuilder = Builders<CommentModel>.Filter;
            var filter = twitFilterBuilder.Eq(t => t.Id, twitId)
                & twitFilterBuilder.ElemMatch
                (
                    t => t.Comments,
                        commentFilterBuilder.Eq(c => c.Id, commentId) 
                        & commentFilterBuilder.Ne(c => c.UserName, userName)
                );

            UpdateDefinition<TwitModel> update = null;

            if (vote)
            {
                update = Builders<TwitModel>.Update.AddToSet("Comments.$.Likes", userName);
            }
            else
            {
                update = Builders<TwitModel>.Update.Pull("Comments.$.Likes", userName);
            }

            twits.UpdateOne(filter, update);
        }

        public void AddComment(string twitId, CommentModel comment, List<string> tags)
        {
            var filter = Builders<TwitModel>.Filter.Eq(t => t.Id, twitId);
            var update = Builders<TwitModel>.Update.Push(t => t.Comments, comment).AddToSetEach(t => t.Tags, tags);
            twits.UpdateOne(filter, update);
        }
    }
}
