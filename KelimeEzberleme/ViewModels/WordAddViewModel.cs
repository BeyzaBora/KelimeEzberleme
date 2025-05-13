using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KelimeEzberleme.ViewModels
{
    public class WordAddViewModel
    {
        [Required]
        public string EngWordName { get; set; } = string.Empty;

        [Required]
        public string TurWordName { get; set; } = string.Empty;

        public IFormFile? Image { get; set; }

        [MinLength(2, ErrorMessage = "En az 2 örnek cümle eklemelisiniz.")]
        public List<string> Samples { get; set; } = new List<string>();
    }
}
