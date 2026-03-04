using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using log4net;
using RoboSharp;
using Toems_ApiCalls;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;
using Toems_Service.Entity;
using Toems_ServiceCore;
using Toems_ServiceCore.EntityServices;
using Toems_ServiceCore.Infrastructure;
using ErrorEventArgs = RoboSharp.ErrorEventArgs;

//https://code.msdn.microsoft.com/windowsdesktop/File-Sync-with-Simple-c497bf87/sourcecode?fileId=19013&pathId=1314099233

namespace Toems_Service.Workflows
{
    public class ImageSync(InfrastructureContext ictx, UncServices uncServices, ServiceDefaultReplicationServer serviceDefaultReplicationServer,
        ServiceClientComServer serviceClientComServer, ServiceImage serviceImage)
    {
        
        public bool RunAllServers()
        {
            if (ictx.Settings.GetSettingValue(SettingStrings.ReplicationInProgress).Equals("True"))
            {
                ictx.Log.Info("A Replication process is already in progress.");
                return true;
            }
            FromCom();
            ToCom();

            return true;
        }

        private bool ToCom()
        {
            var uow = new UnitOfWork();
            var imagesToReplicate = new List<EntityImage>();
            ictx.Log.Info("Starting Image Replication To Com Servers");
            if (!ictx.Settings.GetSettingValue(SettingStrings.StorageType).Equals("SMB")) 
            {
                ictx.Log.Info("Image replication is not used when storage type is set to local.");
                return true;
            }
            if (ictx.Settings.GetSettingValue(SettingStrings.ImageDirectSmb).Equals("True"))
            {
                ictx.Log.Info("Image replication is not used when direct smb imaging is enabled.");
                return true; //Don't need to sync images when direct to smb is used.
            }

            //find all images that need copied from SMB share to com servers
            var completedImages = uow.ImageRepository.Get(x => !string.IsNullOrEmpty(x.LastUploadGuid));
            if (completedImages == null)
            {
                ictx.Log.Info("No Images Found To Replicate");
                return true;
            }

            if (completedImages.Count == 0)
            {
                ictx.Log.Info("No Images Found To Replicate");
                return true;
            }

            //check if images already exist on smb share
            var basePath = ictx.Settings.GetSettingValue(SettingStrings.StoragePath);

                if (uncServices.NetUseWithCredentials() || uncServices.LastError == 1219)
                {
                    foreach (var image in completedImages)
                    {
                        if (string.IsNullOrEmpty(image.Name))
                            continue; //should never happen, but could potential erase all images if so, when syncing

                        var imagePath = Path.Combine(basePath, "images", image.Name);
                        if (!Directory.Exists(imagePath))
                        {
                            imagesToReplicate.Add(image);
                            continue;
                        }
                        var guidPath = Path.Combine(imagePath, "guid");
                        if (!File.Exists(guidPath))
                        {
                            imagesToReplicate.Add(image);
                            continue;
                        }
                        using (StreamReader reader = new StreamReader(guidPath))
                        {
                            var fileGuid = reader.ReadLine() ?? "";
                            if (fileGuid.Equals(image.LastUploadGuid))
                            {
                                //image on smb share matches what database says, this is the master and should be replicated to com servers 
                                imagesToReplicate.Add(image);
                                continue;
                            }
                        }
                    }
                }
                else
                {
                    return false;
                }
            

            if (imagesToReplicate.Count == 0)
            {
                ictx.Log.Info("No Images Found To Replicate");
                return true;
            }

            //find com servers that need this image

            var comServers = uow.ClientComServerRepository.Get(x => x.IsImagingServer && x.ReplicateStorage);
            var intercomKey = ictx.Settings.GetSettingValue(SettingStrings.IntercomKeyEncrypted);
            var decryptedKey = ictx.Encryption.DecryptText(intercomKey);
            var comImageList = new List<DtoRepImageCom>();

            var globalReplicationMode = ictx.Settings.GetSettingValue(SettingStrings.DefaultImageReplication);
            var globalReplicationServers = serviceDefaultReplicationServer.GetDefaultImageReplicationComServers();
            foreach (var com in comServers)
            {
                ictx.Log.Debug("Checking if com server " + com.DisplayName + " Needs any images");
                foreach (var image in imagesToReplicate)
                {
                    ictx.Log.Debug("Checking " + image.Name);
                    if (image.ReplicationMode == Toems_Common.Enum.EnumImageReplication.ReplicationType.None || (image.ReplicationMode == Toems_Common.Enum.EnumImageReplication.ReplicationType.GlobalDefault && globalReplicationMode == "None"))
                    {
                        ictx.Log.Debug("Image is not set to replicate to any com servers, skipping");
                        continue;
                    }
                    else if (image.ReplicationMode == Toems_Common.Enum.EnumImageReplication.ReplicationType.Selective)
                    {
                        var imageReplicationServers = uow.ImageReplicationServerRepository.Get(x => x.ImageId == image.Id && x.ComServerId == com.Id);
                        if (!imageReplicationServers.Any())
                        {
                            ictx.Log.Debug("Image is not set to replicate to this com server, skipping");
                            continue;
                        }
                    }
                    else if (image.ReplicationMode == Toems_Common.Enum.EnumImageReplication.ReplicationType.GlobalDefault && globalReplicationMode == "Selective")
                    {
                        var imageReplicationServers = uow.DefaultImageReplicationServerRepository.Get(x => x.ComServerId == com.Id);
                        if (!imageReplicationServers.Any())
                        {
                            ictx.Log.Debug("Image is not set to replicate to this com server, skipping");
                            continue;
                        }
                    }
                    var hasImage = new APICall().ClientComServerApi.CheckImageExists(com.Url, "", decryptedKey, image.Id);
                    if (hasImage)
                    {
                        ictx.Log.Debug("Com server already has this image, skipping.");
                        continue; //already has image move to next com server

                    }
                    else
                    {
                        ictx.Log.Info("Adding " + image.Name + " to replication list for " + com.DisplayName);
                        var comImage = new DtoRepImageCom();
                        comImage.ComServerId = com.Id;
                        comImage.ImageId = image.Id;
                        comImageList.Add(comImage);

                    }

                }
            }

            if (comImageList.Count == 0)
            {
                ictx.Log.Info("Com Server Images Are All Up To Date.  Skipping Replication.");
                return true;
            }

            foreach (var c in comImageList.GroupBy(x => x.ComServerId))
            {
                var comServerId = c.First().ComServerId;
                var thisComImageList = new List<int>();
                foreach (var imageId in c)
                    thisComImageList.Add(imageId.ImageId);

                var comServer = serviceClientComServer.GetServer(comServerId);
                ictx.Log.Info("Starting Robocopy on " + comServer.DisplayName);
                new APICall().ClientComServerApi.SyncSmbToCom(comServer.Url, "", decryptedKey, thisComImageList);
                ictx.Log.Info("Finished Image Replication on " + comServer.DisplayName);
            }
            return true;
        }

        private bool FromCom()
        {
            var uow = new UnitOfWork();
            var imagesToReplicate = new List<EntityImage>();
            ictx.Log.Info("Starting Image Replication From Com Servers To SMB Share");
            if (!ictx.Settings.GetSettingValue(SettingStrings.StorageType).Equals("SMB")) return true;
            if (ictx.Settings.GetSettingValue(SettingStrings.ImageDirectSmb).Equals("True"))
            {
                ictx.Log.Info("Image replication is not used when direct smb imaging is enabled.");
                return true; //Don't need to sync images when direct to smb is used.
            }
            //find all images that need copied from com servers to the SMB share
            var completedImages = uow.ImageRepository.Get(x => !string.IsNullOrEmpty(x.LastUploadGuid));
            if (completedImages == null)
            {
                ictx.Log.Info("No Images Found To Replicate");
                return true;
            }

            if (completedImages.Count == 0)
            {
                ictx.Log.Info("No Images Found To Replicate");
                return true;
            }

            //check if images already exist on smb share
            var basePath = ictx.Settings.GetSettingValue(SettingStrings.StoragePath);

                if (uncServices.NetUseWithCredentials() || uncServices.LastError == 1219)
                {
                    if (!Directory.Exists(Path.Combine(basePath, "images")))
                    {
                        try
                        {
                            Directory.CreateDirectory(Path.Combine(basePath, "images"));
                        }
                        catch
                        {
                            ictx.Log.Error("Could Not Sync Images.  The images folder could not be created on smb share");
                            return false;
                        }
                    }
                    foreach (var image in completedImages)
                    {
                        if (string.IsNullOrEmpty(image.Name))
                            continue; //should never happen, but could potential erase all images if so, when syncing

                        var imagePath = Path.Combine(basePath, "images", image.Name);
                        if (!Directory.Exists(imagePath))
                        {
                            imagesToReplicate.Add(image);
                            continue;
                        }
                        var guidPath = Path.Combine(imagePath, "guid");
                        if (!File.Exists(guidPath))
                        {
                            imagesToReplicate.Add(image);
                            continue;
                        }
                        using (StreamReader reader = new StreamReader(guidPath))
                        {
                            var fileGuid = reader.ReadLine() ?? "";
                            if (!fileGuid.Equals(image.LastUploadGuid))
                            {
                                //image on smb is older than what database says
                                imagesToReplicate.Add(image);
                                continue;
                            }
                        }
                    }
                }
                else
                {
                    return false;
                }
            

            if (imagesToReplicate.Count == 0)
            {
                ictx.Log.Info("No Images Found To Replicate");
                return true;
            }

            //find com servers with the image to replicate

            var comServers = uow.ClientComServerRepository.Get(x => x.IsImagingServer && x.ReplicateStorage);
            var intercomKey = ictx.Settings.GetSettingValue(SettingStrings.IntercomKeyEncrypted);
            var decryptedKey = ictx.Encryption.DecryptText(intercomKey);

            var comImageDict = new Dictionary<int, int>();

            foreach (var image in imagesToReplicate)
            {
                foreach (var com in comServers)
                {
                    var hasImage = new APICall().ClientComServerApi.CheckImageExists(com.Url, "", decryptedKey, image.Id);
                    if (hasImage)
                    {
                        if (!comImageDict.ContainsKey(image.Id))
                        {
                            comImageDict.Add(image.Id, com.Id);
                            break; //only add image to a single com server, don't want to replicate image multiple times if multiple coms have it
                        }
                    }

                }
            }

            var groupedByServer = comImageDict.GroupBy(x => x.Value).ToDictionary(y => y.Key, y => y.Select(x => x.Key).ToList());
            foreach (var c in groupedByServer)
            {
                var comServerId = c.Key;
                var thisComImageList = new List<int>();
                foreach (var imageId in c.Value)
                    thisComImageList.Add(imageId);

                var comServer = serviceClientComServer.GetServer(comServerId);
                ictx.Log.Info("Starting Robocopy on " + comServer.DisplayName);
                new APICall().ClientComServerApi.SyncComToSmb(comServer.Url, "", decryptedKey, thisComImageList);
                ictx.Log.Info("Image Sync to SMB Share Complete");
            }
            return true;
        }

        public bool SyncToSmb(List<int> imageIds)
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
                    foreach (var imageId in imageIds)
                    {
                        var image = serviceImage.GetImage(imageId);
                        if (string.IsNullOrEmpty(image.Name)) continue;
                        var sourcePath = Path.Combine(thisComServer.LocalStoragePath, "images", image.Name);
                        var destPath = Path.Combine(ictx.Settings.GetSettingValue(SettingStrings.StoragePath),"images",image.Name).TrimEnd('\\');
                        ictx.Log.Info($"Replicating Image {image.Name} From {sourcePath} on {thisComServer.DisplayName} to {destPath} ");
                        using (RoboCommand backup = new RoboCommand())
                        {
                            // events
                            /*backup.OnFileProcessed += backup_OnFileProcessed;
                            backup.OnCommandCompleted += backup_OnCommandCompleted;
                            backup.OnCopyProgressChanged += Backup_OnCopyProgressChanged;
                            */
                            backup.OnError += Backup_OnError;
                            // copy options
                            backup.CopyOptions.Source = sourcePath;
                            backup.CopyOptions.Destination = destPath;
                            backup.SelectionOptions.IncludeSame = true;
                            backup.CopyOptions.CopySubdirectories = true;
                            backup.CopyOptions.UseUnbufferedIo = true;
                            if(thisComServer.ReplicationRateIpg != 0)
                                backup.CopyOptions.InterPacketGap = thisComServer.ReplicationRateIpg;
                            else
                                backup.CopyOptions.InterPacketGap = 0;
                            // select options
                            backup.CopyOptions.Mirror = true;
                            backup.CopyOptions.Purge = true;
                            backup.LoggingOptions.VerboseOutput = false;

                            backup.SelectionOptions.ExcludeFiles = "guid";
                            // retry options
                            backup.RetryOptions.RetryCount = 3;
                            backup.RetryOptions.RetryWaitTime = 60;

                            try
                            {
                                ictx.Settings.UpdateSetting(SettingStrings.ReplicationInProgress, "True");
                                backup.Start().Wait();

                                if (backup.GetResults().Status.ExitCodeValue == 0 || backup.GetResults().Status.ExitCodeValue == 1)
                                {
                                    //backup succesful, copy the guid now
                                    var guidPath = Path.Combine(sourcePath, "guid");
                                    File.Copy(guidPath, destPath + Path.DirectorySeparatorChar + "guid");
                                }
                            }
                            catch (Exception ex)
                            {
                                ictx.Log.Error("Could Not Complete Replication");
                                ictx.Log.Error(ex.Message);
                            }
                            finally
                            {
                                ictx.Settings.UpdateSetting(SettingStrings.ReplicationInProgress, "False");
                            }
                        }
                    }
                }
                else
                {
                    return false;
                }
            
