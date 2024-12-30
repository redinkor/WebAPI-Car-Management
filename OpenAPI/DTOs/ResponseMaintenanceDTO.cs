using System;
namespace OpenAPI.DTOs
{
	public class ResponseMaintenanceDTO
	{
        public long id { get; set; }
        public long carId { get; set; }
        public string carName { get; set; } 
        public string serviceType { get; set; }
        public DateTime scheduledDate { get; set; }
        public long garageId { get; set; }
        public string garageName { get; set; }
    }
}

