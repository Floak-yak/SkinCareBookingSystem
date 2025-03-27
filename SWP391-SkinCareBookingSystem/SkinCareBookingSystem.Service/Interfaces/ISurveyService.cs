using SkinCareBookingSystem.BusinessObject.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Service.Interfaces
{
    public interface ISurveyService
    {
        Task<Survey> GetQuestionByIdAsync(string questionId);
        Task<Survey> GetFirstQuestionAsync();
        Task<Survey> GetNextQuestionAsync(string currentQuestionId, string selectedOptionId);
        Task<bool> IsResultQuestionAsync(string questionId);
        Task<Survey> GetResultByIdAsync(string resultId);
    }
}