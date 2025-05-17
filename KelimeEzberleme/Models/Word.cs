using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KelimeEzberleme.Models
{
    
        [Table("Words")]  // ✅ Doğru tablo adı
        public class Word
        {
            public int WordID { get; set; }

            public string? EngWordName { get; set; }

            public string TurWordName { get; set; }

            public bool IsLearned { get; set; }

            public string? Picture { get; set; }

            public DateTime CreatedAt { get; set; }

            public virtual ICollection<WordProgress> WordProgresses { get; set; }

            public virtual ICollection<WordSample> Samples { get; set; }

            public int? KategoriID { get; set; } // ✅ Veritabanında null olmasını istemiyorsan "int" yap (nullable olmasın)

            public virtual Kategori Kategori { get; set; }
        public ICollection<WordSentence> Sentences { get; set; }
    }
    }
