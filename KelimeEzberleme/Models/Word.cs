using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KelimeEzberleme.Models
{
    [Table("WordProgresses")]
    public class Word
    {
        public int WordID { get; set; }

        public string? EngWordName { get; set; }
        
        public string TurWordName { get; set; }

        public bool IsLearned { get; set; }

        public string? Picture { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual ICollection<WordProgress> WordProgresses { get; set; }  // WordProgress ile ilişki
        public virtual ICollection<WordSample> Samples { get; set; }
    }
}