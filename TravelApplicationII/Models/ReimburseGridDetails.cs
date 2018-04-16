using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravelApplication.Models
{
    public class ReimburseGridDetails
    {
        public string TravelRequestId { get; set; }
        public int BadgeNumber { get; set; }
        public string Purpose { get; set; }
        public string SubmittedByUser {get;set;}
        public DateTime SubmittedDateTime { get; set; }
        public string RequiredApprovers { get; set; }
        public string LastApprovedByUser { get; set; }
        public string LastApprovedDateTime { get; set; }
        public bool EditActionVisible { get; set; }
        public bool ViewActionVisible { get; set; }
        public bool ApproveActionVisible { get; set; }
        public string Status { get; set; }
        public string StrSubmittedDateTime { get; set; }
        public int ReimbursementId { get; set; }

    }
}