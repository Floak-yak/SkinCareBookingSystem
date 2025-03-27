using Microsoft.EntityFrameworkCore;
using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.Repositories.Data;
using SkinCareBookingSystem.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Repositories.Repositories
{
    public class SurveyRepository : ISurveyRepository
    {
        private readonly AppDbContext _context;

        public SurveyRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Survey> GetQuestionByIdAsync(string questionId)
        {
            return await _context.Surveys
                .Include(s => s.Options)
                .FirstOrDefaultAsync(s => s.QuestionIdentifier == questionId);
        }

        public async Task<Survey> GetFirstQuestionAsync()
        {
            // Assuming "Q1" is always the first question
            return await GetQuestionByIdAsync("Q1");
        }

        public async Task<Survey> GetResultByIdAsync(string resultId)
        {
            return await _context.Surveys
                .Include(s => s.Options)
                .FirstOrDefaultAsync(s => s.QuestionIdentifier == resultId && s.IsResult);
        }

        public async Task<IEnumerable<Survey>> GetAllQuestionsAsync()
        {
            return await _context.Surveys
                .Include(s => s.Options)
                .ToListAsync();
        }

        public async Task<bool> SaveSurveyAsync(Survey survey)
        {
            try
            {
                if (survey.QuestionId == 0) // New survey
                {
                    await _context.Surveys.AddAsync(survey);
                }
                else // Update existing
                {
                    _context.Surveys.Update(survey);
                }
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}