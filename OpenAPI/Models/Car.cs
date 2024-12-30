using System;
using System.Text.Json.Serialization;

namespace OpenAPI.Models
{
	public class Car
	{
        public long id { get; set; }
        public string make { get; set; }
        public string model { get; set; }
        public int productionYear { get; set; }
        public string licensePlate { get; set; }

        public long GarageId { get; set; }
        [JsonIgnore]
        public Garage Garage { get; set; } 
    }
}

