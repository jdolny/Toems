using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceUserGroupImagesMembership(EntityContext ectx)
    {
        public DtoActionResult AddOrUpdate(List<EntityUserGroupImages> groupImages, int userGroupId)
        {
            var actionResult = new DtoActionResult();
         
            var pToRemove = ectx.Uow.UserGroupImagesRepository.Get(x => x.UserGroupId == userGroupId);
            foreach (var image in groupImages)
            {
                var existing = ectx.Uow.UserGroupImagesRepository.GetFirstOrDefault(x => x.UserGroupId == userGroupId && x.ImageId == image.ImageId);
                    

                if (existing == null)
                {
                    ectx.Uow.UserGroupImagesRepository.Insert(image);
                }
                else
                {
                    pToRemove.Remove(existing);
                }
                
            }

            //anything left in pToRemove is no longer part of the image management
            foreach (var p in pToRemove)
            {
                ectx.Uow.UserGroupImagesRepository.Delete(p.Id);
            }

            ectx.Uow.Save();
            actionResult.Success = true;
            actionResult.Id = 1;
            return actionResult;
        }
    }
}