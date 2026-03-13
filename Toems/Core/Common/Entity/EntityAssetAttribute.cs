using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("asset_attributes")]
    public class EntityAssetAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("asset_attribute_id")]
        public int Id { get; set; }

        [Column("asset_id")]
        public int AssetId { get; set; }

        [Column("custom_attribute_id")]
        public int CustomAttributeId { get; set; }

        [Column("value")]
        public string Value { get; set; }

    }
}