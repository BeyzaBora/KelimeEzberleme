using System;
using System.ComponentModel.DataAnnotations;

namespace KelimeEzberleme.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }
        public string? FullName { get; set; }

        [Required, MaxLength(255)]
        public string? UserName { get; set; }

        [Required, EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string? Password { get; set; }

        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;
    }
}
