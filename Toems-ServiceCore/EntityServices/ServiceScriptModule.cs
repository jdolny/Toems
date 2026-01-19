using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceScriptModule(EntityContext ectx, ServiceModule moduleService)
    {
        public DtoActionResult AddModule(EntityScriptModule module)
        {

            var validationResult = ValidateModule(module, true);
            var actionResult = new DtoActionResult();
            if (validationResult.Success)
            {
                if(module.ScriptType == EnumScriptModule.ScriptType.ImagingClient_Bash)
                {
                    var fixedLineEnding = module.ScriptContents.Replace("\r\n", "\n");
                    module.ScriptContents = fixedLineEnding;
                }
                ectx.Uow.ScriptModuleRepository.Insert(module);
                var moduleType = new EntityModule();
                moduleType.ModuleType = EnumModule.ModuleType.Script;
                moduleType.Guid = module.Guid;
                ectx.Uow.ModuleRepository.Insert(moduleType);
                ectx.Uow.Save();
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
            if (u == null) return new DtoActionResult {ErrorMessage = "Module Not Found", Id = 0};
            var isActiveModule = moduleService.IsModuleActive(moduleId, EnumModule.ModuleType.Script);
            if (!string.IsNullOrEmpty(isActiveModule)) return new DtoActionResult() { ErrorMessage = isActiveModule, Id = 0 };
            if (string.IsNullOrEmpty(u.Guid)) return new DtoActionResult() { ErrorMessage = "Unknown Guid", Id = 0 };
            ectx.Uow.ModuleRepository.DeleteRange(x => x.Guid == u.Guid);
            //ectx.Uow.ScriptModuleRepository.Delete(moduleId);

            var modulesWithCondition = ectx.Uow.PolicyModulesRepository.Get(x => x.ConditionId == u.Id);
            foreach(var module in modulesWithCondition)
            {
                module.ConditionId = -1;
                ectx.Uow.PolicyModulesRepository.Update(module, module.Id);
            }

            var policiesWithCondition = ectx.Uow.PolicyRepository.Get(x => x.ConditionId == u.Id);
            foreach(var policy in policiesWithCondition)
            {
                policy.ConditionId = -1;
                ectx.Uow.PolicyRepository.Update(policy, policy.Id);
            }

            ectx.Uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.Id;
            return actionResult;
        }

        public EntityScriptModule GetModule(int moduleId)
        {
            return ectx.Uow.ScriptModuleRepository.GetById(moduleId);
        }

        public EntityScriptModule GetModuleByGuid(string guid)
        {
            return ectx.Uow.ScriptModuleRepository.GetFirstOrDefault(x => x.Guid == guid);
        }

        public List<EntityScriptModule> GetAllWithInventory()
        {
            return ectx.Uow.ScriptModuleRepository.Get(x => x.AddInventoryCollection);
        }

        public List<EntityScriptModule> GetImagingScripts()
        {
            return ectx.Uow.ScriptModuleRepository.Get(x => x.ScriptType == (EnumScriptModule.ScriptType.ImagingClient_Bash) || x.ScriptType == (EnumScriptModule.ScriptType.ImagingClient_Powershell));
        }

        public List<EntityScriptModule> SearchModules(DtoSearchFilterCategories filter)
        {
            var list = ectx.Uow.ScriptModuleRepository.Get(s => (s.Name.Contains(filter.SearchText) || s.Guid.Contains(filter.SearchText)) && !s.Archived).OrderBy(x => x.Name).ToList();
            var categoryFilterIds = new List<int>();
            foreach (var catName in filter.Categories)
            {
                var category = ectx.Uow.CategoryRepository.GetFirstOrDefault(x => x.Name.Equals(catName));
                if (category != null)
                    categoryFilterIds.Add(category.Id);
            }

            var toRemove = new List<EntityScriptModule>();
            if (filter.CategoryType.Equals("Any Category"))
                return list.Take(filter.Limit).ToList();
            else if (filter.CategoryType.Equals("And Category"))
            {
                foreach (var module in list)
                {
                    var moduleCategories = moduleService.GetModuleCategories(module.Guid);
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
                    var mCategories = moduleService.GetModuleCategories(module.Guid);
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

        public List<EntityScriptModule> GetArchived(DtoSearchFilterCategories filter)
        {
            return ectx.Uow.ScriptModuleRepository.Get(s => (s.Name.Contains(filter.SearchText) || s.Guid.Contains(filter.SearchText)) && s.Archived).OrderBy(x => x.Name).Take(filter.Limit).ToList();
        }

        public List<EntityScriptModule> GetConditions()
        {
            return ectx.Uow.ScriptModuleRepository.Get(s => s.IsCondition).ToList();
        }

        public string TotalCount()
        {
            return ectx.Uow.ScriptModuleRepository.Count(x => !x.Archived);
        }

        public string ArchivedCount()
        {
            return ectx.Uow.ScriptModuleRepository.Count(x => x.Archived);
        }

        public DtoActionResult UpdateModule(EntityScriptModule module)
        {
            var u = GetModule(module.Id);
            if (u == null) return new DtoActionResult {ErrorMessage = "Module Not Found", Id = 0};
            var isActiveModule = moduleService.IsModuleActive(module.Id, EnumModule.ModuleType.Script);
            if (!string.IsNullOrEmpty(isActiveModule)) return new DtoActionResult() { ErrorMessage = isActiveModule, Id = 0 };
            if(!module.IsCondition && u.IsCondition)
            {
                //condition has been removed, check for policies with this condition
                var modulesWithCondition = ectx.Uow.PolicyModulesRepository.Get(x => x.ConditionId == u.Id);
                foreach (var m in modulesWithCondition)
                {
                    m.ConditionId = -1;
                    ectx.Uow.PolicyModulesRepository.Update(m, m.Id);
                }


                var policiesWithCondition = ectx.Uow.PolicyRepository.Get(x => x.ConditionId == u.Id);
                foreach (var policy in policiesWithCondition)
                {
                    policy.ConditionId = -1;
                    ectx.Uow.PolicyRepository.Update(policy, policy.Id);
                }
            }
            var validationResult = ValidateModule(module, false);
            var actionResult = new DtoActionResult();
            if (validationResult.Success)
            {
                if (module.ScriptType == EnumScriptModule.ScriptType.ImagingClient_Bash)
                {
                    var fixedLineEnding = module.ScriptContents.Replace("\r\n", "\n");
                    module.ScriptContents = fixedLineEnding;
                }
                ectx.Uow.ScriptModuleRepository.Update(module, module.Id);
                ectx.Uow.Save();
                actionResult.Success = true;
                actionResult.Id = module.Id;
            }
            else
            {
                actionResult.ErrorMessage = validationResult.ErrorMessage;
            }

            return actionResult;
        }

        private DtoValidationResult ValidateModule(EntityScriptModule module, bool isNew)
        {
            var validationResult = new DtoValidationResult { Success = true };
            int value;
            if (!int.TryParse(module.Timeout.ToString(), out value))
            {
                validationResult.Success = false;
                validationResult.ErrorMessage = "Invalid Timeout";
                return validationResult;
            }
            try
            {
                if (string.IsNullOrEmpty(module.SuccessCodes))
                {
                    validationResult.Success = false;
                    validationResult.ErrorMessage = "Invalid Success Code";
                    return validationResult;
                }
                if (module.SuccessCodes.Split(',').Any(code => !int.TryParse(code, out value)))
                {
                    validationResult.Success = false;
                    validationResult.ErrorMessage = "Invalid Success Code";
                    return validationResult;
                }
            }
            catch
            {
                validationResult.Success = false;
                validationResult.ErrorMessage = "Invalid Success Code";
                return validationResult;
            }
            if (module.Timeout < 0) module.Timeout = 0;
            if (string.IsNullOrEmpty(module.Name) || !module.Name.All(c => char.IsLetterOrDigit(c) || c == '_' || c == '-' || c == ' ' || c == '.'))
            {
                validationResult.Success = false;
                validationResult.ErrorMessage = "Module Name Is Not Valid";
                return validationResult;
            }

            if (isNew)
            {
                if (ectx.Uow.ScriptModuleRepository.Exists(h => h.Name == module.Name))
                {
                    validationResult.Success = false;
                    validationResult.ErrorMessage = "A Module With This Name Already Exists";
                    return validationResult;
                }
            }
            else
            {
                var originalModule = ectx.Uow.ScriptModuleRepository.GetById(module.Id);
                if (originalModule.Name != module.Name)
                {
                    if (ectx.Uow.ScriptModuleRepository.Exists(h => h.Name == module.Name))
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