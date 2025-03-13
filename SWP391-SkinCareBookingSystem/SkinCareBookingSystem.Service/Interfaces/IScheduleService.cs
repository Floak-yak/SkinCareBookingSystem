using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.Service.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Service.Interfaces
{
    public interface IScheduleService
    {
        Task<IEnumerable<ScheduleDto>> GetAllSchedulesAsync();
        Task<ScheduleDto> GetScheduleByIdAsync(int id);
        Task<IEnumerable<ScheduleDto>> GetWeeklySchedulesAsync();
        Task<ScheduleDto> CreateScheduleAsync(Schedule schedule);
        Task UpdateScheduleAsync(int id, Schedule schedule);
        Task DeleteScheduleAsync(int id);
    }
} 