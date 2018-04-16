using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravelApplication.Models
{
    public class UserRole
    {
        public int UserId { get; set; }

        public string UserName { get; set; }

        public int BadgeNumber { get; set; }

        public IEnumerable<Role> Roles { get; set; }
    }
}