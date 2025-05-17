using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KelimeEzberleme.ViewModels
{
    public class WordAddViewModel

    {
        public int WordID { get; set; }
        [Required]
        public string EngWordName { get; set; } = string.Empty;

        [Required]
        public string TurWordName { get; set; } = string.Empty;

        public IFormFile? Image { get; set; }
        public string? Kategori { get; set; }

        [MinLength(2, ErrorMessage = "En az 2 örnek cümle eklemelisiniz.")]
        public List<string> Samples { get; set; } = new List<string>();
        public int? KategoriID { get; set; } // Bunu ekle
        public List<SelectListItem> CategoryList { get; set; } = new List<SelectListItem>();

        public int SelectedCategoryID { get; set; }
    }
}
