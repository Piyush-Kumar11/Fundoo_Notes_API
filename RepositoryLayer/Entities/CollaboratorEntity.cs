using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace RepositoryLayer.Entities
{
    public class CollaboratorEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CollaboratorId { get; set; }

        [Required]
        public string CollaboratorEmail { get; set; }

        [ForeignKey("Notes")]
        public int NotesId { get; set; }

        [ForeignKey("Users")]
        public int UserID { get; set; }

        [JsonIgnore]
        public virtual NotesEntity Notes { get; set; }

        [JsonIgnore]
        public virtual UserEntity Users { get; set; }
    }
}
