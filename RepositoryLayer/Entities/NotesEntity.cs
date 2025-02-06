using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text;
using RepositoryLayer.Migrations;
using System.Text.Json.Serialization;

namespace RepositoryLayer.Entities
{
    public class NotesEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int NotesId { get; set; }
        public string Title { get; set; }

        public string Description { get; set; }

        public string Color { get; set; }

        public DateTime Remainder { get; set; }

        public string Image { get; set; }

        public bool IsArchive { get; set; }

        public bool IsTrash { get; set; }

        public bool IsPin { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }

        [ForeignKey("NotesUser")]
        public int UserID { get; set; }
        [JsonIgnore]
        public virtual UserEntity NotesUser { get; set; }
    }
}