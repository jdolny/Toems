using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceManifestDownload(ServiceContext ctx)
    {
        public DtoActionResult Add(EntityWingetManifestDownload download)
        {
            var actionResult = new DtoActionResult();


            ctx.Uow.WingetManifestDownloadRepository.Insert(download);
            ctx.Uow.Save();
            actionResult.Success = true;
            actionResult.Id = download.Id;


            return actionResult;
        }


        public EntityWingetManifestDownload GetDownload(int downloadId)
        {
            return ctx.Uow.WingetManifestDownloadRepository.GetById(downloadId);
        }


   

        public DtoActionResult Update(EntityWingetManifestDownload download)
        {
            var u = GetDownload(download.Id);
            if (u == null) return new DtoActionResult {ErrorMessage = "Manifest Download Not Found", Id = 0};

            var actionResult = new DtoActionResult();


            ctx.Uow.WingetManifestDownloadRepository.Update(download, u.Id);
            ctx.Uow.Save();
            actionResult.Success = true;
            actionResult.Id = download.Id;

            return actionResult;
        }






    }
}