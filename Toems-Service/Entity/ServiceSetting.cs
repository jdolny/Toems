using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using log4net;
using Toems_Common;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceSetting
    {
        private readonly UnitOfWork _uow;
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public ServiceSetting()
        {
            _uow = new UnitOfWork();
        }

        public EntitySetting GetSetting(string settingName)
        {

            var setting = _uow.SettingRepository.GetFirstOrDefault(s => s.Name == settingName);
            return setting;
        }

        public static string GetSettingValue(string settingName)
        {
            var setting = new ServiceSetting().GetSetting(settingName);
            return setting != null ? setting.Value : string.Empty;
        }

        public bool UpdatePxeSetting(List<EntitySetting> listSettings)
        {

            foreach (var setting in listSettings)
            {
                if (setting.Name != SettingStrings.IpxeRequiresLogin && setting.Name != SettingStrings.PxeBootloader && setting.Name != SettingStrings.ProxyDhcpEnabled && setting.Name != SettingStrings.ProxyBiosBootloader 
                    && setting.Name != SettingStrings.ProxyEfi32Bootloader && setting.Name != SettingStrings.ProxyEfi64Bootloader)
                    return false;

          
                _uow.SettingRepository.Update(setting, setting.Id);
            }
            _uow.Save();
            return true;
        }

        public bool UpdateSetting(List<EntitySetting> listSettings)
        {
          
            foreach (var setting in listSettings)
            {
                if (setting.Name == SettingStrings.StoragePassword || setting.Name == SettingStrings.SmtpPassword ||
                    setting.Name == SettingStrings.LdapBindPassword ||
                     setting.Name == SettingStrings.ProvisionKeyEncrypted || setting.Name == SettingStrings.IntercomKeyEncrypted || setting.Name == SettingStrings.RemoteAccessAdminPasswordEncrypted
                    || setting.Name == SettingStrings.RemoteAccessControlPasswordEncrypted || setting.Name == SettingStrings.RemoteAccessViewPasswordEncrypted)
                {
                    if (setting.Name == SettingStrings.ProvisionKeyEncrypted && string.IsNullOrEmpty(setting.Value))
                        continue;
                    else if (setting.Name == SettingStrings.ProvisionKeyEncrypted && !string.IsNullOrEmpty(setting.Value))
                    {
                        var isAllowed = ConfigurationManager.AppSettings["AllowProvisionKeyGen"];
                        if (!isAllowed.ToLower().Equals("true"))
                        {
                            Logger.Debug("Provision Key cannot be generated without updating the web.config key AllowProvisionKeyGen");
                            return false;
                        }
                    }
                    setting.Value = new EncryptionServices().EncryptText(setting.Value);
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
                _uow.SettingRepository.Update(setting, setting.Id);
            }
            _uow.Save();
            return true;
        }

        public string GetClientInstallArgs()
        {
            var certEntity = _uow.CertificateRepository.GetFirstOrDefault(x => x.Type == EnumCertificate.CertificateType.Authority);
            if (certEntity == null) return null;
            var pfx = new X509Certificate2(certEntity.PfxBlob, new EncryptionServices().DecryptText(certEntity.Password), X509KeyStorageFlags.Exportable);
            var thumbprint = pfx.Thumbprint;

            var provisionKeyEncrypted = GetSettingValue(SettingStrings.ProvisionKeyEncrypted);
            var provisionKey = new EncryptionServices().DecryptText(provisionKeyEncrypted);
            

            var defaultCluster = _uow.ComServerClusterRepository.GetFirstOrDefault(x => x.IsDefault);
            var clusterServers = _uow.ComServerClusterServerRepository.Get(x => x.ComServerClusterId == defaultCluster.Id);
            var comServers = "";
            foreach (var s in clusterServers)
            {
                var comServer = _uow.ClientComServerRepository.GetById(s.ComServerId);
                comServers += comServer.Url + ",";
            }

            return "SERVER_KEY=" + provisionKey + " CA_THUMBPRINT=" + thumbprint + " COM_SERVERS=" +
                   comServers.Trim(',');
        }

        public string GetMeshUserPass(string type)
        {
            switch (type)
            {
                case "admin":
                    return new EncryptionServices().DecryptText(GetSettingValue(SettingStrings.RemoteAccessAdminPasswordEncrypted));
                case "control":
                    return new EncryptionServices().DecryptText(GetSettingValue(SettingStrings.RemoteAccessControlPasswordEncrypted));
                case "view":
                    return new EncryptionServices().DecryptText(GetSettingValue(SettingStrings.RemoteAccessViewPasswordEncrypted));
                default:
                    return string.Empty;
            }
        }

        public bool CopyMsiToClientUpdate()
        {
            foreach (var type in new List<bool> {true,false })
            {
                var msi = new ServiceMsiUpdater().UpdateMsis(is64bit:type);
                var fileName = new ServiceMsiUpdater().GetNameForExport(is64bit:type);

                var destinationDir = Path.Combine(ServiceSetting.GetSettingValue(SettingStrings.StoragePath), "client_versions");

                using (var unc = new UncServices())
                {
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
                            Logger.Error(ex.Message);
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
           

            return true;
        }
    }
}
