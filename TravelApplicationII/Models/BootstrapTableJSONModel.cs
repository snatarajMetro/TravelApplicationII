using System.Collections.Generic;

namespace TravelApplication.Models
{
    /// <summary>
    /// BootstrapTableJSONModel class
    /// It is model class that returns to view through AJAX call.
    /// It contains the entire number of rows in the table and selected rows by offset and pagination limit
    /// </summary>
    /// <typeparam name="T">General entity class</typeparam>
    public class BootstrapTableJSONModel<T> where T : class
    {
        public int total { get; set; }
        public ICollection<T> rows { get; set; }
    }
}