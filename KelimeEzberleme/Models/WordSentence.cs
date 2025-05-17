namespace KelimeEzberleme.Models
{
    public class WordSentence
    {
        public int Id { get; set; }
        public int WordId { get; set; }
        public string Sentence { get; set; }

        // Navigation property
        public Word Word { get; set; }
    }

}
