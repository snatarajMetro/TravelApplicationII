using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravelApplication.Models
{
    public class ReimbursementInput
    {
        public ReimbursementTravelRequestDetails ReimbursementTravelRequestDetails { get; set; }
        public ReimbursementDetails ReimbursementDetails { get; set; }
        public FIS FIS { get; set; }
    }
}