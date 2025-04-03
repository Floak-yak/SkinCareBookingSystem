using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.BusinessObject.Entity
{
    public class UserSkinTypeScore
    {
        public int Id { get; set; }
        public int SessionId { get; set; }
        public string SkinTypeId { get; set; }
        public int Score { get; set; }
        
        public SurveySession Session { get; set; }
    }
} 