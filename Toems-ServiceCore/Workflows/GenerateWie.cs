using System;
using System.IO;
using System.Text;
using System.Web;
using Toems_Common.Dto;
using Toems_Service.Entity;
using System.Linq;
using Toems_Common.Entity;
using Newtonsoft.Json;
using log4net;
using Toems_DataModel;
using Toems_ApiCalls;
using log4net.Repository.Hierarchy;
using Toems_Common;
using System.Collections.Generic;
using Toems_Common.Dto.client;
using System.IO.Compression;
using System.Text.RegularExpressions;
using Toems_ServiceCore.EntityServices;
using Toems_ServiceCore.Infrastructure;

namespace Toems_Service.Workflows
{

    public class GenerateWie(InfrastructureContext ictx, ServiceWieBuild serviceWieBuild, ServiceFileCopyModule serviceFileCopyModule, ServiceModule serviceModule, FilesystemServices serviceFilesystem)
    {
        private const string _fileName = "ToemsPE-Build.cmd";
        private string _fullPath;
        private string _basePath;
        private DtoWieConfig _config;
        private EntityWieBuild _wieBuild = new();
        private UnitOfWork _uow = new();
        private DtoActionResult _result = new();


        public DtoActionResult Run(DtoWieConfig config)
        {
            _config = config;
            _result.Success = false;
            ictx.Log.Info("Starting Wie Build");
            _wieBuild.Status = "Started";
            _basePath = Path.Combine(ictx.Environment.ContentRootPath, "private", "wie_builder");
            _fullPath = Path.Combine(_basePath, _fileName);
            _wieBuild.StartTime = DateTime.Now;
            _wieBuild.BuildOptions = JsonConvert.SerializeObject(_config);
            _wieBuild.WieGuid = Guid.NewGuid().ToString();
            var validationResult = ValidateConfig();
            if(!validationResult.Equals("Success"))
            {
                _result.Success = false;
                _result.ErrorMessage = validationResult;
                return _result;
            }
            CreateConfigFile();
            AddDrivers();
            if (StartProcess())
            {
                _wieBuild.Status = "Running";
                _uow.WieBuildRepository.Insert(_wieBuild);
                _uow.Save();
                _result.Success = true;
            }
            return _result;
        }

        private string ValidateConfig()
        {
            if (_config == null)
                return "Missing Build Options";

            else if (string.IsNullOrEmpty(_config.ComServers))
                return "No Com Servers Were Selected";


            if(!_config.SkipAdkCheck)
            {
                if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\Windows Kits\10\Assessment and Deployment Kit\Windows Preinstallation Environment\copype.cmd") &&
                    !File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + @"\Windows Kits\10\Assessment and Deployment Kit\Windows Preinstallation Environment\copype.cmd"))
                {
                    return "Windows ADK must be installed before building the WIE.";
                }
            }

            if (serviceWieBuild.GetWieProcess().Any())
                return "An Active Build Process Is Currently Running";

