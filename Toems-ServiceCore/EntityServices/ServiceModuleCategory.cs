using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceModuleCategory(EntityContext ectx)
    {
        public DtoActionResult AddOrUpdate(List<EntityModuleCategory> moduleCategories)
        {
            var first = moduleCategories.FirstOrDefault();
            if (first == null) return new DtoActionResult { ErrorMessage = "No Modules Were In The List", Id = 0 };
            var allSame = moduleCategories.All(x => x.ModuleGuid == first.ModuleGuid);
            if (!allSame) return new DtoActionResult { ErrorMessage = "The List Must Be For A Single Module.", Id = 0 };
            var actionResult = new DtoActionResult();
            var pToRemove = ectx.Uow.ModuleCategoryRepository.Get(x => x.ModuleGuid == first.ModuleGuid);
            foreach (var moduleCategory in moduleCategories)
            {
                var existing = ectx.Uow.ModuleCategoryRepository.GetFirstOrDefault(x => x.ModuleGuid == moduleCategory.ModuleGuid && x.CategoryId == moduleCategory.CategoryId);
                if (existing == null)
                {
                    ectx.Uow.ModuleCategoryRepository.Insert(moduleCategory);
                }
                else
                {
                    pToRemove.Remove(existing);
                }

                actionResult.Id = 1;
            }

            //anything left in pToRemove does not exist anymore
            ectx.Uow.ModuleCategoryRepository.DeleteRange(pToRemove);
            ectx.Uow.Save();
            actionResult.Success = true;
            return actionResult;
        }

        public DtoActionResult DeleteAllForModule(string moduleGuid)
        {
            ectx.Uow.ModuleCategoryRepository.DeleteRange(x => x.ModuleGuid == moduleGuid);
            ectx.Uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = 1;
            return actionResult;
        }
    }
}