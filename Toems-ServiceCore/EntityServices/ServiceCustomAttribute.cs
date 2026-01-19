using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceCustomAttribute(EntityContext ectx)
    {
        public DtoActionResult Add(EntityCustomAttribute customAttribute)
        {
            var actionResult = new DtoActionResult();

            var validationResult = Validate(customAttribute,true);
            if (validationResult.Success)
            {
                ectx.Uow.CustomAttributeRepository.Insert(customAttribute);
                ectx.Uow.Save();
                actionResult.Success = true;
                actionResult.Id = customAttribute.Id;
            }
            else
            {
                return new DtoActionResult() {ErrorMessage = validationResult.ErrorMessage};
            }

            return actionResult;
        }

        public DtoActionResult Delete(int customAttributeId)
        {
            var u = GetCustomAttribute(customAttributeId);
            if (u == null) return new DtoActionResult { ErrorMessage = "Custom Attribute Not Found", Id = 0 };

            ectx.Uow.CustomAttributeRepository.Delete(customAttributeId);
            ectx.Uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.Id;
            return actionResult;
        }

        public EntityCustomAttribute GetCustomAttribute(int customAttributeId)
        {
            return ectx.Uow.CustomAttributeRepository.GetById(customAttributeId);
        }

        public List<EntityCustomAttribute> GetAll()
        {
            return ectx.Uow.CustomAttributeRepository.Get().OrderBy(x => x.Name).ToList();
        }

        public List<EntityCustomAttribute> GetForBuiltInComputers()
        {
            return ectx.Uow.CustomAttributeRepository.Get().OrderBy(x => x.Name).Where(x => x.UsageType == -1 || x.UsageType == -3).ToList();
        }

        public List<EntityCustomAttribute> GetForAssetType(int assetTypeId )
        {
            return ectx.Uow.CustomAttributeRepository.Get().OrderBy(x => x.Name).Where(x => x.UsageType == -3 || x.UsageType == assetTypeId).ToList();
        }

        public List<DtoCustomAttributeWithType> Search(DtoSearchFilter filter)
        {
            var customAttributes = ectx.Uow.CustomAttributeRepository.GetAttributeWithType();
            return customAttributes.Where(x => x.Name.ToLower().Contains(filter.SearchText.ToLower())).OrderBy(x => x.Name).ToList();
        }

        public string TotalCount()
        {
            return ectx.Uow.CustomAttributeRepository.Count();
        }

        public DtoActionResult Update(EntityCustomAttribute customAttribute)
        {
            var u = GetCustomAttribute(customAttribute.Id);
            if (u == null) return new DtoActionResult {ErrorMessage = "Custom Attribute Not Found", Id = 0};

            var actionResult = new DtoActionResult();

               var validationResult = Validate(customAttribute,false);
            if (validationResult.Success)
            {
                ectx.Uow.CustomAttributeRepository.Update(customAttribute, u.Id);
                ectx.Uow.Save();
                actionResult.Success = true;
                actionResult.Id = customAttribute.Id;
            }
            else
            {
                return new DtoActionResult() {ErrorMessage = validationResult.ErrorMessage};
            }
            return actionResult;
        }

        public DtoValidationResult Validate(EntityCustomAttribute customAttribute, bool isNew)
        {
            var validationResult = new DtoValidationResult { Success = true };

            if (string.IsNullOrEmpty(customAttribute.Name) || !customAttribute.Name.All(c => char.IsLetterOrDigit(c) || c == '_' || c == '-'))
            {
                validationResult.Success = false;
                validationResult.ErrorMessage = "Custom Attribute Name Is Not Valid";
                return validationResult;
            }

            if (isNew)
            {
                if (ectx.Uow.CustomAttributeRepository.Exists(h => h.Name == customAttribute.Name))
                {
                    validationResult.Success = false;
                    validationResult.ErrorMessage = "A Custom Attribute With This Name Already Exists";
                    return validationResult;
                }
            }
            else
            {
                var original = ectx.Uow.CustomAttributeRepository.GetById(customAttribute.Id);
                if (original.Name != customAttribute.Name)
                {
                    if (ectx.Uow.CategoryRepository.Exists(h => h.Name == customAttribute.Name))
                    {
                        validationResult.Success = false;
                        validationResult.ErrorMessage = "A Custom Attribute With This Name Already Exists";
                        return validationResult;
                    }
                }
            }

            return validationResult;
        }

      

    


    }
}