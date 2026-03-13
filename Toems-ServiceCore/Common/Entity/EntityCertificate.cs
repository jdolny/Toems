using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Toems_Common.Enum;

namespace Toems_Common.Entity
{
    [Table("certificates")]
    public class EntityCertificate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("certificate_id")]
        public int Id { get; set; }

        [Column("serial_number")]
        public string Serial { get; set; }

        [Column("subject_name")]
        public string SubjectName { get; set; }

        [Column("pfx_blob")]
        public byte[] PfxBlob { get; set; }

        [Column("pfx_password_enc")]
        public string Password { get; set; }

        [Column("not_before_utc")]
        public DateTime NotBefore { get; set; }

        [Column("not_after_utc")]
        public DateTime NotAfter { get; set; }

        [Column("is_revoked")]
        public bool IsRevoked { get; set; }

        [Column("revoked_date_utc")]
        public DateTime RevokedDate { get; set; }

        [Column("certificate_type")]
        public EnumCertificate.CertificateType Type { get; set; }

    }
}