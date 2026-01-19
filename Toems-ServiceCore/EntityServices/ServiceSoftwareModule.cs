using System.Text;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_Service;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceSoftwareModule(EntityContext ectx, ServiceModule moduleService)
    {

        public DtoActionResult AddModule(EntitySoftwareModule module)
        {
            
            var validationResult = ValidateModule(module, true);
            var actionResult = new DtoActionResult();
            if (validationResult.Success)
            {
                ectx.Uow.SoftwareModuleRepository.Insert(module);
                var moduleType = new EntityModule();
                moduleType.ModuleType = EnumModule.ModuleType.Software;
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
            var isActiveModule = moduleService.IsModuleActive(moduleId, EnumModule.ModuleType.Software);
            if (!string.IsNullOrEmpty(isActiveModule)) return new DtoActionResult() {ErrorMessage = isActiveModule, Id = 0};
            if (string.IsNullOrEmpty(u.Guid)) return new DtoActionResult() { ErrorMessage = "Unknown Guid", Id = 0 };
            ectx.Uow.ModuleRepository.DeleteRange(x => x.Guid == u.Guid);
            //ectx.Uow.SoftwareModuleRepository.Delete(moduleId);
            ectx.Uow.Save();
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

        public EntitySoftwareModule GetModule(int moduleId)
        {
            return ectx.Uow.SoftwareModuleRepository.GetById(moduleId);
        }

        public List<EntitySoftwareModule> SearchModules(DtoSearchFilterCategories filter)
        {
            var list = ectx.Uow.SoftwareModuleRepository.Get(s => (s.Name.Contains(filter.SearchText) || s.Guid.Contains(filter.SearchText)) && !s.Archived).OrderBy(x => x.Name).ToList();
            var categoryFilterIds = new List<int>();
            foreach (var catName in filter.Categories)
            {
                var category = ectx.Uow.CategoryRepository.GetFirstOrDefault(x => x.Name.Equals(catName));
                if (category != null)
                    categoryFilterIds.Add(category.Id);
            }

            var toRemove = new List<EntitySoftwareModule>();
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

        public List<EntitySoftwareModule> GetArchived(DtoSearchFilterCategories filter)
        {
            return ectx.Uow.SoftwareModuleRepository.Get(s => (s.Name.Contains(filter.SearchText) || s.Guid.Contains(filter.SearchText)) && s.Archived).OrderBy(x => x.Name).Take(filter.Limit).ToList();
        }

        public string TotalCount()
        {
            return ectx.Uow.SoftwareModuleRepository.Count(x => !x.Archived);
        }

        public string ArchivedCount()
        {
            return ectx.Uow.SoftwareModuleRepository.Count(x => x.Archived);
        }

        public DtoActionResult UpdateModule(EntitySoftwareModule module)
        {
            var u = GetModule(module.Id);
            if (u == null) return new DtoActionResult {ErrorMessage = "Module Not Found", Id = 0};
            var isActiveModule = moduleService.IsModuleActive(module.Id, EnumModule.ModuleType.Software);
            if (!string.IsNullOrEmpty(isActiveModule)) return new DtoActionResult() { ErrorMessage = isActiveModule, Id = 0 };
            var validationResult = ValidateModule(module, false);
            var actionResult = new DtoActionResult();
            if (validationResult.Success)
            {
                ectx.Uow.SoftwareModuleRepository.Update(module, module.Id);
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

        private DtoValidationResult ValidateModule(EntitySoftwareModule module, bool isNew)
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
                if (ectx.Uow.SoftwareModuleRepository.Exists(h => h.Name == module.Name))
                {
                    validationResult.Success = false;
                    validationResult.ErrorMessage = "A Module With This Name Already Exists";
                    return validationResult;
                }
            }
            else
            {
                var originalModule = ectx.Uow.SoftwareModuleRepository.GetById(module.Id);
                if (originalModule.Name != module.Name)
                {
                    if (ectx.Uow.SoftwareModuleRepository.Exists(h => h.Name == module.Name))
                    {
                        validationResult.Success = false;
                        validationResult.ErrorMessage = "A Module With This Name Already Exists";
                        return validationResult;
                    }
                }
            }

            return validationResult;
        }

        public DtoActionResult GenerateArguments(int moduleId)
        {
            var result = new DtoActionResult();
            var module = GetModule(moduleId);
            var files = moduleService.GetModuleFiles(module.Guid);
            var msiList = new List<string>();
            var mspList = new List<string>();
            var mstList = new List<string>();

            foreach (var file in files.OrderBy(x => x.FileName))
            {
                var ext = Path.GetExtension(file.FileName);
                if (ext == null)
                {
                    result.Success = false;
                    result.ErrorMessage = "File Extension Is Missing.";
                    return result;
                }
                switch (ext.ToLower())
                {
                    case ".msi":
                        msiList.Add(file.FileName);
                        break;
                    case ".msp":
                        mspList.Add(file.FileName);
                        break;
                    case ".mst":
                        mstList.Add(file.FileName);
                        break;
                    default:
                        break;
                }
            }

            var arguments = new StringBuilder();
            var command = "";
            if (module.InstallType == EnumSoftwareModule.MsiInstallType.Install)
            {
                if (msiList.Count == 0)
                {
                    result.Success = false;
                    result.ErrorMessage = "No MSI Files Were Found For This Install Type";
                    return result;
                }
                command = msiList.First();
             
                if (mspList.Count > 0)
                {
                    arguments.Append("PATCHES=");
                    foreach (var patch in mspList)
                    {
                        arguments.Append("[cache-path]");
                        arguments.Append(patch);
                        arguments.Append(";");
                    }
                }
                arguments.Append(" ");
                if (mstList.Count > 0)
                {
                    arguments.Append("TRANSFORMS=");
                    foreach (var transform in mstList)
                    {
                        arguments.Append(transform);
                        arguments.Append(";");
                    }
                }
            }
            else if (module.InstallType == EnumSoftwareModule.MsiInstallType.Uninstall)
            {
                if (msiList.Count == 0)
                {
                    result.Success = false;
                    result.ErrorMessage = "No MSI Files Were Found For This Install Type";
                    return result;
                }
                command = msiList.First();            
            }
            else if (module.InstallType == EnumSoftwareModule.MsiInstallType.Patch)
            {
                if (mspList.Count == 0)
                {
                    result.Success = false;
                    result.ErrorMessage = "No MSP Files Were Found For This Install Type";
                    return result;
                }
                command = mspList.First();

                arguments.Append("REINSTALL=ALL REINSTALLMODE=omus");
            }
            else
            {
                result.Success = false;
                result.ErrorMessage = "Invalid Software Install Type";
            }

            module.Command = command;
            module.Arguments = arguments.ToString();
            var updateResult = UpdateModule(module);
            if (updateResult.Success)
            {
                result.Success = true;
                result.Id = moduleId;
            }
            else
            {
                result.Success = false;
                result.ErrorMessage = updateResult.ErrorMessage;
            }
           

            return result;
        }

       
    }
}