            return true;
        }

        public bool SyncToCom(List<int> imageIds)
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
                    foreach (var imageId in imageIds)
                    {
                        var image = serviceImage.GetImage(imageId);
                        if (string.IsNullOrEmpty(image.Name)) continue;

                        var sourcePath = Path.Combine(ictx.Settings.GetSettingValue(SettingStrings.StoragePath), "images", image.Name);
                        var destPath = Path.Combine(thisComServer.LocalStoragePath, "images", image.Name).TrimEnd('\\');
                        ictx.Log.Info($"Replicating Image {image.Name} From {sourcePath} to {destPath} on {thisComServer.DisplayName}");
                        using (RoboCommand backup = new RoboCommand())
                        {
                            // events
                            /*backup.OnFileProcessed += backup_OnFileProcessed;
                            backup.OnCommandCompleted += backup_OnCommandCompleted;
                            backup.OnCopyProgressChanged += Backup_OnCopyProgressChanged;
                            */
                            backup.OnError += Backup_OnError;
                            // copy options
                            backup.CopyOptions.Source = sourcePath;
                            backup.CopyOptions.Destination = destPath;
                            backup.CopyOptions.CopySubdirectories = true;
                            backup.SelectionOptions.IncludeSame = true;
                            backup.CopyOptions.UseUnbufferedIo = true;
                            if (thisComServer.ReplicationRateIpg != 0)
                                backup.CopyOptions.InterPacketGap = thisComServer.ReplicationRateIpg;
                            else
                                backup.CopyOptions.InterPacketGap = 0;
                            // select options
                            backup.CopyOptions.Mirror = true;
                            backup.CopyOptions.Purge = true;
                            backup.LoggingOptions.VerboseOutput = false;

                            backup.SelectionOptions.ExcludeFiles = "guid";
                            // retry options
                            backup.RetryOptions.RetryCount = 3;
                            backup.RetryOptions.RetryWaitTime = 60;

                            try
                            {
                                ictx.Settings.UpdateSetting(SettingStrings.ReplicationInProgress, "True");
                                backup.Start().Wait();

                                if (backup.GetResults().Status.ExitCodeValue == 0 || backup.GetResults().Status.ExitCodeValue == 1)
                                {
                                    //backup succesful, copy the guid now
                                    var guidPath = Path.Combine(sourcePath, "guid");
                                    File.Copy(guidPath, destPath + Path.DirectorySeparatorChar + "guid");
                                }
                            }
                            catch (Exception ex)
                            {
                                ictx.Log.Error("Could Not Complete Replication");
                                ictx.Log.Error(ex.Message);
                            }
                            finally
                            {
                                ictx.Settings.UpdateSetting(SettingStrings.ReplicationInProgress, "False");
                            }
                        }
                    }
                }
                else
                {
                    return false;
                }
            
            return true;
        }

        private void Backup_OnError(object sender, ErrorEventArgs e)
        {
            ictx.Log.Error($"Image Replication Failed.");
            ictx.Log.Error(e.ErrorCode + " " + e.Error);
            ictx.Settings.UpdateSetting(SettingStrings.ReplicationInProgress, "False");
        }


        /*private void Backup_OnCopyProgressChanged(object sender, CopyProgressEventArgs e)
        {
            //Logger.Info(e.CurrentFileProgress);
        }

        void backup_OnFileProcessed(object sender, FileProcessedEventArgs e)
        {
            if(e.ProcessedFile.Name.Equals("schema"))
            {

            }
        }

        void backup_OnCommandCompleted(object sender, RoboCommandCompletedEventArgs e)
        {
            var a = sender as RoboCommand;
            if(a.Process.ExitCode == 0  || a.Process.ExitCode == 1)
            {

            }
            var r = e.Results;
        }
        */
       
    }
}
