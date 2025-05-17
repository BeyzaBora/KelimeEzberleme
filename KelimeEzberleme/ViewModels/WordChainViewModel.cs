using System.Collections.Generic;
using KelimeEzberleme.Models;

namespace KelimeEzberleme.ViewModels
{
    public class WordChainViewModel
    {
        public List<string> Sentences { get; set; }
        public List<Word> LearnedWords { get; set; }

        public string SelectedSentencesParagraph { get; set; }
        public List<int> SelectedWordIds { get; set; } = new List<int>();

        public List<Word> AvailableWords => LearnedWords == null
            ? new List<Word>()
            : LearnedWords.Where(w => !SelectedWordIds.Contains(w.WordID)).ToList();
        public string ImagePath { get; set; }
    }
}
