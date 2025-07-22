using System.ComponentModel.DataAnnotations;

namespace CSharpQuizBlazor.Models
{
    public class Exam
    {
        public int ExamId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        public int NumberOfQuestions { get; set; } = 10;

        public int TimeLimit { get; set; } = 30; // in minutes

        public string AllowedQuestionTypes { get; set; } = "MultipleChoice,TrueFalse,OneWord,ShortAnswer";

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public string CreatedBy { get; set; } = "Admin";

        public bool IsActive { get; set; } = true;

        // Helper methods
        public List<QuestionType> GetAllowedQuestionTypes()
        {
            if (string.IsNullOrEmpty(AllowedQuestionTypes))
                return new List<QuestionType>();

            return AllowedQuestionTypes.Split(',')
                .Select(type => Enum.Parse<QuestionType>(type.Trim()))
                .ToList();
        }

        public void SetAllowedQuestionTypes(List<QuestionType> types)
        {
            AllowedQuestionTypes = string.Join(",", types);
        }
    }
}
