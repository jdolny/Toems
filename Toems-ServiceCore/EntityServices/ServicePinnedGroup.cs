using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServicePinnedGroup(EntityContext ectx)
    {
        public DtoActionResult Add(EntityPinnedGroup pinnedGroup)
        {
            var actionResult = new DtoActionResult();
            var u = ectx.Uow.PinnedGroupRepository.Get(x => x.GroupId == pinnedGroup.GroupId && x.UserId == pinnedGroup.UserId).FirstOrDefault();
            if (u != null) return new DtoActionResult() { ErrorMessage = "Group Is Already Pinned" };
            ectx.Uow.PinnedGroupRepository.Insert(pinnedGroup);
            ectx.Uow.Save();
            actionResult.Success = true;
            actionResult.Id = pinnedGroup.Id;

            return actionResult;
        }

        public DtoActionResult Delete(int groupId, int userId)
        {
            if (groupId == 0 || userId == 0) return new DtoActionResult() {ErrorMessage = "Group Not Defined"};
            var u = ectx.Uow.PinnedGroupRepository.Get(x => x.GroupId == groupId && x.UserId == userId).FirstOrDefault();
            if (u == null) return new DtoActionResult() {ErrorMessage = "Pinned Group Not Found"};
            ectx.Uow.PinnedGroupRepository.Delete(u.Id);
            ectx.Uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.Id;
            return actionResult;
        }

        public EntityPinnedGroup GetPinnedGroup(int pinnedGroupId)
        {
            return ectx.Uow.PinnedGroupRepository.GetById(pinnedGroupId);
        }
    }
}