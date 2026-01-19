using System.ComponentModel;
using System.Net;
using System.Security.Cryptography;
using Toems_Common;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_Service;
using Toems_ServiceCore.EntityServices;

namespace Toems_ServiceCore.Infrastructure
{
    public class ServiceFileDownloader(InfrastructureContext ictx, ServiceExternalDownload externalDownloadService, UncServices uncService, ServiceFileHash fileHashService)
    {
        private EntityExternalDownload EntityDownload;
        private WebClient _webClient;
        private string _destinationDir;
        private int _progress;


        public async Task DownloadFile(EntityExternalDownload externalDownload)
        {
            _progress = 0;
            EntityDownload.DateDownloaded = DateTime.Now;
            EntityDownload.Status = EnumFileDownloader.DownloadStatus.Downloading;

            if (EntityDownload.Id == 0)
                externalDownloadService.Add(EntityDownload);
            else
                externalDownloadService.Update(EntityDownload);

            _destinationDir = Path.Combine(ictx.Settings.GetSettingValue(SettingStrings.StoragePath), "software_uploads", EntityDownload.ModuleGuid);
            var dirResult = CreateDirectory();
            if (dirResult != null)
            {
                EntityDownload.Status = EnumFileDownloader.DownloadStatus.Error;
                EntityDownload.ErrorMessage = dirResult;
                externalDownloadService.Update(EntityDownload);
                return;
            }


            if (uncService.NetUseWithCredentials() || uncService.LastError == 1219)
            {
                using (_webClient = new WebClient())
                {
                    _webClient.DownloadProgressChanged += wc_DownloadProgressChanged;
                    _webClient.DownloadFileCompleted += wc_DownloadFileCompleted;
                    await _webClient.DownloadFileTaskAsync(new Uri(EntityDownload.Url), Path.Combine(_destinationDir, EntityDownload.FileName));
                }
            }
        }

        private void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (_progress == e.ProgressPercentage) return;
            EntityDownload.Progress = e.ProgressPercentage.ToString();
            _progress = e.ProgressPercentage;
            var result = externalDownloadService.Update(EntityDownload);
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
                externalDownloadService.Update(EntityDownload);
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
                        externalDownloadService.Update(EntityDownload);
                    }
                }
                
            }
            else
            {
                EntityDownload.Status = EnumFileDownloader.DownloadStatus.Error;
                EntityDownload.ErrorMessage = e.Error.Message;
                externalDownloadService.Update(EntityDownload);
            }

        }

        private void CalculateSha256()
        {
            EntityDownload.Status = EnumFileDownloader.DownloadStatus.CalculatingSha256;
            externalDownloadService.Update(EntityDownload);
            
            if (uncService.NetUseWithCredentials() || uncService.LastError == 1219)
            {
                try
                {
                    fileHashService.FileHashingProgress += OnFileHashingProgress;
                    using (var stream = new BufferedStream(File.OpenRead(Path.Combine(_destinationDir, EntityDownload.FileName)), 1200000))
                        fileHashService.ComputeHash(stream,SHA256.Create());
                    EntityDownload.Sha256Hash = fileHashService.ToString();
                    EntityDownload.Status = EnumFileDownloader.DownloadStatus.Complete;
                }
                catch (Exception ex)
                {
                    EntityDownload.Status = EnumFileDownloader.DownloadStatus.Error;
                    EntityDownload.ErrorMessage = ex.Message;
                }
            }
            
            externalDownloadService.Update(EntityDownload);
        }

        private void CalculateMd5()
        {
            EntityDownload.Status = EnumFileDownloader.DownloadStatus.CalculatingMd5;
            externalDownloadService.Update(EntityDownload);
            
            if (uncService.NetUseWithCredentials() || uncService.LastError == 1219)
            {
                try
                {
                    fileHashService.FileHashingProgress += OnFileHashingProgress;
                    using (var stream = new BufferedStream(File.OpenRead(Path.Combine(_destinationDir, EntityDownload.FileName)), 1200000))
                        fileHashService.ComputeHash(stream, MD5.Create());
                    EntityDownload.Md5Hash = fileHashService.ToString();
                    EntityDownload.Status = EnumFileDownloader.DownloadStatus.Complete;
                }
                catch (Exception ex)
                {
                    EntityDownload.Status = EnumFileDownloader.DownloadStatus.Error;
                    EntityDownload.ErrorMessage = ex.Message;
                }
            }
            
            externalDownloadService.Update(EntityDownload);
        }

        public void OnFileHashingProgress(object sender, FileHashingProgressArgs e)
        {
            if (_progress == e.Percent) return;
            EntityDownload.Progress = e.Percent.ToString();
            _progress = e.Percent;
            var result = externalDownloadService.Update(EntityDownload);
            if (result == null)
            {
                //task deleted
                fileHashService.Cancel();
                return;
            }

            if (!result.Success)
            {
                //task deleted
                fileHashService.Cancel();
                return;
            }
        }

        private string CreateDirectory()
        {

            if (uncService.NetUseWithCredentials() || uncService.LastError == 1219)
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
}
