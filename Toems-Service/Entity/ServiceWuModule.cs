using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceWuModule
    {
        private readonly UnitOfWork _uow;

        public ServiceWuModule()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult AddModule(EntityWuModule module)
        {
            
            var validationResult = ValidateModule(module, true);
            var actionResult = new DtoActionResult();
            if (validationResult.Success)
            {
                _uow.WindowsUpdateModuleRepository.Insert(module);
                var moduleType = new EntityModule();
                moduleType.ModuleType = EnumModule.ModuleType.Wupdate;
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
            var isActiveModule = new ServiceModule().IsModuleActive(moduleId, EnumModule.ModuleType.Wupdate);
            if (!string.IsNullOrEmpty(isActiveModule)) return new DtoActionResult() {ErrorMessage = isActiveModule, Id = 0};
            if (string.IsNullOrEmpty(u.Guid)) return new DtoActionResult() { ErrorMessage = "Unknown Guid", Id = 0 };
            _uow.ModuleRepository.DeleteRange(x => x.Guid == u.Guid);
            _uow.Save();
            var actionResult = new DtoActionResult();
            var deleteDirectoryResult = new FilesystemServices().DeleteModuleDirectory(u.Guid);
            if (deleteDirectoryResult)
            {
                actionResult.Success = true;
                actionResult.Id = u.Id;
            }
            else
            {
                actionResult.Success = false;
                actionResult.ErrorMessage =
                    "Module Has Been Deleted But File System Cleanup Has Failed.  You Must Manually Delete Folder " + u.Guid;
            }
           
         
            return actionResult;
        }

        public EntityWuModule GetModule(int moduleId)
        {
            return _uow.WindowsUpdateModuleRepository.GetById(moduleId);
        }

        public List<EntityWuModule> SearchModules(DtoSearchFilterCategories filter)
        {
            var list = _uow.WindowsUpdateModuleRepository.Get(s => (s.Name.Contains(filter.SearchText) || s.Guid.Contains(filter.SearchText)) && !s.Archived).OrderBy(x => x.Name).ToList();
            var categoryFilterIds = new List<int>();
            foreach (var catName in filter.Categories)
            {
                var category = _uow.CategoryRepository.GetFirstOrDefault(x => x.Name.Equals(catName));
                if (category != null)
                    categoryFilterIds.Add(category.Id);
            }

            var toRemove = new List<EntityWuModule>();
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

        public List<EntityWuModule> GetArchived(DtoSearchFilterCategories filter)
        {
            return _uow.WindowsUpdateModuleRepository.Get(s => (s.Name.Contains(filter.SearchText) || s.Guid.Contains(filter.SearchText)) && s.Archived).OrderBy(x => x.Name).Take(filter.Limit).ToList();
        }

        public string TotalCount()
        {
            return _uow.WindowsUpdateModuleRepository.Count(x => !x.Archived);
        }

        public string ArchivedCount()
        {
            return _uow.WindowsUpdateModuleRepository.Count(x => x.Archived);
        }

        public DtoActionResult UpdateModule(EntityWuModule module)
        {
            var u = GetModule(module.Id);
            if (u == null) return new DtoActionResult {ErrorMessage = "Module Not Found", Id = 0};
            var isActiveModule = new ServiceModule().IsModuleActive(module.Id, EnumModule.ModuleType.Wupdate);
            if (!string.IsNullOrEmpty(isActiveModule)) return new DtoActionResult() { ErrorMessage = isActiveModule, Id = 0 };
            var validationResult = ValidateModule(module, false);
            var actionResult = new DtoActionResult();
            if (validationResult.Success)
            {
                _uow.WindowsUpdateModuleRepository.Update(module, module.Id);
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

        private DtoValidationResult ValidateModule(EntityWuModule module, bool isNew)
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
                if (_uow.WindowsUpdateModuleRepository.Exists(h => h.Name == module.Name))
                {
                    validationResult.Success = false;
                    validationResult.ErrorMessage = "A Module With This Name Already Exists";
                    return validationResult;
                }
            }
            else
            {
                var originalModule = _uow.WindowsUpdateModuleRepository.GetById(module.Id);
                if (originalModule.Name != module.Name)
                {
                    if (_uow.WindowsUpdateModuleRepository.Exists(h => h.Name == module.Name))
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