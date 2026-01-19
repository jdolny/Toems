using System.Security.Cryptography.X509Certificates;
using log4net;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_DataModel;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceSetting(ILog log, IConfigurationManager config, EncryptionServices encryption, UncServices unc, ServiceMsiUpdater msiUpdater, UnitOfWork uow)
    {
        public EntitySetting GetSetting(string settingName)
        {

            var setting = uow.SettingRepository.GetFirstOrDefault(s => s.Name == settingName);
            return setting;
        }

        public string GetSettingValue(string settingName)
        {
            var setting = GetSetting(settingName);
            return setting != null ? setting.Value : string.Empty;
        }

        public DtoDomainJoinCredentials GetDomainJoinCredentials()
        {
            var creds = new DtoDomainJoinCredentials();
            //get domain join creds
            creds.Username = GetSettingValue(SettingStrings.DomainJoinUser);
            creds.Domain = GetSettingValue(SettingStrings.DomainJoinName);
            creds.Password = encryption.DecryptText(GetSettingValue(SettingStrings.DomainJoinPasswordEncrypted));
            return creds;
        }
        public bool UpdatePxeSetting(List<EntitySetting> listSettings)
        {

            foreach (var setting in listSettings)
            {
                if (setting.Name != SettingStrings.IpxeRequiresLogin && setting.Name != SettingStrings.PxeBootloader && setting.Name != SettingStrings.ProxyDhcpEnabled && setting.Name != SettingStrings.ProxyBiosBootloader 
                    && setting.Name != SettingStrings.ProxyEfi32Bootloader && setting.Name != SettingStrings.ProxyEfi64Bootloader)
                    return false;

          
                uow.SettingRepository.Update(setting, setting.Id);
            }
            uow.Save();
            return true;
        }

        public bool UpdateSetting(string name, string value)
        {
            var setting = new EntitySetting();
            setting.Name = name;
            setting.Value = value;
            setting.Id = GetSetting(name).Id;
            uow.SettingRepository.Update(setting, setting.Id);
            uow.Save();
            return true;
        }
        public bool UpdateSetting(List<EntitySetting> listSettings)
        {
          
            foreach (var setting in listSettings)
            {
                if (setting.Name == SettingStrings.StoragePassword || setting.Name == SettingStrings.SmtpPassword ||
                    setting.Name == SettingStrings.LdapBindPassword ||
                     setting.Name == SettingStrings.ProvisionKeyEncrypted || setting.Name == SettingStrings.IntercomKeyEncrypted || setting.Name == SettingStrings.RemoteAccessAdminPasswordEncrypted
                    || setting.Name == SettingStrings.RemoteAccessControlPasswordEncrypted || setting.Name == SettingStrings.RemoteAccessViewPasswordEncrypted || setting.Name == SettingStrings.DomainJoinPasswordEncrypted)
                {
                    if (setting.Name == SettingStrings.ProvisionKeyEncrypted && string.IsNullOrEmpty(setting.Value))
                        continue;
                    else if (setting.Name == SettingStrings.ProvisionKeyEncrypted && !string.IsNullOrEmpty(setting.Value))
                    {
                        var isAllowed = config["AllowProvisionKeyGen"];
                        if (!isAllowed.ToLower().Equals("true"))
                        {
                            log.Debug("Provision Key cannot be generated without updating the web.config key AllowProvisionKeyGen");
                            return false;
                        }
                    }
                    setting.Value = encryption.EncryptText(setting.Value);
                }

                if(setting.Name == SettingStrings.StoragePath)
                {
                    if (!string.IsNullOrEmpty(setting.Value))
                    {
                        if (setting.Value.StartsWith(@"\\"))
                        {
                            //unc path, ensure it ends with backslash
                            if (!setting.Value.EndsWith(@"\"))
                                setting.Value += @"\";
                        }
                        else
                        {
                            //local path, use os seperator char
                            if (!setting.Value.EndsWith(Path.DirectorySeparatorChar.ToString()))
                                setting.Value += Path.DirectorySeparatorChar.ToString();
                        }
                    }
                }
                uow.SettingRepository.Update(setting, setting.Id);
            }
            uow.Save();
            return true;
        }

        public string GetClientInstallArgs()
        {
            var certEntity = uow.CertificateRepository.GetFirstOrDefault(x => x.Type == EnumCertificate.CertificateType.Authority);
            if (certEntity == null) return null;
            var pass = encryption.DecryptText(certEntity.Password);
            var pfx = new X509Certificate2(certEntity.PfxBlob, pass, X509KeyStorageFlags.Exportable);
            var thumbprint = pfx.Thumbprint;

            var provisionKeyEncrypted = GetSettingValue(SettingStrings.ProvisionKeyEncrypted);
            var provisionKey = encryption.DecryptText(provisionKeyEncrypted);
            

            var defaultCluster = uow.ComServerClusterRepository.GetFirstOrDefault(x => x.IsDefault);
            var clusterServers = uow.ComServerClusterServerRepository.Get(x => x.ComServerClusterId == defaultCluster.Id && x.IsEndpointManagementServer);
            var comServers = "";
            foreach (var s in clusterServers)
            {
                var comServer = uow.ClientComServerRepository.GetById(s.ComServerId);
                comServers += comServer.Url + ",";
            }

            return "SERVER_KEY=" + provisionKey + " CA_THUMBPRINT=" + thumbprint + " COM_SERVERS=" +
                   comServers.Trim(',');
        }

     

        public bool CopyMsiToClientUpdate()
        {
            foreach (var type in new List<bool> { true, false })
            {
                var msi = msiUpdater.UpdateMsis(is64bit: type);
                var fileName = msiUpdater.GetNameForExport(is64bit: type);

                var destinationDir = Path.Combine(GetSettingValue(SettingStrings.StoragePath), "client_versions");
                
                if (unc.NetUseWithCredentials() || unc.LastError == 1219)
                {
                    var directory = new DirectoryInfo(destinationDir);
                    try
                    {
                        if (!directory.Exists)
                            directory.Create();

                        File.WriteAllBytes(Path.Combine(destinationDir, fileName), msi);

                    }
                    catch (Exception ex)
                    {
                        log.Error(ex.Message);
                        return false;
                    }
                }
                else
                {
                    return false;
                }

            }
            return true;
        }
    }
}
