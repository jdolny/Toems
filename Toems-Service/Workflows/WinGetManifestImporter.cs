using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using YamlDotNet.Serialization;
using System.Dynamic;
using System.Net;
using Toems_Common.Enum;
using Toems_Service.Entity;
using Toems_Common.Entity;
using System.IO.Compression;
using log4net;
using Toems_DataModel;
using YamlDotNet.Core;

namespace Toems_Service.Workflows
{
    public class WinGetManifestImporter
    {
        private WebClient _webClient;
        private const string _ManifestDownloadUrl = "https://github.com/microsoft/winget-pkgs/archive/refs/heads/master.zip";
        private EntityWingetManifestDownload _manifestDownload;
        private string _basePath;
        private ServiceManifestDownload _serviceManifestDownload;
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private UnitOfWork _uow;
        public WinGetManifestImporter()
        {
            _manifestDownload = new EntityWingetManifestDownload();
            _serviceManifestDownload = new ServiceManifestDownload();
            _uow = new UnitOfWork();

        }
        public void Run(string path = null)
        {
            Logger.Info("Starting Winget Manifest Import Process");

            if (path == null)
                _basePath = HttpContext.Current.Server.MapPath("~");
            _basePath = Path.Combine(path, "private", "winget_manifests");
            
            if (!DownloadManifests())
                return;

            if (!ExtractManifests())
                return;

            ClearTables();

            if (!ImportManifests())
            {
                CleanupFiles();
                return;
            }

            CleanupFiles();

            Logger.Info("Completed Winget Manifest Import Process");

        }
        private void ClearTables()
        {
            Logger.Info("Clearing Tables");
            new ServiceRawSql().ExecuteQuery("truncate winget_installer_manifests;truncate winget_locale_manifests;truncate winget_version_manifests;");
        }

        private bool CleanupFiles()
        {
            Logger.Info("Cleaning Files");
            var directory = Path.Combine(_basePath, "winget-pkgs-master");
            try
            {
                File.Delete(Path.Combine(_basePath, "master.zip"));
            }
            catch { //ignored
                  }
            if (!Directory.Exists(directory)) return true;
            try
            {
                Directory.Delete(directory, true);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return false;
            }


        }

        private bool ExtractManifests()
        {
            Logger.Info("Extracting Manifests");
            _manifestDownload.Status = EnumManifestImport.ImportStatus.Extracting;
            _serviceManifestDownload.Update(_manifestDownload);
            try
            {
                var path = Path.Combine(_basePath, "master.zip");
                using (FileStream zipToOpen = new FileStream(path, FileMode.Open))
                {
                    using (ZipArchive archive = new ZipArchive(zipToOpen))
                    {
                        ZipArchiveExtensions.ExtractToDirectory(archive, _basePath, true);
                    }
                }
                return true;
            }
            catch(Exception ex)
            {
                Logger.Info("Could not extract manifests");
                Logger.Error(ex.ToString());
                _manifestDownload.ErrorMessage = "Could not extract manifests";
                _manifestDownload.Status = EnumManifestImport.ImportStatus.Error;
                _serviceManifestDownload.Update(_manifestDownload);
            }
            return false;
           
        }
        private bool DownloadManifests()
        {
            Logger.Info("Downloading Updated Manifests");

            _manifestDownload.Url = _ManifestDownloadUrl;
            _manifestDownload.Status = EnumManifestImport.ImportStatus.Downloading;
            _serviceManifestDownload.Add(_manifestDownload);
            try
            {
                using (_webClient = new WebClient())
                {
                    _webClient.DownloadFile(new Uri(_ManifestDownloadUrl), Path.Combine(_basePath, "master.zip"));
                }
                return true;
            }
            catch(Exception ex)
            {
                Logger.Info("Could not download manifests");
                Logger.Error(ex.ToString());
                _manifestDownload.ErrorMessage = "Could not download manifests";
                _manifestDownload.Status = EnumManifestImport.ImportStatus.Error;
                _serviceManifestDownload.Update(_manifestDownload);
            }
            return false;
            
        }
     
