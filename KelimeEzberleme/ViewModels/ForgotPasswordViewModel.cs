using System.ComponentModel.DataAnnotations;

namespace KelimeEzberleme.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }

}
