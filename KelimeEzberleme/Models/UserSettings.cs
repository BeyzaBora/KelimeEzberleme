

using System.ComponentModel.DataAnnotations;

namespace KelimeEzberleme.Models
{ 
    public class UserSettings
{
        [Key]
        public int UserID { get; set; }
        public string UserName { get; set; }
        public int WordCount { get; set; }

        // Kullanıcı ile ilişkilendirme
        public User? User { get; set; }
}
}