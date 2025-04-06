using Microsoft.EntityFrameworkCore;
using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.Repositories.Interfaces;
using SkinCareBookingSystem.Repositories.Data;

namespace SkinCareBookingSystem.Repositories.Repositories
{
    public class SurveyRepository: ISurveyRepository
    {
        private readonly AppDbContext _context;

        public SurveyRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<SurveyQuestion>> GetAllQuestionsAsync()
        {
            return await _context.SurveyQuestions
                .Include(q => q.Options)
                    .ThenInclude(o => o.Question)
                .Include(q => q.Options)
                    .ThenInclude(o => o.Responses)
                .Include(q => q.Options)
                    .ThenInclude(o => o.SkinTypePoints)
                .ToListAsync();
        }

        public async Task<SurveyQuestion> GetQuestionByIdAsync(int id)
        {
            return await _context.SurveyQuestions
                .Include(q => q.Options)
                    .ThenInclude(o => o.Question)
                .Include(q => q.Options)
                    .ThenInclude(o => o.Responses)
                .Include(q => q.Options)
                    .ThenInclude(o => o.SkinTypePoints)
                .FirstOrDefaultAsync(q => q.Id == id);
        }

        public async Task<SurveyQuestion> AddQuestionAsync(SurveyQuestion question)
        {
            _context.SurveyQuestions.Add(question);
            await _context.SaveChangesAsync();
            return question;
        }

        public async Task<SurveyQuestion> UpdateQuestionAsync(SurveyQuestion question)
        {
            _context.Entry(question).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return question;
        }