        private bool ImportManifests()
        {
            Logger.Info("Importing Manifests");
            var path = Path.Combine(_basePath, "winget-pkgs-master", "manifests");
            var allFiles = Directory.GetFiles(path, "*.yaml", SearchOption.AllDirectories);
            var deserializer = new DeserializerBuilder().Build();
            int counter = 0;

            var listOfVersions = new List<EntityWingetVersionManifest>();
            var listOfInstallers = new List<EntityWingetInstallerManifest>();
            var listOfLocales = new List<EntityWingetLocaleManifest>();

            _manifestDownload.Status = EnumManifestImport.ImportStatus.Importing;
            _serviceManifestDownload.Update(_manifestDownload);

            foreach (var file in allFiles)
            {
                counter++;
                dynamic dynObject = deserializer.Deserialize<ExpandoObject>(System.IO.File.ReadAllText(file));
                IDictionary<string, object> manifestDict = dynObject;

                var manifestType = manifestDict.Where(kvp => kvp.Key.Equals("ManifestType"));
                var packageIdentifier = manifestDict.Where(kvp => kvp.Key.Equals("PackageIdentifier"));
                var packageVersion = manifestDict.Where(kvp => kvp.Key.Equals("PackageVersion"));
                if (!manifestType.Any() || !packageIdentifier.Any() || !packageVersion.Any()) continue;

                if (manifestType.First().Value.Equals("version"))
                {
                    var defaultLocale = manifestDict.Where(kvp => kvp.Key.Equals("DefaultLocale"));
                    if (defaultLocale == null)
                        continue;

                    var versionManifest = new EntityWingetVersionManifest();
                    versionManifest.PackageIdentifier = packageIdentifier.First().Value.ToString();
                    versionManifest.PackageVersion = packageVersion.First().Value.ToString();
                    versionManifest.DefaultLocale = defaultLocale.First().Value.ToString();

                    listOfVersions.Add(versionManifest);
                }
                else if (manifestType.First().Value.Equals("installer"))
                {
                    var isUser = false;
                    var isMachine = false;

                    var packageScope = manifestDict.Where(kvp => kvp.Key.Equals("Scope"));
                    if (packageScope.Any())
                    {
                        if (packageScope.First().Value.Equals("user"))
                            isUser = true;
                        else if (packageScope.First().Value.Equals("machine"))
                            isMachine = true;
                    }

                    var installers = manifestDict.Where(kvp => kvp.Key.Equals("Installers"));
                    foreach(var installer in installers)
                    {
                        var installerPropertiesList = installer.Value as List<object>;
                        foreach(var installerProperties in installerPropertiesList)
                        {
                            var dictProperties = (Dictionary<object,object>) installerProperties;
                            var installerScope = dictProperties.Where(kvp => kvp.Key.Equals("Scope"));
                            if (!installerScope.Any()) continue;
                            if (installerScope.First().Value.Equals("user"))
                                isUser = true;
                            if (installerScope.First().Value.Equals("machine"))
                                isMachine = true;
                        }
                    }
                    var installerManifest = new EntityWingetInstallerManifest();
                    installerManifest.PackageIdentifier = packageIdentifier.First().Value.ToString();
                    installerManifest.PackageVersion = packageVersion.First().Value.ToString();
                    if (isUser && isMachine)
                        installerManifest.Scope = "Both";
                    else if (isUser)
                        installerManifest.Scope = "User";
                    else if (isMachine)
                        installerManifest.Scope = "Machine";
                    else
                        installerManifest.Scope = "Not Specified";


                    listOfInstallers.Add(installerManifest);
                }
                else if (manifestType.First().Value.Equals("defaultLocale") || manifestType.First().Value.Equals("locale"))
                {
                    var packageLocale = manifestDict.Where(kvp => kvp.Key.Equals("PackageLocale"));
                    if (!packageLocale.Any())
                        continue;
                    if (!packageLocale.First().Value.Equals("en-US"))
                        continue;

                    var publisher = manifestDict.Where(kvp => kvp.Key.Equals("Publisher")).FirstOrDefault();
                    var publisherUrl = manifestDict.Where(kvp => kvp.Key.Equals("PublisherUrl")).FirstOrDefault();
                    var packageName = manifestDict.Where(kvp => kvp.Key.Equals("PackageName")).FirstOrDefault();
                    var packageUrl = manifestDict.Where(kvp => kvp.Key.Equals("PackageUrl")).FirstOrDefault();
                    var license = manifestDict.Where(kvp => kvp.Key.Equals("License")).FirstOrDefault();
                    var shortDescription = manifestDict.Where(kvp => kvp.Key.Equals("ShortDescription")).FirstOrDefault();
                    var tags = manifestDict.Where(kvp => kvp.Key.Equals("Tags")).FirstOrDefault();
                    var moniker = manifestDict.Where(kvp => kvp.Key.Equals("Moniker")).FirstOrDefault();

                    var localeManifest = new EntityWingetLocaleManifest();
                    localeManifest.PackageIdentifier = packageIdentifier.First().Value.ToString();
                    localeManifest.PackageVersion = packageVersion.First().Value.ToString();
                    localeManifest.Publisher = publisher.Value?.ToString();
                    localeManifest.PublisherUrl = publisherUrl.Value?.ToString();
                    localeManifest.PackageName = packageName.Value?.ToString();
                    localeManifest.PackageUrl = packageUrl.Value?.ToString();
                    localeManifest.License = license.Value?.ToString();
                    localeManifest.ShortDescription = shortDescription.Value?.ToString();
                    localeManifest.Moniker = moniker.Value?.ToString();
                    localeManifest.Locale = packageLocale.First().Value?.ToString();
                    var tagsList = (List<object>)tags.Value;
                    if (tagsList != null)
                    {
                        foreach (var t in tagsList)
                            localeManifest.Tags += t.ToString() + " ";
                        if(!string.IsNullOrEmpty(localeManifest.Tags))
                            localeManifest.Tags = localeManifest.Tags.Trim();
                    }
                    var versionArray = localeManifest.PackageVersion.Split('.');
                    if (versionArray == null) continue;
                    if (versionArray.Length == 0) continue;
                    if (versionArray.Length == 1)
                    {
                        localeManifest.Major = ParseVersion(versionArray[0]);
                    }
                    else if (versionArray.Length == 2)
                    {
                        localeManifest.Major = ParseVersion(versionArray[0]);
                        localeManifest.Minor = ParseVersion(versionArray[1]);
                    }
                    else if (versionArray.Length == 3)
                    {
                        localeManifest.Major = ParseVersion(versionArray[0]);
                        localeManifest.Minor = ParseVersion(versionArray[1]);
                        localeManifest.Build = ParseVersion(versionArray[2]);
                    }
                    else
                    {
                        localeManifest.Major = ParseVersion(versionArray[0]);
                        localeManifest.Minor = ParseVersion(versionArray[1]);
                        localeManifest.Build = ParseVersion(versionArray[2]);
                        localeManifest.Revision = ParseVersion(versionArray[3]);
                    }

                    listOfLocales.Add(localeManifest);

                }
               
                if(counter % 500 == 0 || counter == allFiles.Count())
                {
                    _uow.WingetInstallerManifestRepository.InsertRange(listOfInstallers);
                    _uow.WingetVersionManifestRepository.InsertRange(listOfVersions);
                    _uow.WingetLocaleManifestRepository.InsertRange(listOfLocales);
                    _uow.Save();
                    listOfInstallers.Clear();
                    listOfVersions.Clear();
                    listOfLocales.Clear();
                }
            }

            _manifestDownload.Status = EnumManifestImport.ImportStatus.Complete;
            _manifestDownload.DateDownloaded = DateTime.Now;
            _serviceManifestDownload.Update(_manifestDownload);
            return true;
        }

        private int ParseVersion(string num)
        {
            int value;
            if (!int.TryParse(num, out value))
                return 0;
            return value;
        }

    }
}
