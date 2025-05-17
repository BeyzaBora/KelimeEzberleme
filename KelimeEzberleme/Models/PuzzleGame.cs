namespace KelimeEzberleme.Models
{
    public class PuzzleGame
    {
        public string HiddenWord { get; set; } = string.Empty;
        public List<string> Guesses { get; set; } = new List<string>();
        public List<GuessResult> GuessResults { get; set; } = new List<GuessResult>();
        public int MaxGuesses { get; set; } = 6;
        public bool IsFinished { get; set; } = false;
        public bool IsWon { get; set; } = false;
    }


}
