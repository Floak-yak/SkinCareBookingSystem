using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkinCareBookingSystem.Repositories.Interfaces;

namespace SkinCareBookingSystem.Service.Service
{
    public class SurveyService
    {
        private readonly ISurveyRepository _surveyRepository;

        public SurveyService(ISurveyRepository surveyRepository)
        {
            _surveyRepository = surveyRepository;
        }

        public (string question, Dictionary<string, string> choices) GetQuestion(string questionId)
        {
            var surveyTree = _surveyRepository.LoadSurvey();
            
            if (!surveyTree.ContainsKey(questionId))
                return (null, null);

            var node = surveyTree[questionId];
            return (node.Content, node.Choices);
        }

        public string GetNextQuestionId(string currentQuestionId, string choice)
        {
            var surveyTree = _surveyRepository.LoadSurvey();
            
            if (!surveyTree.ContainsKey(currentQuestionId))
                return null;

            var node = surveyTree[currentQuestionId];
            return node.Choices.ContainsKey(choice) ? node.Choices[choice] : null;
        }

        public bool IsEndQuestion(string questionId)
        {
            return questionId.StartsWith("RESULT_");
        }
    }
}
