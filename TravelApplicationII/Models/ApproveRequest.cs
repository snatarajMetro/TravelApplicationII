using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravelApplication.Models
{
    public class ApproveRequest
    {
        public int ApproverBadgeNumber { get; set; }
        public string TravelRequestId { get; set; }
        public string Comments { get; set; }
        public int SubmittedBadgeNumber { get; set; }
        public int TravelRequestBadgeNumber { get; set; }
        public string RejectReason { get; set; }
        public string DepartmentHeadBadgeNumber { get; set; }
        public string ExecutiveOfficerBadgeNumber { get; set; }
        public string CEOForInternationalBadgeNumber { get; set; }
        public string CEOForAPTABadgeNumber { get; set; }
        public string TravelCoordinatorBadgeNumber { get; set; }
        public string DepartmentHeadName { get; set; }
        public string ExecutiveOfficerName { get; set; }
        public string CEOForInternationalName { get; set; }
        public string CEOForAPTAName { get; set; }
        public string TravelCoordinatorName { get; set; }

}
}