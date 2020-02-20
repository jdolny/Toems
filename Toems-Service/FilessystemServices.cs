using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using log4net;
using Toems_ApiCalls;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;
using Toems_Service.Entity;

namespace Toems_Service
{
    public class FilesystemServices
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(FilesystemServices));

        public bool DeleteModuleFile(EntityUploadedFile file)
        {
            if (file == null) return false;
            if (string.IsNullOrEmpty(file.Name) || file.Name == Path.DirectorySeparatorChar.ToString()) return false;
            if (string.IsNullOrEmpty(file.Guid) || file.Guid == Path.DirectorySeparatorChar.ToString()) return false;
            var basePath = Path.Combine(ServiceSetting.GetSettingValue(SettingStrings.StoragePath), "software_uploads");
            var fullPath = Path.Combine(basePath, file.Guid, file.Name);

            using (var unc = new UncServices())
            {
                if (unc.NetUseWithCredentials() || unc.LastError == 1219)
                {
                    try
                    {
                        File.Delete(fullPath);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        log.Error("Could Not Delete " + fullPath);
                        log.Error(ex.Message);
                        return false;
                    }
                }
                else
                {
                    log.Error("Could Not Reach Storage Path");
                    return false;
                }
            }
        }

        public bool DeleteExternalFile(EntityExternalDownload file)
        {
            if (file == null) return false;
            if (string.IsNullOrEmpty(file.FileName) || file.FileName == Path.DirectorySeparatorChar.ToString()) return false;
            if (string.IsNullOrEmpty(file.ModuleGuid) || file.ModuleGuid == Path.DirectorySeparatorChar.ToString()) return false;
            var basePath = Path.Combine(ServiceSetting.GetSettingValue(SettingStrings.StoragePath), "software_uploads");
            var fullPath = Path.Combine(basePath, file.ModuleGuid, file.FileName);

            using (var unc = new UncServices())
            {
                if (unc.NetUseWithCredentials() || unc.LastError == 1219)
                {
                    try
                    {
                        File.Delete(fullPath);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        log.Error("Could Not Delete " + fullPath);
                        log.Error(ex.Message);
                        return false;
                    }
                }
                else
                {
                    log.Error("Could Not Reach Storage Path");
                    return false;
                }
            }
        }

        public bool DeleteModuleDirectory(string moduleGuid)
        {
            if (string.IsNullOrEmpty(moduleGuid) || moduleGuid == Path.DirectorySeparatorChar.ToString()) return false;
            var basePath = Path.Combine(ServiceSetting.GetSettingValue(SettingStrings.StoragePath), "software_uploads");
            var fullPath = Path.Combine(basePath, moduleGuid);

            using (var unc = new UncServices())
            {
                if (unc.NetUseWithCredentials() || unc.LastError == 1219)
                {
                    if (!Directory.Exists(fullPath)) return true;
                    try
                    {
                        Directory.Delete(fullPath, true);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex.Message);
                        return false;
                    }
                }
                else
                {
                    log.Error("Could Not Reach Storage Path");
                    return false;
                }
            }
        }



        public List<string> GetLogContents(string name, int limit)
        {
            var path = GetServerPaths("logs") + name;
            return File.ReadLines(path).Reverse().Take(limit).Reverse().ToList();
        }

        public string GetServerPaths(string type, string subType = "")
        {
            var basePath = HttpContext.Current.Server.MapPath("~");
            var seperator = Path.DirectorySeparatorChar;
            switch (type)
            {

                case "csv":
                    return basePath + seperator + "private" + seperator + "imports" + seperator + subType;
                case "exports":
                    return basePath + seperator + "private" + seperator + "exports" + seperator;

                case "seperator":
                    return seperator.ToString();
                case "logs":
                    return basePath + seperator + "private" + seperator + "logs" + seperator;
                default:
                    return null;
            }
        }




        public bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        public static List<string> GetBootImages()
        {
            var result = new List<string>();

            var guid = ConfigurationManager.AppSettings["ComServerUniqueId"];
            var thisComServer = new ServiceClientComServer().GetServerByGuid(guid);
            if (thisComServer == null)
            {
                log.Error($"Com Server With Guid {guid} Not Found");
                return result;
            }

            if (string.IsNullOrEmpty(thisComServer.TftpPath))
            {
                log.Error($"Com Server With Guid {guid} Does Not Have A Valid Tftp Path");
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
                log.Error(ex.Message);
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

        public static List<string> GetKernels()
        {
            var result = new List<string>();

            var guid = ConfigurationManager.AppSettings["ComServerUniqueId"];
            var thisComServer = new ServiceClientComServer().GetServerByGuid(guid);
            if (thisComServer == null)
            {
                log.Error($"Com Server With Guid {guid} Not Found");
                return result;
            }

            if (string.IsNullOrEmpty(thisComServer.TftpPath))
            {
                log.Error($"Com Server With Guid {guid} Does Not Have A Valid Tftp Path");
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
                log.Error(ex.Message);
            }
            return result;
        }

        public List<DtoImageFileInfo> GetPartitionFileSize(string imageName, string hd, string partition)
        {
            var basePath = ServiceSetting.GetSettingValue(SettingStrings.StoragePath);
            var imageFileInfo = new DtoImageFileInfo();
            using (var unc = new UncServices())
            {

                if (
                    unc.NetUseWithCredentials() || unc.LastError == 1219)
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
                        log.Error(ex.Message);
                        return null;
                    }
                }
                else
                {
                    log.Error("Failed to connect to " + basePath + "\r\nLastError = " + unc.LastError);
                }
            }

            return new List<DtoImageFileInfo> { imageFileInfo };

        }

        public bool DeleteImageFolders(string imageName)
        {
            //Check again
            if (string.IsNullOrEmpty(imageName)) return false;

            var basePath = ServiceSetting.GetSettingValue(SettingStrings.StoragePath);

            using (var unc = new UncServices())
            {
                if (
                      unc.NetUseWithCredentials() || unc.LastError == 1219)
                {
                    try
                    {
                        Directory.Delete(basePath + @"\images" + @"\" +
                                         imageName, true);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex.Message);
                        return false;
                    }
                }
                else
                {
                    log.Error("Failed to connect to " + basePath + "\r\nLastError = " + unc.LastError);
                    return false;

                }
            }
        }

        public bool RenameImageFolder(string oldName, string newName)
        {
            //Check again
            if (string.IsNullOrEmpty(oldName) || string.IsNullOrEmpty(newName)) return false;

            var basePath = ServiceSetting.GetSettingValue(SettingStrings.StoragePath);

            using (var unc = new UncServices())
            {
                if (
                      unc.NetUseWithCredentials() || unc.LastError == 1219)
                {
                    try
                    {
                        var imagePath = basePath + @"\images\";
                        Directory.Move(imagePath + oldName, imagePath + newName);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex.Message);
                        return false;
                    }
                }
                else
                {
                    log.Error("Failed to connect to " + basePath + "\r\nLastError = " + unc.LastError);
                    return false;

                }
            }
        }

        public string ReadSchemaFile(string imageName)
        {
           
            var schemaText = "";
            var basePath = ServiceSetting.GetSettingValue(SettingStrings.StoragePath);
            var path = basePath + "images" + Path.DirectorySeparatorChar +
                             imageName + Path.DirectorySeparatorChar + "schema";
            using (var unc = new UncServices())
            {
                if (
                      unc.NetUseWithCredentials() || unc.LastError == 1219)
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
                        log.Error("Could Not Read Schema File.");
                        log.Error(ex.Message);
                    }
                }
                else
                {
                    log.Error("Failed to connect to " + basePath + "\r\nLastError = " + unc.LastError);
                    return null;
                }
            }

            return schemaText;
        }

        public string GetHdFileSize(string imageName, string hd)
        {
            var basePath = ServiceSetting.GetSettingValue(SettingStrings.StoragePath);

            using (var unc = new UncServices())
            {

                if (
                    unc.NetUseWithCredentials() || unc.LastError == 1219)
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
                    log.Error("Failed to connect to " + basePath + "\r\nLastError = " + unc.LastError);
                    return "N/A";
                }
            }

        }

        public static List<string> GetLogs()
        {
            var logPath = HttpContext.Current.Server.MapPath("~") + Path.DirectorySeparatorChar + "private" +
                          Path.DirectorySeparatorChar + "logs" + Path.DirectorySeparatorChar;

            var logFiles = Directory.GetFiles(logPath, "*.*");
            var result = new List<string>();
            for (var x = 0; x < logFiles.Length; x++)
                result.Add(Path.GetFileName(logFiles[x]));

            return result;
        }


        public DtoFreeSpace GetSMBFreeSpace()
        {
            var storageType = ServiceSetting.GetSettingValue(SettingStrings.StorageType);
            if (storageType.Equals("Local")) return null; //no smb share setup

            var dpFreeSpace = new DtoFreeSpace();
            var basePath = ServiceSetting.GetSettingValue(SettingStrings.StoragePath);
            using (var unc = new UncServices())
            {

                if (
                    unc.NetUseWithCredentials() || unc.LastError == 1219)
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
                }
                else
                {
                    log.Error("Failed to connect to " + basePath + "\r\nLastError = " + unc.LastError);
                }
            }

            return dpFreeSpace;
        }

        public DtoFreeSpace GetComServerFreeSpace()
        {
            var dpFreeSpace = new DtoFreeSpace();
            var storageType = ServiceSetting.GetSettingValue(SettingStrings.StorageType);
            string path = string.Empty;
            if (storageType.Equals("Local"))
            {
                //if storage type is local, then only a single server with a com, api, and front end are in use
                //just get the size of the global local path
                path = ServiceSetting.GetSettingValue(SettingStrings.StoragePath);
                dpFreeSpace.name = "Local Storage";
            }
            else
            {
                //storage type is set to smb, get this current com server's storage
                var guid = ConfigurationManager.AppSettings["ComServerUniqueId"];
                var thisComServer = new ServiceClientComServer().GetServerByGuid(guid);
                if (thisComServer == null)
                {
                    log.Error($"Com Server With Guid {guid} Not Found");
                    return null;
                }

                if (string.IsNullOrEmpty(thisComServer.TftpPath))
                {
                    log.Error($"Com Server With Guid {guid} Does Not Have A Valid Tftp Path");
                    return null;
                }
                path = thisComServer.LocalStoragePath;
                dpFreeSpace.name = thisComServer.DisplayName;
            }

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
                log.Error(ex.Message);
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

            var basePath = ServiceSetting.GetSettingValue(SettingStrings.StoragePath);

            using (var unc = new UncServices())
            {

                if (
                    unc.NetUseWithCredentials() || unc.LastError == 1219)
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
                        log.Error(ex.Message);
                    }
                }
                else
                {
                    log.Error("Failed to connect to " + basePath + "\r\nLastError = " + unc.LastError);
                    return "N/A";
                }
            }
           

            return filePath;
        }

        public string GetLVMFileNameWithFullPath(string imageName, string schemaHdNumber, string vgName, string lvName,
            string extension)
        {


            var filePath = "";

            var basePath = ServiceSetting.GetSettingValue(SettingStrings.StoragePath);

            using (var unc = new UncServices())
            {

                if (
                    unc.NetUseWithCredentials() || unc.LastError == 1219)
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
                        log.Error(ex.Message);
                    }
                }
                else
                {
                    log.Error("Failed to connect to " + basePath + "\r\nLastError = " + unc.LastError);
                    return "N/A";
                }
            }


            return filePath;


           
        }
    }
}