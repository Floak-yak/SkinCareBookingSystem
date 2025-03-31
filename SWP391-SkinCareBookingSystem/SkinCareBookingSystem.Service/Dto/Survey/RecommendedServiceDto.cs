using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Service.Dto.Survey
{
    public class RecommendedServiceDto
    {
        public int SurveyResultId { get; set; }
        public int ServiceId { get; set; }
        public int Priority { get; set; }
    }
} 