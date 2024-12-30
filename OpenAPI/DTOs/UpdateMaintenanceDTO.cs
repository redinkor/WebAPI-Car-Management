using System;
namespace OpenAPI.DTOs
{
	public class UpdateMaintenanceDTO
	{
        public long carId { get; set; }
        public string serviceType { get; set; }
        public DateTime scheduledDate { get; set; }
        public long garageId { get; set; }
    }
}

