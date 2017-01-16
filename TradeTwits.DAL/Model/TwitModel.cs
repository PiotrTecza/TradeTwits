using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TradeTwits.Data.Models
{
    [BsonIgnoreExtraElements]
    public class TwitModel : CommentModel
    {
        public List<CommentModel> Comments { get; set; }
        public List<string> Tags { get; set; }

        public TwitModel() : base()
        {
            Comments = new List<CommentModel>();
            Tags = new List<string>();
        }

        public TwitModel(string message, string gravatarHash, string userName, List<string> tags) : this()
        {
            this.Message = message;
            this.GravatarHash = gravatarHash;
            this.UserName = userName;
            this.CreatedAt = DateTime.Now;
            this.Tags = tags;
        }

    }
}