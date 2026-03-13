using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("impersonation_accounts")]
    public class EntityImpersonationAccount
    {
        public EntityImpersonationAccount()
        {
            Guid = System.Guid.NewGuid().ToString();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("impersonation_account_id")]
        public int Id { get; set; }

        [Column("username")]
        public string Username { get; set; }

        [Column("password_encrypted")]
        public string Password { get; set; }

        [Column("impersonation_guid")]
        public string Guid { get; set; }

    }
}