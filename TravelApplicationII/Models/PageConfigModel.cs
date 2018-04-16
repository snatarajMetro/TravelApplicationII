namespace TravelApplication.Models
{
    /// <summary>
    /// PageConfigModel class
    /// Contains properties for request to web services,
    /// </summary>
    public class PageConfigModel
    {
        public string Offset { get; set; }
        public string Limit { get; set; }
        public string Sort { get; set; }
        public string Order { get; set; }
        public string Search { get; set; }
        public string Filter { get; set; }
    }
}