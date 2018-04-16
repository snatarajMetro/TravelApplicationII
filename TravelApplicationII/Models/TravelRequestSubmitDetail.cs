using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravelApplication.Models
{
    public class TravelRequestSubmitDetail
    {
        public string TravelRequestId { get; set; }

        public int DepartmentHeadBadgeNumber { get; set; }

        public int DepartmentHeadOtherBadgeNumber { get; set; }

        public string DepartmentHeadOtherName { get; set; }

        public int ExecutiveOfficerBadgeNumber { get; set; }

        public int ExecutiveOfficerOtherBadgeNumber { get; set; }

        public string ExecutiveOfficerOtherName { get; set; }

        public int CEOInternationalBadgeNumber { get; set; }

        public int CEOInternationalOtherBadgeNumber { get; set; }

        public string CEOInternationalOtherName { get; set; }

        public int CEOAPTABadgeNumber { get; set; }

        public int CEOAPTAOtherBadgeNumber { get; set; }

        public string CEOAPTAOtherName { get; set; }

        public int TravelCoordinatorBadgeNumber { get; set; }

        public int TravelCoordinatorOtherBadgeNumber { get; set; }

        public string TravelCoordinatorOtherName { get; set; }

        public bool Agree { get; set; }

        public string SubmitterName { get; set; }

        public bool RejectedTravelRequest { get; set; }
        
    }

    public class TravelRequestSubmitDetailResponse
    {
        public TravelRequestSubmitDetail TravelRequestSubmitDetail { get; set; }
        public bool RequiredExecutiveOfficerApproval { get; set; }

        public bool CEOApprovalRequired { get; set; }
    }
}