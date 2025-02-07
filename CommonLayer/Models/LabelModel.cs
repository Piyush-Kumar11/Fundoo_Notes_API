using System.ComponentModel.DataAnnotations;

namespace CommonLayer.Models
{
    public class LabelModel
    {
        [Required]
        public string LabelName { get; set; }

        [Required]
        public int NotesId { get; set; }
    }
}
