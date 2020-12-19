using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toems_Common.Entity.Remotely
{
    [Table("Devices")]
    public class Device
    {
        [Key]
        public string ID { get; set; }
        public string AgentVersion { get; set; }
        [StringLength(100)]
        public string Alias { get; set; }
        public double CpuUtilization { get; set; } = 0;
        public string CurrentUser { get; set; }
        public string DeviceGroupID { get; set; }
        public string DeviceName { get; set; }
        public string Drives { get; set; }
        public double UsedMemory { get; set; } = 0;
        public double UsedStorage { get; set; } = 0;
        public bool Is64Bit { get; set; } = true;
        public bool IsOnline { get; set; } = false;
        public DateTime LastOnline { get; set; } = DateTime.Now;
        public string OrganizationID { get; set; }
        public int OSArchitecture { get; set; } = 1;
        public string OSDescription { get; set; }
        public string Platform { get; set; }
        public int ProcessorCount { get; set; } = 1;
        public string ServerVerificationToken { get; set; }
        [StringLength(200)]
        public string Tags { get; set; } = "";
        public double TotalMemory { get; set; } = 0;
        public double TotalStorage { get; set; } = 0;
        public string Notes { get; set; }
        public string PublicIP { get; set; }
    }
}
