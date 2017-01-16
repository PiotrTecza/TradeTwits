using MongoDB.Bson.Serialization;
using System;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Serializers;

namespace TradeTwits.Data.MongoSerializers
{
    [BsonSerializer(typeof(MongoDbMoneyFieldSerializer))]
    public class MongoDbMoneyFieldSerializer : SerializerBase<object>
    {
        public override object Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var dbData = context.Reader.ReadInt32();
            decimal? val = (decimal?)dbData / (decimal)100;
            if (val.HasValue)
                return decimal.Round(val.Value, 2, MidpointRounding.AwayFromZero);
            else
                return null;
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
        {
            var realValue = (decimal?)value;
            if (realValue.HasValue)
                context.Writer.WriteInt32(Convert.ToInt32(realValue * 100));
            else
                context.Writer.WriteNull();
        }
    }
}
