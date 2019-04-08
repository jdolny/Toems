using System;
using System.Collections.Generic;
using System.Linq;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceScriptModule
    {
        private readonly UnitOfWork _uow;

        public ServiceScriptModule()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult AddModule(EntityScriptModule module)
        {
           
          
            var validationResult = ValidateModule(module, true);
            var actionResult = new DtoActionResult();
            if (validationResult.Success)
            {
                _uow.ScriptModuleRepository.Insert(module);
                var moduleType = new EntityModule();
                moduleType.ModuleType = EnumModule.ModuleType.Script;
                moduleType.Guid = module.Guid;
                _uow.ModuleRepository.Insert(moduleType);
                _uow.Save();
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
            var isActiveModule = new ServiceModule().IsModuleActive(moduleId, EnumModule.ModuleType.Script);
            if (!string.IsNullOrEmpty(isActiveModule)) return new DtoActionResult() { ErrorMessage = isActiveModule, Id = 0 };
            if (string.IsNullOrEmpty(u.Guid)) return new DtoActionResult() { ErrorMessage = "Unknown Guid", Id = 0 };
            _uow.ModuleRepository.DeleteRange(x => x.Guid == u.Guid);
            //_uow.ScriptModuleRepository.Delete(moduleId);

            var modulesWithCondition = _uow.PolicyModulesRepository.Get(x => x.ConditionId == u.Id);
            foreach(var module in modulesWithCondition)
            {
                module.ConditionId = -1;
                _uow.PolicyModulesRepository.Update(module, module.Id);
            }

            var policiesWithCondition = _uow.PolicyRepository.Get(x => x.ConditionId == u.Id);
            foreach(var policy in policiesWithCondition)
            {
                policy.ConditionId = -1;
                _uow.PolicyRepository.Update(policy, policy.Id);
            }

            _uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.Id;
            return actionResult;
        }

        public EntityScriptModule GetModule(int moduleId)
        {
            return _uow.ScriptModuleRepository.GetById(moduleId);
        }

        public EntityScriptModule GetModuleByGuid(string guid)
        {
            return _uow.ScriptModuleRepository.GetFirstOrDefault(x => x.Guid == guid);
        }

        public List<EntityScriptModule> GetAllWithInventory()
        {
            return _uow.ScriptModuleRepository.Get(x => x.AddInventoryCollection);
        }

        public List<EntityScriptModule> SearchModules(DtoSearchFilterCategories filter)
        {
            var list = _uow.ScriptModuleRepository.Get(s => (s.Name.Contains(filter.SearchText) || s.Guid.Contains(filter.SearchText)) && !s.Archived).OrderBy(x => x.Name).ToList();
            var categoryFilterIds = new List<int>();
            foreach (var catName in filter.Categories)
            {
                var category = _uow.CategoryRepository.GetFirstOrDefault(x => x.Name.Equals(catName));
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
                    var moduleCategories = new ServiceModule().GetModuleCategories(module.Guid);
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
                    var mCategories = new ServiceModule().GetModuleCategories(module.Guid);
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
            return _uow.ScriptModuleRepository.Get(s => (s.Name.Contains(filter.SearchText) || s.Guid.Contains(filter.SearchText)) && s.Archived).OrderBy(x => x.Name).Take(filter.Limit).ToList();
        }

        public List<EntityScriptModule> GetConditions()
        {
            return _uow.ScriptModuleRepository.Get(s => s.IsCondition).ToList();
        }

        public string TotalCount()
        {
            return _uow.ScriptModuleRepository.Count(x => !x.Archived);
        }

        public string ArchivedCount()
        {
            return _uow.ScriptModuleRepository.Count(x => x.Archived);
        }

        public DtoActionResult UpdateModule(EntityScriptModule module)
        {
            var u = GetModule(module.Id);
            if (u == null) return new DtoActionResult {ErrorMessage = "Module Not Found", Id = 0};
            var isActiveModule = new ServiceModule().IsModuleActive(module.Id, EnumModule.ModuleType.Script);
            if (!string.IsNullOrEmpty(isActiveModule)) return new DtoActionResult() { ErrorMessage = isActiveModule, Id = 0 };
            if(!module.IsCondition && u.IsCondition)
            {
                //condition has been removed, check for policies with this condition
                var modulesWithCondition = _uow.PolicyModulesRepository.Get(x => x.ConditionId == u.Id);
                foreach (var m in modulesWithCondition)
                {
                    m.ConditionId = -1;
                    _uow.PolicyModulesRepository.Update(m, m.Id);
                }


                var policiesWithCondition = _uow.PolicyRepository.Get(x => x.ConditionId == u.Id);
                foreach (var policy in policiesWithCondition)
                {
                    policy.ConditionId = -1;
                    _uow.PolicyRepository.Update(policy, policy.Id);
                }
            }
            var validationResult = ValidateModule(module, false);
            var actionResult = new DtoActionResult();
            if (validationResult.Success)
            {
                _uow.ScriptModuleRepository.Update(module, module.Id);
                _uow.Save();
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
                if (_uow.ScriptModuleRepository.Exists(h => h.Name == module.Name))
                {
                    validationResult.Success = false;
                    validationResult.ErrorMessage = "A Module With This Name Already Exists";
                    return validationResult;
                }
            }
            else
            {
                var originalModule = _uow.ScriptModuleRepository.GetById(module.Id);
                if (originalModule.Name != module.Name)
                {
                    if (_uow.ScriptModuleRepository.Exists(h => h.Name == module.Name))
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