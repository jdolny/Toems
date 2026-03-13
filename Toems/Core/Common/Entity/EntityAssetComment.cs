using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("asset_comments")]
    public class EntityAssetComment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("asset_comment_id")]
        public int Id { get; set; }

        [Column("asset_id")]
        public int AssetId { get; set; }

        [Column("comment_id")]
        public int CommentId { get; set; }   

    }
}