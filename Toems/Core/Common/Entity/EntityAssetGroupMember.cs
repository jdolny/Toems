using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("asset_group_members")]
    public class EntityAssetGroupMember
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("asset_group_member_id")]
        public int Id { get; set; }

        [Column("asset_group_id")]
        public int AssetGroupId { get; set; }

        [Column("asset_id")]
        public int AssetId { get; set; }

    }
}