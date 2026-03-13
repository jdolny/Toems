using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("assets")]
    public class EntityAsset
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("asset_id")]
        public int Id { get; set; }

        [Column("asset_type_id")]
        public int AssetTypeId { get; set; }

        [Column("asset_display_name")]
        public string DisplayName { get; set; }

        [Column("is_archived")]
        public bool IsArchived { get; set; }

        [Column("archived_datetime_local")]
        public DateTime? ArchivedDateTime { get; set; }

    }
}