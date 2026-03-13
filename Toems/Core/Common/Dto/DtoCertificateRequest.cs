using System;
using Org.BouncyCastle.Asn1.X509;

namespace Toems_Common.Dto
{
    public class DtoCertificateRequest
    {
        public string SubjectName { get; set; }
        public DateTime NotBefore { get; set; }
        public DateTime NotAfter { get; set; }
        public KeyPurposeID[] Usages { get; set; }
    }
}