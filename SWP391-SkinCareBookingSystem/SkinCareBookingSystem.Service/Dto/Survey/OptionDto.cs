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
        public List<SkinTypePointsDto> SkinTypePoints { get; set; } = new List<SkinTypePointsDto>();
    }
}