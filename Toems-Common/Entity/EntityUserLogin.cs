using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("user_logins")]
    public class EntityUserLogin
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("user_login_id")]
        public int Id { get; set; }

        [Column("computer_id")]
        public int ComputerId { get; set; }

        [Column("username")]
        public string UserName { get; set; }

        [Column("login_date_time_utc")]
        public DateTime LoginDateTime { get; set; }

        [Column("logout_date_time_utc")]
        public DateTime LogoutDateTime { get; set; }

        [Column("client_login_id")]
        public int? ClientLoginId { get; set; }
    }
}
