using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("client_com_servers")]
    public class EntityClientComServer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("client_com_server_id")]
        public int Id { get; set; }

        [Column("display_name")]
        public string DisplayName { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("url")]
        public string Url { get; set; }

        [Column("replicate_storage")]
        public bool ReplicateStorage { get; set; }

    }
}