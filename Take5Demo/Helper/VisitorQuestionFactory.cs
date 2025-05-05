using System.Collections.ObjectModel;
using Take5Demo.Model;

namespace Take5Demo.Helper
{
    public class VisitorQuestionFactory
    {
        public static ObservableCollection<Question> CreateVisitorQuestionsFromConfig(ConfigModel config)
        {
            var questions = new ObservableCollection<Question>();

            if (config != null)
            {
                try
                {
                    var siteCrewFeature = config.DataLists?.FirstOrDefault(d => d.FeatureKey == "site-crew");
                    var visitorGroup = siteCrewFeature?.QuestionSet?.QuestionGroups?.FirstOrDefault(g => g.Name.Contains("Visitor"));

                    if (visitorGroup?.Questions != null && visitorGroup.Questions.Count > 0)
                    {
                        int index = 0;
                        foreach (var sourceQuestion in visitorGroup.Questions)
                        {
                            var questionCopy = new Question
                            {
                                Name = sourceQuestion.Name,
                                Description = sourceQuestion.Description ?? "",
                                ResponseType = sourceQuestion.ResponseType,
                                IsMandatoryValue = sourceQuestion.IsMandatoryValue,
                                Value = sourceQuestion.Value != null ? new List<string>(sourceQuestion.Value) : new List<string>(),
                                Logic = sourceQuestion.Logic,
                                Answer = null, 
                                Index = index++
                            };

                            questions.Add(questionCopy);
                        }

                        return questions;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating visitor questions from config: {ex.Message}");
                }
            }

            return CreateDefaultVisitorQuestions();
        }

        private static ObservableCollection<Question> CreateDefaultVisitorQuestions()
        {
            var defaultQuestions = new ObservableCollection<Question>
            {
                new Question
                {
                    Name = "Visitor Name",
                    Description = "Enter the full name of the visitor",
                    ResponseType = "Text",
                    IsMandatoryValue = 1,
                    Value = new List<string>(),
                    Index = 0
                },
                new Question
                {
                    Name = "Visitor Company",
                    Description = "Enter the visitor's company name",
                    ResponseType = "Text",
                    IsMandatoryValue = 1,
                    Value = new List<string>(),
                    Index = 1
                },
                new Question
                {
                    Name = "Reason for Visit",
                    Description = "Select the reason for this visit",
                    ResponseType = "Picker",
                    IsMandatoryValue = 1,
                    Value = new List<string>
                    {
                        "Hot Work Monitoring",
                        "Site Inspection",
                        "Delivery",
                        "Consultation",
                        "Maintenance",
                        "Other"
                    },
                    Index = 2
                },
                new Question
                {
                    Name = "Site Visitor Signature",
                    Description = "I confirm that I have been briefed on site hazards and emergency procedures",
                    ResponseType = "Signature",
                    IsMandatoryValue = 1,
                    Value = new List<string>(),
                    Index = 3
                }
            };

            return defaultQuestions;
        }

        public static void SyncVisitorDataToQuestions(Visitor visitor)
        {
            if (visitor.Questions == null || visitor.Questions.Count == 0)
            {
                visitor.Questions = CreateDefaultVisitorQuestions();
            }

            foreach (var question in visitor.Questions)
            {
                if (question.Name.Contains("Visitor Name"))
                {
                    question.Answer = visitor.VisitorName;
                }
                else if (question.Name.Contains("Visitor Company"))
                {
                    question.Answer = visitor.VisitorCompany;
                }
                else if (question.Name.Contains("Reason for Visit"))
                {
                    question.Answer = visitor.VisitReason;
                }
                else if (question.Name.Contains("Signature"))
                {
                    question.Answer = visitor.SignatureBase64;
                }
            }
        }
        public static void SyncQuestionsToVisitorData(Visitor visitor)
        {
            if (visitor.Questions == null) return;

            foreach (var question in visitor.Questions)
            {
                if (question.Name.Contains("Visitor Name"))
                {
                    visitor.VisitorName = question.Answer;
                }
                else if (question.Name.Contains("Visitor Company"))
                {
                    visitor.VisitorCompany = question.Answer;
                }
                else if (question.Name.Contains("Reason for Visit"))
                {
                    visitor.VisitReason = question.Answer;
                }
                else if (question.Name.Contains("Signature"))
                {
                    visitor.SignatureBase64 = question.SignatureImageSource;
                    visitor.HasSignature = question.HasSignature;
                }
            }
        }

        public static Visitor CreateVisitorWithQuestions(ConfigModel config)
        {
            var visitor = new Visitor
            {
                VisitorName = "",
                VisitorCompany = "",
                VisitReason = null,
                HasSignature = false,
                SignatureBase64 = null,
                SignaturePadId = Guid.NewGuid().ToString(),
                Questions = CreateVisitorQuestionsFromConfig(config)
            };

            return visitor;
        }
    }
}
