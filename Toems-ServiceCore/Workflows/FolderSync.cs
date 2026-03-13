using RoboSharp;
using Toems_Common;
using Toems_DataModel;
using Toems_ServiceCore.EntityServices;
using Toems_ServiceCore.Infrastructure;

//https://code.msdn.microsoft.com/windowsdesktop/File-Sync-with-Simple-c497bf87/sourcecode?fileId=19013&pathId=1314099233

namespace Toems_ServiceCore.Workflows
{
    public class FolderSync(ServiceContext ctx)
    {
        public bool RunAllServers()
        {
           
            if (!ctx.Setting.GetSettingValue(SettingStrings.StorageType).Equals("SMB")) return true;
            if (ctx.Setting.GetSettingValue(SettingStrings.ReplicationInProgress).Equals("True"))
            {
                ctx.Log.Info("A Replication process is already in progress.");
                return true;
            }
            
            var comServers = ctx.Uow.ClientComServerRepository.Get(x => x.ReplicateStorage);
           
            var intercomKey = ctx.Setting.GetSettingValue(SettingStrings.IntercomKeyEncrypted);
            var decryptedKey = ctx.Encryption.DecryptText(intercomKey);
            foreach (var com in comServers)
            {
                ctx.Log.Info("Replicating Storage To Com Server, images will be replicated after this task. " + com.DisplayName);
                //todo - fix
                //new APICall().ClientComServerApi.SyncStorage(com.Url, "", decryptedKey);
                ctx.Log.Info("Finished Replicating Storage To Com Server " + com.DisplayName);
            }

            //sync images separately after the modules are synced
            ctx.ImageSync.RunAllServers();

            ctx.Log.Info("All replication tasks complete");
            return true;

        }

        public bool Sync()
        {
            if (!ctx.Setting.GetSettingValue(SettingStrings.StorageType).Equals("SMB")) return true;

            var guid = ctx.Config["ComServerUniqueId"];
            var thisComServer = ctx.ClientComServer.GetServerByGuid(guid);
            if (thisComServer == null)
            {
                ctx.Log.Error($"Com Server With Guid {guid} Not Found");
                return false;
            }


           

                if (ctx.Unc.NetUseWithCredentials() || ctx.Unc.LastError == 1219)
                {
                    foreach (var folder in new []{ "client_versions", "software_uploads" })
                    {
                        
                        RoboCommand backup = new RoboCommand();
                        // events
                        backup.OnError += Backup_OnError;

                        // copy options
                        backup.CopyOptions.Source = ctx.Setting.GetSettingValue(SettingStrings.StoragePath) + folder;
                        backup.CopyOptions.Destination = thisComServer.LocalStoragePath + folder.Trim('\\');
                        ctx.Log.Info($"Replicating Folder {folder} From {backup.CopyOptions.Source} to {backup.CopyOptions.Destination} on {thisComServer.DisplayName}");
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
                            ctx.Setting.UpdateSetting(SettingStrings.ReplicationInProgress, "True");
                            backup.Start().Wait();
                        }
                        catch(Exception ex)
                        {
                            ctx.Log.Error("Could Not Start Replication");
                            ctx.Log.Error(ex.Message);
                        }
                        finally 
                        {
                            ctx.Setting.UpdateSetting(SettingStrings.ReplicationInProgress, "False");
                        }
                        
                    }
                    return true;
                    
                }
                return false;
            
        }

        private void Backup_OnError(object sender, RoboSharp.ErrorEventArgs e)
        {
            ctx.Log.Error($"Folder Replication Failed.");
            ctx.Log.Error(e.ErrorCode + " " + e.Error);
            ctx.Setting.UpdateSetting(SettingStrings.ReplicationInProgress, "False");
        }
    }
}
