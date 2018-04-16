 
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravelApplication.Models
{
    public class SupportingDocument
    {
        public int Id { get; set; }

        public int TravelRequestId { get; set; }

        public string FileName { get; set; }

        public string UploadDateTime { get; set; }

        public string DownloadUrl { get; set; }

        public string Notes { get; set; }
    }
}
 