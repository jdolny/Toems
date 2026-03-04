using System;
using System.Configuration;
using System.IO;
using log4net;
using RoboSharp;
using Toems_ApiCalls;
using Toems_Common;
using Toems_DataModel;
using Toems_Service.Entity;
using Toems_ServiceCore;
using Toems_ServiceCore.EntityServices;
using Toems_ServiceCore.Infrastructure;

//https://code.msdn.microsoft.com/windowsdesktop/File-Sync-with-Simple-c497bf87/sourcecode?fileId=19013&pathId=1314099233

namespace Toems_Service.Workflows
{
    public class FolderSync(InfrastructureContext ictx, ServiceClientComServer serviceClientComServer, UncServices uncServices, ImageSync imageSync)
    {
        public bool RunAllServers()
        {
           
            if (!ictx.Settings.GetSettingValue(SettingStrings.StorageType).Equals("SMB")) return true;
            if (ictx.Settings.GetSettingValue(SettingStrings.ReplicationInProgress).Equals("True"))
            {
                ictx.Log.Info("A Replication process is already in progress.");
                return true;
            }

            var uow = new UnitOfWork();
            var comServers = uow.ClientComServerRepository.Get(x => x.ReplicateStorage);
           
            var intercomKey = ictx.Settings.GetSettingValue(SettingStrings.IntercomKeyEncrypted);
            var decryptedKey = ictx.Encryption.DecryptText(intercomKey);
            foreach (var com in comServers)
            {
                ictx.Log.Info("Replicating Storage To Com Server, images will be replicated after this task. " + com.DisplayName);
                new APICall().ClientComServerApi.SyncStorage(com.Url, "", decryptedKey);
                ictx.Log.Info("Finished Replicating Storage To Com Server " + com.DisplayName);
            }

            //sync images separately after the modules are synced
            imageSync.RunAllServers();

            ictx.Log.Info("All replication tasks complete");
            return true;

        }

        public bool Sync()
        {
            if (!ictx.Settings.GetSettingValue(SettingStrings.StorageType).Equals("SMB")) return true;

            var guid = ictx.Config["ComServerUniqueId"];
            var thisComServer = serviceClientComServer.GetServerByGuid(guid);
            if (thisComServer == null)
            {
                ictx.Log.Error($"Com Server With Guid {guid} Not Found");
                return false;
            }


           

                if (uncServices.NetUseWithCredentials() || uncServices.LastError == 1219)
                {
                    foreach (var folder in new []{ "client_versions", "software_uploads" })
                    {
                        
                        RoboCommand backup = new RoboCommand();
                        // events
                        backup.OnError += Backup_OnError;

                        // copy options
                        backup.CopyOptions.Source = ictx.Settings.GetSettingValue(SettingStrings.StoragePath) + folder;
                        backup.CopyOptions.Destination = thisComServer.LocalStoragePath + folder.Trim('\\');
                        ictx.Log.Info($"Replicating Folder {folder} From {backup.CopyOptions.Source} to {backup.CopyOptions.Destination} on {thisComServer.DisplayName}");
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
                            ictx.Settings.UpdateSetting(SettingStrings.ReplicationInProgress, "True");
                            backup.Start().Wait();
                        }
                        catch(Exception ex)
                        {
                            ictx.Log.Error("Could Not Start Replication");
                            ictx.Log.Error(ex.Message);
                        }
                        finally 
                        {
                            ictx.Settings.UpdateSetting(SettingStrings.ReplicationInProgress, "False");
                        }
                        
                    }
                    return true;
                    
                }
                return false;
            
        }

        private void Backup_OnError(object sender, RoboSharp.ErrorEventArgs e)
        {
            ictx.Log.Error($"Folder Replication Failed.");
            ictx.Log.Error(e.ErrorCode + " " + e.Error);
            ictx.Settings.UpdateSetting(SettingStrings.ReplicationInProgress, "False");
        }
    }
}
