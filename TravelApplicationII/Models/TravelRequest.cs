using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TravelApplication.Models
{
    public class TravelRequest
    {
        public int TravelRequestId { get; set; }

        public string TravelRequestIdNew { get; set; }
        public int UserId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter the badge number")]
        public int BadgeNumber { get; set; }
        public string Name { get; set; }
        public string Division { get; set; }
        public string Section { get; set; }
        public string Organization { get; set; }
        public string MeetingLocation { get; set; }
        public DateTime MeetingBeginDateTime { get; set; }
        public DateTime MeetingEndDateTime { get; set; }
        public DateTime DepartureDateTime { get; set; }
        public DateTime ReturnDateTime { get; set; }
        public DateTime CreationDateTime { get; set; }
        public DateTime LastUpdatedDateTime { get; set; }
        public int LastUpdatedByUserId { get; set; }
        public int SubmittedByUserId { get; set; }
        public DateTime SubmittedDateTime { get; set; }
        public string Agreed { get; set; }
        public string LoginId { get; set; }
        public int SelectedRoleId { get; set; }
        public int SubmittedByBadgeNumber { get; set; }
        public string Purpose { get; set; }
        public string StrMeetingBeginDateTime { get; set; }
        public string StrMeetingEndDateTime { get; set; }
        public string StrDepartureDateTime { get; set; }
        public string StrReturnDateTime { get; set; }
    }
}