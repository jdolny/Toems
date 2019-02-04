using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Toems_Common.Enum;

namespace Toems_Common.Entity
{
    [Table("policy_com_servers")]
    public class EntityPolicyComServer
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("policy_com_server_id")]
        public int Id { get; set; }

        [Column("policy_id")]
        public int PolicyId { get; set; }

        [Column("com_server_id")]
        public int ComServerId { get; set; }

     


       
       

       
    }
}