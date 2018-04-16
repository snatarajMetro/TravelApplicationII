using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravelApplication.Models
{
    public class FIS
    {
        public List<FISDetails> FISDetails { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class FISDetails
    {
        public string TravelRequestId { get; set; }

        public string CostCenterId { get; set; }

        public string LineItem { get; set; }

        public string ProjectId { get; set; }

        public string Task { get; set; }

        public decimal Amount { get; set; }
    }
}