            return "Success";
        }

        private bool StartProcess()
        {
            var sha = serviceFilesystem.GetFileSha256(_fullPath);
            return serviceFilesystem.WritePath(Path.Combine(_basePath,"Queue",_wieBuild.WieGuid+".queue"), sha);
        }
        
        private static readonly Regex SafeValueRegex = new Regex(@"^[A-Za-z0-9\-_\.:\/@ ]{0,200}$", RegexOptions.Compiled);

        private bool IsSafe(string value)
        {
            if (string.IsNullOrEmpty(value)) return true;
            return SafeValueRegex.IsMatch(value);
        }

        private string SanitizeForBatch(string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            // Trim and normalize newlines
            value = value.Replace("\r", " ").Replace("\n", " ").Trim();

            // Double any percent signs (prevents variable expansion).
            value = value.Replace("%", "%%");

            // Remove or escape dangerous characters
            value = value.Replace("\"", "'"); // convert double quotes to single quotes (batch will drop enclosing quotes)
            return value;
        }
        private bool CreateConfigFile()
        {
            if (!IsSafe(_config.Timezone)) { _result.ErrorMessage = "Invalid TimeZone"; return false; }
            if (!IsSafe(_config.InputLocale)) { _result.ErrorMessage = "Invalid InputLocale"; return false; }
            if (!IsSafe(_config.Language)) { _result.ErrorMessage = "Invalid Language"; return false; }
            if (!IsSafe(_config.ComServers)) { _result.ErrorMessage = "Invalid ComServers"; return false; }
            if (!IsSafe(_config.Token)) { _result.ErrorMessage = "Invalid Token"; return false; }

            var configContents = new StringBuilder();
            configContents.AppendLine("@echo off");
            configContents.AppendLine("pushd %~dp0");
            configContents.AppendLine($"set \"TimeZone={SanitizeForBatch(_config.Timezone)}\"");
            configContents.AppendLine($"set \"InputLocale={SanitizeForBatch(_config.InputLocale)}\"");
            configContents.AppendLine($"set \"MyLang={SanitizeForBatch(_config.Language)}\"");
            configContents.AppendLine($"set \"ComServerURL={SanitizeForBatch(_config.ComServers)}\"");
            configContents.AppendLine($"set \"UniversalToken={SanitizeForBatch(_config.Token)}\"");
            configContents.AppendLine($"set \"RestrictComServers={_config.RestrictComServers.ToString()}\"");
            configContents.AppendLine("set CreateISO=true");
            configContents.AppendLine("set LoginDebug=false");
            configContents.AppendLine("set PLATFORM=x64");
            configContents.AppendLine("set Pass=1");
            configContents.AppendLine("mkdir Status");
            configContents.AppendLine($"call .\\Scripts\\MakePE.cmd > .\\Status\\{_wieBuild.WieGuid}.log");
            configContents.AppendLine($"echo complete > .\\Status\\{_wieBuild.WieGuid}.complete");

            return serviceFilesystem.WritePath(_fullPath, configContents.ToString());
          
        }

        private bool AddDrivers()
        {
            var filesToDownload = new List<DtoClientFileRequest>();
            foreach (var driverId in _config.Drivers)
            {
                var module = serviceFileCopyModule.GetModule(driverId);
               

                var moduleFiles = serviceModule.GetModuleFiles(module.Guid);
                foreach (var file in moduleFiles.OrderBy(x => x.FileName))
                {
                    var fr = new DtoClientFileRequest();
                    fr.FileName = file.FileName;
                    fr.ModuleGuid = module.Guid;
                    filesToDownload.Add(fr);
                }
            }

            var comServer = _uow.ClientComServerRepository.Get().FirstOrDefault();

            var intercomKey = ictx.Settings.GetSettingValue(SettingStrings.IntercomKeyEncrypted);
            var decryptedKey = ictx.Encryption.DecryptText(intercomKey);

            foreach (var file in filesToDownload)
            {
                var destination = Path.Combine(_basePath, "Optional", "Drivers", "WinPE 10 x64", file.ModuleGuid);
                try
                {
                    Directory.CreateDirectory(destination);
                }
                catch { }
                new APICall().WieBuildApi.GetWinPeDriver(file, destination + "\\" + file.FileName,comServer.Url,"",decryptedKey);

                var extension = Path.GetExtension(file.FileName);
                var name = Path.GetFileNameWithoutExtension(file.FileName);
                if (!string.IsNullOrEmpty(extension) && extension.ToLower().Equals(".zip"))
                {
                    try
                    {
                        var path = Path.Combine(destination, file.FileName);
                        using (FileStream zipToOpen = new FileStream(path, FileMode.Open))
                        {
                            using (ZipArchive archive = new ZipArchive(zipToOpen))
                            {
                                ZipArchiveExtensions.ExtractToDirectory(archive, $"{destination}", true);
                            }
                        }
                        File.Delete(path);
                    }
                    catch(Exception ex)
                    {
                        ictx.Log.Error("Could not unzip file");
                        ictx.Log.Error(ex.Message);
                    }
                }
            }
            return true;
        }
    }
}