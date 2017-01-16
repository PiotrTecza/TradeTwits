using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TradeTwits.Data.MongoSerializers;

namespace TradeTwits.Data.Models
{
    [BsonIgnoreExtraElements]
    public class StockDataModel
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Symbol { get; set; }
        public double? Maximum { get; set; }
        public double? Opening { get; set; }
        public double? Minimum { get; set; }
        public double? Price { get; set; }
        public double? Change { get; set; }
        public double? ChangeAmount { get; set; }
        public double? BidPrice { get; set; }
        public int? BidAmount { get; set; }
        public double? AskPrice { get; set; }
        public int? AskAmount { get; set; }
        public int? VolumeAmount { get; set; }
        public int? VolumeWorth { get; set; }
        public string Guid { get; set; }
        public double? PreviousDayPrice { get; set; }
    }
}