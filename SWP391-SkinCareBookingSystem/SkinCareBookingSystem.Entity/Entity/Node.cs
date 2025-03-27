using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.BusinessObject.Entity
{
    public class Node
    {
        public string Id { get; set; }
        public string Content { get; set; }
        
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
        
        public string ParentNodeId { get; set; }
        public string NextNodeId { get; set; }
        
        public int SurveyId { get; set; }
        public Survey Survey { get; set; }
    }
}
