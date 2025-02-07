using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json.Serialization;

namespace RepositoryLayer.Entities
{
    public class LabelEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LabelId { get; set; }
        public string LabelName { get; set; }

        [ForeignKey("LabelNote")]
        public int NotesId {  get; set; }

        [ForeignKey("LabelUser")]
        public int UserID { get; set; }

        [JsonIgnore]
        public virtual NotesEntity LabelNote { get; set; }

        [JsonIgnore]
        public virtual UserEntity LabelUser { get; set; }
    }
}
