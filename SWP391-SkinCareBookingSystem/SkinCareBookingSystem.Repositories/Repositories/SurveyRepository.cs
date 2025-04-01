using Microsoft.EntityFrameworkCore;
using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.Repositories.Interfaces;
using SkinCareBookingSystem.Repositories.Data;
using System.IO;

namespace SkinCareBookingSystem.Repositories.Repositories
{
    public class SurveyRepository: ISurveyRepository
    {
        private readonly string _filePath;
        private readonly AppDbContext _context;

        public SurveyRepository(AppDbContext context)
        {
            _context = context;
            
            // Get the base directory and construct the absolute path to the file
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            // Navigate up to the solution root
            string solutionDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "..\\..\\..\\.."));
            _filePath = Path.Combine(solutionDirectory, "SkinCareBookingSystem.Repositories", "SkinTestQuestion.txt");
        }

        public Dictionary<string, Node> LoadSurvey()
        {
            var surveyTree = new Dictionary<string, Node>();

            try
            {
                if (!File.Exists(_filePath))
                {
                    throw new FileNotFoundException($"Survey file not found at path: {_filePath}");
                }

                foreach (string line in File.ReadAllLines(_filePath))
                {
                    var parts = line.Split('|');
                    if (parts.Length < 2) continue;

                    var node = new Node { Id = parts[0], Content = parts[1] };

                    for (int i = 2; i < parts.Length; i++)
                    {
                        var choiceParts = parts[i].Split(':');
                        if (choiceParts.Length == 2)
                        {
                            node.Choices[choiceParts[0]] = choiceParts[1];
                        }
                    }

                    surveyTree[node.Id] = node;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading survey: {ex.Message}");
            }

            return surveyTree;
        }
        public void SaveSurvey(Dictionary<string, Node> surveyTree)
        {
            using (StreamWriter writer = new StreamWriter("survey.txt"))
            {
                foreach (var node in surveyTree.Values)
                {
                    string line = node.Id + "|" + node.Content;

                    foreach (var choice in node.Choices)
                    {
                        line += "|" + choice.Key + ":" + choice.Value;
                    }

                    writer.WriteLine(line);
                }
            }
        }

        public async Task<List<SurveyQuestion>> GetAllQuestionsAsync()
        {
            return await _context.SurveyQuestions
                .Include(q => q.Options)
                .ToListAsync();
        }

        public async Task<SurveyQuestion> GetQuestionByIdAsync(int id)
        {
            return await _context.SurveyQuestions
                .Include(q => q.Options)
                .FirstOrDefaultAsync(q => q.Id == id);
        }

        public async Task<SurveyQuestion> GetQuestionByQuestionIdAsync(string questionId)
        {
            return await _context.SurveyQuestions
                .Include(q => q.Options)
                .FirstOrDefaultAsync(q => q.QuestionId == questionId);
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
                .ToListAsync();
        }

        public async Task<SurveyOption> GetOptionByIdAsync(int id)
        {
            return await _context.SurveyOptions.FindAsync(id);
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

            // Update the properties of the existing result
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

        public async Task<SurveySession> GetSessionByIdAsync(int id)
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
            if (session == null) return null;

            session.SurveyResultId = resultId;
            session.IsCompleted = true;
            session.CompletedDate = DateTime.Now;

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

        public async Task<List<SurveyResponse>> GetResponsesBySessionIdAsync(int sessionId)
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

        public async Task<List<RecommendedService>> GetRecommendedServicesByResultIdAsync(int resultId)
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
            if (service == null) return false;

            _context.RecommendedServices.Remove(service);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}