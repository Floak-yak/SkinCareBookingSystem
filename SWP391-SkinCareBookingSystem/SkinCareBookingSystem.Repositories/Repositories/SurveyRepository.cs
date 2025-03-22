using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.Repositories.Interfaces;

namespace SkinCareBookingSystem.Repositories.Repositories
{
    public class SurveyRepository : ISurveyRepository
    {
        private readonly string _filePath;

        public SurveyRepository(string filePath = "SkinTestQuestion.txt")
        {
            // Try to find the file in multiple possible locations
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string projectDirectory = Directory.GetCurrentDirectory();
            
            // Possible file paths in order of preference
            var possiblePaths = new[]
            {
                Path.Combine(baseDirectory, filePath),                                  // bin directory
                Path.Combine(projectDirectory, filePath),                               // current directory
                Path.Combine(projectDirectory, "SkinCareBookingSystem.Repositories", filePath), // project repository folder
                Path.GetFullPath(Path.Combine(baseDirectory, "..", "..", "..", "SkinCareBookingSystem.Repositories", filePath)) // try going up from bin
            };
            
            foreach (var path in possiblePaths)
            {
                if (File.Exists(path))
                {
                    _filePath = path;
                    Console.WriteLine($"Found survey file at: {_filePath}");
                    return;
                }
            }
            
            // Default to the first option if none found
            _filePath = possiblePaths[0];
            Console.WriteLine($"Could not find survey file. Will try to use: {_filePath}");
        }

        public Dictionary<string, SurveyQuestion> LoadSurvey()
        {
            var surveyTree = new Dictionary<string, SurveyQuestion>();

            try
            {
                if (!File.Exists(_filePath))
                {
                    // For debugging purposes, log all the places we looked
                    Console.WriteLine($"Survey file not found at: {_filePath}");
                    Console.WriteLine($"Current directory: {Directory.GetCurrentDirectory()}");
                    Console.WriteLine($"Base directory: {AppDomain.CurrentDomain.BaseDirectory}");
                    
                    throw new FileNotFoundException($"Survey file not found at: {_filePath}");
                }

                string content = File.ReadAllText(_filePath);
                
                // Parse the structure using regex
                var questionMatches = Regex.Matches(content, @"(\w+):\s*\{([^{}]+(?:\{[^{}]*\}[^{}]*)*)\}", RegexOptions.Singleline);
                
                foreach (Match questionMatch in questionMatches)
                {
                    string questionId = questionMatch.Groups[1].Value.Trim();
                    string questionContent = questionMatch.Groups[2].Value.Trim();
                    
                    // Extract the question text - now handling concatenated strings
                    string questionText = ExtractQuestionText(questionContent);
                    if (string.IsNullOrEmpty(questionText)) continue;
                    
                    // Create the question object
                    var question = new SurveyQuestion
                    {
                        Id = questionId,
                        Question = questionText,
                        Options = new List<SurveyOption>()
                    };
                    
                    // Extract the options
                    var optionsMatch = Regex.Match(questionContent, @"options:\s*\[(.*?)\]", RegexOptions.Singleline);
                    if (optionsMatch.Success)
                    {
                        string optionsText = optionsMatch.Groups[1].Value;
                        
                        // Parse each option
                        var optionMatches = Regex.Matches(optionsText, @"\{([^{}]+)\}", RegexOptions.Singleline);
                        foreach (Match optionMatch in optionMatches)
                        {
                            string optionContent = optionMatch.Groups[1].Value;
                            
                            // Extract label
                            var labelMatch = Regex.Match(optionContent, @"label:\s*""([^""]+)""");
                            if (!labelMatch.Success) continue;
                            
                            string label = labelMatch.Groups[1].Value;
                            
                            // Extract nextId
                            var nextIdMatch = Regex.Match(optionContent, @"nextId:\s*""([^""]+)""");
                            string nextId = nextIdMatch.Success ? nextIdMatch.Groups[1].Value : "END";
                            
                            question.Options.Add(new SurveyOption
                            {
                                Label = label,
                                NextId = nextId
                            });
                        }
                    }
                    
                    surveyTree[questionId] = question;
                }
                
                Console.WriteLine($"Successfully loaded {surveyTree.Count} questions from survey file.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading survey: {ex.Message}");
                // Return an empty dictionary in case of error
                return new Dictionary<string, SurveyQuestion>();
            }

            return surveyTree;
        }

        // Helper method to extract and concatenate multi-line question text
        private string ExtractQuestionText(string content)
        {
            // Check if the question section exists
            if (!content.Contains("question:")) return null;
            
            // Handle concatenated multi-line question
            string questionSection = content.Substring(content.IndexOf("question:"));
            // Cut at options if exists
            if (questionSection.Contains("options:"))
            {
                questionSection = questionSection.Substring(0, questionSection.IndexOf("options:"));
            }
            
            // Check if it contains "+" which indicates concatenation
            bool isMultiLine = questionSection.Contains("+");
            
            if (!isMultiLine)
            {
                // Simple case - single line question
                var singleLineMatch = Regex.Match(questionSection, @"question:\s*""([^""]+)""", RegexOptions.Singleline);
                if (singleLineMatch.Success)
                {
                    return singleLineMatch.Groups[1].Value;
                }
            }
            
            // Extract each quoted string and concatenate
            var stringMatches = Regex.Matches(questionSection, @"""([^""\\]*(?:\\.[^""\\]*)*)""");
            if (stringMatches.Count == 0) return null;
            
            StringBuilder result = new StringBuilder();
            foreach (Match m in stringMatches)
            {
                result.Append(m.Groups[1].Value);
            }
            
            return result.ToString();
        }

        public void SaveSurvey(Dictionary<string, SurveyQuestion> surveyTree)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(_filePath))
                {
                    foreach (var entry in surveyTree)
                    {
                        writer.WriteLine($"{entry.Key}: {{");
                        writer.WriteLine($"  question: \"{entry.Value.Question}\",");
                        writer.WriteLine("  options: [");
                        
                        for (int i = 0; i < entry.Value.Options.Count; i++)
                        {
                            var option = entry.Value.Options[i];
                            writer.WriteLine($"    {{");
                            writer.WriteLine($"      label: \"{option.Label}\",");
                            writer.WriteLine($"      nextId: \"{option.NextId}\"");
                            writer.Write($"    }}");
                            
                            if (i < entry.Value.Options.Count - 1)
                            {
                                writer.WriteLine(",");
                            }
                            else
                            {
                                writer.WriteLine();
                            }
                        }
                        
                        writer.WriteLine("  ]");
                        writer.WriteLine("},");
                    }
                }
                
                Console.WriteLine($"Successfully saved {surveyTree.Count} questions to survey file at: {_filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving survey: {ex.Message}");
            }
        }
    }
}