using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkinCareBookingSystem.BusinessObject.Entity;

namespace SkinCareBookingSystem.Service.Interfaces
{
    public interface ISurveyService
    {
        (string question, List<SurveyOption> options) GetQuestion(string questionId);
        string GetNextQuestionId(string currentQuestionId, string optionIndex);
        bool IsEndQuestion(string questionId);
        Dictionary<string, SurveyQuestion> GetAllQuestions();
        bool UpdateQuestion(SurveyQuestion updatedQuestion);
        bool AddQuestion(SurveyQuestion newQuestion);
        bool DeleteQuestion(string questionId);
    }
}
