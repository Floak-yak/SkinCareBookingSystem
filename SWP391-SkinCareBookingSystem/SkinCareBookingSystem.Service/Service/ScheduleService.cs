using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.Repositories.Interfaces;
using SkinCareBookingSystem.Service.Dto;
using SkinCareBookingSystem.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Service.Service
{
    public class ScheduleService : IScheduleService
    {
        private readonly IScheduleRepository _scheduleRepository;

        public ScheduleService(IScheduleRepository scheduleRepository)
        {
            _scheduleRepository = scheduleRepository;
        }

        public async Task<IEnumerable<ScheduleDto>> GetAllSchedulesAsync()
        {
            var schedules = await _scheduleRepository.GetAllAsync();
            return schedules.Select(s => new ScheduleDto
            {
                Id = s.Id,
                DateWork = s.DateWork,
                UserId = s.UserId,
                UserName = s.User?.FullName
            });
        }

        public async Task<ScheduleDto> GetScheduleByIdAsync(int id)
        {
            var schedule = await _scheduleRepository.GetByIdAsync(id);
            if (schedule == null)
                return null;

            return new ScheduleDto
            {
                Id = schedule.Id,
                DateWork = schedule.DateWork,
                UserId = schedule.UserId,
                UserName = schedule.User?.FullName
            };
        }

        public async Task<IEnumerable<ScheduleDto>> GetWeeklySchedulesAsync()
        {
            var today = DateTime.Today;
            var startDate = today.AddDays(-(int)today.DayOfWeek + 1); // Get last Monday
            var endDate = startDate.AddDays(7); // Get next Monday

            var schedules = await _scheduleRepository.GetSchedulesByDateRangeAsync(startDate, endDate);
            return schedules.Select(s => new ScheduleDto
            {
                Id = s.Id,
                DateWork = s.DateWork,
                UserId = s.UserId,
                UserName = s.User?.FullName
            });
        }

        public async Task<ScheduleDto> CreateScheduleAsync(Schedule schedule)
        {
            var createdSchedule = await _scheduleRepository.CreateAsync(schedule);
            return new ScheduleDto
            {
                Id = createdSchedule.Id,
                DateWork = createdSchedule.DateWork,
                UserId = createdSchedule.UserId,
                UserName = createdSchedule.User?.FullName
            };
        }

        public async Task UpdateScheduleAsync(int id, Schedule schedule)
        {
            if (id != schedule.Id)
                throw new ArgumentException("ID mismatch");

            await _scheduleRepository.UpdateAsync(schedule);
        }

        public async Task DeleteScheduleAsync(int id)
        {
            await _scheduleRepository.DeleteAsync(id);
        }
    }
} 