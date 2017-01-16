using AspNet.Identity.MongoDB;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace TradeTwits.Data.Models
{
    [BsonIgnoreExtraElements]
    public class ApplicationUser : IdentityUser
    {
        public string EmailAddress { get; set; }
        public List<string> FollowedIds { get; set; }
        public List<string> BlockedIds { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public bool IsUser { get; set; }
        public string GravatarHash { get; set; }
        public int ReputationPoints { get; set; }

        public ApplicationUser()
        {
            FollowedIds = new List<string>();
            BlockedIds = new List<string>();
        }
    }
}