using System;
namespace OpenAPI.DTOs
{
	public class UpdateCarDTO
	{
        public string make { get; set; }
        public string model { get; set; }
        public int productionYear { get; set; }
        public string licensePlate { get; set; }
        public List<long> garageIds { get; set; }
    }
}

