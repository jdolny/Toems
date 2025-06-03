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
        [Column("image_id")] public int Id { get; set; }
        [Column("image_description")] public string Description { get; set; } = null;
        [Column("image_enabled")] public bool Enabled { get; set; } = true;
        [Column("image_environment")] public string Environment { get; set; } = null;
        [Column("image_is_viewable_ond")] public bool IsVisible { get; set; } = true;
        [Column("last_upload_guid")] public string LastUploadGuid { get; set; } = null;
        [Column("image_name")] public string Name { get; set; } = string.Empty;
        [Column("image_is_protected")] public bool Protected { get; set; } = false;
        [Column("image_type")] public string Type { get; set; } = null;
        [Column("replication_mode")] public EnumImageReplication.ReplicationType ReplicationMode { get; set; } = EnumImageReplication.ReplicationType.None;
    }

    [NotMapped]
    public class ImageWithDate : EntityImage
    {
        public DateTime? LastUsed { get; set; }
        public string SizeOnServer { get; set; }
    }
}
