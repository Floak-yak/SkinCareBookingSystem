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
        public Dictionary<string, string> Choices { get; set; } = new Dictionary<string, string>();
    }
}
