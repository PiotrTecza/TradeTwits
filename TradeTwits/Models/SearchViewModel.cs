using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TradeTwits.Models
{
    public class SearchViewModel
    {
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string Gravatar { get; set; }
        public bool IsFirstStock { get; set; }

        public SearchViewModel(string userName, string fullName, string gravatar = "", bool isFirstStock = false)
        {
            this.UserName = userName;
            this.DisplayName = String.IsNullOrEmpty(fullName) ? userName : String.Format("{0} ({1})", userName, fullName);
            this.Gravatar = gravatar;
            this.IsFirstStock = IsFirstStock;
        }
    }
}