using System.ComponentModel.DataAnnotations;

namespace CommonLayer.Models
{
    public class CollaboratorModel
    {
        [Required]
        public string CollaboratorEmail { get; set; }

        [Required]
        public int NotesId { get; set; }
    }
}
