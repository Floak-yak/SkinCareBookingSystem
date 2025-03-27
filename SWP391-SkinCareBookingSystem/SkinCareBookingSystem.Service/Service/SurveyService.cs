using Microsoft.EntityFrameworkCore;
using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.Repositories.Interfaces;
using SkinCareBookingSystem.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Service.Service
{
    public class SurveyService : ISurveyService
    {
        private readonly ISurveyRepository _surveyRepository;

        public SurveyService(ISurveyRepository surveyRepository)
        {
            _surveyRepository = surveyRepository;
        }

        public async Task<Survey> GetQuestionByIdAsync(string questionId)
        {
            return await _surveyRepository.GetQuestionByIdAsync(questionId);
        }

        public async Task<Survey> GetFirstQuestionAsync()
        {
            return await _surveyRepository.GetFirstQuestionAsync();
        }

        public async Task<Survey> GetNextQuestionAsync(string currentQuestionId, string selectedOptionId)
        {
            // Get the current question
            var currentQuestion = await _surveyRepository.GetQuestionByIdAsync(currentQuestionId);
            if (currentQuestion == null)
                return null;

            // Find the selected option
            var selectedOption = currentQuestion.Options.FirstOrDefault(o => o.Id == selectedOptionId);
            if (selectedOption == null)
                return null;

            // Get the next question
            return await _surveyRepository.GetQuestionByIdAsync(selectedOption.NextQuestionId);
        }

        public async Task<bool> IsResultQuestionAsync(string questionId)
        {
            var question = await _surveyRepository.GetQuestionByIdAsync(questionId);
            return question?.IsResult ?? false;
        }

        public async Task<Survey> GetResultByIdAsync(string resultId)
        {
            return await _surveyRepository.GetResultByIdAsync(resultId);
        }
    }
}