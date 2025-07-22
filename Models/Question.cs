using System.ComponentModel.DataAnnotations;

namespace CSharpQuizBlazor.Models
{
    public class Question
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 10)]
        public string QuestionText { get; set; } = string.Empty;

        [Required]
        public QuestionType Type { get; set; } = QuestionType.MultipleChoice;

        // For Multiple Choice and True/False questions
        [StringLength(200)]
        public string? Option1 { get; set; }

        [StringLength(200)]
        public string? Option2 { get; set; }

        [StringLength(200)]
        public string? Option3 { get; set; }

        [StringLength(200)]
        public string? Option4 { get; set; }

        // Correct answer index for Multiple Choice/True-False (1-based indexing)
        [Range(1, 4)]
        public int? CorrectAnswer { get; set; }

        // For text-based answers (OneWord, ShortAnswer, LongAnswer)
        [StringLength(1000)]
        public string? CorrectTextAnswer { get; set; }

        // For comprehension questions - reading passage
        [StringLength(2000)]
        public string? Passage { get; set; }

        // Scoring and difficulty
        [Range(1, 10)]
        public int Points { get; set; } = 1;

        [Range(1, 5)]
        public int DifficultyLevel { get; set; } = 1;

        // Time limit per question (in seconds) - optional
        [Range(10, 600)]
        public int? TimeLimit { get; set; }

        // Category/Subject classification
        [StringLength(100)]
        public string? Category { get; set; }

        [StringLength(100)]
        public string? Subject { get; set; }

        // Metadata
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? LastModified { get; set; }

        [StringLength(100)]
        public string? CreatedBy { get; set; }

        // Explanation for the correct answer
        [StringLength(1000)]
        public string? Explanation { get; set; }

        // Tags for better organization
        public string? Tags { get; set; } // Comma-separated tags

        // Active/Inactive status
        public bool IsActive { get; set; } = true;

        // Validation methods
        public bool IsValidMultipleChoice()
        {
            return Type == QuestionType.MultipleChoice &&
                   !string.IsNullOrWhiteSpace(Option1) &&
                   !string.IsNullOrWhiteSpace(Option2) &&
                   CorrectAnswer.HasValue &&
                   CorrectAnswer.Value >= 1 &&
                   CorrectAnswer.Value <= GetOptionCount();
        }

        public bool IsValidTrueFalse()
        {
            return Type == QuestionType.TrueFalse &&
                   CorrectAnswer.HasValue &&
                   (CorrectAnswer.Value == 1 || CorrectAnswer.Value == 2);
        }

        public bool IsValidTextAnswer()
        {
            return (Type == QuestionType.OneWord ||
                    Type == QuestionType.ShortAnswer ||
                    Type == QuestionType.LongAnswer) &&
                   !string.IsNullOrWhiteSpace(CorrectTextAnswer);
        }

        public bool IsValidComprehension()
        {
            return Type == QuestionType.Comprehension &&
                   !string.IsNullOrWhiteSpace(Passage) &&
                   IsValidMultipleChoice();
        }

        // Helper methods
        public int GetOptionCount()
        {
            int count = 0;
            if (!string.IsNullOrWhiteSpace(Option1)) count++;
            if (!string.IsNullOrWhiteSpace(Option2)) count++;
            if (!string.IsNullOrWhiteSpace(Option3)) count++;
            if (!string.IsNullOrWhiteSpace(Option4)) count++;
            return count;
        }

        public string GetOptionText(int optionIndex)
        {
            return optionIndex switch
            {
                0 => Option1 ?? "",
                1 => Option2 ?? "",
                2 => Option3 ?? "",
                3 => Option4 ?? "",
                _ => ""
            };
        }

        public List<string> GetAllOptions()
        {
            var options = new List<string>();
            if (!string.IsNullOrWhiteSpace(Option1)) options.Add(Option1);
            if (!string.IsNullOrWhiteSpace(Option2)) options.Add(Option2);
            if (!string.IsNullOrWhiteSpace(Option3)) options.Add(Option3);
            if (!string.IsNullOrWhiteSpace(Option4)) options.Add(Option4);
            return options;
        }

        public bool ValidateAnswer(object userAnswer)
        {
            return Type switch
            {
                QuestionType.MultipleChoice or QuestionType.Comprehension =>
                    userAnswer is int intAnswer && intAnswer == (CorrectAnswer - 1),

                QuestionType.TrueFalse =>
                    userAnswer is int boolAnswer && boolAnswer == (CorrectAnswer - 1),

                QuestionType.OneWord or QuestionType.ShortAnswer or QuestionType.LongAnswer =>
                    userAnswer is string textAnswer &&
                    string.Equals(textAnswer.Trim(), CorrectTextAnswer?.Trim(), StringComparison.OrdinalIgnoreCase),

                _ => false
            };
        }

        public string GetCorrectAnswerDisplay()
        {
            return Type switch
            {
                QuestionType.MultipleChoice or QuestionType.Comprehension =>
                    GetOptionText((CorrectAnswer ?? 1) - 1),

                QuestionType.TrueFalse =>
                    (CorrectAnswer == 1) ? "True" : "False",

                _ => CorrectTextAnswer ?? "No answer provided"
            };
        }

        public string GetQuestionTypeDisplay()
        {
            return Type switch
            {
                QuestionType.MultipleChoice => "Multiple Choice",
                QuestionType.TrueFalse => "True/False",
                QuestionType.OneWord => "One Word Answer",
                QuestionType.Comprehension => "Reading Comprehension",
                QuestionType.ShortAnswer => "Short Answer",
                QuestionType.LongAnswer => "Long Answer",
                _ => Type.ToString()
            };
        }

        // Get tags as a list
        public List<string> GetTagsList()
        {
            if (string.IsNullOrWhiteSpace(Tags))
                return new List<string>();

            return Tags.Split(',', StringSplitOptions.RemoveEmptyEntries)
                      .Select(tag => tag.Trim())
                      .Where(tag => !string.IsNullOrWhiteSpace(tag))
                      .ToList();
        }

        // Set tags from a list
        public void SetTags(List<string> tagsList)
        {
            Tags = tagsList != null && tagsList.Any()
                ? string.Join(", ", tagsList.Where(tag => !string.IsNullOrWhiteSpace(tag)))
                : null;
        }

        // Calculate estimated time to answer based on question type and difficulty
        public int GetEstimatedAnswerTime()
        {
            if (TimeLimit.HasValue)
                return TimeLimit.Value;

            return Type switch
            {
                QuestionType.MultipleChoice => 30 + (DifficultyLevel * 10),
                QuestionType.TrueFalse => 15 + (DifficultyLevel * 5),
                QuestionType.OneWord => 20 + (DifficultyLevel * 10),
                QuestionType.Comprehension => 90 + (DifficultyLevel * 30),
                QuestionType.ShortAnswer => 120 + (DifficultyLevel * 60),
                QuestionType.LongAnswer => 300 + (DifficultyLevel * 120),
                _ => 60
            };
        }

        // Clone method for creating similar questions
        public Question Clone()
        {
            return new Question
            {
                QuestionText = QuestionText,
                Type = Type,
                Option1 = Option1,
                Option2 = Option2,
                Option3 = Option3,
                Option4 = Option4,
                CorrectAnswer = CorrectAnswer,
                CorrectTextAnswer = CorrectTextAnswer,
                Passage = Passage,
                Points = Points,
                DifficultyLevel = DifficultyLevel,
                TimeLimit = TimeLimit,
                Category = Category,
                Subject = Subject,
                Explanation = Explanation,
                Tags = Tags,
                IsActive = IsActive,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = CreatedBy
            };
        }
    }
}

