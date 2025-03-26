using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.Service.Dto;
using SkinCareBookingSystem.Service.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Controller.Controllers
{
    [Route("api/[controller]")]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleService _scheduleService;
        private readonly IMapper _mapper;

        public ScheduleController(IScheduleService scheduleService, IMapper mapper)
        {
            _scheduleService = scheduleService;
            _mapper = mapper;
        }

        [HttpGet("Gets")]
        public async Task<IActionResult> GetAllSchedules()
        {
            var schedules = await _scheduleService.GetAllSchedulesAsync();
            return Ok(schedules);
        }

        [HttpGet("GetScheduleBySkinTherapistId")]
        public Task<IActionResult> GetScheduleBySkinTherapistId([FromQuery] int skinTherapistId)
        {
            throw new NotImplementedException();
        }

        [HttpGet("Weekly")]
        public async Task<IActionResult> GetWeeklySchedules()
        {
            var schedules = await _scheduleService.GetWeeklySchedulesAsync();
            return Ok(schedules);
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetSchedule(int id)
        {
            var schedule = await _scheduleService.GetScheduleByIdAsync(id);
            if (schedule == null)
                return BadRequest("Schedule not found");
            return Ok(schedule);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateSchedule([FromBody] Schedule schedule)
        {
            if (schedule == null)
                return BadRequest("Schedule data is required");

            var createdSchedule = await _scheduleService.CreateScheduleAsync(schedule);
            if (createdSchedule == null)
                return BadRequest("Failed to create schedule");

            return Ok(createdSchedule);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> UpdateSchedule(int id, [FromBody] Schedule schedule)
        {
            if (schedule == null)
                return BadRequest("Schedule data is required");

            try
            {
                await _scheduleService.UpdateScheduleAsync(id, schedule);
                return Ok("Schedule updated successfully");
            }
            catch (System.ArgumentException)
            {
                return BadRequest("Invalid schedule data");
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteSchedule(int id)
        {
            await _scheduleService.DeleteScheduleAsync(id);
            return Ok("Schedule deleted successfully");
        }
    }
} 