using System.Text.Json.Serialization;

namespace OpenAPI.Models
{
    public class Garage
    {
        public long id { get; set; }
        public string name { get; set; }
        public string location { get; set; }
        public string city { get; set; }
        public int capacity { get; set; }
        [JsonIgnore]
        public List<Car> Cars { get; set; } 
    }
}