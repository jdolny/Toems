using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("certificate_inventory")]
    public class EntityCertificateInventory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("certificate_inventory_id")]
        public int Id { get; set; }

        [Column("store")]
        public string Store { get; set; }

        [Column("subject")]
        public string Subject { get; set; }

        [Column("friendlyname")]
        public string FriendlyName { get; set; }

        [Column("thumbprint")]
        public string Thumbprint { get; set; }

        [Column("serial")]
        public string Serial { get; set; }

        [Column("issuer")]
        public string Issuer { get; set; }

        [Column("notbefore_utc")]
        public DateTime NotBefore { get; set; }

        [Column("notafter_utc")]
        public DateTime NotAfter { get; set; }

    }
}
