using System.Collections.Generic;
using System.Linq;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceAssetAttributes
    {
        private readonly UnitOfWork _uow;

        public ServiceAssetAttributes()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult AddOrUpdate(List<EntityAssetAttribute> attributes)
        {
            var first = attributes.FirstOrDefault();
            if (first == null) return new DtoActionResult { ErrorMessage = "No Attributes Were In The List", Id = 0 };
            var allSame = attributes.All(x => x.AssetId == first.AssetId);
            if (!allSame) return new DtoActionResult { ErrorMessage = "The List Must Be For A Single Asset.", Id = 0 };
            var actionResult = new DtoActionResult();

            foreach (var ca in attributes)
            {
                if(_uow.CustomAttributeRepository.GetById(ca.CustomAttributeId) == null)
                    continue;

                var existingRelationship =
                    _uow.AssetAttributeRepository.GetFirstOrDefault(x => x.AssetId == first.AssetId && x.CustomAttributeId == ca.CustomAttributeId);

                if (existingRelationship == null && !string.IsNullOrEmpty(ca.Value))
                {
                    //add it
                    _uow.AssetAttributeRepository.Insert(ca);
                    _uow.Save();
                }

                else if(existingRelationship != null && string.IsNullOrEmpty(ca.Value))
                {
                    //delete it
                    _uow.AssetAttributeRepository.Delete(existingRelationship.Id);
                    _uow.Save();
                }

                else if (existingRelationship != null && !string.IsNullOrEmpty(ca.Value))
                {
                    //update it
                    existingRelationship.Value = ca.Value;
                    _uow.AssetAttributeRepository.Update(ca,ca.Id);
                    _uow.Save();
                }
            }


            actionResult.Success = true;
            return actionResult;
        }
    }
}