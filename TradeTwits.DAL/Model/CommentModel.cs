using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradeTwits.Data.Models
{
    [BsonIgnoreExtraElements]
    public class CommentModel
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserName { get; set; }
        public string GravatarHash { get; set; }
        public List<string> Likes { get; set; }
        public string ImageURL { get; set; }
        public string MediumImageURL { get; set; }
        public string BigImageURL { get; set; }

        public CommentModel()
        {
            Likes = new List<string>();
        }

        public CommentModel(string message, string userName, string gravatarHash, string Id): this()
        {
            this.Id = Id;
            this.Message = message;
            this.UserName = userName;
            this.GravatarHash = gravatarHash;
            this.CreatedAt = DateTime.Now;
        }
    }
}
