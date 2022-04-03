using System.ComponentModel.DataAnnotations;

namespace ImageTextExtraction.Models
{
    public class UserDocument
    {
        [Key]
        public int DocumentId { get; set; }
        public string DocumentTitle { get; set; }
        public string DocumentBody { get; set; }
    }
}
