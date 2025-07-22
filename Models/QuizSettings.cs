using System.ComponentModel.DataAnnotations;

namespace CSharpQuizBlazor.Models
{
    public class QuizSettings
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string QuizTitle { get; set; } = "C# Programming Quiz";

        [Range(5, 50)]
        public int NumberOfQuestions { get; set; } = 10;

        [Range(5, 180)]
        public int TimeLimit { get; set; } = 30; // minutes

        public List<QuestionType> AllowedTypes { get; set; } = new List<QuestionType>
        {
            QuestionType.MultipleChoice
        };

        [Range(1, 10)]
        public int DifficultyLevel { get; set; } = 1;

        public bool ShuffleQuestions { get; set; } = true;

        public bool ShowCorrectAnswers { get; set; } = true;

        public bool AllowReview { get; set; } = true;

        public bool ShowTimer { get; set; } = true;

        // Calculated properties
        public int TotalTimeInSeconds => TimeLimit * 60;

        public bool HasMultipleQuestionTypes => AllowedTypes.Count > 1;

        public string FormattedTimeLimit => TimeLimit < 60
            ? $"{TimeLimit} minutes"
            : $"{TimeLimit / 60}h {TimeLimit % 60}m";

        // Validation method
        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(QuizTitle) &&
                   NumberOfQuestions > 0 &&
                   TimeLimit > 0 &&
                   AllowedTypes.Any();
        }

        // Default configurations
        public static QuizSettings CreateQuickQuiz()
        {
            return new QuizSettings
            {
                QuizTitle = "Quick C# Quiz",
                NumberOfQuestions = 5,
                TimeLimit = 10,
                AllowedTypes = new List<QuestionType> { QuestionType.MultipleChoice, QuestionType.TrueFalse }
            };
        }

        public static QuizSettings CreateStandardQuiz()
        {
            return new QuizSettings
            {
                QuizTitle = "Standard C# Quiz",
                NumberOfQuestions = 15,
                TimeLimit = 30,
                AllowedTypes = new List<QuestionType>
                {
                    QuestionType.MultipleChoice,
                    QuestionType.TrueFalse,
                    QuestionType.OneWord
                }
            };
        }

        public static QuizSettings CreateComprehensiveQuiz()
        {
            return new QuizSettings
            {
                QuizTitle = "Comprehensive C# Assessment",
                NumberOfQuestions = 25,
                TimeLimit = 60,
                AllowedTypes = Enum.GetValues<QuestionType>().ToList(),
                DifficultyLevel = 3
            };
        }
    }
}
