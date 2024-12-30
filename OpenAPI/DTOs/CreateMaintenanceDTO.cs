using System;
namespace OpenAPI.DTOs
{
	public class CreateMaintenanceDTO
	{
        public long garageId { get; set; }
        public long carId { get; set; }
        public string serviceType { get; set; }
        public DateTime scheduledDate { get; set; }
    }
}

