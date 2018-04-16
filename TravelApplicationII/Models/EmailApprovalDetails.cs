using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravelApplication.Models
{
    public class EmailApprovalDetails : ApproveRequest
    {
        public string Comments { get; set; }

       // public string TravelRequestId { get; set; }

        public int BadgeNumber { get; set; }

    }
}