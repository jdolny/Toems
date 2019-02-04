using System.Collections.Generic;
using System.Linq;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceGroupCategory
    {
        private readonly UnitOfWork _uow;

        public ServiceGroupCategory()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult AddOrUpdate(List<EntityGroupCategory> groupCategories)
        {
            var first = groupCategories.FirstOrDefault();
            if (first == null) return new DtoActionResult { ErrorMessage = "No Groups Were In The List", Id = 0 };
            var allSame = groupCategories.All(x => x.GroupId == first.GroupId);
            if (!allSame) return new DtoActionResult { ErrorMessage = "The List Must Be For A Single Group.", Id = 0 };
            var actionResult = new DtoActionResult();
            var pToRemove = _uow.GroupCategoryRepository.Get(x => x.GroupId == first.GroupId);
            foreach (var groupCategory in groupCategories)
            {
                var existing = _uow.GroupCategoryRepository.GetFirstOrDefault(x => x.GroupId == groupCategory.GroupId && x.CategoryId == groupCategory.CategoryId);
                if (existing == null)
                {
                    _uow.GroupCategoryRepository.Insert(groupCategory);
                }
                else
                {
                    pToRemove.Remove(existing);
                }

                actionResult.Id = 1;
            }

            //anything left in pToRemove does not exist anymore
            _uow.GroupCategoryRepository.DeleteRange(pToRemove);
            _uow.Save();

            //assign categories to all group members
            _uow.ComputerCategoryRepository.DeleteRange(x => x.GroupId == first.GroupId);
            _uow.Save();


            var catList = new List<EntityComputerCategory>();
            var groupMembers = _uow.GroupRepository.GetGroupMembers(first.GroupId);
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

            _uow.ComputerCategoryRepository.InsertRange(catList);
            _uow.Save();
            actionResult.Id = 1;
            actionResult.Success = true;
            return actionResult;
        }

        public DtoActionResult DeleteAllForGroup(int groupId)
        {
            _uow.GroupCategoryRepository.DeleteRange(x => x.GroupId == groupId);
            _uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = 1;
            return actionResult;
        }
    }
}