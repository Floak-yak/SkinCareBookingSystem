using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.Service.Dto;
using SkinCareBookingSystem.Service.Dto.Schedule;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Service.Interfaces
{
    public interface IScheduleService
    {
        Task<IEnumerable<ScheduleDto>> GetAllSchedulesAsync();
        Task<List<ScheduleResponse>> GetSchedulesBySkinTherapistId(int skintherapistId);
        Task<ScheduleDto> GetScheduleByIdAsync(int id);
        Task<IEnumerable<ScheduleDto>> GetWeeklySchedulesAsync();
        Task<ScheduleDto> CreateScheduleAsync(Schedule schedule);
        Task UpdateScheduleAsync(int id, Schedule schedule);
        Task DeleteScheduleAsync(int id);
    }
} 