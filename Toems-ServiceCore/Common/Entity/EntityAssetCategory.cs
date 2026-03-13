using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("custom_asset_categories")]
    public class EntityAssetCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("custom_asset_category_id")]
        public int Id { get; set; }

        [Column("custom_asset_id")]
        public int AssetId { get; set; }

        [Column("category_id")]
        public int CategoryId { get; set; }

    }
}