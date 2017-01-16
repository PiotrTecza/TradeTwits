using System.Collections.Generic;
using TradeTwits.Data.Models;

namespace TradeTwits.Data.Services
{
    public interface IStockDataService
    {
        void InsertMany(List<StockDataModel> stocks);
        void RemoveOld(string currentGuid);
        StockDataModel GetStockData(string stockName);
    }
}