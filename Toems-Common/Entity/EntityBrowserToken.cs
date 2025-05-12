using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("browser_download_tokens")]
    public class EntityBrowserToken
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("browser_download_token_id")]
        public int Id { get; set; }

        [Column("token")]
        public string Token { get; set; }

        [Column("expires_at_utc")]
        public DateTime ExpiresAtUtc { get; set; }
        
        [Column("user_id")]
        public int UserId { get; set; }



    }
}