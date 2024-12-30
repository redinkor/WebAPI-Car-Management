using System;
namespace OpenAPI.DTOs
{
	public class CreateGarageDTO
	{
        public string name { get; set; }
        public string location { get; set; }
        public string city { get; set; }
        public int capacity { get; set; }
    }
}

