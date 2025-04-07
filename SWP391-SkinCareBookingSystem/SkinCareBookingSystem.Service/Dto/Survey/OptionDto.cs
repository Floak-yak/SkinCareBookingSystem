using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Service.Dto.Survey
{
    public class OptionDto
    {
        public string OptionText { get; set; }
        public int Points { get; set; } = 0;
        public string SkinTypeId { get; set; }
    }
}