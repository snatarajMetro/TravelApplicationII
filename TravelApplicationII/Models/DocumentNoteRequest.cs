using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravelApplication.Models
{
    public class JustifactionRequest
    {
        public  DocumentNoteRequest documentNoteRequest { get; set; }
    }


    public class DocumentNoteRequest
    {
        public string TravelRequestId { get; set; }

        public string DocumentId { get; set; }

        public string Notes { get; set; }

        public string NotesOption { get; set; }
    }

}