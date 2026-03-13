using Toems_Common.Dto;
using Toems_ServiceCore.EntityServices;

namespace Toems_ServiceCore.Workflows
{
    public class SubmitInventory(ServiceComputer serviceComputer, ServiceBiosInventory serviceBiosInventory, ServiceComputerSystemInventory serviceComputerSystemInventory, 
        ServiceComputerGpuInventory serviceComputerGpuInventory, ServiceOsInventory serviceOsInventory, ServiceProcessorInventory serviceProcessorInventory, 
        ServicePrinterInventory servicePrinterInventory, ServiceHardDriveInventory serviceHardDriveInventory, ServiceNicInventory serviceNicInventory, 
        ServiceWuInventory serviceWuInventory, ServiceComputerUpdates serviceComputerUpdates, ServiceSoftwareInventory serviceSoftwareInventory, 
        ServiceComputerSoftware serviceComputerSoftware, ServiceCertificateInventory serviceCertificateInventory, ServiceComputerCertificate serviceComputerCertificate, 
        ServiceAntivirusInventory serviceAntivirusInventory, ServiceLogicalVolumeInventory serviceLogicalVolumeInventory, ServiceBitlockerInventory serviceBitlockerInventory, 
        ServiceFirewallInventory serviceFirewallInventory, ServiceImagingClientId serviceImagingClientId)
    {
        public bool Run(DtoInventoryCollection collection,string clientIdentifier)
        {
            var client = serviceComputer.GetByGuid(clientIdentifier);
            if (client == null) return false;

            serviceBiosInventory.AddOrUpdate(collection.Bios,client.Id);
            serviceComputerSystemInventory.AddOrUpdate(collection.ComputerSystem,client.Id);
            serviceComputerGpuInventory.AddOrUpdate(collection.Gpu, client.Id);
            serviceOsInventory.AddOrUpdate(collection.Os,client.Id);
            serviceProcessorInventory.AddOrUpdate(collection.Processor,client.Id);
            servicePrinterInventory.AddOrUpdate(collection.Printers, client.Id);
            serviceHardDriveInventory.AddOrUpdate(collection.HardDrives, client.Id);
            serviceNicInventory.AddOrUpdate(collection.NetworkAdapters, client.Id);
            serviceWuInventory.Add(collection.WindowsUpdates);
            serviceComputerUpdates.AddOrUpdate(collection.WindowsUpdates, client.Id);
            serviceSoftwareInventory.Add(collection.Software);
            serviceComputerSoftware.AddOrUpdate(collection.Software, client.Id);
            serviceCertificateInventory.Add(collection.Certificates);
            serviceComputerCertificate.AddOrUpdate(collection.Certificates, client.Id);
            serviceAntivirusInventory.AddOrUpdate(collection.AntiVirus, client.Id);
            serviceLogicalVolumeInventory.AddOrUpdate(collection.LogicalVolume, client.Id);
            serviceBitlockerInventory.AddOrUpdate(collection.Bitlocker, client.Id);
            serviceFirewallInventory.AddOrUpdate(collection.Firewall,client.Id);

            client.LastInventoryTime = DateTime.Now;
            if(!string.IsNullOrEmpty(collection.ClientVersion))
                client.ClientVersion = collection.ClientVersion;
            if(!string.IsNullOrEmpty(collection.PushUrl))
                client.PushUrl = collection.PushUrl;
            if (!string.IsNullOrEmpty(collection.HardwareUUID))
                client.UUID = collection.HardwareUUID;
            serviceComputer.UpdateComputer(client);
            serviceImagingClientId.AddOrUpdate(client.Id);


            return true;
        }
    }
}
