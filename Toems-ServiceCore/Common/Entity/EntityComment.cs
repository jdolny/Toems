using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("comments")]
    public class EntityComment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("comment_id")]
        public int Id { get; set; }

        [Column("comment_text")]
        public string CommentText { get; set; }

        [Column("comment_username")]
        public string Username { get; set; }

        [Column("comment_time_local")]
        public DateTime CommentTime { get; set; }

    }
}