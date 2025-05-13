namespace KelimeEzberleme.Models
{
    public class InteractiveQuizViewModel
    {
        public int CurrentQuestionIndex { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectCount { get; set; }
        public int IncorrectCount { get; set; }

        public int WordID { get; set; }
        public string? EngWordName { get; set; }
        public string? TurWordName { get; set; }
        public string? Picture { get; set; }

        public List<WordSample> Samples { get; set; }  // İngilizce cümleleri tutacak
        public string? UserAnswer { get; set; }
        public bool? IsCorrect { get; set; }
    }
}
