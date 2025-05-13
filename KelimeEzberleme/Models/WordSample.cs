using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KelimeEzberleme.Models
{
    public class WordSample
    {
        [Key]
        public int WordSamplesID { get; set; }

        [Required]
        public string Samples { get; set; } = string.Empty; // İngilizce ya da Türkçe örnek cümle

        // İlişki
        public int WordID { get; set; }

        [ForeignKey("WordID")]
        public Word? Word { get; set; }
    }
}
