using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("multicast_ports")]
    public class PortEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("multicast_port_id")]
        public int Id { get; set; }

        [Column("multicast_port_number")]
        public int Number { get; set; }

        [Column("com_server_id")]
        public int ComServerId { get; set; }
    }
}
