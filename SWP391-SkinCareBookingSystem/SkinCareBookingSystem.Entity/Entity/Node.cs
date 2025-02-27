using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.BusinessObject.Entity
{
    public class Node
    {
        public string Question { get; set; }
        public Dictionary<string, Node> Next { get; set; }
        public string Recommendation { get; set; }

        public Node(string question)
        {
            Question = question;
            Next = new Dictionary<string, Node>();
        }
    }
}
