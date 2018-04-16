using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravelApplication.Models
{
    public class EmployeeDetails
    {
        public int BadgeNumber { get; set; }
        public string Position { get; set; }
        public string Department { get; set; }
        public string CostCenter { get; set; }
        public string EmployeeFirstName { get; set; }
        public string EmployeeLastName { get; set; }
        public string EmployeeEmailAddress { get; set; }
        public string EmployeeCellPhone { get; set; }
        public string EmployeeHomePhone { get; set; }
        public string EmployeeWorkPhone { get; set; }
        public string EmployeeType { get; set; }
    }
}