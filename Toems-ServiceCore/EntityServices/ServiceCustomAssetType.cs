using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceCustomAssetType(EntityContext ectx)
    {
        public DtoActionResult Add(EntityCustomAssetType assetType)
        {
            var actionResult = new DtoActionResult();

            var validationResult = Validate(assetType,true);
            if (validationResult.Success)
            {
                ectx.Uow.CustomAssetTypeRepository.Insert(assetType);
                ectx.Uow.Save();
                actionResult.Success = true;
                actionResult.Id = assetType.Id;
            }
            else
            {
                return new DtoActionResult() {ErrorMessage = validationResult.ErrorMessage};
            }

            return actionResult;
        }

        public DtoActionResult Delete(int assetTypeId)
        {
            var u = GetCustomAssetType(assetTypeId);
            if (u == null) return new DtoActionResult { ErrorMessage = "Custom Asset Type Not Found", Id = 0 };
            if(u.Id < 0) return new DtoActionResult { ErrorMessage = "Built-In Usage Types Cannot Be Modified", Id = 0 };

            var assets = ectx.Uow.AssetRepository.Get(x => x.AssetTypeId == assetTypeId);
            if (assets.Any())
                return new DtoActionResult()
                    {ErrorMessage = "Could Not Delete Asset Type.  There Are Currently Assets Using This Type"};

            var customAttributes = ectx.Uow.CustomAttributeRepository.Get(x => x.UsageType == assetTypeId);
            foreach (var ca in customAttributes)
            {
                ca.UsageType = -3;
                ectx.Uow.CustomAttributeRepository.Update(ca, ca.Id);
            }

            ectx.Uow.CustomAssetTypeRepository.Delete(assetTypeId);
            ectx.Uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.Id;
            return actionResult;
        }

        public EntityCustomAssetType GetCustomAssetType(int assetTypeId)
        {
            return ectx.Uow.CustomAssetTypeRepository.GetById(assetTypeId);
        }

        public List<EntityCustomAssetType> GetAll()
        {
            return ectx.Uow.CustomAssetTypeRepository.Get().OrderBy(x => x.Name).ToList();
        }

        public List<EntityCustomAssetType> Search(DtoSearchFilter filter)
        {
            return ectx.Uow.CustomAssetTypeRepository.Get(x => x.Name.Contains(filter.SearchText)).OrderBy(x => x.Name).ToList();
        }

        public string TotalCount()
        {
            return ectx.Uow.CustomAssetTypeRepository.Count();
        }

        public DtoActionResult Update(EntityCustomAssetType customAssetType)
        {
            var u = GetCustomAssetType(customAssetType.Id);
            if (u == null) return new DtoActionResult {ErrorMessage = "Custom Asset Type Not Found", Id = 0};
            if (u.Id < 0) return new DtoActionResult { ErrorMessage = "Built-In Usage Types Cannot Be Modified", Id = 0 };
            var actionResult = new DtoActionResult();

               var validationResult = Validate(customAssetType,false);
            if (validationResult.Success)
            {
                ectx.Uow.CustomAssetTypeRepository.Update(customAssetType, u.Id);
                ectx.Uow.Save();
                actionResult.Success = true;
                actionResult.Id = customAssetType.Id;
            }
            else
            {
                return new DtoActionResult() {ErrorMessage = validationResult.ErrorMessage};
            }
            return actionResult;
        }

        public DtoValidationResult Validate(EntityCustomAssetType assetType,bool isNew)
        {

            var validationResult = new DtoValidationResult { Success = true };

            if (string.IsNullOrEmpty(assetType.Name) || !assetType.Name.All(c => char.IsLetterOrDigit(c) || c == '_' || c == '-' || c == ' '))
            {
                validationResult.Success = false;
                validationResult.ErrorMessage = "Asset Type Name Is Not Valid";
                return validationResult;
            }

            if (isNew)
            {
                if (ectx.Uow.CustomAssetTypeRepository.Exists(h => h.Name == assetType.Name))
                {
                    validationResult.Success = false;
                    validationResult.ErrorMessage = "An Asset Type With This Name Already Exists";
                    return validationResult;
                }
            }
            else
            {
                var original = ectx.Uow.CustomAssetTypeRepository.GetById(assetType.Id);
                if (original.Name != assetType.Name)
                {
                    if (ectx.Uow.CustomAssetTypeRepository.Exists(h => h.Name == assetType.Name))
                    {
                        validationResult.Success = false;
                        validationResult.ErrorMessage = "An Asset Type With This Name Already Exists";
                        return validationResult;
                    }
                }
            }

            return validationResult;
        }

      

    


    }
}