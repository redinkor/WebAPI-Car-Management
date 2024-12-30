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
    public class GaragesController : ControllerBase
    {
        private readonly CarCatalogDbContext _context;

        public GaragesController(CarCatalogDbContext context)
        {
            _context = context;
        }

        // Получение всех сервисов
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResponseGarageDTO>>> GetAllGarages(string city = null)
        {
            var query = _context.Garages.AsQueryable();

            if (!string.IsNullOrEmpty(city))
            {
                query = query.Where(g => g.city == city);
            }

            var garages = await query.Select(g => new ResponseGarageDTO
            {
                id = g.id,
                name = g.name,
                location = g.location,
                city = g.city,
                capacity = g.capacity
            }).ToListAsync();

            return Ok(garages);
        }

        // Получение одного сервиса по ID
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseGarageDTO>> GetGarage(long id)
        {
            var garage = await _context.Garages.FindAsync(id);

            if (garage == null)
            {
                return NotFound();
            }

            return new ResponseGarageDTO
            {
                id = garage.id,
                name = garage.name,
                location = garage.location,
                city = garage.city,
                capacity = garage.capacity
            };
        }

        // Создание нового сервиса
        [HttpPost]
        public async Task<ActionResult<ResponseGarageDTO>> CreateGarage(CreateGarageDTO dto)
        {
            var garage = new Garage
            {
                name = dto.name,
                location = dto.location,
                city = dto.city,
                capacity = dto.capacity
            };

            _context.Garages.Add(garage);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetGarage), new { id = garage.id }, new ResponseGarageDTO
            {
                id = garage.id,
                name = garage.name,
                location = garage.location,
                city = garage.city,
                capacity = garage.capacity
            });
        }

        // Обновление сервиса
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGarage(long id, UpdateGarageDTO dto)
        {
            var garage = await _context.Garages.FindAsync(id);
            if (garage == null)
            {
                return NotFound();
            }

            garage.name = dto.name;
            garage.location = dto.location;
            garage.city = dto.city;
            garage.capacity = dto.capacity;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Удаление сервиса
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGarage(long id)
        {
            var garage = await _context.Garages.FindAsync(id);
            if (garage == null)
            {
                return NotFound();
            }

            _context.Garages.Remove(garage);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/garages/report/{garageId}/{date}
        [HttpGet("report/{garageId}/{date}")]
        public async Task<ActionResult<GarageDailyAvailabilityReportDTO>> GetDailyAvailabilityReport(long garageId, DateTime date)
        {
            var garage = await _context.Garages.FindAsync(garageId);
            if (garage == null)
            {
                return NotFound("Garage not found.");
            }

            // Counting maintenance requests for the specified date
            var requestsCount = await _context.MaintenanceRequests
                .Where(m => m.garageId == garageId && m.scheduledDate.Date == date.Date)
                .CountAsync();

            var report = new GarageDailyAvailabilityReportDTO
            {
                date = date,
                requests = requestsCount,
                availableCapacity = garage.capacity - requestsCount
            };

            return Ok(report);
        }
    }
}

