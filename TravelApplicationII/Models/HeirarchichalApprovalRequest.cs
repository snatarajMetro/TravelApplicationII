using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravelApplication.Models
{
    public class HeirarchichalApprovalRequest
    {
        public string TravelRequestId { get; set; }
        public int TravelRequestBadgeNumber { get; set; }
        public string TravelRequestName { get; set; }

        public int SignedInBadgeNumber { get; set;}
        public List<HeirarchichalOrder> ApproverList { get; set; }
        public bool AgreedToTermsAndConditions { get; set; }

        public string SubmittedByUserName { get; set; }

        public DateTime SubmittedDatetime { get; set; }
    }

    public class HeirarchichalOrder
    {
        public string ApproverPosition { get; set; }
        public string ApproverName { get; set; }
        public int ApproverBadgeNumber { get; set; }
        public int ApproverOtherBadgeNumber { get; set; }
        public int ApprovalOrder { get; set; }

    }
}