using System.ComponentModel.DataAnnotations;

namespace KelimeEzberleme.Models
{
    public class LetterStatus
    {
        [Key]
        public int Id { get; set; }
        public char Letter { get; set; }
        public string Color { get; set; }
        public int GuessResultId { get; set; }
    }
}
