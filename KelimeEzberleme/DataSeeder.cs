using System.Collections.Generic;
using System.Linq;
using KelimeEzberleme.Data;
using KelimeEzberleme.Models;

namespace KelimeEzberleme.Data
{
    public class DataSeeder
    {
        private readonly ApplicationDbContext _context;

        public DataSeeder(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Seed()
        {
            bool wordsExist = _context.Words.Any();
            bool sentencesExist = _context.WordSentences.Any();

            if (!wordsExist)
            {
                var wordList = new List<Word>
                {
                    new Word
                    {
                        EngWordName = "apple",
                        TurWordName = "elma",
                        Sentences = new List<WordSentence>
                        {
                            new WordSentence { Sentence = "Çocuk ormanda yürürken bir apple ağacı ile karşılaştı." }
                        }
                    },
                    new Word
                    {
                        EngWordName = "dog",
                        TurWordName = "köpek",
                        Sentences = new List<WordSentence>
                        {
                            new WordSentence { Sentence = "Çocuk dogunu gezdirmeye ormana gitmişti." }
                        }
                    },
                    new Word
                    {
                        EngWordName = "house",
                        TurWordName = "ev",
                        Sentences = new List<WordSentence>
                        {
                            new WordSentence { Sentence = "Çocuk housedan çok uzaktaydı." }
                        }
                    },
                    new Word
                    {
                        EngWordName = "moon",
                        TurWordName = "ay",
                        Sentences = new List<WordSentence>
                        {
                            new WordSentence { Sentence = "Moon çok güzel parlıyordu." }
                        }
                    },
                    new Word
                    {
                        EngWordName = "rain",
                        TurWordName = "yağmur",
                        Sentences = new List<WordSentence>
                        {
                            new WordSentence { Sentence = "Rain yapma ihtimali yüksekti." }
                        }
                    },
                    new Word
                    {
                        EngWordName = "glasses",
                        TurWordName = "gözlük",
                        Sentences = new List<WordSentence>
                        {
                            new WordSentence { Sentence = "Gözlüklerini takmadığı için net göremiyordu." }
                        }
                    }
                };

                _context.Words.AddRange(wordList);
                _context.SaveChanges();
            }
            else if (!sentencesExist)
            {
                // Cümleleri ayrı ekleme, kelimelere bağlı şekilde kontrol et
                AddSentenceIfNotExists("apple", "Çocuk ormanda yürürken bir apple ağacı ile karşılaştı.");
                AddSentenceIfNotExists("dog", "Çocuk dogunu gezdirmeye ormana gitmişti.");
                AddSentenceIfNotExists("house", "Çocuk housedan çok uzaktaydı.");
                AddSentenceIfNotExists("moon", "Moon çok güzel parlıyordu.");
                AddSentenceIfNotExists("rain", "Rain yapma ihtimali yüksekti.");
                AddSentenceIfNotExists("glasses", "Gözlüklerini takmadığı için net göremiyordu.");

                _context.SaveChanges();
            }
        }

        private void AddSentenceIfNotExists(string engWord, string sentence)
        {
            var word = _context.Words.FirstOrDefault(w => w.EngWordName == engWord);
            if (word != null && !_context.WordSentences.Any(ws => ws.WordId == word.WordID && ws.Sentence == sentence))
            {
                _context.WordSentences.Add(new WordSentence
                {
                    WordId = word.WordID,
                    Sentence = sentence
                });
            }
        }
    }
}
