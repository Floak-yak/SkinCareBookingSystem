﻿using SkinCareBookingSystem.BusinessObject.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Service.Dto
{
    public class GetBookingsResponse
    {
        public int Id { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime Date { get; set; }
        public decimal TotalPrice { get; set; }
        public BookingStatus Status { get; set; }
        public ViewUser User { get; set; }
    }
}
