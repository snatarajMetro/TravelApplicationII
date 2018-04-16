using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravelApplication.Models
{
    public class SharePointUpload
    {
        public string siteUrl { get; set; }
        public string documentListName { get; set; }
        public string documentListURL { get; set; }
        public string documentName { get; set; }
        public byte[] documentStream { get; set; }
        public string subFolder { get; set; }

        public string folder { get; set; }
    }
}