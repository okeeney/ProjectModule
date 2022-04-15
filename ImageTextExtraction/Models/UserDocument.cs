using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImageTextExtraction.Models
{
    public class UserDocument
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DocumentId { get; set; }
        public string DocumentTitle { get; set; }
        public string DocumentBody { get; set; }
    }
}
