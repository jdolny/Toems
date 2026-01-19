using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using log4net;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.EntityServices;

namespace Toems_ServiceCore.Infrastructure
{
    public class FilesystemServices(InfrastructureContext ictx,
        UncServices uncService,
        IWebHostEnvironment env,
        ServiceClientComServer clientComServerService,
        ServiceImage imageService)
    {
        public bool DeleteModuleFile(EntityUploadedFile file)
        {
            if (file == null) return false;
            if (string.IsNullOrEmpty(file.Name) || file.Name == Path.DirectorySeparatorChar.ToString()) return false;
            if (string.IsNullOrEmpty(file.Guid) || file.Guid == Path.DirectorySeparatorChar.ToString()) return false;
            var basePath = Path.Combine(ictx.Settings.GetSettingValue(SettingStrings.StoragePath), "software_uploads");
            var fullPath = Path.Combine(basePath, file.Guid, file.Name);


            if (uncService.NetUseWithCredentials() || uncService.LastError == 1219)
            {
                try
                {
                    File.Delete(fullPath);
                    return true;
                }
                catch (Exception ex)
                {
                    ictx.Log.Error("Could Not Delete " + fullPath);
                    ictx.Log.Error(ex.Message);
                    return false;
                }
            }
            else
            {
                ictx.Log.Error("Could Not Reach Storage Path");
                return false;
            }

        }

        public bool DeleteExternalFile(EntityExternalDownload file)
        {
            if (file == null) return false;
            if (string.IsNullOrEmpty(file.FileName) || file.FileName == Path.DirectorySeparatorChar.ToString()) return false;
            if (string.IsNullOrEmpty(file.ModuleGuid) || file.ModuleGuid == Path.DirectorySeparatorChar.ToString()) return false;
            var basePath = Path.Combine(ictx.Settings.GetSettingValue(SettingStrings.StoragePath), "software_uploads");
            var fullPath = Path.Combine(basePath, file.ModuleGuid, file.FileName);
            
            if (uncService.NetUseWithCredentials() || uncService.LastError == 1219)
            {
                try
                {
                    File.Delete(fullPath);
                    return true;
                }
                catch (Exception ex)
                {
                    ictx.Log.Error("Could Not Delete " + fullPath);
                    ictx.Log.Error(ex.Message);
                    return false;
                }
            }
            else
            {
                ictx.Log.Error("Could Not Reach Storage Path");
                return false;
            }

        }

        public bool DeleteModuleDirectory(string moduleGuid)
        {
            if (string.IsNullOrEmpty(moduleGuid) || moduleGuid == Path.DirectorySeparatorChar.ToString()) return false;
            var basePath = Path.Combine(ictx.Settings.GetSettingValue(SettingStrings.StoragePath), "software_uploads");
            var fullPath = Path.Combine(basePath, moduleGuid);


            if (uncService.NetUseWithCredentials() || uncService.LastError == 1219)
            {
                if (!Directory.Exists(fullPath)) return true;
                try
                {
                    Directory.Delete(fullPath, true);
                    return true;
                }
                catch (Exception ex)
                {
                    ictx.Log.Error(ex.Message);
                    return false;
                }
            }
            else
            {
                ictx.Log.Error("Could Not Reach Storage Path");
                return false;
            }

        }
        
        public List<string> GetLogContents(string name, int limit)
        {
            var path = Path.Combine(env.ContentRootPath, "private", "logs", name);
            return File.ReadLines(path).Reverse().Take(limit).Reverse().ToList();
        }

        public List<string> GetComServerLogContents(string name, int limit, int comServerId)
        {
            var comServer = clientComServerService.GetServer(comServerId);
            var intercomKey = ictx.Settings.GetSettingValue(SettingStrings.IntercomKeyEncrypted);
            var decryptedKey = ictx.Encryption.DecryptText(intercomKey);

            return new APICall().ClientComServerApi.GetComServerLogContents(comServer.Url, "", decryptedKey, name, limit);
        }
        
        public bool FileExists(string filePath)
        {
            try
            {
                return File.Exists(filePath);
            }
            catch
            {
                return false;
            }
        }

        public List<string> GetBootImages()
        {
            var result = new List<string>();

            var guid = ictx.Config["ComServerUniqueId"];
            var thisComServer = clientComServerService.GetServerByGuid(guid);
            if (thisComServer == null)
            {
                ictx.Log.Error($"Com Server With Guid {guid} Not Found");
                return result;
            }

            if (string.IsNullOrEmpty(thisComServer.TftpPath))
            {
                ictx.Log.Error($"Com Server With Guid {guid} Does Not Have A Valid Tftp Path");
                return result;
            }

            var kernelPath = Path.Combine(thisComServer.TftpPath, "images");

            try
            {
                var kernelFiles = Directory.GetFiles(kernelPath, "*.*");

                for (var x = 0; x < kernelFiles.Length; x++)
                    result.Add(Path.GetFileName(kernelFiles[x]));
            }

            catch (Exception ex)
            {
                ictx.Log.Error(ex.Message);
            }
            return result;
        }

        public void Copy(string sourceDirectory, string targetDirectory)
        {
            var diSource = new DirectoryInfo(sourceDirectory);
            var diTarget = new DirectoryInfo(targetDirectory);

            CopyAll(diSource, diTarget);
        }

        private void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (var fi in source.GetFiles())
            {
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (var diSourceSubDir in source.GetDirectories())
            {
                var nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }

        public List<string> GetKernels()
        {
            var result = new List<string>();

            var guid = ictx.Config["ComServerUniqueId"];
            var thisComServer = clientComServerService.GetServerByGuid(guid);
            if (thisComServer == null)
            {
                ictx.Log.Error($"Com Server With Guid {guid} Not Found");
                return result;
            }

            if (string.IsNullOrEmpty(thisComServer.TftpPath))
            {
                ictx.Log.Error($"Com Server With Guid {guid} Does Not Have A Valid Tftp Path");
                return result;
            }

            var kernelPath = Path.Combine(thisComServer.TftpPath, "kernels");

            try
            {
                var kernelFiles = Directory.GetFiles(kernelPath, "*.*");

                for (var x = 0; x < kernelFiles.Length; x++)
                    result.Add(Path.GetFileName(kernelFiles[x]));
            }

            catch (Exception ex)
            {
                ictx.Log.Error(ex.Message);
            }
            return result;
        }

        public List<DtoImageFileInfo> GetPartitionFileSize(string imageName, string hd, string partition)
        {
            var basePath = ictx.Settings.GetSettingValue(SettingStrings.StoragePath);
            var imageFileInfo = new DtoImageFileInfo();

            if (uncService.NetUseWithCredentials() || uncService.LastError == 1219)
            {
                try
                {
                    var imageFile =
                        Directory.GetFiles(
                            basePath + "images" + Path.DirectorySeparatorChar + imageName +
                            Path.DirectorySeparatorChar + "hd" + hd +
                            Path.DirectorySeparatorChar,
                            "part" + partition + ".*").FirstOrDefault();

                    var fi = new FileInfo(imageFile);
                    imageFileInfo = new DtoImageFileInfo
                    {
                        FileName = fi.Name,
                        FileSize = (fi.Length / 1024f / 1024f).ToString("#.##") + " MB"
                    };
                }
                catch (Exception ex)
                {
                    ictx.Log.Error(ex.Message);
                    return null;
                }
            }
            else
            {
                ictx.Log.Error("Failed to connect to " + basePath + "\r\nLastError = " + uncService.LastError);
            }


            return new List<DtoImageFileInfo> { imageFileInfo };

        }

        public bool CheckImageExists(int imageId)
        {
            //storage type is set to smb, get this current com server's storage
            var guid = ictx.Config["ComServerUniqueId"];
            var thisComServer = clientComServerService.GetServerByGuid(guid);
            if (thisComServer == null)
            {
                ictx.Log.Error($"Com Server With Guid {guid} Not Found");
                return false;
            }

            var image = imageService.GetImage(imageId);

            var basePath = thisComServer.LocalStoragePath;

            if (string.IsNullOrEmpty(image.Name))
                return false;


            var imagePath = Path.Combine(basePath, "images", image.Name);
            if (!Directory.Exists(imagePath))
            {
                return false;
            }
            var guidPath = Path.Combine(imagePath, "guid");
            if (!File.Exists(guidPath))
            {
                return false;
            }
            using (StreamReader reader = new StreamReader(guidPath))
            {
                var fileGuid = reader.ReadLine() ?? "";
                if (fileGuid.Equals(image.LastUploadGuid))
                {
                    return true;
                }
            }


            return false;
        }

        public bool DeleteImageFolders(string imageName)
        {
            //Check again
            if (string.IsNullOrEmpty(imageName)) return false;

            var basePath = ictx.Settings.GetSettingValue(SettingStrings.StoragePath);


            if (uncService.NetUseWithCredentials() || uncService.LastError == 1219)
            {
                try
                {
                    Directory.Delete(basePath + @"\images" + @"\" +
                                     imageName, true);
                    return true;
                }
                catch (Exception ex)
                {
                    ictx.Log.Error(ex.Message);
                    return false;
                }
            }
            else
            {
                ictx.Log.Error("Failed to connect to " + basePath + "\r\nLastError = " + uncService.LastError);
                return false;

            }

        }

        public bool RenameImageFolder(string oldName, string newName)
        {
            //Check again
            if (string.IsNullOrEmpty(oldName) || string.IsNullOrEmpty(newName)) return false;

            var basePath = ictx.Settings.GetSettingValue(SettingStrings.StoragePath);

            if (uncService.NetUseWithCredentials() || uncService.LastError == 1219)
            {
                try
                {
                    var imagePath = basePath + @"\images\";
                    Directory.Move(imagePath + oldName, imagePath + newName);
                    return true;
                }
                catch (Exception ex)
                {
                    ictx.Log.Error(ex.Message);
                    return false;
                }
            }
            else
            {
                ictx.Log.Error("Failed to connect to " + basePath + "\r\nLastError = " + uncService.LastError);
                return false;

            }
        }

        public string ReadSchemaFile(string imageName)
        {

            var schemaText = "";
            var basePath = ictx.Settings.GetSettingValue(SettingStrings.StoragePath);
            var path = basePath + "images" + Path.DirectorySeparatorChar +
                       imageName + Path.DirectorySeparatorChar + "schema";

            if (uncService.NetUseWithCredentials() || uncService.LastError == 1219)
            {
                try
                {
                    using (var reader = new StreamReader(path))
                    {
                        schemaText = reader.ReadLine() ?? "";
                    }
                }
                catch (Exception ex)
                {
                    ictx.Log.Error("Could Not Read Schema File.");
                    ictx.Log.Error(ex.Message);
                }
            }
            else
            {
                ictx.Log.Error("Failed to connect to " + basePath + "\r\nLastError = " + uncService.LastError);
                return null;
            }
            
            return schemaText;
        }

        public string GetHdFileSize(string imageName, string hd)
        {
            var basePath = ictx.Settings.GetSettingValue(SettingStrings.StoragePath);


            if (uncService.NetUseWithCredentials() || uncService.LastError == 1219)
            {
                try
                {
                    var imagePath = basePath + "images" + Path.DirectorySeparatorChar + imageName +
                                    Path.DirectorySeparatorChar + "hd" + hd;
                    var size = GetDirectorySize(new DirectoryInfo(imagePath)) / 1024f / 1024f / 1024f;
                    return Math.Abs(size) < 0.1f ? "< 100M" : size.ToString("#.##") + " GB";
                }
                catch
                {
                    return "N/A";
                }
            }
            else
            {
                ictx.Log.Error("Failed to connect to " + basePath + "\r\nLastError = " + uncService.LastError);
                return "N/A";
            }


        }

        public List<string> GetComServerLogs(int comServerId)
        {
            var comServer = clientComServerService.GetServer(comServerId);
            var intercomKey = ictx.Settings.GetSettingValue(SettingStrings.IntercomKeyEncrypted);
            var decryptedKey = ictx.Encryption.DecryptText(intercomKey);

            return new APICall().ClientComServerApi.GetComServerLogs(comServer.Url, "", decryptedKey);
        }

        public List<string> GetLogs()
        {
            var logPath = env.ContentRootPath + Path.DirectorySeparatorChar + "private" +
                          Path.DirectorySeparatorChar + "logs" + Path.DirectorySeparatorChar;

            var logFiles = Directory.GetFiles(logPath, "*.*");
            var result = new List<string>();
            for (var x = 0; x < logFiles.Length; x++)
                result.Add(Path.GetFileName(logFiles[x]));

            return result;
        }


        public DtoFreeSpace GetSMBFreeSpace()
        {
            var storageType = ictx.Settings.GetSettingValue(SettingStrings.StorageType);
            if (storageType.Equals("Local")) return null; //no smb share setup

            var dpFreeSpace = new DtoFreeSpace();
            var basePath = ictx.Settings.GetSettingValue(SettingStrings.StoragePath);

            if (uncService.NetUseWithCredentials() || uncService.LastError == 1219)
            {
                ulong freespace = 0;
                ulong total = 0;
                var success = DriveFreeBytes(basePath, out freespace, out total);

                if (!success) return null;

                var freePercent = 0;
                var usedPercent = 0;

                if (total > 0 && freespace > 0)
                {
                    freePercent = (int)(0.5f + 100f * Convert.ToInt64(freespace) / Convert.ToInt64(total));
                    usedPercent =
                        (int)(0.5f + 100f * Convert.ToInt64(total - freespace) / Convert.ToInt64(total));
                }

                dpFreeSpace.freespace = freespace;
                dpFreeSpace.total = total;
                dpFreeSpace.freePercent = freePercent;
                dpFreeSpace.usedPercent = usedPercent;
                dpFreeSpace.dPPath = basePath;
            }
            else
            {
                ictx.Log.Error("Failed to connect to " + basePath + "\r\nLastError = " + uncService.LastError);
            }
            
            return dpFreeSpace;
        }

        public DtoFreeSpace GetComServerFreeSpace()
        {
            var dpFreeSpace = new DtoFreeSpace();
            string path = string.Empty;

                var guid = ictx.Config["ComServerUniqueId"];
                var thisComServer = clientComServerService.GetServerByGuid(guid);
                if (thisComServer == null)
                {
                    ictx.Log.Error($"Com Server With Guid {guid} Not Found");
                    return null;
                }

                path = thisComServer.LocalStoragePath;
                dpFreeSpace.name = thisComServer.DisplayName;
            

            dpFreeSpace.dPPath = path;

            if (Directory.Exists(path))
            {

                ulong freespace = 0;
                ulong total = 0;


                bool success;

                success = DriveFreeBytes(path, out freespace, out total);



                if (!success) return null;

                var freePercent = 0;
                var usedPercent = 0;

                if (total > 0 && freespace > 0)
                {
                    freePercent = (int)(0.5f + 100f * Convert.ToInt64(freespace) / Convert.ToInt64(total));
                    usedPercent =
                        (int)(0.5f + 100f * Convert.ToInt64(total - freespace) / Convert.ToInt64(total));
                }
                dpFreeSpace.freespace = freespace;
                dpFreeSpace.total = total;
                dpFreeSpace.freePercent = freePercent;
                dpFreeSpace.usedPercent = usedPercent;
            }
            return dpFreeSpace;
        }


        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetDiskFreeSpaceEx(string lpDirectoryName,
            out ulong lpFreeBytesAvailable,
            out ulong lpTotalNumberOfBytes,
            out ulong lpTotalNumberOfFreeBytes);

        public static bool DriveFreeBytes(string folderName, out ulong freespace, out ulong total)
        {
            freespace = 0;
            total = 0;
            if (string.IsNullOrEmpty(folderName))
            {
                throw new ArgumentNullException("folderName");
            }

            if (!folderName.EndsWith("\\"))
            {
                folderName += '\\';
            }

            ulong free = 0, tot = 0, dummy2 = 0;

            if (GetDiskFreeSpaceEx(folderName, out free, out tot, out dummy2))
            {
                freespace = free;
                total = tot;
                return true;
            }
            return false;
        }

        public bool WritePath(string path, string contents)
        {
            try
            {
                using (var file = new StreamWriter(path))
                {
                    file.WriteLine(contents);
                }
                return true;
            }
            catch (Exception ex)
            {
                ictx.Log.Error(ex.Message);
                return false;
            }
        }

        public long GetDirectorySize(DirectoryInfo d)
        {
            long size = 0;
            var fis = d.GetFiles();
            foreach (var fi in fis)
            {
                size += fi.Length;
            }

            return size;
        }

        public string GetFileNameWithFullPath(string imageName, string schemaHdNumber, string partitionNumber,
            string extension)
        {

            var filePath = "";

            var basePath = ictx.Settings.GetSettingValue(SettingStrings.StoragePath);

            if (uncService.NetUseWithCredentials() || uncService.LastError == 1219)
            {
                var imagePath = basePath + "images" +
                                Path.DirectorySeparatorChar + imageName + Path.DirectorySeparatorChar + "hd" +
                                schemaHdNumber;
                try
                {
                    filePath =
                        Directory.GetFiles(
                                imagePath + Path.DirectorySeparatorChar, "part" + partitionNumber + "." + extension + ".*")
                            .FirstOrDefault();
                }
                catch (Exception ex)
                {
                    ictx.Log.Error(ex.Message);
                }
            }
            else
            {
                ictx.Log.Error("Failed to connect to " + basePath + "\r\nLastError = " + uncService.LastError);
                return "N/A";
            }
            
            return filePath;
        }

        public string GetMulticastFileNameWithFullPath(string imageName, string schemaHdNumber, string partitionNumber,
           string extension, string basePath)
        {

            var filePath = "";


                if (uncService.NetUseWithCredentials() || uncService.LastError == 1219)
                {
                    var imagePath = basePath + "images" +
                                Path.DirectorySeparatorChar + imageName + Path.DirectorySeparatorChar + "hd" +
                                schemaHdNumber;
                    try
                    {
                        filePath =
                            Directory.GetFiles(
                                imagePath + Path.DirectorySeparatorChar, "part" + partitionNumber + "." + extension + ".*")
                                .FirstOrDefault();
                    }
                    catch (Exception ex)
                    {
                        ictx.Log.Error(ex.Message);
                    }
                }
                else
                {
                    ictx.Log.Error("Failed to connect to " + basePath + "\r\nLastError = " + uncService.LastError);
                    return "N/A";
                }
            


            return filePath;
        }

        public string GetLVMFileNameWithFullPath(string imageName, string schemaHdNumber, string vgName, string lvName,
            string extension)
        {


            var filePath = "";

            var basePath = ictx.Settings.GetSettingValue(SettingStrings.StoragePath);


            if (uncService.NetUseWithCredentials() || uncService.LastError == 1219)
            {
                var imagePath = basePath + "images" +
                                Path.DirectorySeparatorChar + imageName + Path.DirectorySeparatorChar + "hd" +
                                schemaHdNumber;
                try
                {
                    filePath =
                        Directory.GetFiles(
                                imagePath + Path.DirectorySeparatorChar,
                                vgName + "-" + lvName + "." + extension + ".*")
                            .FirstOrDefault();
                }
                catch (Exception ex)
                {
                    ictx.Log.Error(ex.Message);
                }
            }
            else
            {
                ictx.Log.Error("Failed to connect to " + basePath + "\r\nLastError = " + uncService.LastError);
                return "N/A";
            }



            return filePath;

        }

        public string GetMulticastLVMFileNameWithFullPath(string imageName, string schemaHdNumber, string vgName, string lvName,
            string extension, string basePath)
        {


            var filePath = "";


            if (uncService.NetUseWithCredentials() || uncService.LastError == 1219)
            {
                var imagePath = basePath + "images" +
                                Path.DirectorySeparatorChar + imageName + Path.DirectorySeparatorChar + "hd" +
                                schemaHdNumber;
                try
                {
                    filePath =
                        Directory.GetFiles(
                                imagePath + Path.DirectorySeparatorChar,
                                vgName + "-" + lvName + "." + extension + ".*")
                            .FirstOrDefault();
                }
                catch (Exception ex)
                {
                    ictx.Log.Error(ex.Message);
                }
            }
            else
            {
                ictx.Log.Error("Failed to connect to " + basePath + "\r\nLastError = " + uncService.LastError);
                return "N/A";
            }
            
            return filePath;
            
        }

        public List<DtoReplicationProcess> GetRunningSyncProcess()
        {
            var list = new List<DtoReplicationProcess>();
            var processes = Process.GetProcessesByName("Robocopy");
            foreach(var p in processes)
            {
                var proc = new DtoReplicationProcess();
                proc.Pid = p.Id;
                proc.ProcessName = p.ProcessName;
                proc.ProcessArguments = p.StartTime.ToString();
                list.Add(proc);
            }

            return list;
        }

        public bool KillRoboProcess(int pid)
        {
            try
            {
                var proc = Process.GetProcessById(pid);
                if(proc.ProcessName.Equals("Robocopy"))
                    proc.Kill();
            }
            catch
            {
                //ignored
            }
            return true;
        }

        public string GetDefaultBootMenuPath(string type,int comServerId)
        {
            string path = null;
            var comServer = clientComServerService.GetServer(comServerId);
            var tftpPath = comServer.TftpPath;
            var mode = ictx.Settings.GetSettingValue(SettingStrings.PxeBootloader);
            var proxyDhcp = ictx.Settings.GetSettingValue(SettingStrings.ProxyDhcpEnabled);

            if (proxyDhcp == "Yes")
            {
                var biosFile = ictx.Settings.GetSettingValue(SettingStrings.ProxyBiosBootloader);
                var efi32File = ictx.Settings.GetSettingValue(SettingStrings.ProxyEfi32Bootloader);
                var efi64File = ictx.Settings.GetSettingValue(SettingStrings.ProxyEfi64Bootloader);

                if (type == "bios")
                {
                    if (biosFile.Contains("ipxe"))
                        path = tftpPath + "proxy" + Path.DirectorySeparatorChar +
                               type + Path.DirectorySeparatorChar + "pxelinux.cfg" +
                               Path.DirectorySeparatorChar + "default.ipxe";
                    else
                        path = tftpPath + "proxy" + Path.DirectorySeparatorChar +
                               type + Path.DirectorySeparatorChar + "pxelinux.cfg" +
                               Path.DirectorySeparatorChar + "default";
                }

                if (type == "efi32")
                {
                    if (efi32File.Contains("ipxe"))
                        path = tftpPath + "proxy" + Path.DirectorySeparatorChar +
                               type + Path.DirectorySeparatorChar + "pxelinux.cfg" +
                               Path.DirectorySeparatorChar + "default.ipxe";
                    else
                        path = tftpPath + "proxy" + Path.DirectorySeparatorChar +
                               type + Path.DirectorySeparatorChar + "pxelinux.cfg" +
                               Path.DirectorySeparatorChar + "default";
                }

                if (type == "efi64")
                {
                    if (efi64File.Contains("ipxe"))
                        path = tftpPath + "proxy" + Path.DirectorySeparatorChar +
                               type + Path.DirectorySeparatorChar + "pxelinux.cfg" +
                               Path.DirectorySeparatorChar + "default.ipxe";
                    else if (efi64File.Contains("grub"))
                        path = tftpPath + "grub" + Path.DirectorySeparatorChar + "grub.cfg";
                    else
                        path = tftpPath + "proxy" + Path.DirectorySeparatorChar +
                               type + Path.DirectorySeparatorChar + "pxelinux.cfg" +
                               Path.DirectorySeparatorChar + "default";
                }
            }
            else
            {
                if (mode.Contains("ipxe"))
                    path = tftpPath + "pxelinux.cfg" + Path.DirectorySeparatorChar +
                           "default.ipxe";
                else if (mode.Contains("grub"))
                {
                    path = tftpPath + "grub" + Path.DirectorySeparatorChar + "grub.cfg";
                }
                else
                    path = tftpPath + "pxelinux.cfg" + Path.DirectorySeparatorChar + "default";
            }
            return path;
        }

        
        public string ReadAllText(string path)
        {
            string fileText = null;

            try
            {
                fileText = File.ReadAllText(path);
            }
            catch (Exception ex)
            {
                ictx.Log.Error(ex.Message);
                fileText = "Could Not Read File";
            }

            return fileText;
        }

        public bool EditDefaultBootMenu(DtoCoreScript script)
        {
            try
            {
                var path = GetDefaultBootMenuPath(script.Name, script.ComServerId);
                using (var file = new StreamWriter(path))
                {
                    file.WriteLine(script.Contents);
                }

                return true;
            }

            catch (Exception ex)
            {
                ictx.Log.Error(ex.Message);
                return false;
            }
        }

    }
}