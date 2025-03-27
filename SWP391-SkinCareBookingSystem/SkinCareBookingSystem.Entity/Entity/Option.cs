using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.BusinessObject.Entity
{
    public class Option
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public string NextQuestionId { get; set; }
        
        public bool IsResultOption => NextQuestionId?.StartsWith("RESULT_") ?? false;
        
        public int QuestionId { get; set; }
        public Survey Question { get; set; }
    }
}
