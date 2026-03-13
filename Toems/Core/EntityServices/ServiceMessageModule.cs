using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceMessageModule(ServiceContext ctx)
    {
        public DtoActionResult AddModule(EntityMessageModule module)
        {
            var validationResult = ValidateModule(module, true);
            var actionResult = new DtoActionResult();
            if (validationResult.Success)
            {
               
                var moduleType = new EntityModule();
                moduleType.ModuleType = EnumModule.ModuleType.Message;
                moduleType.Guid = module.Guid;
                ctx.Uow.ModuleRepository.Insert(moduleType);
                ctx.Uow.Save();
                ctx.Uow.MessageModuleRepository.Insert(module);
                ctx.Uow.Save();
                actionResult.Success = true;
                actionResult.Id = module.Id;
            }
            else
            {
                actionResult.ErrorMessage = validationResult.ErrorMessage;
            }

            return actionResult;
        }

        public DtoActionResult DeleteModule(int moduleId)
        {
            var u = GetModule(moduleId);
            if (u == null) return new DtoActionResult { ErrorMessage = "Module Not Found", Id = 0 };
            var isActiveModule = ctx.Module.IsModuleActive(moduleId, EnumModule.ModuleType.Message);
            if (!string.IsNullOrEmpty(isActiveModule)) return new DtoActionResult() { ErrorMessage = isActiveModule, Id = 0 };
            if (string.IsNullOrEmpty(u.Guid)) return new DtoActionResult() { ErrorMessage = "Unknown Guid", Id = 0 };
            ctx.Uow.ModuleRepository.DeleteRange(x => x.Guid == u.Guid);
            ctx.Uow.Save();
            var actionResult = new DtoActionResult();

            actionResult.Success = true;
            actionResult.Id = u.Id;

            return actionResult;
        }

        public EntityMessageModule GetModule(int moduleId)
        {
            return ctx.Uow.MessageModuleRepository.GetById(moduleId);
        }

        public List<EntityMessageModule> SearchModules(DtoSearchFilterCategories filter)
        {
            var list = ctx.Uow.MessageModuleRepository.Get(s => (s.Name.Contains(filter.SearchText) || s.Guid.Contains(filter.SearchText)) && !s.Archived).OrderBy(x => x.Name).ToList();
            var categoryFilterIds = new List<int>();
            foreach (var catName in filter.Categories)
            {
                var category = ctx.Uow.CategoryRepository.GetFirstOrDefault(x => x.Name.Equals(catName));
                if (category != null)
                    categoryFilterIds.Add(category.Id);
            }

            var toRemove = new List<EntityMessageModule>();
            if (filter.CategoryType.Equals("Any Category"))
                return list.Take(filter.Limit).ToList();
            else if (filter.CategoryType.Equals("And Category"))
            {
                foreach (var module in list)
                {
                    var moduleCategories = ctx.Module.GetModuleCategories(module.Guid);
                    if (moduleCategories == null) continue;

                    if (filter.Categories.Count == 0)
                    {
                        if (moduleCategories.Count > 0)
                        {
                            toRemove.Add(module);
                            continue;
                        }
                    }

                    foreach (var id in categoryFilterIds)
                    {
                        if (moduleCategories.Any(x => x.CategoryId == id)) continue;
                        toRemove.Add(module);
                        break;
                    }
                }
            }
            else if (filter.CategoryType.Equals("Or Category"))
            {
                foreach (var module in list)
                {
                    var mCategories = ctx.Module.GetModuleCategories(module.Guid);
                    if (mCategories == null) continue;
                    if (filter.Categories.Count == 0)
                    {
                        if (mCategories.Count > 0)
                        {
                            toRemove.Add(module);
                            continue;
                        }
                    }

                    var catFound = false;
                    foreach (var id in categoryFilterIds)
                    {
                        if (mCategories.Any(x => x.CategoryId == id))
                        {
                            catFound = true;
                            break;
                        }

                    }
                    if (!catFound)
                        toRemove.Add(module);
                }
            }

            foreach (var p in toRemove)
            {
                list.Remove(p);
            }

            return list.Take(filter.Limit).ToList();
        }

        public List<EntityMessageModule> GetArchived(DtoSearchFilterCategories filter)
        {
            return ctx.Uow.MessageModuleRepository.Get(s => (s.Name.Contains(filter.SearchText) || s.Guid.Contains(filter.SearchText)) && s.Archived).OrderBy(x => x.Name).Take(filter.Limit).ToList();
        }

        public string TotalCount()
        {
            return ctx.Uow.MessageModuleRepository.Count(x => !x.Archived);
        }

        public string ArchivedCount()
        {
            return ctx.Uow.MessageModuleRepository.Count(x => x.Archived);
        }

        public DtoActionResult UpdateModule(EntityMessageModule module)
        {
            var u = GetModule(module.Id);
            if (u == null) return new DtoActionResult {ErrorMessage = "Module Not Found", Id = 0};
            var isActiveModule = ctx.Module.IsModuleActive(module.Id, EnumModule.ModuleType.Message);
            if (!string.IsNullOrEmpty(isActiveModule)) return new DtoActionResult() { ErrorMessage = isActiveModule, Id = 0 };
            var validationResult = ValidateModule(module, false);
            var actionResult = new DtoActionResult();
            if (validationResult.Success)
            {
                ctx.Uow.MessageModuleRepository.Update(module, module.Id);
                ctx.Uow.Save();
                actionResult.Success = true;
                actionResult.Id = module.Id;
            }
            else
            {
                actionResult.ErrorMessage = validationResult.ErrorMessage;
            }

            return actionResult;
        }

        private DtoValidationResult ValidateModule(EntityMessageModule module, bool isNew)
        {
            var validationResult = new DtoValidationResult { Success = true };

            if (string.IsNullOrEmpty(module.Name) || !module.Name.All(c => char.IsLetterOrDigit(c) || c == '_' || c == '-' || c == ' ' || c == '.'))
            {
                validationResult.Success = false;
                validationResult.ErrorMessage = "Module Name Is Not Valid";
                return validationResult;
            }

            if (isNew)
            {
                if (ctx.Uow.MessageModuleRepository.Exists(h => h.Name == module.Name))
                {
                    validationResult.Success = false;
                    validationResult.ErrorMessage = "A Module With This Name Already Exists";
                    return validationResult;
                }
            }
            else
            {
                var originalModule = ctx.Uow.MessageModuleRepository.GetById(module.Id);
                if (originalModule.Name != module.Name)
                {
                    if (ctx.Uow.MessageModuleRepository.Exists(h => h.Name == module.Name))
                    {
                        validationResult.Success = false;
                        validationResult.ErrorMessage = "A Module With This Name Already Exists";
                        return validationResult;
                    }
                }
            }

            return validationResult;
        }
    }
}