using System.ComponentModel.DataAnnotations;

namespace ImageTextExtraction.Models
{
    public class FileUploadFormModel
    {
        [Required]
        [Display(Name="File")]
        public IFormFile FormFile { get; set; }
    }
}
