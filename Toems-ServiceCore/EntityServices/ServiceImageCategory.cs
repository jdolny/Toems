using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceImageCategory(EntityContext ectx)
    {
        public DtoActionResult AddOrUpdate(List<EntityImageCategory> imageCategories)
        {
            var first = imageCategories.FirstOrDefault();
            if (first == null) return new DtoActionResult { ErrorMessage = "No Images Were In The List", Id = 0 };
            var allSame = imageCategories.All(x => x.ImageId == first.ImageId);
            if (!allSame) return new DtoActionResult { ErrorMessage = "The List Must Be For A Single Image.", Id = 0 };
            var actionResult = new DtoActionResult();
            var pToRemove = ectx.Uow.ImageCategoryRepository.Get(x => x.ImageId == first.ImageId);
            foreach (var imageCategory in imageCategories)
            {
                var existing = ectx.Uow.ImageCategoryRepository.GetFirstOrDefault(x => x.ImageId == imageCategory.ImageId && x.CategoryId == imageCategory.CategoryId);
                if (existing == null)
                {
                    ectx.Uow.ImageCategoryRepository.Insert(imageCategory);
                }
                else
                {
                    pToRemove.Remove(existing);
                }

                actionResult.Id = 1;
            }

            //anything left in pToRemove does not exist anymore
            ectx.Uow.ImageCategoryRepository.DeleteRange(pToRemove);
            ectx.Uow.Save();
            actionResult.Success = true;
            return actionResult;
        }

        public DtoActionResult DeleteAllForImage(int imageId)
        {
            ectx.Uow.ImageCategoryRepository.DeleteRange(x => x.ImageId == imageId);
            ectx.Uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = 1;
            return actionResult;
        }
    }
}