using System.Configuration;
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

            var uow = new UnitOfWork();
            var comServers = uow.ClientComServerRepository.Get(x => x.ReplicateStorage);
           
            var intercomKey = ServiceSetting.GetSettingValue(SettingStrings.IntercomKeyEncrypted);
            var decryptedKey = new EncryptionServices().DecryptText(intercomKey);
            foreach (var com in comServers)
            {
                new APICall().ClientComServerApi.SyncStorage(com.Url, "", decryptedKey);
            }

            return true;

        }

        public bool Sync()
        {
            if (!ServiceSetting.GetSettingValue(SettingStrings.StorageType).Equals("SMB")) return true;

            using (var unc = new UncServices())
            {
                if (unc.NetUseWithCredentials() || unc.LastError == 1219)
                {
                    RoboCommand backup = new RoboCommand();
                    // events
                    backup.OnFileProcessed += backup_OnFileProcessed;
                    backup.OnCommandCompleted += backup_OnCommandCompleted;
                    // copy options
                    backup.CopyOptions.Source = ServiceSetting.GetSettingValue(SettingStrings.StoragePath);
                    backup.CopyOptions.Destination = ConfigurationManager.AppSettings["LocalStoragePath"].Trim('\\');
                    backup.CopyOptions.CopySubdirectories = true;
                    backup.CopyOptions.UseUnbufferedIo = true;
                    // select options
                    //backup.SelectionOptions.OnlyCopyArchiveFilesAndResetArchiveFlag = true;
                    backup.CopyOptions.Mirror = true;
                    backup.CopyOptions.Purge = false;
                    backup.SelectionOptions.ExcludeOlder = true;
                    backup.LoggingOptions.VerboseOutput = true;
                    // retry options
                    backup.RetryOptions.RetryCount = 1;
                    backup.RetryOptions.RetryWaitTime = 2;
                    backup.Start();
                    return true;
                }
                return false;
            }
        }

        void backup_OnFileProcessed(object sender, FileProcessedEventArgs e)
        {

            Logger.Info(e.ProcessedFile.FileClass);
            Logger.Info(e.ProcessedFile.Name);
            Logger.Info(e.ProcessedFile.Size.ToString());
           
        }

        void backup_OnCommandCompleted(object sender, RoboCommandCompletedEventArgs e)
        {
        Logger.Info(e.ToString() + "Complete");
            
        }

       
    }
}
