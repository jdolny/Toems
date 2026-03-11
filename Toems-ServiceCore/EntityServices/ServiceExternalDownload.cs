using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_DataModel;
using Toems_ServiceCore.Infrastructure;
using Toems_ServiceCore.Workflows;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceExternalDownload(ServiceContext ctx)
    {
        public DtoActionResult Add(EntityExternalDownload download)
        {
            var actionResult = new DtoActionResult();


            ctx.Uow.ExternalDownloadRepository.Insert(download);
            ctx.Uow.Save();
            actionResult.Success = true;
            actionResult.Id = download.Id;


            return actionResult;
        }

        public DtoActionResult Delete(int downloadId)
        {
            var u = GetDownload(downloadId);
            if (u == null) return new DtoActionResult { ErrorMessage = "External Download Not Found", Id = 0 };

            var f = ctx.Module.GetModuleIdFromGuid(u.ModuleGuid);
            if (f != null)
            {
                var isActive = ctx.Module.IsModuleActive(f.moduleId, f.moduleType);
                if (!string.IsNullOrEmpty(isActive)) return new DtoActionResult() { ErrorMessage = isActive, Id = 0 };
            }

            ctx.Uow.ExternalDownloadRepository.Delete(downloadId);
            ctx.Uow.Save();
            ctx.Filessystem.DeleteExternalFile(u);

            var module = ctx.Uow.SoftwareModuleRepository.Get(x => x.Guid == u.ModuleGuid).FirstOrDefault();
            //arguments may need changed now that file is deleted, update arguments.
            if (module != null)
                ctx.SoftwareModule.GenerateArguments(module.Id);

            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.Id;
            return actionResult;
            
        }

        public EntityExternalDownload GetDownload(int downloadId)
        {
            return ctx.Uow.ExternalDownloadRepository.GetById(downloadId);
        }

        public List<EntityExternalDownload> GetForModule(string moduleGuid)
        {

            return ctx.Uow.ExternalDownloadRepository.Get(x => x.ModuleGuid.Equals(moduleGuid));
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
            ctx.Uow.ExternalDownloadRepository.InsertRange(entityList);
            ctx.Uow.Save();

            foreach (var ed in entityList)
            {
                await ctx.FileDownloader.DownloadFile(ed);
            }

            var first = fileDownloads.FirstOrDefault();
            if (first != null)
            {
                if (first.SyncWhenDone)
                {
                    ctx.FolderSync.RunAllServers();
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
            ctx.FileDownloader.DownloadFile(entityExternalDownload);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        }
        
        public string TotalCount()
        {
            return ctx.Uow.ExternalDownloadRepository.Count();
        }

        public DtoActionResult Update(EntityExternalDownload download)
        {
            var u = GetDownload(download.Id);
            if (u == null) return new DtoActionResult {ErrorMessage = "External Download Not Found", Id = 0};

            var actionResult = new DtoActionResult();


            ctx.Uow.ExternalDownloadRepository.Update(download, u.Id);
            ctx.Uow.Save();
            actionResult.Success = true;
            actionResult.Id = download.Id;

            return actionResult;
        }






    }
}