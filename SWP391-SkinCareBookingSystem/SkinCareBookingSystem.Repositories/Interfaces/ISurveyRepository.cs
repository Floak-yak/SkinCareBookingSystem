using SkinCareBookingSystem.BusinessObject.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Repositories.Interfaces
{
    public interface ISurveyRepository
    {
        public Dictionary<string, Node> LoadSurvey();
        public void SaveSurvey(Dictionary<string, Node> surveyTree);
    }
}
