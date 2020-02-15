using System.Collections.Generic;
using System.Linq;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceImageCategory
    {
        private readonly UnitOfWork _uow;

        public ServiceImageCategory()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult AddOrUpdate(List<EntityImageCategory> imageCategories)
        {
            var first = imageCategories.FirstOrDefault();
            if (first == null) return new DtoActionResult { ErrorMessage = "No Images Were In The List", Id = 0 };
            var allSame = imageCategories.All(x => x.ImageId == first.ImageId);
            if (!allSame) return new DtoActionResult { ErrorMessage = "The List Must Be For A Single Image.", Id = 0 };
            var actionResult = new DtoActionResult();
            var pToRemove = _uow.ImageCategoryRepository.Get(x => x.ImageId == first.ImageId);
            foreach (var imageCategory in imageCategories)
            {
                var existing = _uow.ImageCategoryRepository.GetFirstOrDefault(x => x.ImageId == imageCategory.ImageId && x.CategoryId == imageCategory.CategoryId);
                if (existing == null)
                {
                    _uow.ImageCategoryRepository.Insert(imageCategory);
                }
                else
                {
                    pToRemove.Remove(existing);
                }

                actionResult.Id = 1;
            }

            //anything left in pToRemove does not exist anymore
            _uow.ImageCategoryRepository.DeleteRange(pToRemove);
            _uow.Save();
            actionResult.Success = true;
            return actionResult;
        }

        public DtoActionResult DeleteAllForImage(int imageId)
        {
            _uow.ImageCategoryRepository.DeleteRange(x => x.ImageId == imageId);
            _uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = 1;
            return actionResult;
        }
    }
}