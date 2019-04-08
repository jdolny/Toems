using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("active_socket_connections")]
    public class EntityActiveSocket
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("active_socket_connection_id")]
        public int Id { get; set; }

        [Column("computer_id")]
        public int ComputerId { get; set; }

        [Column("connection_id")]
        public string ConnectionId { get; set; }

        [Column("com_server")]
        public string ComServer { get; set; }



    }
}