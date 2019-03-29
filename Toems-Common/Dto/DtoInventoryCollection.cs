using System.Collections.Generic;
using Toems_Common.Entity;

namespace Toems_Common.Dto
{
    public class DtoInventoryCollection
    {
        public EntityBiosInventory Bios { get; set; }
        public EntityComputerSystemInventory ComputerSystem { get; set; }
        public List<EntitySoftwareInventory> Software { get; set; }
        public EntityOsInventory Os { get; set; }
        public EntityProcessorInventory Processor { get; set; }
        public List<EntityHardDriveInventory> HardDrives { get; set; }
        public List<EntityPrinterInventory> Printers { get; set; }
        public List<EntityWindowsUpdateInventory> WindowsUpdates { get; set; }
        public List<EntityNicInventory> NetworkAdapters { get; set; }
        public List<EntityNicInventory> Nics { get; set; }
        public List<EntityAntivirusInventory> AntiVirus { get; set; }
        public List<EntityBitlockerInventory> Bitlocker { get; set; }
        public List<EntityLogicalVolumeInventory> LogicalVolume { get; set; }
        public List<EntityCertificateInventory> Certificates { get; set; }
        public EntityFirewallInventory Firewall { get; set; }
        public string ClientVersion { get; set; }
        public string PushUrl { get; set; }

        public DtoInventoryCollection()
        {
            WindowsUpdates = new List<EntityWindowsUpdateInventory>();
        }
    }
}
