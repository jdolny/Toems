using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Toems_Common.Enum;

namespace Toems_Common.Entity
{
    [Table("images")]
    public class EntityImage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("image_id")]
        public int Id { get; set; }

        [Column("image_description")]
        public string Description { get; set; }

        [Column("image_enabled")]
        public bool Enabled { get; set; }

        [Column("image_environment")]
        public string Environment { get; set; }

       
        [Column("image_is_viewable_ond")]
        public bool IsVisible { get; set; }

        [Column("last_upload_guid")]
        public string LastUploadGuid { get; set; }

        [Column("image_name")]
        public string Name { get; set; }

        [Column("image_is_protected")]
        public bool Protected { get; set; }

        [Column("image_type")]
        public string Type { get; set; }

        [Column("replication_mode")]
        public EnumImageReplication.ReplicationType ReplicationMode { get; set; }
    }

    [NotMapped]
    public class ImageWithDate : EntityImage
    {
        public DateTime? LastUsed { get; set; }
    }
}
