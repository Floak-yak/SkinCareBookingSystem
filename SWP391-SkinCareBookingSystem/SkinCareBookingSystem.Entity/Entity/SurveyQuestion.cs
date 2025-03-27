using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.BusinessObject.Entity
{
    public class SurveyQuestion
    {
        public int Id { get; set; }
        public string QuestionId { get; set; } // Matches the ID in the Node (e.g., Q1, Q2, etc.)
        public string QuestionText { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Relationships
        public ICollection<SurveyOption> Options { get; set; }
        public ICollection<SurveyResponse> Responses { get; set; }
    }
} 