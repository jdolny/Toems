using System.Collections.Generic;
using Toems_Common.Entity;

namespace Toems_Common.Dto
{
    public class DtoInventoryCollection
    {
        public EntityBiosInventory Bios { get; set; } = new EntityBiosInventory();
        public EntityComputerSystemInventory ComputerSystem { get; set; } = new EntityComputerSystemInventory();
        public List<EntityComputerGpuInventory> Gpu { get; set; } = new List<EntityComputerGpuInventory>();
        public List<EntitySoftwareInventory> Software { get; set; } = new List<EntitySoftwareInventory>();
        public EntityOsInventory Os { get; set; } = new EntityOsInventory();
        public EntityProcessorInventory Processor { get; set; } = new EntityProcessorInventory();
        public List<EntityHardDriveInventory> HardDrives { get; set; } = new List<EntityHardDriveInventory>();
        public List<EntityPrinterInventory> Printers { get; set; } = new List<EntityPrinterInventory>();
        public List<EntityWindowsUpdateInventory> WindowsUpdates { get; set; } = new List<EntityWindowsUpdateInventory>();
        public List<EntityNicInventory> NetworkAdapters { get; set; } = new List<EntityNicInventory>();
        public List<EntityNicInventory> Nics { get; set; } = new List<EntityNicInventory>();
        public List<EntityAntivirusInventory> AntiVirus { get; set; } = new List<EntityAntivirusInventory>();
        public List<EntityBitlockerInventory> Bitlocker { get; set; } = new List<EntityBitlockerInventory>();
        public List<EntityLogicalVolumeInventory> LogicalVolume { get; set; } = new List<EntityLogicalVolumeInventory>();
        public List<EntityCertificateInventory> Certificates { get; set; } = new List<EntityCertificateInventory>();
        public EntityFirewallInventory Firewall { get; set; } = new EntityFirewallInventory();
        public string ClientVersion { get; set; }
        public string PushUrl { get; set; }
        public string HardwareUUID { get; set; }
      
    }
}
