using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using log4net;
using Toems_ApiCalls;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;
using Toems_Service.Entity;
using Toems_ServiceCore.EntityServices;

namespace Toems_Service.Workflows
{
    public class PowerManagement
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly UnitOfWork _uow;

        public PowerManagement()
        {
            _uow = new UnitOfWork();
        }

        public bool ShutdownComputer(int computerId)
        {
            Logger.Debug("Starting Shutdown Task");
            var shutdownDelay = ServiceSetting.GetSettingValue(SettingStrings.ShutdownDelay);
            return Shutdown(computerId,shutdownDelay);
        }

        public bool RebootComputer(int computerId)
        {
            Logger.Debug("Starting Reboot Task");
            var shutdownDelay = ServiceSetting.GetSettingValue(SettingStrings.ShutdownDelay);
            return Reboot(computerId,shutdownDelay);
        }

        public bool WakeupComputer(int computerId)
        {
            Logger.Debug("Starting Wakeup Task");
            var computer = _uow.ComputerRepository.GetById(computerId);
            if (computer == null) return false;
            var computerList = new List<EntityComputer>();
            computerList.Add(computer);
            Task.Run(() => Wakeup(computerList)); //don't wait for result
            //Wakeup(computerList);
            return true;
        }

        public bool ShutdownGroups(List<EntityGroup> groups)
        {
            Logger.Debug("Starting Shutdown Tasks");
            var shutdownDelay = ServiceSetting.GetSettingValue(SettingStrings.ShutdownDelay);

            //get a distinct list of computers for each group
            var shutdownMembers = new List<EntityComputer>();
            foreach (var group in groups)
                shutdownMembers = _uow.GroupRepository.GetGroupMembers(group.Id, "");
            var distinctShutdown = shutdownMembers.GroupBy(x => x.Id).Select(y => y.First());

            foreach (var computer in distinctShutdown)
                Shutdown(computer.Id,shutdownDelay);

            return true;
        }

        public bool RebootGroups(List<EntityGroup> groups)
        {
            Logger.Debug("Starting Reboot Tasks");
            var shutdownDelay = ServiceSetting.GetSettingValue(SettingStrings.ShutdownDelay);
            //get a distinct list of computers for each group
            var rebootMembers = new List<EntityComputer>();
            foreach (var group in groups)
                rebootMembers = _uow.GroupRepository.GetGroupMembers(group.Id, "");
            var distinctReboot = rebootMembers.GroupBy(x => x.Id).Select(y => y.First());

            foreach (var computer in distinctReboot)
                Reboot(computer.Id,shutdownDelay);

            return true;
        }

      

        public bool WakeupGroups(List<EntityGroup> groups)
        {
            Logger.Debug("Starting Wakeup Tasks");
            //get a distinct list of computers for each group
            var wakeUpMembers = new List<EntityComputer>();
            foreach (var group in groups)
                wakeUpMembers = _uow.GroupRepository.GetGroupMembers(group.Id, "");
            var distinctWakeup = wakeUpMembers.GroupBy(x => x.Id).Select(y => y.First());
            Task.Run(() => Wakeup(distinctWakeup)); // don't wait for result
            //Wakeup(distinctWakeup);

            return true;
        }

     
        private void Wakeup(IEnumerable<EntityComputer> computers)
        {
            var relayTasks = GenerateWakeupTask(computers);
            var intercomKey = ServiceSetting.GetSettingValue(SettingStrings.IntercomKeyEncrypted);
            var decryptedKey = new EncryptionServices().DecryptText(intercomKey);
            var checkinInterval = ServiceSetting.GetSettingValue(SettingStrings.CheckinInterval);
            var dateCutoff = DateTime.Now - TimeSpan.FromMinutes(Convert.ToInt32(checkinInterval));
            foreach (var relayTask in relayTasks)
            {
                if (relayTask.Macs.Count == 0)
                    continue;

                var destinationRelay = _uow.WolRelayRepository.GetFirstOrDefault(x => x.Gateway.Equals(relayTask.Gateway));
                if (destinationRelay == null)
                {
                    Logger.Debug("No WOL Relays Defined For Gateway: " + relayTask.Gateway +
                                 " Looking For Available Computers To Serve As Relay");
                    //find up to 10 computers on that network that have checked in recently
                    var potentialRelays = _uow.ComputerRepository.GetPotentialWOLRelays(relayTask.Gateway, dateCutoff);
                    if (potentialRelays == null)
                    {
                        Logger.Debug("No Computers Were Found To Act As A Relay For: " + relayTask.Gateway +
                                     " Skipping Computers For This Network");
                        continue;
                    }
                    if (potentialRelays.Count == 0)
                    {
                        Logger.Debug("No Computers Were Found To Act As A Relay For: " + relayTask.Gateway +
                                     " Skipping Computers For This Network");
                        continue;
                    }

                    var listActiveComputers = new List<EntityComputer>();
                    foreach (var computer in potentialRelays)
                    {
                        var isPoweredOn = new ServiceComputer().GetStatus(computer.Id);
                        if (isPoweredOn)
                        {
                            listActiveComputers.Add(computer);
                        }
                    }

                    if (listActiveComputers.Count == 0)
                    {
                        Logger.Debug("No Computers Were Found To Act As A Relay For: " + relayTask.Gateway +
                                     " Skipping Computers For This Network");
                        continue;
                    }

                    //send the woltask to two computers
                    int counter = 0;
                    foreach (var computer in listActiveComputers)
                    {
                        counter++;
                        if (counter == 3) break;
                        if (computer.CertificateId == -1) continue;
                        if (string.IsNullOrEmpty(computer.PushUrl)) continue;
                        var deviceCertEntity = _uow.CertificateRepository.GetById(computer.CertificateId);
                        var deviceCert = new X509Certificate2(deviceCertEntity.PfxBlob, new EncryptionServices().DecryptText(deviceCertEntity.Password), X509KeyStorageFlags.Exportable);
                        new APICall().ClientApi.SendWolTask(computer.PushUrl, deviceCert, relayTask);
                        
                    }
                }
                else
                {
                    //send relay task to destination relay
                    var comServer = _uow.ClientComServerRepository.GetById(destinationRelay.ComServerId);
                    new APICall().ClientComServerApi.WakeupComputers(comServer.Url, "", decryptedKey, relayTask);
                }
            }
        }

