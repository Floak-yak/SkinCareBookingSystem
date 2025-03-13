using System;

namespace SkinCareBookingSystem.Service.Dto
{
    public class ScheduleDto
    {
        public int Id { get; set; }
        public DateTime DateWork { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
    }
} 