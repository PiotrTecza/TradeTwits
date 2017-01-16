using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TradeTwits.Models
{
    public class StockViewModel
    {
        public string Category { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public bool IsFollowed { get; set; }
        public int Followers { get; set; }
        public int TwitsCount { get; set; }
        public double? Price { get; set; }
        public double? Change { get; set; }
        public double? ChangeAmount { get; set; }

        public StockViewModel(string category, string name, string userName, string fullName, bool isFollowed, int followers, int twitsCount)
        {
            Category = category;
            Name = name;
            UserName = userName;
            FullName = fullName;
            IsFollowed = isFollowed;
            Followers = followers;
            TwitsCount = twitsCount;
        }
        public StockViewModel(string category, string name, string userName, string fullName, bool isFollowed, int followers, int twitsCount, double? price, double? change, double? changeAmount)
        {
            Category = category;
            Name = name;
            UserName = userName;
            FullName = fullName;
            IsFollowed = isFollowed;
            Followers = followers;
            TwitsCount = twitsCount;
            Price = price;
            Change = change;
            ChangeAmount = changeAmount;
        }
    }
}