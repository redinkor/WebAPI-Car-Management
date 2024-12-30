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
    public class ReportsController : ControllerBase
    {
        private readonly CarCatalogDbContext _context;

        public ReportsController(CarCatalogDbContext context)
        {
            _context = context;
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseMaintenanceDTO>> GetMaintenance(long id)
        {
            var maintenanceRequest = await _context.MaintenanceRequests
                .Include(m => m.car)
                .Include(m => m.garage)
                .Select(mr => new ResponseMaintenanceDTO
                {
                    id = mr.id,
                    carId = mr.carId,
                    carName = mr.car.make + " " + mr.car.model,
                    serviceType = mr.serviceType,
                    scheduledDate = mr.scheduledDate,
                    garageId = mr.garageId,
                    garageName = mr.garage.name
                })
                .FirstOrDefaultAsync(mr => mr.id == id);

            if (maintenanceRequest == null)
            {
                return NotFound();
            }

            return maintenanceRequest;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMaintenance(long id, UpdateMaintenanceDTO dto)
        {
            var maintenanceRequest = await _context.MaintenanceRequests
                                            .FirstOrDefaultAsync(m => m.id == id);
            if (maintenanceRequest == null)
            {
                return NotFound("Maintenance request not found.");
            }

            // Получаем данные текущего гаража для заявки
            var currentGarage = await _context.Garages.FindAsync(maintenanceRequest.garageId);
            if (currentGarage == null)
            {
                return NotFound("Current garage not found.");
            }

            // Проверяем изменение даты и наличие свободных мест
            if (maintenanceRequest.scheduledDate.Date != dto.scheduledDate.Date || maintenanceRequest.garageId != dto.garageId)
            {
                var maintenanceCount = await _context.MaintenanceRequests
                    .Where(mr => mr.garageId == dto.garageId && mr.scheduledDate.Date == dto.scheduledDate.Date)
                    .CountAsync();

                var targetGarage = await _context.Garages.FindAsync(dto.garageId);
                if (targetGarage == null)
                {
                    return NotFound("Target garage not found.");
                }

                if (maintenanceCount >= targetGarage.capacity)
                {
                    return BadRequest("No available slots on the new date in the selected garage.");
                }
            }

            // Обновление данных заявки
            maintenanceRequest.serviceType = dto.serviceType;
            maintenanceRequest.carId = dto.carId;
            maintenanceRequest.garageId = dto.garageId;
            maintenanceRequest.scheduledDate = dto.scheduledDate;

            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpPost]
        public async Task<IActionResult> CreateMaintenance(CreateMaintenanceDTO dto)
        {
            // Проверка наличия гаража
            var garageExists = await _context.Garages.AnyAsync(g => g.id == dto.garageId);
            if (!garageExists)
            {
                return NotFound("Garage not found.");
            }

            // Получение количества запланированных заявок на обслуживание в этом гараже на выбранную дату
            var maintenanceCountOnDate = await _context.MaintenanceRequests
                .Where(mr => mr.garageId == dto.garageId && mr.scheduledDate.Date == dto.scheduledDate.Date)
                .CountAsync();

            // Получение вместимости гаража
            var garageCapacity = await _context.Garages
                .Where(g => g.id == dto.garageId)
                .Select(g => g.capacity)
                .FirstOrDefaultAsync();

            // Проверяем наличие свободных мест
            if (maintenanceCountOnDate >= garageCapacity)
            {
                return BadRequest("No available slots on the selected date.");
            }

            var maintenanceRequest = new MaintenanceRequest
            {
                carId = dto.carId,
                garageId = dto.garageId,
                serviceType = dto.serviceType,
                scheduledDate = dto.scheduledDate
            };

            _context.MaintenanceRequests.Add(maintenanceRequest);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMaintenance), new { id = maintenanceRequest.id }, maintenanceRequest);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMaintenance(long id)
        {
            var maintenanceRequest = await _context.MaintenanceRequests.FindAsync(id);
            if (maintenanceRequest == null)
            {
                return NotFound("Maintenance request not found.");
            }

            _context.MaintenanceRequests.Remove(maintenanceRequest);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        // GET: api/reports/monthly-maintenance
        [HttpGet("monthly-maintenance")]
        public async Task<ActionResult<IEnumerable<MonthlyRequestsReportDTO>>> GetMonthlyMaintenanceReport(long garageId, DateTime startDate, DateTime endDate)
        {
            var maintenanceRequests = await _context.MaintenanceRequests
                .Where(mr => mr.garageId == garageId && mr.scheduledDate >= startDate && mr.scheduledDate <= endDate)
                .GroupBy(
                    mr => new { year = mr.scheduledDate.Year, month = mr.scheduledDate.Month },
                    mr => mr,
                    (key, group) => new MonthlyRequestsReportDTO
                    {
                        yearMonth = $"{key.year}-{key.month:D2}",
                        requests = group.Count()
                    })
                .ToListAsync();

            return Ok(maintenanceRequests);
        }
    }
}

