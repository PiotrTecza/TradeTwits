using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TradeTwits.Models
{
    public class BlockedViewModel
    {
        public string UserName { get; set; }
        public string Gravatar { get; set; }

        public BlockedViewModel(string userName, string gravatar = "")
        {
            this.UserName = userName;
            this.Gravatar = gravatar;
        }
    }
}