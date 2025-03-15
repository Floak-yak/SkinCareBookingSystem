﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.BusinessObject.Entity
{
    public class SkincareService
    {
        public int Id { get; set; }
        public string ServiceName { get; set; }
        public string ServiceDescription { get; set; }
        public int ImageId { get; set; }
        public Image Image { get; set; }

        #region Relationship
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public Decimal Price { get; set; }
        public int WorkTime { get; set; } //Minute
        public ICollection<BookingServiceSchedule> BookingServiceSchedules { get; set; }
        #endregion
    }
}
