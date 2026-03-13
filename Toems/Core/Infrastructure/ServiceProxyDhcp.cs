using Toems_Common.Dto;
using Toems_DataModel;
using Toems_ServiceCore.EntityServices;
using Toems_ServiceCore.Workflows;

namespace Toems_ServiceCore.Infrastructure
{
    public class ServiceProxyDhcp(ServiceContext ctx)
    {
        
        public DtoTftpServer GetAllTftpServers()
        {
            var tftpDto = new DtoTftpServer();
            tftpDto.TftpServers = new List<string>();
            var comServers = ctx.Uow.ClientComServerRepository.Get(x => x.IsTftpServer);
            foreach (var com in comServers)
            {
                tftpDto.TftpServers.Add(com.TftpInterfaceIp);
            }

            return tftpDto;
        }

        public DtoTftpServer GetComputerTftpServers(string mac)
        {
            var tftpDto = new DtoTftpServer();
            tftpDto.TftpServers = new List<string>();
            var computer = ctx.Uow.ComputerRepository.Get(x => x.ImagingMac.ToUpper().Equals(mac.ToUpper())).FirstOrDefault();
            var comServers = ctx.GetCompTftpServers.Run(computer.Id);
            foreach (var com in comServers)
            {
                tftpDto.TftpServers.Add(com.TftpInterfaceIp);
            }

            return tftpDto;
        }

        public DtoProxyReservation GetProxyReservation(string mac)
        {
            var bootClientReservation = new DtoProxyReservation();

            var computer = ctx.Uow.ComputerRepository.Get(x => x.ImagingMac.ToUpper().Equals(mac.ToUpper())).FirstOrDefault();
            if (computer == null)
            {
                bootClientReservation.BootFile = "NotFound";
                return bootClientReservation;
            }
            var computerGroupMemberships = ctx.Computer.GetAllGroupMemberships(computer.Id);
            var computerGroups = ctx.Uow.ComputerRepository.GetAllComputerGroups(computer.Id).OrderBy(x => x.ImagingPriority).ThenBy(x => x.Name).ToList();

            if (computerGroups.Count == 0)
            {
                bootClientReservation.BootFile = "NotEnabled";
                return bootClientReservation;
            }

            var bootFile = "";
            foreach (var group in computerGroups)
            {
                if(!string.IsNullOrEmpty(group.ProxyBootloader))
                {
                    bootFile = group.ProxyBootloader;
                    break;
                }
              
            }

            if (string.IsNullOrEmpty(bootFile))
            {
                bootClientReservation.BootFile = "NotEnabled";
                return bootClientReservation;
            }

            var comServers = ctx.GetCompTftpServers.Run(computer.Id);
            foreach (var com in comServers)
            {
                bootClientReservation.NextServer = com.TftpInterfaceIp;
            }

            
            switch (bootFile)
            {
                case "bios_pxelinux":
                    bootClientReservation.BootFile = @"proxy/bios/pxelinux.0";
                    break;
                case "bios_ipxe":
                    bootClientReservation.BootFile = @"proxy/bios/undionly.kpxe";
                    break;
                case "bios_x86_winpe":
                    bootClientReservation.BootFile = @"proxy/bios/pxeboot.n12";
                    bootClientReservation.BcdFile = @"/boot/BCDx86";
                    break;
                case "bios_x64_winpe":
                    bootClientReservation.BootFile = @"proxy/bios/pxeboot.n12";
                    bootClientReservation.BcdFile = @"/boot/BCDx64";
                    break;
                case "efi_x86_syslinux":
                    bootClientReservation.BootFile = @"proxy/efi32/syslinux.efi";
                    break;
                case "efi_x86_ipxe":
                    bootClientReservation.BootFile = @"proxy/efi32/ipxe.efi";
                    break;
                case "efi_x86_winpe":
                    bootClientReservation.BootFile = @"proxy/efi32/bootmgfw.efi";
                    bootClientReservation.BcdFile = @"/boot/BCDx86";
                    break;
                case "efi_x64_syslinux":
                    bootClientReservation.BootFile = @"proxy/efi64/syslinux.efi";
                    break;
                case "efi_x64_ipxe":
                    bootClientReservation.BootFile = @"proxy/efi64/ipxe.efi";
                    break;
                case "efi_x64_winpe":
                    bootClientReservation.BootFile = @"proxy/efi64/bootmgfw.efi";
                    bootClientReservation.BcdFile = @"/boot/BCDx64";
                    break;
                case "efi_x64_grub":
                    bootClientReservation.BootFile = @"proxy/efi64/bootx64.efi";
                    break;
            }

            return bootClientReservation;
        }
    }
}
