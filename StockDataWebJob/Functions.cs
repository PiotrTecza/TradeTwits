using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Azure.WebJobs;
using System.Net;
using System.Xml.Linq;
using System;
using TradeTwits.Data.Models;
using TradeTwits.Data.Services;

namespace StockDataWebJob
{
    public class Functions
    {
        private readonly IStockDataService stockDataService;

        public Functions(IStockDataService stockDataService)
        {
            this.stockDataService = stockDataService;
        }
        // This function will be triggered based on the schedule you have set for this WebJob
        // This function will enqueue a message on an Azure Queue called queue
        [NoAutomaticTrigger]
        public void ManualTrigger(TextWriter log, int value, [Queue("queue")] out string message)
        {
            string remoteUri = "http://bossa.pl/rawXML.jsp?layout=extDat&page=fullRating&zakladka=akcje";

            string xml;
            using (var webClient = new WebClient())
            {
                xml = webClient.DownloadString(remoteUri);
            }

            XDocument doc = XDocument.Parse(xml);
            IEnumerable<XElement> xelements = doc.Descendants("notowania").Where(x => x.Attribute("typ").Value.Replace(" ", "") == "Akcje").Elements();
            List<StockDataModel> stocks = new List<StockDataModel>();
            string guid = Guid.NewGuid().ToString();

            foreach (XElement xe in xelements)
            {
                StockDataModel stock = new StockDataModel();
                stock.Symbol = xe.Attribute("kod").Value;
                stock.Maximum = !string.IsNullOrEmpty(xe.Element("maksimum").Value) ? Convert.ToDouble(xe.Element("maksimum").Value) : (double?)null;
                stock.Opening = !string.IsNullOrEmpty(xe.Element("otwarcie").Value) ? Convert.ToDouble(xe.Element("otwarcie").Value) : (double?)null;
                stock.Minimum = !string.IsNullOrEmpty(xe.Element("minimum").Value) ? Convert.ToDouble(xe.Element("minimum").Value) : (double?)null;

                stock.Price = !string.IsNullOrEmpty(xe.TryGetElementValue("tkoTransKurs")) ? Convert.ToDouble(xe.TryGetElementValue("tkoTransKurs")) : (double?)null;

                stock.Change = !string.IsNullOrEmpty(xe.TryGetElementValue("zmiana")) ? Convert.ToDouble(xe.TryGetElementValue("zmiana").Replace("%", "")) : (double?)null;
                if (stock.Change.HasValue && stock.Price.HasValue)
                {
                    stock.PreviousDayPrice = stock.Price / (1 + (stock.Change / 100));
                }
                if (stock.PreviousDayPrice.HasValue)
                {
                    stock.PreviousDayPrice = Math.Round(stock.PreviousDayPrice.Value, 2);
                    double? changeAmount = stock.Price - stock.PreviousDayPrice;
                    stock.ChangeAmount = Math.Round(changeAmount.Value, 2);
                }


                decimal bidPrice;
                if (decimal.TryParse(xe.Element("kupno").Attribute("cena").Value, out bidPrice))
                {
                    stock.BidPrice = !string.IsNullOrEmpty(xe.Element("kupno").Attribute("cena").Value) ? Convert.ToDouble(xe.Element("kupno").Attribute("cena").Value) : (double?)null;
                }
                stock.BidAmount = !string.IsNullOrEmpty(xe.Element("kupno").Attribute("ilosc").Value) ? Convert.ToInt32(xe.Element("kupno").Attribute("ilosc").Value) : (int?)null;

                decimal askPrice;
                if (decimal.TryParse(xe.Element("sprzedaz").Attribute("cena").Value, out askPrice))
                {
                    stock.AskPrice = !string.IsNullOrEmpty(xe.Element("sprzedaz").Attribute("cena").Value) ? Convert.ToDouble(xe.Element("sprzedaz").Attribute("cena").Value) : (double?)null;
                }
                stock.AskAmount = !string.IsNullOrEmpty(xe.Element("sprzedaz").Attribute("ilosc").Value) ? Convert.ToInt32(xe.Element("sprzedaz").Attribute("ilosc").Value) : (int?)null;

                stock.VolumeAmount = !string.IsNullOrEmpty(xe.Element("wolumenObrotu").Value) ? Convert.ToInt32(xe.Element("wolumenObrotu").Value) : (int?)null;
                stock.VolumeWorth = !string.IsNullOrEmpty(xe.Element("wartoscObrotu").Value) ? Convert.ToInt32(xe.Element("wartoscObrotu").Value) : (int?)null;
                stock.Guid = guid;

                stocks.Add(stock);
            }

            stockDataService.InsertMany(stocks);
            stockDataService.RemoveOld(guid);

            log.WriteLine("Function is invoked with value={0}", value);
            message = value.ToString();
            log.WriteLine("Following message will be written on the Queue={0}", message);
        }
    }
}
