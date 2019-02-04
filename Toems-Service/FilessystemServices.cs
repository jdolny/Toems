using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using log4net;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
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
            var fullPath = Path.Combine(basePath,file.Guid, file.Name);

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


        public DtoFreeSpace GetStorageFreeSpace(bool isRemote)
        {
            var storageType = ServiceSetting.GetSettingValue(SettingStrings.StorageType);
            if (storageType.Equals("Local") && isRemote) return null; //no remote share setup


            var dpFreeSpace = new DtoFreeSpace();
            if (isRemote)
            {
                var basePath = ServiceSetting.GetSettingValue(SettingStrings.StoragePath);
                var username = ServiceSetting.GetSettingValue(SettingStrings.StorageUsername);
                var domain = ServiceSetting.GetSettingValue(SettingStrings.StorageDomain);
                var password = ServiceSetting.GetSettingValue(SettingStrings.StoragePassword);
                dpFreeSpace.dPPath = basePath;
                using (var unc = new UncServices())
                {
                    var smbPassword = new EncryptionServices().DecryptText(password);
                    if (
                        unc.NetUseWithCredentials(basePath, username, domain,
                            smbPassword) || unc.LastError == 1219)
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
            }
            else
            {
                string path;
                if (storageType.Equals("Local"))
                    path = ServiceSetting.GetSettingValue(SettingStrings.StoragePath);
                else
                {
                    path = ConfigurationManager.AppSettings["LocalStoragePath"];
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

       
    }
}