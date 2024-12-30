using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenAPI.Context;
using OpenAPI.DTOs;
using OpenAPI.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OpenAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarsController : ControllerBase
    {
        private readonly CarCatalogDbContext _context;

        public CarsController(CarCatalogDbContext context)
        {
            _context = context;
        }

        // GET: api/cars/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseCarDTO>> GetCar(long id)
        {
            var car = await _context.Cars.Include(c => c.Garage)
                .Select(c => new ResponseCarDTO
                {
                    id = c.id,
                    make = c.make,
                    model = c.model,
                    productionYear = c.productionYear,
                    licensePlate = c.licensePlate,
                    garages = c.Garage != null ? new List<ResponseGarageDTO>
                    {
                        new ResponseGarageDTO
                        {
                            id = c.Garage.id,
                            name = c.Garage.name,
                            location = c.Garage.location,
                            city = c.Garage.city,
                            capacity = c.Garage.capacity
                        }
                    } : null
                })
                .FirstOrDefaultAsync(c => c.id == id);

            if (car == null)
            {
                return NotFound();
            }

            return car;
        }

        // GET: api/cars
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResponseCarDTO>>> GetCars([FromQuery] string make, [FromQuery] long? garageId, [FromQuery] int? fromYear, [FromQuery] int? toYear)
        {
            var query = _context.Cars.AsQueryable();

            if (!string.IsNullOrEmpty(make))
            {
                query = query.Where(c => c.make == make);
            }
            if (garageId.HasValue)
            {
                query = query.Where(c => c.GarageId == garageId.Value);
            }
            if (fromYear.HasValue)
            {
                query = query.Where(c => c.productionYear >= fromYear.Value);
            }
            if (toYear.HasValue)
            {
                query = query.Where(c => c.productionYear <= toYear.Value);
            }

            var cars = await query.Select(c => new ResponseCarDTO
            {
                id = c.id,
                make = c.make,
                model = c.model,
                productionYear = c.productionYear,
                licensePlate = c.licensePlate,
                garages = c.Garage != null ? new List<ResponseGarageDTO>
                {
                    new ResponseGarageDTO
                    {
                        id = c.Garage.id,
                        name = c.Garage.name,
                        location = c.Garage.location,
                        city = c.Garage.city,
                        capacity = c.Garage.capacity
                    }
                } : null
            }).ToListAsync();

            return Ok(cars);
        }

        // POST: api/cars
        [HttpPost]
        public async Task<ActionResult<ResponseCarDTO>> CreateCar([FromBody] CreateCarDTO carDto)
        {
            var car = new Car
            {
                make = carDto.make,
                model = carDto.model,
                productionYear = carDto.productionYear,
                licensePlate = carDto.licensePlate,
                GarageId = carDto.garageIds.FirstOrDefault()  // Assuming single garage association
            };

            _context.Cars.Add(car);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCar", new { id = car.id }, new ResponseCarDTO
            {
                id = car.id,
                make = car.make,
                model = car.model,
                productionYear = car.productionYear,
                licensePlate = car.licensePlate
            });
        }

        // PUT: api/cars/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCar(long id, [FromBody] UpdateCarDTO carDto)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }

            car.make = carDto.make;
            car.model = carDto.model;
            car.productionYear = carDto.productionYear;
            car.licensePlate = carDto.licensePlate;

            // Если у автомобиля должен быть только один гараж, обновляем GarageId; предполагаем, что в списке один ID
            if (carDto.garageIds != null && carDto.garageIds.Any())
            {
                car.GarageId = carDto.garageIds.FirstOrDefault();
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }


        // DELETE: api/cars/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCar(long id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }

            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}

