namespace CSharpQuizBlazor.Models
{
    public class ExamQuestion
    {
        public int ExamQuestionId { get; set; }
        public int ExamId { get; set; }
        public int QuestionId { get; set; }
        public int OrderIndex { get; set; }
    }
}
