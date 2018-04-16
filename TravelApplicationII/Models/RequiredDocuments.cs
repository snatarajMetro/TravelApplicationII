using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravelApplication.Models
{
    /// <summary>
    ///  Properties to display Required documents in the attachment section 
    /// </summary>
    public class RequiredDocuments
    {
        public string TravelRequestId { get; set; }
        public int DocumentNumber { get; set; }
        public string DocumentName { get; set; }
        public string FileName { get; set; }
        public bool Visible { get; set; }
    }

}