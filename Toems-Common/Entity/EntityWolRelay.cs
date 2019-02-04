using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("wol_relays")]
    public class EntityWolRelay
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("wol_relay_id")]
        public int Id { get; set; }

        [Column("wol_gateway")]
        public string Gateway { get; set; }

        [Column("com_server_id")]
        public int ComServerId { get; set; }

      

    }
}