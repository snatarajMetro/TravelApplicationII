using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravelApplication.Models
{
    public class ReimbursementAllTravelInformation
    {
        public ReimbursementTravelRequestDetails TravelReimbursementDetails { get; set; }
        public FIS Fis { get; set; }
        public decimal CashAdvance { get; set; }
        public decimal PersonalTravelExpense { get; set; }
    }
}