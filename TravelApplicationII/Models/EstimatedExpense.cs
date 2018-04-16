using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravelApplication.Models
{
    public class EstimatedExpense
    {
        public int EstimatedExpenseId { get; set; }
        public int TravelRequestId { get; set; }
        public decimal AdvanceLodging { get; set; }
        public decimal AdvanceAirFare { get; set; }
        public decimal AdvanceRegistration { get; set; }
        public decimal AdvanceMeals { get; set; }
        public decimal AdvanceCarRental { get; set; }
        public decimal AdvanceMiscellaneous { get; set; }
        public decimal AdvanceTotal { get; set; }
        public decimal TotalEstimatedLodge { get; set; }
        public decimal TotalEstimatedAirFare { get; set; }
        public decimal TotalEstimatedRegistration { get; set; }
        public decimal TotalEstimatedMeals { get; set; }
        public decimal TotalEstimatedCarRental { get; set; }
        public decimal TotalEstimatedMiscellaneous { get; set; }
        public decimal TotalEstimatedTotal { get; set; }
        public string HotelNameAndAddress { get; set; }
        public string Schedule { get; set; }
        public string PayableToAndAddress { get; set; }
        public string Note { get; set; }
        public string AgencyNameAndReservation { get; set; }
        public string Shuttle { get; set; }
        public decimal CashAdvance { get; set; }
        public DateTime DateNeededBy { get; set; }
        public decimal PersonalTravelExpense { get; set; }

        public decimal TotalOtherEstimatedLodge { get; set; }
        public decimal TotalOtherEstimatedAirFare { get; set; }
        public decimal TotalOtherEstimatedMeals { get; set; }
        public decimal TotalOtherEstimatedTotal { get; set; }

        public decimal TotalActualEstimatedLodge { get; set; }
        public decimal TotalActualEstimatedAirFare { get; set; }
        public decimal TotalActualEstimatedMeals { get; set; }
        public decimal TotalActualEstimatedTotal { get; set; }

        public bool PersonalTravelIncluded { get; set; }

        public string SpecialInstruction { get; set; }

        public bool CashAdvanceRequired { get; set; }
    }
}