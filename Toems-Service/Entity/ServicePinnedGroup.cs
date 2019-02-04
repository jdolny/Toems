using System.Linq;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServicePinnedGroup
    {
        private readonly UnitOfWork _uow;

        public ServicePinnedGroup()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult Add(EntityPinnedGroup pinnedGroup)
        {
            var actionResult = new DtoActionResult();
            var u = _uow.PinnedGroupRepository.Get(x => x.GroupId == pinnedGroup.GroupId && x.UserId == pinnedGroup.UserId).FirstOrDefault();
            if (u != null) return new DtoActionResult() { ErrorMessage = "Group Is Already Pinned" };
            _uow.PinnedGroupRepository.Insert(pinnedGroup);
            _uow.Save();
            actionResult.Success = true;
            actionResult.Id = pinnedGroup.Id;

            return actionResult;
        }

        public DtoActionResult Delete(int groupId, int userId)
        {
            if (groupId == 0 || userId == 0) return new DtoActionResult() {ErrorMessage = "Group Not Defined"};
            var u = _uow.PinnedGroupRepository.Get(x => x.GroupId == groupId && x.UserId == userId).FirstOrDefault();
            if (u == null) return new DtoActionResult() {ErrorMessage = "Pinned Group Not Found"};
            _uow.PinnedGroupRepository.Delete(u.Id);
            _uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.Id;
            return actionResult;
        }

        public EntityPinnedGroup GetPinnedGroup(int pinnedGroupId)
        {
            return _uow.PinnedGroupRepository.GetById(pinnedGroupId);
        }
    }
}