        public async Task<bool> DeleteQuestionAsync(int id)
        {
            var question = await _context.SurveyQuestions.FindAsync(id);
            if (question == null) return false;

            _context.SurveyQuestions.Remove(question);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<SurveyOption>> GetOptionsForQuestionAsync(int questionId)
        {
            return await _context.SurveyOptions
                .Where(o => o.QuestionId == questionId)
                .Include(o => o.Question)
                .Include(o => o.Responses)
                .Include(o => o.SkinTypePoints)
                .ToListAsync();
        }

        public async Task<SurveyOption> GetOptionByIdAsync(int id)
        {
            return await _context.SurveyOptions
                .Include(o => o.SkinTypePoints)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<SurveyOption> AddOptionAsync(SurveyOption option)
        {
            _context.SurveyOptions.Add(option);
            await _context.SaveChangesAsync();
            return option;
        }

        public async Task<SurveyOption> UpdateOptionAsync(SurveyOption option)
        {
            _context.Entry(option).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return option;
        }

        public async Task<bool> DeleteOptionAsync(int id)
        {
            var option = await _context.SurveyOptions.FindAsync(id);
            if (option == null) return false;

            _context.SurveyOptions.Remove(option);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<SurveyResult>> GetAllResultsAsync()
        {
            return await _context.SurveyResults
                .Include(r => r.RecommendedServices)
                .ToListAsync();
        }

        public async Task<SurveyResult> GetResultByIdAsync(int id)
        {
            return await _context.SurveyResults.FindAsync(id);
        }

        public async Task<SurveyResult> GetResultByResultIdAsync(string resultId)
        {
            return await _context.SurveyResults
                .FirstOrDefaultAsync(r => r.ResultId == resultId);
        }

        public async Task<SurveyResult> AddResultAsync(SurveyResult result)
        {
            _context.SurveyResults.Add(result);
            await _context.SaveChangesAsync();
            return result;
        }

        public async Task<SurveyResult> UpdateResultAsync(SurveyResult result)
        {
            var existingResult = await _context.SurveyResults.FindAsync(result.Id);
            if (existingResult == null)
            {
                return null;
            }

            existingResult.SkinType = result.SkinType;
            existingResult.ResultText = result.ResultText;
            existingResult.RecommendationText = result.RecommendationText;

            _context.SurveyResults.Update(existingResult);
            await _context.SaveChangesAsync();

            return existingResult;
        }

        public async Task<bool> DeleteResultAsync(int id)
        {
            var result = await _context.SurveyResults.FindAsync(id);
            if (result == null) return false;

            _context.SurveyResults.Remove(result);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<SurveySession> CreateSessionAsync(SurveySession session)
        {
            _context.SurveySessions.Add(session);
            await _context.SaveChangesAsync();
            return session;
        }

        public async Task<SurveySession> GetSessionAsync(int id)
        {
            return await _context.SurveySessions
                .Include(s => s.Responses)
                .Include(s => s.SurveyResult)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<List<SurveySession>> GetSessionsByUserIdAsync(int userId)
        {
            return await _context.SurveySessions
                .Include(s => s.SurveyResult)
                .Where(s => s.UserId == userId && s.IsCompleted)
                .OrderByDescending(s => s.CompletedDate)
                .ToListAsync();
        }

        public async Task<SurveySession> CompleteSessionAsync(int sessionId, int resultId)
        {
            var session = await _context.SurveySessions.FindAsync(sessionId);
            if (session == null)
                return null;

            session.SurveyResultId = resultId;
            session.IsCompleted = true;
            session.CompletedDate = DateTime.Now;

            _context.Entry(session).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return session;
        }

        public async Task<SurveySession> UpdateSessionAsync(SurveySession session)
        {
            _context.Entry(session).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return session;
        }

        public async Task<SurveyResponse> AddResponseAsync(SurveyResponse response)
        {
            _context.SurveyResponses.Add(response);
            await _context.SaveChangesAsync();
            return response;
        }

        public async Task<List<SurveyResponse>> GetResponsesAsync(int sessionId)
        {
            return await _context.SurveyResponses
                .Where(r => r.SessionId == sessionId)
                .ToListAsync();
        }

        public async Task<SkincareService> GetServiceByIdAsync(int serviceId)
        {
            return await _context.SkincareServices.FindAsync(serviceId);
        }

        public async Task AddRecommendedServiceAsync(RecommendedService recommendedService)
        {
            _context.RecommendedServices.Add(recommendedService);
            await _context.SaveChangesAsync();
        }

        public async Task<List<RecommendedService>> GetRecommendedServicesAsync(int resultId)
        {
            return await _context.RecommendedServices
                .Where(rs => rs.SurveyResultId == resultId)
                .ToListAsync();
        }

        public async Task<List<SkincareService>> GetServicesByIdsAsync(List<int> serviceIds)
        {
            return await _context.SkincareServices
                .Where(service => serviceIds.Contains(service.Id))
                .ToListAsync();
        }

        public async Task<bool> DeleteRecommendedServiceAsync(int id)
        {
            var service = await _context.RecommendedServices.FindAsync(id);
            if (service == null)
                return false;

            _context.RecommendedServices.Remove(service);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task UpdateRecommendedServiceAsync(RecommendedService recommendedService)
        {
            _context.RecommendedServices.Update(recommendedService);
            await _context.SaveChangesAsync();
        }

        public async Task<UserSkinTypeScore> UpdateSkinTypeScoreAsync(int sessionId, string skinTypeId, int pointsToAdd)
        {
            var score = await _context.UserSkinTypeScores
                .FirstOrDefaultAsync(s => s.SessionId == sessionId && s.SkinTypeId == skinTypeId);
                
            if (score == null)
            {
                score = new UserSkinTypeScore
                {
                    SessionId = sessionId,
                    SkinTypeId = skinTypeId,
                    Score = pointsToAdd
                };
                _context.UserSkinTypeScores.Add(score);
            }
            else
            {
                score.Score += pointsToAdd;
                _context.Entry(score).State = EntityState.Modified;
            }
            
            await _context.SaveChangesAsync();
            return score;
        }
        
        public async Task<List<UserSkinTypeScore>> GetSkinTypeScoresAsync(int sessionId)
        {
            return await _context.UserSkinTypeScores
                .Where(s => s.SessionId == sessionId)
                .ToListAsync();
        }
        
        public async Task<string> GetSkinTypeAsync(int sessionId)
        {
            var scores = await _context.UserSkinTypeScores
                .Where(s => s.SessionId == sessionId)
                .ToListAsync();
                
            if (scores == null || !scores.Any())
                return null;
                
            var maxScore = scores.Max(s => s.Score);
            
            var topSkinTypes = scores.Where(s => s.Score == maxScore).ToList();
            
            if (topSkinTypes.Count > 1)
            {
                return null;
            }
            
            return topSkinTypes.FirstOrDefault()?.SkinTypeId;
        }

        public async Task<List<OptionSkinTypePoints>> GetOptionSkinTypePointsAsync(int optionId)
        {
            return await _context.OptionSkinTypePoints
                .Where(p => p.OptionId == optionId)
                .ToListAsync();
        }

        public async Task<OptionSkinTypePoints> GetOptionSkinTypePointByIdAsync(int id)
        {
            return await _context.OptionSkinTypePoints.FindAsync(id);
        }

        public async Task<OptionSkinTypePoints> AddOptionSkinTypePointsAsync(OptionSkinTypePoints points)
        {
            _context.OptionSkinTypePoints.Add(points);
            await _context.SaveChangesAsync();
            return points;
        }

        public async Task<OptionSkinTypePoints> UpdateOptionSkinTypePointsAsync(OptionSkinTypePoints points)
        {
            _context.Entry(points).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return points;
        }

        public async Task<bool> DeleteOptionSkinTypePointsAsync(int id)
        {
            var points = await _context.OptionSkinTypePoints.FindAsync(id);
            if (points == null) return false;

            _context.OptionSkinTypePoints.Remove(points);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}