using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.Repositories.Interfaces;
using SkinCareBookingSystem.Service.Interfaces;

namespace SkinCareBookingSystem.Service.Service
{
    public class SurveyService : ISurveyService
    {
        private readonly ISurveyRepository _surveyRepository;

        public SurveyService(ISurveyRepository surveyRepository)
        {
            _surveyRepository = surveyRepository;
        }

        public (string question, List<SurveyOption> options) GetQuestion(string questionId)
        {
            var surveyTree = _surveyRepository.LoadSurvey();
            
            if (!surveyTree.ContainsKey(questionId))
                return (null, null);

            var question = surveyTree[questionId];
            return (question.Question, question.Options);
        }

        public string GetNextQuestionId(string currentQuestionId, string optionIndex)
        {
            var surveyTree = _surveyRepository.LoadSurvey();
            
            if (!surveyTree.ContainsKey(currentQuestionId))
                return null;

            var question = surveyTree[currentQuestionId];
            
            if (!int.TryParse(optionIndex, out int index) || index < 0 || index >= question.Options.Count)
                return null;
                
            return question.Options[index].NextId;
        }

        public bool IsEndQuestion(string questionId)
        {
            return questionId.StartsWith("RESULT_");
        }
        
        public Dictionary<string, SurveyQuestion> GetAllQuestions()
        {
            return _surveyRepository.LoadSurvey();
        }
        
        public bool UpdateQuestion(SurveyQuestion updatedQuestion)
        {
            if (string.IsNullOrEmpty(updatedQuestion.Id))
                return false;
                
            var surveyTree = _surveyRepository.LoadSurvey();
            
            if (!surveyTree.ContainsKey(updatedQuestion.Id))
                return false;
                
            surveyTree[updatedQuestion.Id] = updatedQuestion;
            _surveyRepository.SaveSurvey(surveyTree);
            
            return true;
        }
        
        public bool AddQuestion(SurveyQuestion newQuestion)
        {
            if (string.IsNullOrEmpty(newQuestion.Id))
                return false;
                
            var surveyTree = _surveyRepository.LoadSurvey();
            
            if (surveyTree.ContainsKey(newQuestion.Id))
                return false; // Question ID already exists
                
            surveyTree[newQuestion.Id] = newQuestion;
            _surveyRepository.SaveSurvey(surveyTree);
            
            return true;
        }
        
        public bool DeleteQuestion(string questionId)
        {
            var surveyTree = _surveyRepository.LoadSurvey();
            
            if (!surveyTree.ContainsKey(questionId))
                return false;
                
            surveyTree.Remove(questionId);
            _surveyRepository.SaveSurvey(surveyTree);
            
            return true;
        }
    }
}
