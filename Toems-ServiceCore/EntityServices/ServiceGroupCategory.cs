using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceGroupCategory(ServiceContext ctx)
    {
        public DtoActionResult AddOrUpdate(List<EntityGroupCategory> groupCategories)
        {
            var first = groupCategories.FirstOrDefault();
            if (first == null) return new DtoActionResult { ErrorMessage = "No Groups Were In The List", Id = 0 };
            var allSame = groupCategories.All(x => x.GroupId == first.GroupId);
            if (!allSame) return new DtoActionResult { ErrorMessage = "The List Must Be For A Single Group.", Id = 0 };
            var actionResult = new DtoActionResult();
            var pToRemove = ctx.Uow.GroupCategoryRepository.Get(x => x.GroupId == first.GroupId);
            foreach (var groupCategory in groupCategories)
            {
                var existing = ctx.Uow.GroupCategoryRepository.GetFirstOrDefault(x => x.GroupId == groupCategory.GroupId && x.CategoryId == groupCategory.CategoryId);
                if (existing == null)
                {
                    ctx.Uow.GroupCategoryRepository.Insert(groupCategory);
                }
                else
                {
                    pToRemove.Remove(existing);
                }

                actionResult.Id = 1;
            }

            //anything left in pToRemove does not exist anymore
            ctx.Uow.GroupCategoryRepository.DeleteRange(pToRemove);
            ctx.Uow.Save();

            //assign categories to all group members
            ctx.Uow.ComputerCategoryRepository.DeleteRange(x => x.GroupId == first.GroupId);
            ctx.Uow.Save();


            var catList = new List<EntityComputerCategory>();
            var groupMembers = ctx.Uow.GroupRepository.GetGroupMembers(first.GroupId);
            foreach (var groupCategory in groupCategories)
            {
                foreach (var computer in groupMembers)
                {

                    var computerCategory = new EntityComputerCategory();
                    computerCategory.ComputerId = computer.Id;
                    computerCategory.CategoryId = groupCategory.CategoryId;
                    computerCategory.GroupId = first.GroupId;
                    catList.Add(computerCategory);
                }
            }

            ctx.Uow.ComputerCategoryRepository.InsertRange(catList);
            ctx.Uow.Save();
            actionResult.Id = 1;
            actionResult.Success = true;
            return actionResult;
        }

        public DtoActionResult DeleteAllForGroup(int groupId)
        {
            ctx.Uow.GroupCategoryRepository.DeleteRange(x => x.GroupId == groupId);
            ctx.Uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = 1;
            return actionResult;
        }
    }
}