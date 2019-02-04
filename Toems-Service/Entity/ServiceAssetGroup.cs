using System;
using System.Collections.Generic;
using System.Linq;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceAssetGroup
    {
        private readonly UnitOfWork _uow;

        public ServiceAssetGroup()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult Add(EntityAssetGroup assetGroupType)
        {
            var actionResult = new DtoActionResult();

            var validationResult = Validate(assetGroupType,true);
            if (validationResult.Success)
            {
                _uow.AssetGroupRepository.Insert(assetGroupType);
                _uow.Save();
                actionResult.Success = true;
                actionResult.Id = assetGroupType.Id;
            }
            else
            {
                return new DtoActionResult() {ErrorMessage = validationResult.ErrorMessage};
            }

            return actionResult;
        }

        public DtoActionResult Delete(int assetGroupTypeId)
        {
            var u = GetAssetGroup(assetGroupTypeId);
            if (u == null) return new DtoActionResult { ErrorMessage = "Asset Group Not Found", Id = 0 };

            _uow.AssetGroupRepository.Delete(assetGroupTypeId);
            _uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.Id;
            return actionResult;
        }

        public EntityAssetGroup GetAssetGroup(int assetGroupTypeId)
        {
            return _uow.AssetGroupRepository.GetById(assetGroupTypeId);
        }

        public List<EntityAssetGroup> GetAll()
        {
            return _uow.AssetGroupRepository.Get().OrderBy(x => x.Name).ToList();
        }

        public List<EntityAssetGroup> Search(DtoSearchFilter filter)
        {
            return _uow.AssetGroupRepository.Get(x => x.Name.Contains(filter.SearchText)).OrderBy(x => x.Name).ToList();
           
        }

        public string TotalCount()
        {
            return _uow.AssetGroupRepository.Count();
        }

        public DtoActionResult Update(EntityAssetGroup assetGroup)
        {
            var u = GetAssetGroup(assetGroup.Id);
            if (u == null) return new DtoActionResult {ErrorMessage = "Asset Group Not Found", Id = 0};

            var actionResult = new DtoActionResult();

               var validationResult = Validate(assetGroup,false);
            if (validationResult.Success)
            {
                _uow.AssetGroupRepository.Update(assetGroup, u.Id);
                _uow.Save();
                actionResult.Success = true;
                actionResult.Id = assetGroup.Id;
            }
            else
            {
                return new DtoActionResult() {ErrorMessage = validationResult.ErrorMessage};
            }
            return actionResult;
        }

        public DtoValidationResult Validate(EntityAssetGroup assetGroup, bool isNew)
        {
            var validationResult = new DtoValidationResult { Success = true };

            if (string.IsNullOrEmpty(assetGroup.Name) || !assetGroup.Name.All(c => char.IsLetterOrDigit(c) || c == '_' || c == '-' || c == ' '))
            {
                validationResult.Success = false;
                validationResult.ErrorMessage = "Asset Group Name Is Not Valid";
                return validationResult;
            }

            if (isNew)
            {
                if (_uow.AssetGroupRepository.Exists(h => h.Name == assetGroup.Name))
                {
                    validationResult.Success = false;
                    validationResult.ErrorMessage = "An Asset Group With This Name Already Exists";
                    return validationResult;
                }
            }
            else
            {
                var original = _uow.AssetGroupRepository.GetById(assetGroup.Id);
                if (original.Name != assetGroup.Name)
                {
                    if (_uow.AssetGroupRepository.Exists(h => h.Name == assetGroup.Name))
                    {
                        validationResult.Success = false;
                        validationResult.ErrorMessage = "An Asset Group With This Name Already Exists";
                        return validationResult;
                    }
                }
            }

            return validationResult;
        }

        public List<DtoAssetWithType> GetGroupMembers(int assetGroupId)
        {
            return _uow.AssetGroupRepository.GetAssetGroupMembers(assetGroupId);
        }



       

      
     
      

       
    }
}