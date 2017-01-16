using MongoDB.Driver;
using System.Collections.Generic;
using TradeTwits.Data.Models;
using System.Linq;

namespace TradeTwits.Data.Services
{
    public class StockDataService : IStockDataService
    {
        private readonly IMongoCollection<StockDataModel> StocksData;

        public StockDataService(IMongoCollection<StockDataModel> stockData)
        {
            this.StocksData = stockData;
        }

        public void InsertMany(List<StockDataModel> stocks)
        {
            StocksData.InsertMany(stocks);
        }

        public void RemoveOld(string currentGuid)
        {
            var filter = Builders<StockDataModel>.Filter.Ne(s => s.Guid, currentGuid);
            StocksData.DeleteMany(filter);
        }

        public StockDataModel GetStockData(string stockName)
        {
            return StocksData.AsQueryable().SingleOrDefault(u => u.Symbol == stockName);
        }
    }
}
