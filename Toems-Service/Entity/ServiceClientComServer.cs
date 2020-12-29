using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Web.Security;
using Toems_ApiCalls;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceClientComServer
    {
        private readonly UnitOfWork _uow;

        public ServiceClientComServer()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult Add(EntityClientComServer clientServer)
        {
            var actionResult = new DtoActionResult();
            if (!clientServer.Url.EndsWith("/"))
                clientServer.Url += "/";
            clientServer.Url = clientServer.Url.ToLower();
            if (!string.IsNullOrEmpty(clientServer.LocalStoragePath))
            {
                if (!clientServer.LocalStoragePath.EndsWith(Path.DirectorySeparatorChar.ToString()))
                    clientServer.LocalStoragePath += Path.DirectorySeparatorChar.ToString();
            }
            if (!string.IsNullOrEmpty(clientServer.TftpPath))
            {
                if (!clientServer.TftpPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
                    clientServer.TftpPath += Path.DirectorySeparatorChar.ToString();
            }
            if(!string.IsNullOrEmpty(clientServer.RemoteAccessUrl))
            {
                if (!clientServer.RemoteAccessUrl.EndsWith("/"))
                    clientServer.RemoteAccessUrl += "/";
            }


            clientServer.DecompressImageOn = "client";
            clientServer.EmMaxBps = 0;
            clientServer.EmMaxClients = 0;
            clientServer.ImagingMaxBps = 0;
            clientServer.ImagingMaxClients = 5;
            clientServer.IsEndpointManagementServer = true;
            clientServer.IsImagingServer = true;
            clientServer.IsMulticastServer = true;
            clientServer.IsTftpServer = true;
            clientServer.MulticastEndPort = 10000;
            clientServer.MulticastStartPort = 9000;
            clientServer.ReplicateStorage = true;
            clientServer.ReplicationRateIpg = 0;
            clientServer.TftpPath = @"C:\Program Files\Theopenem\tftpboot\";
            clientServer.IsTftpInfoServer = true;
            clientServer.UniqueId = Guid.NewGuid().ToString();

            var validationResult = Validate(clientServer, true);
            if (validationResult.Success)
            {
                _uow.ClientComServerRepository.Insert(clientServer);
                _uow.Save();
                actionResult.Success = true;
                actionResult.Id = clientServer.Id;
            }
            else
            {
                return new DtoActionResult() { ErrorMessage = validationResult.ErrorMessage };
            }

            return actionResult;
        }

        public DtoActionResult Delete(int clientServerId)
        {
            var u = GetServer(clientServerId);
            if (u == null) return new DtoActionResult { ErrorMessage = "Com Server Not Found", Id = 0 };
            _uow.ClientComServerRepository.Delete(clientServerId);
            _uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.Id;
            return actionResult;
        }

        public EntityClientComServer GetServer(int clientServerId)
        {
            return _uow.ClientComServerRepository.GetById(clientServerId);
        }

        public EntityClientComServer GetServerByGuid(string Guid)
        {
            return _uow.ClientComServerRepository.Get(x => x.UniqueId.Equals(Guid)).FirstOrDefault();
        }

        public List<EntityClientComServer> GetAll()
        {
            return _uow.ClientComServerRepository.Get();
        }

        public List<EntityClientComServer> Search(DtoSearchFilter filter)
        {
            return _uow.ClientComServerRepository.Get(x => x.DisplayName.Contains(filter.SearchText));
        }

        public string TotalCount()
        {
            return _uow.ClientComServerRepository.Count();
        }

        public DtoActionResult Update(EntityClientComServer clientServer)
        {
            var u = GetServer(clientServer.Id);
            if (u == null) return new DtoActionResult { ErrorMessage = "Com Server Not Found", Id = 0 };
            var actionResult = new DtoActionResult();
            if (!clientServer.Url.EndsWith("/"))
                clientServer.Url += "/";
            clientServer.Url = clientServer.Url.ToLower();
            if (!string.IsNullOrEmpty(clientServer.LocalStoragePath))
            {
                if (!clientServer.LocalStoragePath.EndsWith(Path.DirectorySeparatorChar.ToString()))
                    clientServer.LocalStoragePath += Path.DirectorySeparatorChar.ToString();
            }
            if (!string.IsNullOrEmpty(clientServer.TftpPath))
            {
                if (!clientServer.TftpPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
                    clientServer.TftpPath += Path.DirectorySeparatorChar.ToString();
            }
            if (!string.IsNullOrEmpty(clientServer.RemoteAccessUrl))
            {
                if (!clientServer.RemoteAccessUrl.EndsWith("/"))
                    clientServer.RemoteAccessUrl += "/";
            }

            var validationResult = Validate(clientServer, false);
            if (validationResult.Success)
            {
                _uow.ClientComServerRepository.Update(clientServer, u.Id);
                _uow.Save();
                actionResult.Success = true;
                actionResult.Id = clientServer.Id;
            }
            else
            {
                return new DtoActionResult() { ErrorMessage = validationResult.ErrorMessage };
            }

            return actionResult;
        }

        public DtoValidationResult Validate(EntityClientComServer comServer, bool isNew)
        {
            var validationResult = new DtoValidationResult { Success = true };

            if (string.IsNullOrEmpty(comServer.DisplayName) || !comServer.DisplayName.All(c => char.IsLetterOrDigit(c) || c == '_' || c == '-' || c == ' '))
            {
                validationResult.Success = false;
                validationResult.ErrorMessage = "Com Server Name Is Not Valid";
                return validationResult;
            }

            if (isNew)
            {
                if (_uow.ClientComServerRepository.Exists(h => h.DisplayName == comServer.DisplayName))
                {
                    validationResult.Success = false;
                    validationResult.ErrorMessage = "A Com Server With This Name Already Exists";
                    return validationResult;
                }
            }
            else
            {
                var original = _uow.ClientComServerRepository.GetById(comServer.Id);
                if (original.DisplayName != comServer.DisplayName)
                {
                    if (_uow.ClientComServerRepository.Exists(h => h.DisplayName == comServer.DisplayName))
                    {
                        validationResult.Success = false;
                        validationResult.ErrorMessage = "A Com Server With This Name Already Exists";
                        return validationResult;
                    }
                }
            }

            //remove for now, it's possible that you might want to com servers on the same server and still wouldn't need smb
            /*
            var comServerCount = Convert.ToInt32(TotalCount());
            if(comServerCount > 0)
            {
                //verify storage type before allowing more than one com server
                var storageType = ServiceSetting.GetSettingValue(SettingStrings.StorageType);
                if(storageType.Equals("Local"))
                {
                    validationResult.Success = false;
                    validationResult.ErrorMessage = "Could Not Add Server.  If Using More Than 1 Com Server, The Storage Type Must Be Set To SMB In Admin Settings->Storage Location";
                    return validationResult;
                }
            }*/

            if(string.IsNullOrEmpty(comServer.LocalStoragePath))
            {
                validationResult.Success = false;
                validationResult.ErrorMessage = "Local Storage Path Must Be Populated.  If Global Storage is set to Local, verify Admin Settings-> Storage Location is populated.";
                return validationResult;
            }

            Regex r = new Regex(@"^(?<proto>\w+)://[^/]+?(?<port>:\d+)?/",
                                     RegexOptions.None, TimeSpan.FromMilliseconds(150));
            Match m = r.Match(comServer.Url);
            if (m.Success)
            {
                var port = r.Match(comServer.Url).Result("${port}");
                if (string.IsNullOrEmpty(port))
                {
                    validationResult.Success = false;
                    validationResult.ErrorMessage = "The URL Must Include The Port Number";
                    return validationResult;
                }
            }
            else
            {
                validationResult.Success = false;
                validationResult.ErrorMessage = "The URL Is Invalid";
                return validationResult;
            }


            return validationResult;


        }

        public byte[] GenerateComCert(int comServerId)
        {
            var comServer = GetServer(comServerId);
            if (comServer == null) return null;

            var iCert = new ServiceCertificate().GetIntermediate();
            var site = new Uri(comServer.Url);
            var intermediateEntity = new ServiceCertificate().GetIntermediateEntity();
            var pass = new EncryptionServices().DecryptText(intermediateEntity.Password);
            var intermediateCert = new X509Certificate2(intermediateEntity.PfxBlob, pass, X509KeyStorageFlags.Exportable);
            var certRequest = new CertificateRequest();
            var organization = ServiceSetting.GetSettingValue(SettingStrings.CertificateOrganization);
            certRequest.SubjectName = string.Format($"CN={site.Host}");
            certRequest.NotBefore = DateTime.UtcNow;
            certRequest.NotAfter = certRequest.NotBefore.AddYears(10);
            var certificate = new ServiceGenerateCertificate(certRequest).IssueCertificate(intermediateCert, false, true);

            var bytes = certificate.Export(X509ContentType.Pfx);

            return bytes;
        }

        public byte[] GenerateRemoteAccessCert(int comServerId)
        {
            var comServer = GetServer(comServerId);
            if (comServer == null) return null;

            var iCert = new ServiceCertificate().GetIntermediate();
            var site = new Uri(comServer.RemoteAccessUrl);
            var intermediateEntity = new ServiceCertificate().GetIntermediateEntity();
            var pass = new EncryptionServices().DecryptText(intermediateEntity.Password);
            var intermediateCert = new X509Certificate2(intermediateEntity.PfxBlob, pass, X509KeyStorageFlags.Exportable);
            var certRequest = new CertificateRequest();
            var organization = ServiceSetting.GetSettingValue(SettingStrings.CertificateOrganization);
            certRequest.SubjectName = string.Format($"CN={site.Host}");
            certRequest.NotBefore = DateTime.UtcNow;
            certRequest.NotAfter = certRequest.NotBefore.AddYears(10);
            var certificate = new ServiceGenerateCertificate(certRequest).IssueCertificate(intermediateCert, false, true);

            var bytes = certificate.Export(X509ContentType.Pfx);

            return bytes;
        }

        public List<DtoReplicationProcess> GetReplicationProcesses(int comServerId)
        {
            var comServer = _uow.ClientComServerRepository.GetById(comServerId);
            var intercomKey = ServiceSetting.GetSettingValue(SettingStrings.IntercomKeyEncrypted);
            var decryptedKey = new EncryptionServices().DecryptText(intercomKey);

            return new APICall().ClientComServerApi.GetReplicationProcesses(comServer.Url, "", decryptedKey);
        }

        public bool KillProcess(int comServerId, int pid)
        {
            var comServer = _uow.ClientComServerRepository.GetById(comServerId);
            var intercomKey = ServiceSetting.GetSettingValue(SettingStrings.IntercomKeyEncrypted);
            var decryptedKey = new EncryptionServices().DecryptText(intercomKey);

            return new APICall().ClientComServerApi.KillProcess(comServer.Url, "", decryptedKey,pid);

        }

     


    }
}