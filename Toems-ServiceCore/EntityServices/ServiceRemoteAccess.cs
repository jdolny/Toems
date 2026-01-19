using System.Net;
using Newtonsoft.Json;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Service;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceRemoteAccess(EntityContext ectx, ServiceClientComServer clientComServerService, UncServices uncService)
    {

        public string GetRemotelyInstallArgs()
        {
            var comServer = ectx.Uow.ClientComServerRepository.Get(x => x.IsRemoteAccessServer).FirstOrDefault();
            if (comServer == null)
            {
                return "Error: Could Not Get Install Args.  No Remote Access Com Servers Were Found";
            }
            return $"-install -quiet -organizationid {comServer.RaOrganizationId} -serverurl {comServer.RemoteAccessUrl}";
        }

        public int GetRemoteAccessServerCount()
        {
            var result = ectx.Uow.ClientComServerRepository.Count(x => x.IsRemoteAccessServer);
            if (string.IsNullOrEmpty(result)) return -1;
            return Convert.ToInt16(result);
        }


        public DtoActionResult UpdateComputerRemoteAccessId(DtoRemotelyConnectionInfo conInfo, string clientIdentity)
        {
            var client = ectx.Uow.ComputerRepository.GetFirstOrDefault(x => x.Guid == clientIdentity);
            if (client == null) return new DtoActionResult() { ErrorMessage = "Client Not Found", Success = false };
            client.RemoteAccessId = conInfo.DeviceID;
            ectx.Uow.ComputerRepository.Update(client, client.Id);
            ectx.Uow.Save();
            return new DtoActionResult() { Success = true, Id = client.Id };
        }

        public bool VerifyRemoteAccessInstalled(int comServerId)
        {
            var comServer = ectx.Uow.ClientComServerRepository.GetById(comServerId);
            var intercomKey = ectx.Settings.GetSettingValue(SettingStrings.IntercomKeyEncrypted);
            var decryptedKey = ectx.Encryption.DecryptText(intercomKey);

            return new APICall().ClientComServerApi.VerifyRemoteAccessInstalled(comServer.Url, "", decryptedKey);
        }

        public DtoActionResult InitializeRemotelyServer(int comServerId)
        {
            var comServer = ectx.Uow.ClientComServerRepository.GetById(comServerId);
            if (comServer == null)
            {
                return new DtoActionResult() { Success = false, ErrorMessage = "Com Server Not Found" };
            }

            if (string.IsNullOrEmpty(comServer.RemoteAccessUrl))
            {
                return new DtoActionResult { Success = false, ErrorMessage = "Could Not Initialize Remote Access.  The Url Was Empty" };
            }

            var servicePassGen = new PasswordGenerator();
            var remotelyUser = new RemotelyUser();
            remotelyUser.Username = servicePassGen.GeneratePassword(true, true, true, false, 10);

            for (int i = 0; i < 1000; i++)
            {
                remotelyUser.Password = servicePassGen.GeneratePassword(true, true, true, true, 16);
                if (servicePassGen.ValidatePassword(remotelyUser.Password, true))
                    break;
            }

            var response = new APICall().RemoteAccessApi.CreateRemotelyFirstUser(comServer.RemoteAccessUrl, remotelyUser);

            if (response == null)
            {
                return new DtoActionResult { Success = false, ErrorMessage = "Unknown Error While Initializing The Remote Access Server.  Check The Logs." };
            }

            var checkError = response;
            checkError = checkError.Replace("\"", "");
            checkError = checkError.Replace("\\", "");

            if (checkError.Equals("The Remote Access Server Has Already Been Initialized"))
                return new DtoActionResult() { Success = false, ErrorMessage = "The Remote Access Server Has Already Been Initialized" };

            try
            {
                var remotelyInfo = JsonConvert.DeserializeObject<RemotelyInfo>(response);
                comServer.RaUsername = remotelyUser.Username;
                comServer.RaPasswordEncrypted = ectx.Encryption.EncryptText(remotelyUser.Password);
                comServer.RaAuthHeaderEncrypted = ectx.Encryption.EncryptText(remotelyInfo.AuthHeader);
                comServer.RaOrganizationId = remotelyInfo.OrganizationID;
            }
            catch
            {
                return new DtoActionResult { Success = false, ErrorMessage = "Error: Could Not Deserialize Response." };
            }



            var result = clientComServerService.Update(comServer);
            if (result != null)
            {
                if (result.Success)
                {
                    CopyAgentInstallerToStorage();
                    return new DtoActionResult { Success = true };
                }
                else
                    return new DtoActionResult { Success = false, ErrorMessage = result.ErrorMessage };
            }
            return new DtoActionResult { Success = false, ErrorMessage = "Unknown Error" };
        }

        public string UpdateWebRtc(string deviceId, string rtcMode)
        {
            if (string.IsNullOrEmpty(deviceId))
            {
                return "Error: This Computer Does Not Have The Remote Control Agent Installed.";
            }
            var comServer = ectx.Uow.ClientComServerRepository.Get(x => x.IsRemoteAccessServer).FirstOrDefault();
            if (comServer == null)
            {
                return "Error: No Remote Access Com Servers Were Found";
            }
            var auth = ectx.Encryption.DecryptText(comServer.RaAuthHeaderEncrypted);
            var response = new APICall().RemoteAccessApi.RemotelyUpdateWebRtc(comServer.RemoteAccessUrl, deviceId,rtcMode, auth);
            var result = response.Replace("\"", "");
            result = result.Replace("\\", "");
            return result;
        }

        public string IsWebRtcEnabled(string deviceId)
        {
            if (string.IsNullOrEmpty(deviceId))
            {
                return "Error: This Computer Does Not Have The Remote Control Agent Installed.";
            }
            var comServer = ectx.Uow.ClientComServerRepository.Get(x => x.IsRemoteAccessServer).FirstOrDefault();
            if (comServer == null)
            {
                return "Error: No Remote Access Com Servers Were Found";
            }
            var auth = ectx.Encryption.DecryptText(comServer.RaAuthHeaderEncrypted);
            var response = new APICall().RemoteAccessApi.RemotelyIsWebRtcEnabled(comServer.RemoteAccessUrl, deviceId, auth);
            var result = response.Replace("\"", "");
            result = result.Replace("\\", "");
            return result;
        }

        public string IsDeviceOnline(string deviceId)
        {
            if (string.IsNullOrEmpty(deviceId))
            {
                return "Error: This Computer Does Not Have The Remote Control Agent Installed.";
            }
            var comServer = ectx.Uow.ClientComServerRepository.Get(x => x.IsRemoteAccessServer).FirstOrDefault();
            if (comServer == null)
            {
                return "Error: No Remote Access Com Servers Were Found";
            }
            var auth = ectx.Encryption.DecryptText(comServer.RaAuthHeaderEncrypted);
            var response = new APICall().RemoteAccessApi.RemotelyIsDeviceOnline(comServer.RemoteAccessUrl, deviceId, auth);
            var result = response.Replace("\"", "");
            result = result.Replace("\\", "");
            return result;
        }

        public string GetRemoteControlUrl(string deviceId)
        {
            if (string.IsNullOrEmpty(deviceId))
            {
                return "Error: This Computer Does Not Have The Remote Control Agent Installed.";
            }
            var comServer = ectx.Uow.ClientComServerRepository.Get(x => x.IsRemoteAccessServer).FirstOrDefault();
            if (comServer == null)
            {
                return "Error: No Remote Access Com Servers Were Found";
            }

            var auth = ectx.Encryption.DecryptText(comServer.RaAuthHeaderEncrypted);
            var response = new APICall().RemoteAccessApi.GetRemoteUrl(comServer.RemoteAccessUrl, deviceId, auth);
            var result = response.Replace("\"", "");
            result = result.Replace("\\", "");
            return result;

        }

        public DtoActionResult CopyAgentInstallerToStorage()
        {
            var comServer = ectx.Uow.ClientComServerRepository.Get(x => x.IsRemoteAccessServer).FirstOrDefault();
            if (comServer == null)
            {
                return new DtoActionResult() { Success = false, ErrorMessage = "No Remote Access Com Servers Were Found" };
            }

            DownloadRemotelyFile(comServer.RemoteAccessUrl, "Remotely_Installer.exe");
            DownloadRemotelyFile(comServer.RemoteAccessUrl, "Remotely-Win10-x64.zip");
            DownloadRemotelyFile(comServer.RemoteAccessUrl, "Remotely-Win10-x86.zip");

            return new DtoActionResult() { Success = true };
        }

        private void DownloadRemotelyFile(string remotelyUrl, string fileName)
        {
            var destinationDir = Path.Combine(ectx.Settings.GetSettingValue(SettingStrings.StoragePath), "software_uploads", "99999999-9999-9999-9999-999999999999");

           
                if (uncService.NetUseWithCredentials() || uncService.LastError == 1219)
                {
                    var directory = new DirectoryInfo(destinationDir);
                    try
                    {
                        if (!directory.Exists)
                            directory.Create();

                        using (var webClient = new WebClient())
                        {
                            webClient.DownloadFile(new Uri(remotelyUrl + "/Downloads/" + fileName), Path.Combine(destinationDir, fileName));
                        }
                    }
                    catch (Exception)
                    {

                        return;
                    }
                }
                else
                {
                    return;
                }
            


        }

        public DtoActionResult RunHealthCheck()
        {
            var comServer = ectx.Uow.ClientComServerRepository.Get(x => x.IsRemoteAccessServer).FirstOrDefault();
            if (comServer == null)
            {
                return new DtoActionResult() { ErrorMessage = "No Active Remote Access Servers Were Found" };
            }

            //check connection to remotely api
            var status = new APICall().RemoteAccessApi.RemotelyStatus(comServer.RemoteAccessUrl);
            if(status == null)
                return new DtoActionResult() { ErrorMessage = "Could Not Contact Remote Access API" };
            status = status.Replace("\"", "");
            status = status.Replace("\\", "");
            if(status != "true")
            {
                return new DtoActionResult() { ErrorMessage = "Could Not Contact Remote Access API" };
            }

            //check api connection with auth header, should return false with fake device id
            var auth = ectx.Encryption.DecryptText(comServer.RaAuthHeaderEncrypted);
            var online = new APICall().RemoteAccessApi.RemotelyIsDeviceOnline(comServer.RemoteAccessUrl, "abc", auth);
            if(online == null)
                return new DtoActionResult() { ErrorMessage = "Remote Access API Unauthorized" };
            online = online.Replace("\"", "");
            online = online.Replace("\\", "");
            if(online != "false")
                return new DtoActionResult() { ErrorMessage = "Remote Access API Unauthorized" };

            //verify files exist
            var basePath = Path.Combine(ectx.Settings.GetSettingValue(SettingStrings.StoragePath), "software_uploads", "99999999-9999-9999-9999-999999999999");

                if (uncService.NetUseWithCredentials() || uncService.LastError == 1219)
                {
                    try
                    {
                        var installer = Path.Combine(basePath, "Remotely_Installer.exe");
                        var x64 = Path.Combine(basePath, "Remotely-Win10-x64.zip");
                        var x86 = Path.Combine(basePath, "Remotely-Win10-x86.zip");

                        //no need to check if exists, exception will catch when looking for size
                        var hasSize = new FileInfo(installer).Length > 0;
                        if (!hasSize)
                            return new DtoActionResult() { ErrorMessage = "Remotely_Installer.exe Is Missing From Storage Path" };
                        hasSize = new FileInfo(x64).Length > 0;
                        if (!hasSize)
                            return new DtoActionResult() { ErrorMessage = "Remotely x64 File Is Missing From Storage Path" };
                        hasSize = new FileInfo(x86).Length > 0;
                        if (!hasSize)
                            return new DtoActionResult() { ErrorMessage = "Remotely x86 File Is Missing From Storage Path" };

                    }
                    catch
                    {
                        return new DtoActionResult() { ErrorMessage = "Remote Access Files Have Not Been Copied To The Storage Path" };
                    }
                    
                }
                else
                {
                    return new DtoActionResult() { ErrorMessage = "Could Not Determine If Remote Access Installer Files Are Available" };
                }
            
            return new DtoActionResult() { Success = true };
        }

    }
}
