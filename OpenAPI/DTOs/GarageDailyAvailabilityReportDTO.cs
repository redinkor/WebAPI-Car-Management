using System;
namespace OpenAPI.DTOs
{
	public class GarageDailyAvailabilityReportDTO
	{
        public DateTime date { get; set; }  
        public int requests { get; set; }
        public int availableCapacity { get; set; }
    }
}

