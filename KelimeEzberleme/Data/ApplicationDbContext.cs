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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // WordProgress ve Word arasındaki ilişkiyi belirliyoruz
            modelBuilder.Entity<WordProgress>()
                .HasOne(wp => wp.Word)
                .WithMany(w => w.WordProgresses)
                .HasForeignKey(wp => wp.WordID);

            // Tablo adlarını açıkça belirtiyoruz (eğer tablo adı farklıysa)
            modelBuilder.Entity<Word>().ToTable("Words");
            modelBuilder.Entity<WordProgress>().ToTable("WordProgresses");  // Tablo adı burada belirtiliyor
        }

    }
}

