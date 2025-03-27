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
        
        // Can be used to store additional data or state for this node
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
        
        // For managing the navigation between questions
        public string ParentNodeId { get; set; }
        public string NextNodeId { get; set; }
        
        // Reference to the survey this node belongs to
        public int SurveyId { get; set; }
        public Survey Survey { get; set; }
    }
}
