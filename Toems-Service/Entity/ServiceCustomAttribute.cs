using System.Collections.Generic;
using System.Linq;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceCustomAttribute
    {
        private readonly UnitOfWork _uow;

        public ServiceCustomAttribute()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult Add(EntityCustomAttribute customAttribute)
        {
            var actionResult = new DtoActionResult();

            var validationResult = Validate(customAttribute,true);
            if (validationResult.Success)
            {
                _uow.CustomAttributeRepository.Insert(customAttribute);
                _uow.Save();
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

            _uow.CustomAttributeRepository.Delete(customAttributeId);
            _uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.Id;
            return actionResult;
        }

        public EntityCustomAttribute GetCustomAttribute(int customAttributeId)
        {
            return _uow.CustomAttributeRepository.GetById(customAttributeId);
        }

        public List<EntityCustomAttribute> GetAll()
        {
            return _uow.CustomAttributeRepository.Get().OrderBy(x => x.Name).ToList();
        }

        public List<EntityCustomAttribute> GetForBuiltInComputers()
        {
            return _uow.CustomAttributeRepository.Get().OrderBy(x => x.Name).Where(x => x.UsageType == -1 || x.UsageType == -3).ToList();
        }

        public List<EntityCustomAttribute> GetForAssetType(int assetTypeId )
        {
            return _uow.CustomAttributeRepository.Get().OrderBy(x => x.Name).Where(x => x.UsageType == -3 || x.UsageType == assetTypeId).ToList();
        }

        public List<DtoCustomAttributeWithType> Search(DtoSearchFilter filter)
        {
            var customAttributes = _uow.CustomAttributeRepository.GetAttributeWithType();
            return customAttributes.Where(x => x.Name.ToLower().Contains(filter.SearchText.ToLower())).OrderBy(x => x.Name).ToList();
        }

        public string TotalCount()
        {
            return _uow.CustomAttributeRepository.Count();
        }

        public DtoActionResult Update(EntityCustomAttribute customAttribute)
        {
            var u = GetCustomAttribute(customAttribute.Id);
            if (u == null) return new DtoActionResult {ErrorMessage = "Custom Attribute Not Found", Id = 0};

            var actionResult = new DtoActionResult();

               var validationResult = Validate(customAttribute,false);
            if (validationResult.Success)
            {
                _uow.CustomAttributeRepository.Update(customAttribute, u.Id);
                _uow.Save();
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
                if (_uow.CustomAttributeRepository.Exists(h => h.Name == customAttribute.Name))
                {
                    validationResult.Success = false;
                    validationResult.ErrorMessage = "A Custom Attribute With This Name Already Exists";
                    return validationResult;
                }
            }
            else
            {
                var original = _uow.CustomAttributeRepository.GetById(customAttribute.Id);
                if (original.Name != customAttribute.Name)
                {
                    if (_uow.CategoryRepository.Exists(h => h.Name == customAttribute.Name))
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