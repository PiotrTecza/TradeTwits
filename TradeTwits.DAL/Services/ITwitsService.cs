using System.Collections.Generic;
using TradeTwits.Data.Models;

namespace TradeTwits.Data.Services
{
    public interface ITwitsService
    {
        void Replace(TwitModel user);
        void Insert(TwitModel twit);
        void VoteForTwit(string twitId, bool vote, string userName);
        void VoteForComment(string twitId, string commentId, bool vote, string userName);
        void AddComment(string twitId, CommentModel comment, List<string> tags);
    }
}
