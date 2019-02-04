using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Toems_Common.Enum;

namespace Toems_Common.Entity
{
    [Table("software_asset_softwares")]
    public class EntitySoftwareAssetSoftware
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("software_asset_softwares_id")]
        public int Id { get; set; }

        [Column("asset_id")]
        public int AssetId { get; set; }

        [Column("software_inventory_id")]
        public int SoftwareInventoryId { get; set; }

        [Column("match_type")]
        public EnumSoftwareAsset.SoftwareMatchType MatchType { get; set; }

    }
}