using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Toems_Common;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_Service.Entity;

namespace Toems_Service
{
    public class ServiceFileDownloader
    {
        private EntityExternalDownload EntityDownload;
        private WebClient _webClient;
        private ServiceFileHash _hasher;
        private readonly ServiceExternalDownload _serviceExternalDownload;
        private string _destinationDir;
        private int _progress;

        public ServiceFileDownloader(EntityExternalDownload externalDownload)
        {
            EntityDownload = externalDownload;
            _serviceExternalDownload = new ServiceExternalDownload();
            _progress = 0;
        }

        public async Task DownloadFile()
        {
            EntityDownload.DateDownloaded = DateTime.Now;
            EntityDownload.Status = EnumFileDownloader.DownloadStatus.Downloading;

            if (EntityDownload.Id == 0)
                _serviceExternalDownload.Add(EntityDownload);
            else
                _serviceExternalDownload.Update(EntityDownload);

            _destinationDir = Path.Combine(ServiceSetting.GetSettingValue(SettingStrings.StoragePath), "software_uploads", EntityDownload.ModuleGuid);
            var dirResult = CreateDirectory();
            if (dirResult != null)
            {
                EntityDownload.Status = EnumFileDownloader.DownloadStatus.Error;
                EntityDownload.ErrorMessage = dirResult;
                _serviceExternalDownload.Update(EntityDownload);
                return;
            }

            using (_webClient = new WebClient())
            {
                _webClient.DownloadProgressChanged += wc_DownloadProgressChanged;
                _webClient.DownloadFileCompleted += wc_DownloadFileCompleted;
                await _webClient.DownloadFileTaskAsync(new Uri(EntityDownload.Url), Path.Combine(_destinationDir,EntityDownload.FileName));
            }

          
        }

        private void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (_progress == e.ProgressPercentage) return;
            EntityDownload.Progress = e.ProgressPercentage.ToString();
            _progress = e.ProgressPercentage;
            var result = new ServiceExternalDownload().Update(EntityDownload);
            if (result == null)
            {
                //task deleted
                _webClient.CancelAsync();
                return;
            }

            if (!result.Success)
            {
                //task deleted
                _webClient.CancelAsync();
                return;
            }
        }

        private void wc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                return;
            }

            if (e.Error == null)
            {
                EntityDownload.DateDownloaded = DateTime.Now;
                _serviceExternalDownload.Update(EntityDownload);
                _progress = 0;
                CalculateMd5();
                if (!string.IsNullOrEmpty(EntityDownload.Sha256Hash))
                {
                    var expectedHash = EntityDownload.Sha256Hash.ToLower();
                    _progress = 0;
                    CalculateSha256();
                    if (!expectedHash.Equals(EntityDownload.Sha256Hash.ToLower()))
                    {
                        EntityDownload.Status = EnumFileDownloader.DownloadStatus.Error;
                        EntityDownload.ErrorMessage = "File Hash Mismatch";
                        _serviceExternalDownload.Update(EntityDownload);
                    }
                }
                
            }
            else
            {
                EntityDownload.Status = EnumFileDownloader.DownloadStatus.Error;
                EntityDownload.ErrorMessage = e.Error.Message;
                _serviceExternalDownload.Update(EntityDownload);
            }

        }

        private void CalculateSha256()
        {
            EntityDownload.Status = EnumFileDownloader.DownloadStatus.CalculatingSha256;
            _serviceExternalDownload.Update(EntityDownload);

            try
            {
                _hasher = new ServiceFileHash(SHA256.Create());
                _hasher.FileHashingProgress += OnFileHashingProgress;
                using (var stream = new BufferedStream(File.OpenRead(Path.Combine(_destinationDir,EntityDownload.FileName)), 1200000))
                    _hasher.ComputeHash(stream);
                EntityDownload.Sha256Hash = _hasher.ToString();
                EntityDownload.Status = EnumFileDownloader.DownloadStatus.Complete;
            }
            catch (Exception ex)
            {
                EntityDownload.Status = EnumFileDownloader.DownloadStatus.Error;
                EntityDownload.ErrorMessage = ex.Message;
            }
              
            _serviceExternalDownload.Update(EntityDownload);
        }

        private void CalculateMd5()
        {
            EntityDownload.Status = EnumFileDownloader.DownloadStatus.CalculatingMd5;
            _serviceExternalDownload.Update(EntityDownload);

            try
            {
                _hasher = new ServiceFileHash(MD5.Create());
                _hasher.FileHashingProgress += OnFileHashingProgress;
                using (var stream = new BufferedStream(File.OpenRead(Path.Combine(_destinationDir, EntityDownload.FileName)), 1200000))
                    _hasher.ComputeHash(stream);
                EntityDownload.Md5Hash = _hasher.ToString();
                EntityDownload.Status = EnumFileDownloader.DownloadStatus.Complete;
            }
            catch (Exception ex)
            {
                EntityDownload.Status = EnumFileDownloader.DownloadStatus.Error;
                EntityDownload.ErrorMessage = ex.Message;
            }

            _serviceExternalDownload.Update(EntityDownload);
        }

        public void OnFileHashingProgress(object sender, FileHashingProgressArgs e)
        {
            if (_progress == e.Percent) return;
            EntityDownload.Progress = e.Percent.ToString();
            _progress = e.Percent;
            var result = _serviceExternalDownload.Update(EntityDownload);
            if (result == null)
            {
                //task deleted
                _hasher.Cancel();
                return;
            }

            if (!result.Success)
            {
                //task deleted
                _hasher.Cancel();
                return;
            }
        }

        private string CreateDirectory()
        {
            using (var unc = new UncServices())
            {
                if (unc.NetUseWithCredentials() || unc.LastError == 1219)
                {
                    var directory = new DirectoryInfo(_destinationDir);
                    try
                    {
                        if (!directory.Exists)
                            directory.Create();
                        return null;
                    }
                    catch (Exception ex)
                    {
                        return ex.Message;
                    }
                }
                else
                {
                    return "Could Not Reach Storage Path";
                }
            }

        }

        /*proxy example
         WebProxy proxy = new WebProxy();
proxy.Address = new Uri("mywebproxyserver.com");
proxy.Credentials = new NetworkCredential("usernameHere", "pa****rdHere");  //These can be replaced by user input
proxy.UseDefaultCredentials = false;
proxy.BypassProxyOnLocal = false;  //still use the proxy for local addresses

WebClient client = new WebClient();
client.Proxy = proxy;

string doc = client.DownloadString("http://www.google.com/");
*/


    }
}
