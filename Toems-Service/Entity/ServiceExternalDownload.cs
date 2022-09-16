using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_DataModel;
using Toems_Service.Workflows;

namespace Toems_Service.Entity
{
    public class ServiceExternalDownload
    {
        private readonly UnitOfWork _uow;

        public ServiceExternalDownload()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult Add(EntityExternalDownload download)
        {
            var actionResult = new DtoActionResult();


            _uow.ExternalDownloadRepository.Insert(download);
            _uow.Save();
            actionResult.Success = true;
            actionResult.Id = download.Id;


            return actionResult;
        }

        public DtoActionResult Delete(int downloadId)
        {
            var u = GetDownload(downloadId);
            if (u == null) return new DtoActionResult { ErrorMessage = "External Download Not Found", Id = 0 };

            var f = new ServiceModule().GetModuleIdFromGuid(u.ModuleGuid);
            if (f != null)
            {
                var isActive = new ServiceModule().IsModuleActive(f.moduleId, f.moduleType);
                if (!string.IsNullOrEmpty(isActive)) return new DtoActionResult() { ErrorMessage = isActive, Id = 0 };
            }

            _uow.ExternalDownloadRepository.Delete(downloadId);
            _uow.Save();
            new FilesystemServices().DeleteExternalFile(u);

            var module = _uow.SoftwareModuleRepository.Get(x => x.Guid == u.ModuleGuid).FirstOrDefault();
            //arguments may need changed now that file is deleted, update arguments.
            if (module != null)
                new ServiceSoftwareModule().GenerateArguments(module.Id);

            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.Id;
            return actionResult;
            
        }

        public EntityExternalDownload GetDownload(int downloadId)
        {
            return _uow.ExternalDownloadRepository.GetById(downloadId);
        }

        public List<EntityExternalDownload> GetForModule(string moduleGuid)
        {

            return _uow.ExternalDownloadRepository.Get(x => x.ModuleGuid.Equals(moduleGuid));
        }

        public async Task BatchDownload(List<DtoFileDownload> fileDownloads)
        {
            if (fileDownloads.Count == 0) return;
            var entityList = new List<EntityExternalDownload>();
            foreach (var fileDownload in fileDownloads)
            {
                var entityExternalDownload = new EntityExternalDownload();
                entityExternalDownload.FileName = fileDownload.FileName;
                entityExternalDownload.Url = fileDownload.Url;
                entityExternalDownload.ModuleGuid = fileDownload.ModuleGuid;
                entityExternalDownload.Sha256Hash = fileDownload.Sha256;
                entityExternalDownload.Status = EnumFileDownloader.DownloadStatus.Queued;
                entityList.Add(entityExternalDownload);
            }
            _uow.ExternalDownloadRepository.InsertRange(entityList);
            _uow.Save();

            foreach (var ed in entityList)
            {
                await new ServiceFileDownloader(ed).DownloadFile();
            }

            var first = fileDownloads.FirstOrDefault();
            if (first != null)
            {
                if (first.SyncWhenDone)
                {
                    new FolderSync().RunAllServers();
                }
            }
        }

        public void DownloadFile(DtoFileDownload fileDownload)
        {
            var entityExternalDownload = new EntityExternalDownload();
            entityExternalDownload.FileName = fileDownload.FileName;
            entityExternalDownload.Url = fileDownload.Url;
            entityExternalDownload.ModuleGuid = fileDownload.ModuleGuid;
            entityExternalDownload.Status = EnumFileDownloader.DownloadStatus.Queued;
            entityExternalDownload.Id = fileDownload.ExternalDownloadId;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            new ServiceFileDownloader(entityExternalDownload).DownloadFile();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        }





        public string TotalCount()
        {
            return _uow.ExternalDownloadRepository.Count();
        }

        public DtoActionResult Update(EntityExternalDownload download)
        {
            var u = GetDownload(download.Id);
            if (u == null) return new DtoActionResult {ErrorMessage = "External Download Not Found", Id = 0};

            var actionResult = new DtoActionResult();


            _uow.ExternalDownloadRepository.Update(download, u.Id);
            _uow.Save();
            actionResult.Success = true;
            actionResult.Id = download.Id;

            return actionResult;
        }






    }
}