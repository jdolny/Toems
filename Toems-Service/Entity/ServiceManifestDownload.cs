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
    public class ServiceManifestDownload
    {
        private readonly UnitOfWork _uow;

        public ServiceManifestDownload()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult Add(EntityWingetManifestDownload download)
        {
            var actionResult = new DtoActionResult();


            _uow.WingetManifestDownloadRepository.Insert(download);
            _uow.Save();
            actionResult.Success = true;
            actionResult.Id = download.Id;


            return actionResult;
        }


        public EntityWingetManifestDownload GetDownload(int downloadId)
        {
            return _uow.WingetManifestDownloadRepository.GetById(downloadId);
        }


   

        public DtoActionResult Update(EntityWingetManifestDownload download)
        {
            var u = GetDownload(download.Id);
            if (u == null) return new DtoActionResult {ErrorMessage = "Manifest Download Not Found", Id = 0};

            var actionResult = new DtoActionResult();


            _uow.WingetManifestDownloadRepository.Update(download, u.Id);
            _uow.Save();
            actionResult.Success = true;
            actionResult.Id = download.Id;

            return actionResult;
        }






    }
}