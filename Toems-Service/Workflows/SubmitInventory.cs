using System;
using Toems_Common.Dto;
using Toems_Service.Entity;

namespace Toems_Service.Workflows
{
    public class SubmitInventory
    {
        public bool Run(DtoInventoryCollection collection,string clientIdentifier)
        {
            var client = new ServiceComputer().GetByGuid(clientIdentifier);
            if (client == null) return false;

            new ServiceBiosInventory().AddOrUpdate(collection.Bios,client.Id);
            new ServiceComputerSystemInventory().AddOrUpdate(collection.ComputerSystem,client.Id);
            new ServiceOsInventory().AddOrUpdate(collection.Os,client.Id);
            new ServiceProcessorInventory().AddOrUpdate(collection.Processor,client.Id);
            new ServicePrinterInventory().AddOrUpdate(collection.Printers, client.Id);
            new ServiceHardDriveInventory().AddOrUpdate(collection.HardDrives, client.Id);
            new ServiceNicInventory().AddOrUpdate(collection.NetworkAdapters, client.Id);
            new ServiceWuInventory().Add(collection.WindowsUpdates);
            new ServiceComputerUpdates().AddOrUpdate(collection.WindowsUpdates, client.Id);
            new ServiceSoftwareInventory().Add(collection.Software);
            new ServiceComputerSoftware().AddOrUpdate(collection.Software, client.Id);
            new ServiceAntivirusInventory().AddOrUpdate(collection.AntiVirus, client.Id);
            new ServiceLogicalVolumeInventory().AddOrUpdate(collection.LogicalVolume, client.Id);
            new ServiceBitlockerInventory().AddOrUpdate(collection.Bitlocker, client.Id);
            new ServiceFirewallInventory().AddOrUpdate(collection.Firewall,client.Id);

            client.LastInventoryTime = DateTime.Now;
            if(!string.IsNullOrEmpty(collection.ClientVersion))
                client.ClientVersion = collection.ClientVersion;
            if(!string.IsNullOrEmpty(collection.PushUrl))
                client.PushUrl = collection.PushUrl;
            new ServiceComputer().UpdateComputer(client);

            return true;
        }
    }
}
