using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravelApplication.Models
{
    public class UserResponse
    {
        public bool result { get; set; }
        public int userId { get; set; }
        public string userName { get; set; }
        public int BadgeNumber { get; set; }
    }
}