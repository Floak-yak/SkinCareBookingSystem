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
        Task<Survey> GetQuestionByIdAsync(string questionId);
        Task<Survey> GetFirstQuestionAsync();
        Task<Survey> GetResultByIdAsync(string resultId);
        Task<IEnumerable<Survey>> GetAllQuestionsAsync();
        Task<bool> SaveSurveyAsync(Survey survey);
    }
}