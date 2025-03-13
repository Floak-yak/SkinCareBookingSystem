using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Service.Interfaces
{
    public interface ISurveyService
    {
        (string question, Dictionary<string, string> choices) GetQuestion(string questionId);
        string GetNextQuestionId(string currentQuestionId, string choice);
        bool IsEndQuestion(string questionId);
    }
}