        private List<DtoWolTask> GenerateWakeupTask(IEnumerable<EntityComputer> computersToWakeup)
        {
            //get a list of all known gateways
            var allKnownGateways = _uow.NicInventoryRepository.Get().GroupBy(x => x.Gateways).Select(y => y.First());
            var gateways = new List<string>();
            foreach (var nic in allKnownGateways.Where(x => !string.IsNullOrEmpty(x.Gateways)))
            {
                gateways.AddRange(nic.Gateways.Split(','));
            }
            gateways = gateways.Distinct().ToList();

            var wolRelayInfoList = new List<DtoWolTask>();
            foreach (var gateway in gateways)
            {
                var wolRelayInfo = new DtoWolTask();
                wolRelayInfo.Gateway = gateway;

                foreach (var computer in computersToWakeup)
                {
                    var localComputer = computer;
                    var computerNics = _uow.NicInventoryRepository.Get(x => x.ComputerId == localComputer.Id);

                    var localGateway = gateway;
                    foreach (var nic in computerNics.Where(x => !string.IsNullOrEmpty(x.Gateways)))
                    {
                        if (nic.Gateways.Contains(localGateway))
                        {
                            foreach (var g in nic.Gateways.Split(','))
                            {
                                if (g.Equals(gateway))
                                    wolRelayInfo.Macs.Add(nic.Mac);
                            }
                        }
                    }
                }

                wolRelayInfoList.Add(wolRelayInfo);
            }

            return wolRelayInfoList;
        }

        private bool Reboot(int computerId, string delay)
        {
            var computer = _uow.ComputerRepository.GetById(computerId);
            if (computer == null) return false;
            if (computer.CertificateId == -1) return false;
            var compPreventShutdownGroups = _uow.ComputerRepository.GetComputerPreventShutdownGroups(computerId);
            if (compPreventShutdownGroups.Count > 0) return true; //computer is in a prevent shutdown group continue on
            var socket = _uow.ActiveSocketRepository.GetFirstOrDefault(x => x.ComputerId == computer.Id);
            if (socket != null)
            {
                var deviceCertEntity = _uow.CertificateRepository.GetById(computer.CertificateId);
                var deviceCert = new X509Certificate2(deviceCertEntity.PfxBlob, new EncryptionServices().DecryptText(deviceCertEntity.Password), X509KeyStorageFlags.Exportable);
                var intercomKey = ServiceSetting.GetSettingValue(SettingStrings.IntercomKeyEncrypted);
                var decryptedKey = new EncryptionServices().DecryptText(intercomKey);
                var socketRequest = new DtoSocketRequest();
                socketRequest.connectionIds.Add(socket.ConnectionId);
                socketRequest.action = "Reboot";
                socketRequest.message = delay;
                new APICall().ClientComServerApi.SendAction(socket.ComServer, "", decryptedKey, socketRequest);
            }
            return true;
        }

        private bool Shutdown(int computerId, string delay)
        {
            var computer = _uow.ComputerRepository.GetById(computerId);
            if (computer == null) return false;
            if (computer.CertificateId == -1) return false;
            var compPreventShutdownGroups = _uow.ComputerRepository.GetComputerPreventShutdownGroups(computerId);
            if (compPreventShutdownGroups.Count > 0) return true; //computer is in a prevent shutdown group continue on
            var socket = _uow.ActiveSocketRepository.GetFirstOrDefault(x => x.ComputerId == computer.Id);
            if (socket != null)
            {
                var deviceCertEntity = _uow.CertificateRepository.GetById(computer.CertificateId);
                var deviceCert = new X509Certificate2(deviceCertEntity.PfxBlob, new EncryptionServices().DecryptText(deviceCertEntity.Password), X509KeyStorageFlags.Exportable);
                var intercomKey = ServiceSetting.GetSettingValue(SettingStrings.IntercomKeyEncrypted);
                var decryptedKey = new EncryptionServices().DecryptText(intercomKey);
                var socketRequest = new DtoSocketRequest();
                socketRequest.connectionIds.Add(socket.ConnectionId);
                socketRequest.action = "Shutdown";
                socketRequest.message = delay;
                new APICall().ClientComServerApi.SendAction(socket.ComServer, "", decryptedKey, socketRequest);
            }
            return true;
        }
    }
}
