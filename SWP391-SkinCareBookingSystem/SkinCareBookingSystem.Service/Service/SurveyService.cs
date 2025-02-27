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

        public string StartSurvey()
        {
            var surveyTree = _surveyRepository.LoadSurvey();
            string currentNodeId = "Q1";
            var result = "";

            while (surveyTree.ContainsKey(currentNodeId))
            {
                var node = surveyTree[currentNodeId];
                result += node.Content + "\n";

                if (node.Choices.Count == 0) break;

                foreach (var choice in node.Choices)
                {
                    result += $"- {choice.Key}\n";
                }

                currentNodeId = node.Choices["Có"];
            }

            if (surveyTree.ContainsKey(currentNodeId))
            {
                result += surveyTree[currentNodeId].Content;
            }

            return result;
        }
    }

}
