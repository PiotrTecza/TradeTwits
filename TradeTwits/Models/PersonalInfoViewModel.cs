using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TradeTwits.Models
{
    public class PersonalInfoViewModel
    {
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Display(Name = "Bio")]
        public string Description { get; set; }
    }
}