using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravelApplication.Models
{
    public class TravelRequestDetails
    {
        public int TravelRequestId { get; set; }

        public string Description { get; set; }

        public string SubmittedByUser { get; set; }

        public string SubmittedDateTime { get; set; }

        public string RequiredApprovers { get; set; }

        public string LastApproveredByUser { get; set; }

        public string LastApprovedDateTime { get; set; }

        public bool ViewActionVisible { get; set; }

        public bool EditActionVisible { get; set; }

        public bool ApproveActionVisible { get; set; }

        public  string Status { get; set; }

        public string BadgeNumber { get; set; }

        public string Purpose { get; set; }

        public bool CancelActionVisible { get; set; }

        public bool ShowAlert { get; set; }


        public bool ShowApproverAlert { get; set; }

    }
}