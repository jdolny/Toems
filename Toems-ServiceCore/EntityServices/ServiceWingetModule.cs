using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceWingetModule(EntityContext ectx, ServiceModule moduleService)
    {
        public DtoActionResult AddModule(EntityWingetModule module)
        {
          
            var validationResult = ValidateModule(module, true);
            var actionResult = new DtoActionResult();
            if (validationResult.Success)
            {
               
                var moduleType = new EntityModule();
                moduleType.ModuleType = EnumModule.ModuleType.Winget;
                moduleType.Guid = module.Guid;
                ectx.Uow.ModuleRepository.Insert(moduleType);
                ectx.Uow.Save();
                ectx.Uow.WingetModuleRepository.Insert(module);
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
            var isActiveModule = moduleService.IsModuleActive(moduleId, EnumModule.ModuleType.Winget);
            if (!string.IsNullOrEmpty(isActiveModule)) return new DtoActionResult() { ErrorMessage = isActiveModule, Id = 0 };
            if (string.IsNullOrEmpty(u.Guid)) return new DtoActionResult() { ErrorMessage = "Unknown Guid", Id = 0 };
            ectx.Uow.ModuleRepository.DeleteRange(x => x.Guid == u.Guid);
            ectx.Uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.Id;
            return actionResult;
        }

        public EntityWingetModule GetModule(int moduleId)
        {
            return ectx.Uow.WingetModuleRepository.GetById(moduleId);
        }

        public List<EntityWingetModule> SearchModules(DtoSearchFilterCategories filter)
        {
            var list = ectx.Uow.WingetModuleRepository.Get(s => (s.Name.Contains(filter.SearchText) || s.Guid.Contains(filter.SearchText)) && !s.Archived).OrderBy(x => x.Name).ToList();
            if (list.Count == 0) return list;

            var categoryFilterIds = new List<int>();
            foreach (var catName in filter.Categories)
            {
                var category = ectx.Uow.CategoryRepository.GetFirstOrDefault(x => x.Name.Equals(catName));
                if (category != null)
                    categoryFilterIds.Add(category.Id);
            }

            var toRemove = new List<EntityWingetModule>();
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

        public List<EntityWingetModule> GetArchived(DtoSearchFilterCategories filter)
        {
            return ectx.Uow.WingetModuleRepository.Get(s => (s.Name.Contains(filter.SearchText) || s.Guid.Contains(filter.SearchText)) && s.Archived).OrderBy(x => x.Name).Take(filter.Limit).ToList();
        }

        public string TotalCount()
        {
            return ectx.Uow.WingetModuleRepository.Count(x => !x.Archived);
        }

        public string ArchivedCount()
        {
            return ectx.Uow.WingetModuleRepository.Count(x => x.Archived);
        }

        public DtoActionResult UpdateModule(EntityWingetModule module)
        {
            var u = GetModule(module.Id);
            if (u == null) return new DtoActionResult {ErrorMessage = "Module Not Found", Id = 0};
            var isActiveModule = moduleService.IsModuleActive(module.Id, EnumModule.ModuleType.Winget);
            if (!string.IsNullOrEmpty(isActiveModule)) return new DtoActionResult() { ErrorMessage = isActiveModule, Id = 0 };
            var validationResult = ValidateModule(module, false);
            var actionResult = new DtoActionResult();
            if (validationResult.Success)
            {
                ectx.Uow.WingetModuleRepository.Update(module, module.Id);
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

        private DtoValidationResult ValidateModule(EntityWingetModule module, bool isNew)
        {
            var validationResult = new DtoValidationResult { Success = true };
            int value;
            if (!int.TryParse(module.Timeout.ToString(), out value))
            {
                validationResult.Success = false;
                validationResult.ErrorMessage = "Invalid Timeout";
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
                if (ectx.Uow.WingetModuleRepository.Exists(h => h.Name == module.Name))
                {
                    validationResult.Success = false;
                    validationResult.ErrorMessage = "A Module With This Name Already Exists";
                    return validationResult;
                }
            }
            else
            {
                var originalModule = ectx.Uow.WingetModuleRepository.GetById(module.Id);
                if (originalModule.Name != module.Name)
                {
                    if (ectx.Uow.WingetModuleRepository.Exists(h => h.Name == module.Name))
                    {
                        validationResult.Success = false;
                        validationResult.ErrorMessage = "A Module With This Name Already Exists";
                        return validationResult;
                    }
                }
            }

            return validationResult;
        }

        public List<EntityWingetLocaleManifest> SearchManifests(DtoWingetSearchFilter filter)
        {
            var fakeSearch = "jfj84822ojj4hsllhh44whtlhshgteuhuhkwerwer";
            filter.Searchstring = filter.Searchstring.ToLower();
            var packageIdSearch = fakeSearch;
            var packageNameSearch = fakeSearch;
            var packagePublisherSearch = fakeSearch;
            var packageTagSearch = fakeSearch;
            var packageMonikerSearch = fakeSearch;

            if (filter.IncludePackageIdentifier)
                packageIdSearch = filter.Searchstring;
            if (filter.IncludePackageName)
                packageNameSearch = filter.Searchstring;
            if (filter.IncludePublisher)
                packagePublisherSearch = filter.Searchstring;
            if (filter.IncludeTags)
                packageTagSearch = filter.Searchstring;
            if (filter.IncludeMoniker)
                packageMonikerSearch = filter.Searchstring;

            var list = new List<EntityWingetLocaleManifest>();
            if (string.IsNullOrEmpty(filter.Searchstring))
            {
                list = ectx.Uow.WingetLocaleManifestRepository.Get().OrderBy(x => Guid.NewGuid()).Take(25).ToList();
                list = list.OrderBy(x => x.PackageName).ThenByDescending(x => x.Major).ThenByDescending(x => x.Minor).ThenByDescending(x => x.Build).ThenByDescending(x => x.Revision).ToList();
            }

            else if (filter.Searchstring.Length == 1)
                return null;

            else if (filter.ExactMatch)
            {
                list = ectx.Uow.WingetLocaleManifestRepository.Get(x => x.PackageIdentifier.ToLower().Equals(packageIdSearch) || x.PackageName.ToLower().Equals(packageNameSearch)
                || x.Publisher.ToLower().Equals(packagePublisherSearch) || x.Tags.ToLower().Equals(packageTagSearch) || x.Moniker.ToLower().Equals(packageMonikerSearch));

                list = list.OrderBy(x => x.PackageName).ThenByDescending(x => x.Major).ThenByDescending(x => x.Minor).ThenByDescending(x => x.Build).ThenByDescending(x => x.Revision).ToList();
            }
            else if (!filter.ExactMatch)
            {
                var list1 = ectx.Uow.WingetLocaleManifestRepository.Get(x => x.PackageIdentifier.ToLower().Contains(packageIdSearch) || x.PackageName.ToLower().Contains(packageNameSearch)
                  || x.Publisher.ToLower().Contains(packagePublisherSearch) || x.Tags.ToLower().Contains(packageTagSearch) || x.Moniker.ToLower().Contains(packageMonikerSearch));
                list1 = list1.OrderBy(x => x.PackageName).ThenByDescending(x => x.Major).ThenByDescending(x => x.Minor).ThenByDescending(x => x.Build).ThenByDescending(x => x.Revision).ToList();
                
                var tempList = new List<EntityWingetLocaleManifest>();
                
                //place most relevant search at top of list
                
                foreach(var item in list1)
                {
                    if (item.PackageName.ToLower().Equals(packageNameSearch))
                    {
                        tempList.Add(item);
                        list.Remove(item);
                    }
                }
                foreach (var item in list1)
                {
                    if (item.PackageName.ToLower().StartsWith(packageNameSearch + " "))
                    {
                        tempList.Add(item);
                        list.Remove(item);
                    }
                }
                foreach (var item in list1)
                {
                    if (item.PackageName.ToLower().Contains(" " + packageIdSearch + " "))
                    {
                        tempList.Add(item);
                        list.Remove(item);
                    }
                }
                foreach (var item in list1)
                {
                    if (item.PackageName.ToLower().EndsWith(" " + packageNameSearch))
                    {
                        tempList.Add(item);
                        list.Remove(item);
                    }
                }
                list.AddRange(tempList);
                list.AddRange(list1);
            }


            if (filter.LatestVersionOnly)
                list = list.GroupBy(x => x.PackageIdentifier).Select(x => x.First()).Take(filter.Limit).ToList();
            else
                list = list.Take(filter.Limit).ToList();

            foreach (var l in list)
            {
                var iManifest = ectx.Uow.WingetInstallerManifestRepository.Get(x => x.PackageIdentifier.Equals(l.PackageIdentifier) && x.PackageVersion.Equals(l.PackageVersion)).FirstOrDefault();
                if (iManifest != null)
                    l.Scope = iManifest.Scope;
            }

            return list;

        }

        public EntityWingetLocaleManifest GetLocaleManifest(int id)
        {
            return ectx.Uow.WingetLocaleManifestRepository.GetById(id);
        }

        public string GetLastImportTime()
        {
            var result =ectx.Uow.WingetManifestDownloadRepository.Get(x => x.Status == EnumManifestImport.ImportStatus.Complete).OrderByDescending(x => x.Id).FirstOrDefault();
            if (result != null) return result.DateDownloaded.ToString();
            return string.Empty;
        }
    }
}