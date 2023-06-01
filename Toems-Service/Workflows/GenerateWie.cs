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

namespace Toems_Service.Workflows
{

    public class GenerateWie
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(GenerateWie));

        private const string _fileName = "ToemsPE-Build.cmd";
        private string _fullPath;
        private string _basePath;
        private DtoWieConfig _config;
        private EntityWieBuild _wieBuild;
        private UnitOfWork _uow;
        private DtoActionResult _result;

        public GenerateWie(DtoWieConfig config)
        {
            _config = config;
            _wieBuild = new EntityWieBuild();
            _uow = new UnitOfWork();
            _result = new DtoActionResult();
            _result.Success = false;
        }
        public DtoActionResult Run()
        {
            _log.Info("Starting Wie Build");
            _wieBuild.Status = "Started";
            _basePath = Path.Combine(HttpContext.Current.Server.MapPath("~"), "private", "wie_builder");
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
            else if (_config.ImpersonationId == -1)
                return "Missing Impersonation User";
            else if (string.IsNullOrEmpty(_config.ComServers))
                return "No Com Servers Were Selected";


            if (new ServiceWieBuild().GetWieProcess().Any())
                return "An Active Build Process Is Currently Running";

            return "Success";
        }

        private bool StartProcess()
        {
            var imp = new ServiceImpersonationAccount().GetAccount(_config.ImpersonationId);
            var domain = string.Empty;
            var username = string.Empty;
            var password = new EncryptionServices().DecryptText(imp.Password);
            if (imp.Username.Contains("\\"))
            {
                username = imp.Username.Split('\\').Last();
                domain = imp.Username.Split('\\').First();
            }
            else
            {
                username = imp.Username;
                domain = Environment.MachineName;
            }

            try
            {
                _wieBuild.Pid = new RunasCs().RunAs(username, password, _fullPath, domain, 0, 2, 2, null, true, true, false);
                return true;
            }
            catch(Exception ex)
            {
                _log.Error("Could Not Start Wie Build Process.");
                _log.Error(ex.Message);
                _result.ErrorMessage = "Could Not Start Wie Build Process.";
                _wieBuild.Status = "Failed";
            }
            return false;

        }
        private bool CreateConfigFile()
        {
            var configContents = new StringBuilder();
            configContents.AppendLine("@echo off");
            configContents.AppendLine("pushd %~dp0");
            configContents.AppendLine("set TimeZone=" + _config.Timezone);
            configContents.AppendLine("set InputLocale=" + _config.InputLocale);
            configContents.AppendLine("set MyLang=" + _config.Language);
            configContents.AppendLine("set ComServerURL=" + _config.ComServers);
            configContents.AppendLine("set UniversalToken=" + _config.Token);
            configContents.AppendLine("set RestrictComServers=" + _config.RestrictComServers.ToString());
            configContents.AppendLine("set CreateISO=true");
            configContents.AppendLine("set LoginDebug=false");
            configContents.AppendLine("set PLATFORM=x64");
            configContents.AppendLine("set Pass=1");
            configContents.AppendLine("mkdir Status");
            configContents.AppendLine($"call .\\Scripts\\MakePE.cmd > .\\Status\\{_wieBuild.WieGuid}.log");
            configContents.AppendLine($"echo complete > .\\Status\\{_wieBuild.WieGuid}.complete");

            return new FilesystemServices().WritePath(_fullPath, configContents.ToString());

        }

        private bool AddDrivers()
        {
            var filesToDownload = new List<DtoClientFileRequest>();
            foreach (var driverId in _config.Drivers)
            {
                var module = new ServiceFileCopyModule().GetModule(driverId);
               

                var moduleFiles = new ServiceModule().GetModuleFiles(module.Guid);
                foreach (var file in moduleFiles.OrderBy(x => x.FileName))
                {
                    var fr = new DtoClientFileRequest();
                    fr.FileName = file.FileName;
                    fr.ModuleGuid = module.Guid;
                    filesToDownload.Add(fr);
                }
            }

            var comServer = _uow.ClientComServerRepository.Get().FirstOrDefault();

            var intercomKey = ServiceSetting.GetSettingValue(SettingStrings.IntercomKeyEncrypted);
            var decryptedKey = new EncryptionServices().DecryptText(intercomKey);

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
                        _log.Error("Could not unzip file");
                        _log.Error(ex.Message);
                    }
                }
            }
            return true;
        }
    }
}
