using System.Collections.Generic;
using System.Linq;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceModuleCategory
    {
        private readonly UnitOfWork _uow;

        public ServiceModuleCategory()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult AddOrUpdate(List<EntityModuleCategory> moduleCategories)
        {
            var first = moduleCategories.FirstOrDefault();
            if (first == null) return new DtoActionResult { ErrorMessage = "No Modules Were In The List", Id = 0 };
            var allSame = moduleCategories.All(x => x.ModuleGuid == first.ModuleGuid);
            if (!allSame) return new DtoActionResult { ErrorMessage = "The List Must Be For A Single Module.", Id = 0 };
            var actionResult = new DtoActionResult();
            var pToRemove = _uow.ModuleCategoryRepository.Get(x => x.ModuleGuid == first.ModuleGuid);
            foreach (var moduleCategory in moduleCategories)
            {
                var existing = _uow.ModuleCategoryRepository.GetFirstOrDefault(x => x.ModuleGuid == moduleCategory.ModuleGuid && x.CategoryId == moduleCategory.CategoryId);
                if (existing == null)
                {
                    _uow.ModuleCategoryRepository.Insert(moduleCategory);
                }
                else
                {
                    pToRemove.Remove(existing);
                }

                actionResult.Id = 1;
            }

            //anything left in pToRemove does not exist anymore
            _uow.ModuleCategoryRepository.DeleteRange(pToRemove);
            _uow.Save();
            actionResult.Success = true;
            return actionResult;
        }

        public DtoActionResult DeleteAllForModule(string moduleGuid)
        {
            _uow.ModuleCategoryRepository.DeleteRange(x => x.ModuleGuid == moduleGuid);
            _uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = 1;
            return actionResult;
        }
    }
}