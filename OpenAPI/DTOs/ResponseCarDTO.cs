using System;
namespace OpenAPI.DTOs
{
	public class ResponseCarDTO
	{
        public long id { get; set; }
        public string make { get; set; }
        public string model { get; set; }
        public int productionYear { get; set; }
        public string licensePlate { get; set; }
        public List<ResponseGarageDTO> garages { get; set; }
    }
}

