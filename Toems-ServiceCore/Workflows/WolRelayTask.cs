using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using log4net;
using Toems_Common.Dto;

namespace Toems_Service.Workflows
{
    public class WolRelayTask
    {
        private static readonly ILog Logger =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static List<DtoIpInfo> GetInterfaceInfo()
        {
            Logger.Debug("Gathering Local Interfaces:");
            var ipInfoList = new List<DtoIpInfo>();
            foreach (var adapter in NetworkInterface.GetAllNetworkInterfaces())
            {
                var ipInfo = new DtoIpInfo();

                try
                {
                    foreach (
                        var ip in
                            adapter.GetIPProperties()
                                .UnicastAddresses.Where(ip => ip.Address.AddressFamily == AddressFamily.InterNetwork))
                    {
                        ipInfo.IpAddress = ip.Address;
                        ipInfo.Netmask = ip.IPv4Mask;
                        break;
                    }


                    foreach (
                        var gateway in
                            adapter.GetIPProperties()
                                .GatewayAddresses.Where(ip => ip.Address.AddressFamily == AddressFamily.InterNetwork))
                    {
                        ipInfo.Gateway = gateway.Address;
                    }

                    //https://stackoverflow.com/questions/25281099/how-to-get-the-local-ip-broadcast-address-dynamically-c-sharp
                    uint ipAddress = BitConverter.ToUInt32(ipInfo.IpAddress.GetAddressBytes(), 0);
                    uint ipMaskV4 = BitConverter.ToUInt32(ipInfo.Netmask.GetAddressBytes(), 0);
                    uint broadCastIpAddress = ipAddress | ~ipMaskV4;

                    ipInfo.Broadcast = new IPAddress(BitConverter.GetBytes(broadCastIpAddress));
                }
                catch(Exception ex)
                {
                    Logger.Debug("Failed To Enumerate An Interface");
                    Logger.Debug(ex.Message);
                }
                ipInfoList.Add(ipInfo);
            }
            return ipInfoList;
        }

        public static void WakeUp(DtoWolTask wolTask)
        {
            Logger.Debug("WakeUp Task Received");
            //Send it 2 times waiting 20 seconds b/w each one
            for (int i = 1; i <= 2; i++)
            {
                Logger.Debug("Wake Up Pass: " + i);
                var interfaces = GetInterfaceInfo();
                if (interfaces.Count == 0) return;

                interfaces = interfaces.Where(x => x.Gateway != null).ToList();

                var nic = interfaces.FirstOrDefault(x => x.Gateway.ToString().Equals(wolTask.Gateway));
                if (nic != null)
                {
                    Logger.Debug("Found Matching Interface For Gateway: " + wolTask.Gateway);
                    Logger.Debug("Broadcast Address Set To: " + nic.Broadcast);
                    var bcast = nic.Broadcast;
                    if (bcast == null) return;

                    foreach (var mac in wolTask.Macs)
                        SendMagicPacket(mac, bcast);
                }

                Task.Delay(20 * 1000).Wait();
            }

        }

        private static void SendMagicPacket(string mac, IPAddress broadcast)
        {
            Logger.Debug("Sending Magic Packet To: " + mac);
            try
            {
                var value = long.Parse(mac, NumberStyles.HexNumber,
                    CultureInfo.CurrentCulture.NumberFormat);
                var macBytes = BitConverter.GetBytes(value);

                Array.Reverse(macBytes);
                var macAddress = new byte[6];

                for (var j = 0; j < 6; j++)
                    macAddress[j] = macBytes[j + 2];

                var packet = new byte[17*6];

                for (var i = 0; i < 6; i++)
                    packet[i] = 0xff;

                for (var i = 1; i <= 16; i++)
                {
                    for (var j = 0; j < 6; j++)
                        packet[i*6 + j] = macAddress[j];
                }

                var client = new UdpClient();
                client.Connect(broadcast, 9);
                client.Send(packet, packet.Length);
            }
            catch(Exception ex)
            {
                Logger.Debug("Could Not Send Magic Packet");
                Logger.Debug(ex.Message);
            }
        }
    }
}
