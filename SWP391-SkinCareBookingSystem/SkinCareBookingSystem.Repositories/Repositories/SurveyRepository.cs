using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.Repositories.Interfaces;

namespace SkinCareBookingSystem.Repositories.Repositories
{
    public class SurveyRepository: ISurveyRepository
    {
        private readonly string _filePath = "survey.txt";

        public Dictionary<string, Node> LoadSurvey()
        {
            var surveyTree = new Dictionary<string, Node>();

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
    }
}