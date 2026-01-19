using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_DataModel;
using Toems_Service;
using Toems_Service.Entity;
using Toems_Service.Workflows;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceExternalDownload(EntityContext ectx)
    {
        public DtoActionResult Add(EntityExternalDownload download)
        {
            var actionResult = new DtoActionResult();


            ectx.Uow.ExternalDownloadRepository.Insert(download);
            ectx.Uow.Save();
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

            ectx.Uow.ExternalDownloadRepository.Delete(downloadId);
            ectx.Uow.Save();
            new FilesystemServices().DeleteExternalFile(u);

            var module = ectx.Uow.SoftwareModuleRepository.Get(x => x.Guid == u.ModuleGuid).FirstOrDefault();
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
            return ectx.Uow.ExternalDownloadRepository.GetById(downloadId);
        }

        public List<EntityExternalDownload> GetForModule(string moduleGuid)
        {

            return ectx.Uow.ExternalDownloadRepository.Get(x => x.ModuleGuid.Equals(moduleGuid));
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
            ectx.Uow.ExternalDownloadRepository.InsertRange(entityList);
            ectx.Uow.Save();

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
            return ectx.Uow.ExternalDownloadRepository.Count();
        }

        public DtoActionResult Update(EntityExternalDownload download)
        {
            var u = GetDownload(download.Id);
            if (u == null) return new DtoActionResult {ErrorMessage = "External Download Not Found", Id = 0};

            var actionResult = new DtoActionResult();


            ectx.Uow.ExternalDownloadRepository.Update(download, u.Id);
            ectx.Uow.Save();
            actionResult.Success = true;
            actionResult.Id = download.Id;

            return actionResult;
        }






    }
}