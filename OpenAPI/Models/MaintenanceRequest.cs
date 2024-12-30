using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpenAPI.Models
{
	public class MaintenanceRequest
	{
        [Key]
        public long id { get; set; }

        [ForeignKey("Car")]
        public long carId { get; set; }
        public virtual Car car { get; set; }

        [ForeignKey("Garage")]
        public long garageId { get; set; }
        public virtual Garage garage { get; set; }

        [Required]
        public string serviceType { get; set; }

        [Required]
        public DateTime scheduledDate { get; set; }
    }
}

