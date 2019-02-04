using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("asset_attachments")]
    public class EntityAssetAttachment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("asset_attachment_id")]
        public int Id { get; set; }

        [Column("asset_id")]
        public int AssetId { get; set; }

        [Column("attachment_id")]
        public int AttachmentId { get; set; }   

    }
}