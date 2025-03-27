using Microsoft.EntityFrameworkCore;
using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.Repositories.Interfaces;
using SkinCareBookingSystem.Service.Dto;
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
            var currentQuestion = await _surveyRepository.GetQuestionByIdAsync(currentQuestionId);
            if (currentQuestion == null)
                return null;

            var selectedOption = currentQuestion.Options.FirstOrDefault(o => o.Id == selectedOptionId);
            if (selectedOption == null)
                return null;

            return await _surveyRepository.GetQuestionByIdAsync(selectedOption.NextQuestionId);
        }

        public async Task<Survey> GetResultByIdAsync(string resultId)
        {
            return await _surveyRepository.GetResultByIdAsync(resultId);
        }

        public async Task<IEnumerable<Survey>> GetAllQuestionsAsync()
        {
            return await _surveyRepository.GetAllQuestionsAsync();
        }

        public async Task<SurveyResponseDto> CreateSurveyAsync(CreateSurveyDto createSurveyDto)
        {
            try
            {
                var validationErrors = ValidateCreateSurveyDto(createSurveyDto);
                if (validationErrors.Any())
                {
                    return new SurveyResponseDto
                    {
                        Success = false,
                        Message = "Validation failed.",
                        Errors = validationErrors
                    };
                }

                var existingSurvey = await _surveyRepository.GetQuestionByIdAsync(createSurveyDto.QuestionIdentifier);
                if (existingSurvey != null)
                {
                    return new SurveyResponseDto
                    {
                        Success = false,
                        Message = $"Survey question with identifier {createSurveyDto.QuestionIdentifier} already exists.",
                        Errors = new List<string> { $"Survey question with identifier {createSurveyDto.QuestionIdentifier} already exists." }
                    };
                }

                var survey = new Survey
                {
                    QuestionIdentifier = createSurveyDto.QuestionIdentifier,
                    Question = createSurveyDto.Question,
                    IsResult = createSurveyDto.IsResult,
                    Options = createSurveyDto.Options.Select(o => new Option
                    {
                        OptionText = o.OptionText,
                        NextQuestionId = o.NextQuestionId
                    }).ToList()
                };

                var success = await _surveyRepository.SaveSurveyAsync(survey);
                if (success)
                {
                    return new SurveyResponseDto
                    {
                        Success = true,
                        Message = "Survey question created successfully.",
                        Data = survey
                    };
                }

                return new SurveyResponseDto
                {
                    Success = false,
                    Message = "Failed to create survey question.",
                    Errors = new List<string> { "Database operation failed." }
                };
            }
            catch (Exception ex)
            {
                return new SurveyResponseDto
                {
                    Success = false,
                    Message = $"Error creating survey question: {ex.Message}",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<SurveyResponseDto> UpdateSurveyAsync(string questionId, UpdateSurveyDto updateSurveyDto)
        {
            try
            {
                var validationErrors = ValidateUpdateSurveyDto(updateSurveyDto);
                if (validationErrors.Any())
                {
                    return new SurveyResponseDto
                    {
                        Success = false,
                        Message = "Validation failed.",
                        Errors = validationErrors
                    };
                }

                var existingSurvey = await _surveyRepository.GetQuestionByIdAsync(questionId);
                if (existingSurvey == null)
                {
                    return new SurveyResponseDto
                    {
                        Success = false,
                        Message = $"Survey question with identifier {questionId} not found.",
                        Errors = new List<string> { $"Survey question with identifier {questionId} not found." }
                    };
                }

                existingSurvey.Question = updateSurveyDto.Question;
                existingSurvey.IsResult = updateSurveyDto.IsResult;

                var existingOptionIds = existingSurvey.Options.Select(o => o.Id).ToList();
                var updatedOptionIds = updateSurveyDto.Options.Select(o => o.Id).Where(id => id != 0).ToList();
                
                var optionsToRemove = existingSurvey.Options.Where(o => !updatedOptionIds.Contains(o.Id)).ToList();
                foreach (var option in optionsToRemove)
                {
                    existingSurvey.Options.Remove(option);
                }

                foreach (var optionDto in updateSurveyDto.Options)
                {
                    if (optionDto.Id == 0)
                    {
                        existingSurvey.Options.Add(new Option
                        {
                            OptionText = optionDto.OptionText,
                            NextQuestionId = optionDto.NextQuestionId
                        });
                    }
                    else
                    {
                        var existingOption = existingSurvey.Options.FirstOrDefault(o => o.Id == optionDto.Id);
                        if (existingOption != null)
                        {
                            existingOption.OptionText = optionDto.OptionText;
                            existingOption.NextQuestionId = optionDto.NextQuestionId;
                        }
                    }
                }

                var success = await _surveyRepository.SaveSurveyAsync(existingSurvey);
                if (success)
                {
                    return new SurveyResponseDto
                    {
                        Success = true,
                        Message = "Survey question updated successfully.",
                        Data = existingSurvey
                    };
                }

                return new SurveyResponseDto
                {
                    Success = false,
                    Message = "Failed to update survey question.",
                    Errors = new List<string> { "Database operation failed." }
                };
            }
            catch (Exception ex)
            {
                return new SurveyResponseDto
                {
                    Success = false,
                    Message = $"Error updating survey question: {ex.Message}",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<SurveyResponseDto> DeleteSurveyAsync(string questionId)
        {
            try
            {
                var success = await _surveyRepository.DeleteSurveyAsync(questionId);
                if (success)
                {
                    return new SurveyResponseDto
                    {
                        Success = true,
                        Message = $"Survey question with identifier {questionId} deleted successfully."
                    };
                }

                return new SurveyResponseDto
                {
                    Success = false,
                    Message = $"Survey question with identifier {questionId} not found or could not be deleted.",
                    Errors = new List<string> { $"Survey question with identifier {questionId} not found or could not be deleted." }
                };
            }
            catch (Exception ex)
            {
                return new SurveyResponseDto
                {
                    Success = false,
                    Message = $"Error deleting survey question: {ex.Message}",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        #region Validation Methods
        private List<string> ValidateCreateSurveyDto(CreateSurveyDto dto)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(dto.QuestionIdentifier))
                errors.Add("Question identifier is required.");

            if (string.IsNullOrWhiteSpace(dto.Question))
                errors.Add("Question text is required.");

            if (!dto.IsResult && (dto.Options == null || !dto.Options.Any()))
                errors.Add("Non-result questions must have at least one option.");

            if (dto.Options != null)
            {
                for (int i = 0; i < dto.Options.Count; i++)
                {
                    var option = dto.Options[i];
                    if (string.IsNullOrWhiteSpace(option.OptionText))
                        errors.Add($"Option {i+1} text is required.");

                    if (!dto.IsResult && string.IsNullOrWhiteSpace(option.NextQuestionId))
                        errors.Add($"Option {i+1} next question ID is required for non-result questions.");
                }
            }

            return errors;
        }

        private List<string> ValidateUpdateSurveyDto(UpdateSurveyDto dto)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(dto.Question))
                errors.Add("Question text is required.");

            if (!dto.IsResult && (dto.Options == null || !dto.Options.Any()))
                errors.Add("Non-result questions must have at least one option.");

            if (dto.Options != null)
            {
                for (int i = 0; i < dto.Options.Count; i++)
                {
                    var option = dto.Options[i];
                    if (string.IsNullOrWhiteSpace(option.OptionText))
                        errors.Add($"Option {i+1} text is required.");

                    if (!dto.IsResult && string.IsNullOrWhiteSpace(option.NextQuestionId))
                        errors.Add($"Option {i+1} next question ID is required for non-result questions.");
                }
            }

            return errors;
        }
        #endregion
    }
}