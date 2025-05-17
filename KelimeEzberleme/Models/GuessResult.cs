using System.ComponentModel.DataAnnotations;

namespace KelimeEzberleme.Models
{
    public class GuessResult
    {
        [Key]
        public int Id { get; set; }
        public string Guess { get; set; }
        public List<LetterStatus> Letters { get; set; } = new List<LetterStatus>();
    }
}


