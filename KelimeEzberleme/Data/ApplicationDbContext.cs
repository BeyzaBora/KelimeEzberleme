using Microsoft.EntityFrameworkCore;
using KelimeEzberleme.Models;

namespace KelimeEzberleme.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Word> Words { get; set; }
        public DbSet<WordSample> WordSamples { get; set; }
        public DbSet<WordProgress> WordProgresses { get; set; }
        public DbSet<UserSettings> UserSettings { get; set; }
        public DbSet<Kategori> Kategoriler { get; set; }
        public DbSet<LetterStatus> LetterStatuses { get; set; }
        public DbSet<GuessResult> GuessResults { get; set; }
        public DbSet<WordSentence> WordSentences { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Kategoriler ekleniyor
            modelBuilder.Entity<Kategori>().HasData(
     new Kategori { KategoriID = 1, KategoriAd = "Meyve" },
     new Kategori { KategoriID = 2, KategoriAd = "Sebze" },
     new Kategori { KategoriID = 3, KategoriAd = "Nesne" },
     new Kategori { KategoriID = 4, KategoriAd = "Bitki" },
     new Kategori { KategoriID = 5, KategoriAd = "Özne" },
     new Kategori { KategoriID = 6, KategoriAd = "Hayvan" },
     new Kategori { KategoriID = 7, KategoriAd = "Gıda" },
     new Kategori { KategoriID = 8, KategoriAd = "Atmosfer" },
      new Kategori { KategoriID = 9, KategoriAd = "Fiil" }
 ); // <- Noktalı virgül burada olacak
            {
                modelBuilder.Entity<Word>()
                    .HasMany(w => w.Sentences)
                    .WithOne(s => s.Word)
                    .HasForeignKey(s => s.WordId)
                    .OnDelete(DeleteBehavior.Cascade);
            }


            // İlişkiler
            modelBuilder.Entity<WordProgress>()
                .HasOne(wp => wp.Word)
                .WithMany(w => w.WordProgresses)
                .HasForeignKey(wp => wp.WordID);

            modelBuilder.Entity<WordSample>()
                .HasOne(ws => ws.Kategori)
                .WithMany()
                .HasForeignKey(ws => ws.KategoriID)
                .IsRequired(false);

            // GuessResult ile LetterStatus arasındaki ilişki
            modelBuilder.Entity<GuessResult>()
                .HasMany(gr => gr.Letters)
                .WithOne()
                .HasForeignKey(ls => ls.GuessResultId)
                .IsRequired();

            modelBuilder.Entity<LetterStatus>()
                .Property<int>("GuessResultId");

            // Tablo isimleri
            modelBuilder.Entity<Word>().ToTable("Words");
            modelBuilder.Entity<WordProgress>().ToTable("WordProgresses");
            modelBuilder.Entity<Kategori>().ToTable("Kategoriler");
            modelBuilder.Entity<GuessResult>().ToTable("GuessResults");
            modelBuilder.Entity<LetterStatus>().ToTable("LetterStatuses");

            // Kategori adı unique
            modelBuilder.Entity<Kategori>()
                .HasIndex(k => k.KategoriAd)
                .IsUnique();
       
    }

    }
}

