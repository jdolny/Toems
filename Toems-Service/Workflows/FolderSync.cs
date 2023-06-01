using System;
using System.Configuration;
using System.IO;
using log4net;
using RoboSharp;
using Toems_ApiCalls;
using Toems_Common;
using Toems_DataModel;
using Toems_Service.Entity;

//https://code.msdn.microsoft.com/windowsdesktop/File-Sync-with-Simple-c497bf87/sourcecode?fileId=19013&pathId=1314099233

namespace Toems_Service.Workflows
{
    public class FolderSync
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public bool RunAllServers()
        {
           
            if (!ServiceSetting.GetSettingValue(SettingStrings.StorageType).Equals("SMB")) return true;
            if (ServiceSetting.GetSettingValue(SettingStrings.ReplicationInProgress).Equals("True"))
            {
                Logger.Info("A Replication process is already in progress.");
                return true;
            }

            var uow = new UnitOfWork();
            var comServers = uow.ClientComServerRepository.Get(x => x.ReplicateStorage);
           
            var intercomKey = ServiceSetting.GetSettingValue(SettingStrings.IntercomKeyEncrypted);
            var decryptedKey = new EncryptionServices().DecryptText(intercomKey);
            foreach (var com in comServers)
            {
                Logger.Info("Replicating Storage To Com Server, images will be replicated after this task. " + com.DisplayName);
                new APICall().ClientComServerApi.SyncStorage(com.Url, "", decryptedKey);
                Logger.Info("Finished Replicating Storage To Com Server " + com.DisplayName);
            }

            //sync images separately after the modules are synced
            new ImageSync().RunAllServers();

            Logger.Info("All replication tasks complete");
            return true;

        }

        public bool Sync()
        {
            if (!ServiceSetting.GetSettingValue(SettingStrings.StorageType).Equals("SMB")) return true;

            var guid = ConfigurationManager.AppSettings["ComServerUniqueId"];
            var thisComServer = new ServiceClientComServer().GetServerByGuid(guid);
            if (thisComServer == null)
            {
                Logger.Error($"Com Server With Guid {guid} Not Found");
                return false;
            }


           
            using (var unc = new UncServices())
            {
                if (unc.NetUseWithCredentials() || unc.LastError == 1219)
                {
                    foreach (var folder in new []{ "client_versions", "software_uploads" })
                    {
                        
                        RoboCommand backup = new RoboCommand();
                        // events
                        backup.OnError += Backup_OnError;

                        // copy options
                        backup.CopyOptions.Source = ServiceSetting.GetSettingValue(SettingStrings.StoragePath) + folder;
                        backup.CopyOptions.Destination = thisComServer.LocalStoragePath + folder.Trim('\\');
                        Logger.Info($"Replicating Folder {folder} From {backup.CopyOptions.Source} to {backup.CopyOptions.Destination} on {thisComServer.DisplayName}");
                        backup.CopyOptions.CopySubdirectories = true;
                        backup.CopyOptions.UseUnbufferedIo = true;
                        if (thisComServer.ReplicationRateIpg != 0)
                            backup.CopyOptions.InterPacketGap = thisComServer.ReplicationRateIpg;
                        else
                            backup.CopyOptions.InterPacketGap = 0;
                        // select options
                        backup.CopyOptions.Mirror = true;
                        backup.CopyOptions.Purge = true;

                        backup.LoggingOptions.VerboseOutput = false;
                        // retry options
                        backup.RetryOptions.RetryCount = 3;
                        backup.RetryOptions.RetryWaitTime = 10;
                        try
                        {
                            new ServiceSetting().UpdateSetting(SettingStrings.ReplicationInProgress, "True");
                            backup.Start().Wait();
                        }
                        catch(Exception ex)
                        {
                            Logger.Error("Could Not Start Replication");
                            Logger.Error(ex.Message);
                        }
                        finally 
                        {
                            new ServiceSetting().UpdateSetting(SettingStrings.ReplicationInProgress, "False");
                        }
                        
                    }
                    return true;
                    
                }
                return false;
            }
        }

        private void Backup_OnError(object sender, RoboSharp.ErrorEventArgs e)
        {
            Logger.Error($"Folder Replication Failed.");
            Logger.Error(e.ErrorCode + " " + e.Error);
            new ServiceSetting().UpdateSetting(SettingStrings.ReplicationInProgress, "False");
        }
    }
}
