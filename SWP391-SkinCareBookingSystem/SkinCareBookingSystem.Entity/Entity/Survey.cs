using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.BusinessObject.Entity
{
    public class Survey
    {
        public int QuestionId { get; set; }
        public string QuestionIdentifier { get; set; } // e.g., "Q1", "Q2A", etc.
        public string Question { get; set; }
        public bool IsResult { get; set; } // Indicates if this is a result node

        #region Relationship
        // Collection of options for this question
        public ICollection<Option> Options { get; set; } = new List<Option>();
        
        // Collection of nodes that make up this test
        public ICollection<Node> Nodes { get; set; } = new List<Node>();
        #endregion
    }
}
