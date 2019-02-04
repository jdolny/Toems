using System.Collections.Generic;
using System.Linq;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceAssetCategory
    {
        private readonly UnitOfWork _uow;

        public ServiceAssetCategory()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult AddOrUpdate(List<EntityAssetCategory> assetCategories)
        {
            var first = assetCategories.FirstOrDefault();
            if (first == null) return new DtoActionResult { ErrorMessage = "No Assets Were In The List", Id = 0 };
            var allSame = assetCategories.All(x => x.AssetId == first.AssetId);
            if (!allSame) return new DtoActionResult { ErrorMessage = "The List Must Be For A Single Asset.", Id = 0 };
            var actionResult = new DtoActionResult();
            var pToRemove = _uow.AssetCategoryRepository.Get(x => x.AssetId == first.AssetId);
            foreach (var assetCategory in assetCategories)
            {
                var existing = _uow.AssetCategoryRepository.GetFirstOrDefault(x => x.AssetId == assetCategory.AssetId && x.CategoryId == assetCategory.CategoryId);
                if (existing == null)
                {
                    _uow.AssetCategoryRepository.Insert(assetCategory);
                }
                else
                {
                    pToRemove.Remove(existing);
                }

                actionResult.Id = 1;
            }

            //anything left in pToRemove does not exist anymore
            foreach (var p in pToRemove)
            {
                _uow.AssetCategoryRepository.Delete(p.Id);
            }

            _uow.Save();
            actionResult.Success = true;
            return actionResult;
        }

        public DtoActionResult DeleteAllForAsset(int assetId)
        {
            _uow.AssetCategoryRepository.DeleteRange(x => x.AssetId == assetId);
            _uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = 1;
            return actionResult;
        }
    }
}