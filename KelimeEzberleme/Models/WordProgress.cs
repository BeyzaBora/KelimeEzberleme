using System.ComponentModel.DataAnnotations;

namespace KelimeEzberleme.Models
{
    public class WordProgress
    {
        public int WordProgressID { get; set; }

        public int UserID { get; set; }

        public int WordID { get; set; }

        public int CorrectCount { get; set; } = 0;  // Varsayılan 0
        public DateTime NextRepeat { get; set; } = DateTime.Now;  // Varsayılan bugünün tarihi
        public DateTime? LastAnswered { get; set; } // Nullable
        public virtual Word Word { get; set; }  // Word tablosuna ilişki
    }

}
