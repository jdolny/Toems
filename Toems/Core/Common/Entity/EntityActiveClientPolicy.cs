using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Toems_Common.Entity
{
    [Table("active_client_policies")]
    public class EntityActiveClientPolicy
    {
        [Key]
        [Column("active_client_policy_id")]
        public int Id { get; set; }

        [Column("policy_id")]
        public int PolicyId { get; set; }

        [Column("json_string")]
        public string JsonString { get; set; } = null!;
    }
}