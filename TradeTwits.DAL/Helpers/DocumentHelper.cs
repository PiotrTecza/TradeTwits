using MongoDB.Bson;

namespace TradeTwits.Data.Helpers
{
    public class DocumentHelper : IDocumentHelper
    {
        public string NewDocumentId()
        {
            return ObjectId.GenerateNewId().ToString();
        }
    }
}
