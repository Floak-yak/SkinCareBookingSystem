using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.BusinessObject.Entity
{
    public class SurveyQuestion
    {
        public string Id { get; set; }
        public string Question { get; set; }
        public List<SurveyOption> Options { get; set; } = new List<SurveyOption>();
    }

    public class SurveyOption
    {
        public string Label { get; set; }
        public string NextId { get; set; }
    }
} 