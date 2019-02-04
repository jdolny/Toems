using System.Collections.Generic;
using System.Linq;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceSoftwareAssetSoftware
    {
        private readonly UnitOfWork _uow;

        public ServiceSoftwareAssetSoftware()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult AddOrUpdate(List<EntitySoftwareAssetSoftware> software)
        {
            var first = software.FirstOrDefault();
            if (first == null) return new DtoActionResult { ErrorMessage = "No Software Was In The List", Id = 0 };
            var allSame = software.All(x => x.AssetId == first.AssetId);
            if (!allSame) return new DtoActionResult { ErrorMessage = "The List Must Be For A Single Asset.", Id = 0 };
            var actionResult = new DtoActionResult();

            foreach (var s in software)
            {
              
                var existingRelationship =
                    _uow.SoftwareAssetSoftwareRepository.GetFirstOrDefault(x => x.AssetId == first.AssetId && x.SoftwareInventoryId == s.SoftwareInventoryId);

                if (existingRelationship == null)
                {
                    //add it
                    _uow.SoftwareAssetSoftwareRepository.Insert(s);
                    _uow.Save();
                }
                else
                {
                    existingRelationship.MatchType = s.MatchType;
                    _uow.SoftwareAssetSoftwareRepository.Update(existingRelationship,existingRelationship.Id);
                    _uow.Save();
                }
            }


            actionResult.Success = true;
            return actionResult;
        }

        public DtoActionResult Delete(int id)
        {
            var sas = _uow.SoftwareAssetSoftwareRepository.GetById(id);
            if (sas == null)
                return new DtoActionResult() {ErrorMessage = "Could Not Find A Software Asset With That ID."};
            _uow.SoftwareAssetSoftwareRepository.Delete(id);
            _uow.Save();
            return new DtoActionResult() {Success = true,Id = sas.Id};
        }
    }
}