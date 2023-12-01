using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using Newtonsoft.Json;
using Toems_Common.Dto;
using Toems_Common.Dto.exports;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_DataModel;
using Toems_Service.Workflows;

namespace Toems_Service.Entity
{
    public class ServicePolicy
    {
        private readonly UnitOfWork _uow;

        public ServicePolicy()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult AddPolicy(EntityPolicy policy)
        {
            policy.Guid = Guid.NewGuid().ToString();
            var validationResult = ValidatePolicy(policy, true);
            var actionResult = new DtoActionResult();
            if (validationResult.Success)
            {
                _uow.PolicyRepository.Insert(policy);
                _uow.Save();
                actionResult.Success = true;
                actionResult.Id = policy.Id;
            }
            else
            {
                actionResult.ErrorMessage = validationResult.ErrorMessage;
            }

            return actionResult;
        }

        public DtoActionResult RestorePolicy(int policyId)
        {
            var u = GetPolicy(policyId);
            if (u == null) return new DtoActionResult { ErrorMessage = "Policy Not Found", Id = 0 };
            u.Archived = false;
            u.ArchiveDateTime = null;
            u.Name = u.Name.Split('#').First();
            if (_uow.PolicyRepository.Exists(x => x.Name.Equals(u.Name)))
            {
                return new DtoActionResult()
                    {ErrorMessage = "Could Not Restore Policy.  A Policy With The Name " + u.Name + " Already Exists"};
            }

            _uow.PolicyRepository.Update(u,u.Id);
            _uow.Save();

            return new DtoActionResult() {Success = true, Id = u.Id};
        }

        public DtoActionResult ArchivePolicy(int policyId)
        {
            var u = GetPolicy(policyId);
            if (u == null) return new DtoActionResult { ErrorMessage = "Policy Not Found", Id = 0 };
            var activePolicy = GetActivePolicy(policyId);
            if (activePolicy != null) return new DtoActionResult() { ErrorMessage = "Active Policies Cannot Be Archived.  You Must Deactivate It First.",Id = 0, Success = false};
            if (u.Archived) return new DtoActionResult() { Id = u.Id, Success = true };
            u.Archived = true;
            u.Name = u.Name + "#" + DateTime.Now.ToString("MM-dd-yyyy_HH:mm");
            u.ArchiveDateTime = DateTime.Now;

            var policyWithModules = _uow.PolicyRepository.GetDetailed(policyId);
            var moduleService = new ServiceModule();
            var moduleArchiveError = false;

            foreach (var m in policyWithModules.CommandModules)
                if (!moduleService.ArchiveModule(m.Id, EnumModule.ModuleType.Command).Success)
                    moduleArchiveError = true;
            foreach (var m in policyWithModules.FileCopyModules)
                if (!moduleService.ArchiveModule(m.Id, EnumModule.ModuleType.FileCopy).Success)
                    moduleArchiveError = true;
            foreach (var m in policyWithModules.PrinterModules)
                if (!moduleService.ArchiveModule(m.Id, EnumModule.ModuleType.Printer).Success)
                    moduleArchiveError = true;
            foreach (var m in policyWithModules.ScriptModules)
                if (!moduleService.ArchiveModule(m.Id, EnumModule.ModuleType.Script).Success)
                    moduleArchiveError = true;
            foreach (var m in policyWithModules.SoftwareModules)
                if (!moduleService.ArchiveModule(m.Id, EnumModule.ModuleType.Software).Success)
                    moduleArchiveError = true;
            foreach (var m in policyWithModules.WuModules)
                if (!moduleService.ArchiveModule(m.Id, EnumModule.ModuleType.Wupdate).Success)
                    moduleArchiveError = true;
            foreach (var m in policyWithModules.MessageModules)
                if (!moduleService.ArchiveModule(m.Id, EnumModule.ModuleType.Message).Success)
                    moduleArchiveError = true;
            foreach (var m in policyWithModules.WinPeModules)
                if (!moduleService.ArchiveModule(m.Id, EnumModule.ModuleType.WinPE).Success)
                    moduleArchiveError = true;
            _uow.GroupPolicyRepository.DeleteRange(x => x.PolicyId == policyId);
            _uow.Save();
            if (moduleArchiveError)
                return new DtoActionResult()
                {
                    ErrorMessage = "Some Modules Could Not Be Archived Because They Are Active With Other Policies.",
                    Success = false,
                    Id = 0
                };
            else
            {
                _uow.PinnedPolicyRepository.DeleteRange(x => x.PolicyId == policyId);
                _uow.Save();
                return new DtoActionResult() {Id = u.Id, Success = true};
            }

        }

       
        public DtoActionResult DeletePolicy(int policyId)
        {
            var u = GetPolicy(policyId);
            if (u == null) return new DtoActionResult {ErrorMessage = "Policy Not Found", Id = 0};
            var activePolicy = GetActivePolicy(policyId);
            if (activePolicy != null) return new DtoActionResult() { ErrorMessage = "Active Policies Cannot Be Deleted.  You Must Deactivate It First.",Id = 0,Success = false};
            _uow.PolicyRepository.Delete(policyId);
            _uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.Id;
            return actionResult;
        }

        public EntityPolicy GetPolicy(int policyId)
        {
            return _uow.PolicyRepository.GetById(policyId);
        }

        public List<EntityPolicy> SearchPolicies(DtoSearchFilterCategories filter)
        {  
            if (filter.Limit == 0) filter.Limit = int.MaxValue;
            if (string.IsNullOrEmpty(filter.SearchText)) filter.SearchText = "";
            var list = _uow.PolicyRepository.Get(s => (s.Name.Contains(filter.SearchText) || s.Guid.Contains(filter.SearchText)) && !s.Archived).OrderBy(x => x.Name).ToList();
            if (list.Count == 0) return list;

            var categoryFilterIds = new List<int>();
            foreach (var catName in filter.Categories)
            {
                var category = _uow.CategoryRepository.GetFirstOrDefault(x => x.Name.Equals(catName));
                if (category != null)
                    categoryFilterIds.Add(category.Id);
            }

            var toRemove = new List<EntityPolicy>();
            if (filter.CategoryType.Equals("Any Category"))
                return list.Take(filter.Limit).ToList();
            else if (filter.CategoryType.Equals("And Category"))
            {
                foreach (var policy in list)
                {
                    var pCategories = GetPolicyCategories(policy.Id);
                    if (pCategories == null) continue;

                    if (filter.Categories.Count == 0)
                    {
                        if (pCategories.Count > 0)
                        {
                            toRemove.Add(policy);
                            continue;
                        }
                    }

                    foreach (var id in categoryFilterIds)
                    {
                        if (pCategories.Any(x => x.CategoryId == id)) continue;
                        toRemove.Add(policy);
                        break;
                    }
                }
            }
            else if (filter.CategoryType.Equals("Or Category"))
            {
                foreach (var policy in list)
                {
                    var pCategories = GetPolicyCategories(policy.Id);
                    if (pCategories == null) continue;
                    if (filter.Categories.Count == 0)
                    {
                        if (pCategories.Count > 0)
                        {
                            toRemove.Add(policy);
                            continue;
                        }
                    }
                    var catFound = false;
                    foreach (var id in categoryFilterIds)
                    {
                        if (pCategories.Any(x => x.CategoryId == id))
                        {
                            catFound = true;
                            break;
                        }

                    }
                    if (!catFound)
                        toRemove.Add(policy);
                }
            }

            foreach (var p in toRemove)
            {
                list.Remove(p);
            }

            return list.Take(filter.Limit).ToList();

        }

        public List<EntityPolicy> GetArchived(DtoSearchFilterCategories filter)
        {
            return _uow.PolicyRepository.Get(s => (s.Name.Contains(filter.SearchText) || s.Guid.Contains(filter.SearchText)) && s.Archived).OrderBy(x => x.Name).Take(filter.Limit).ToList();
        }

        public string TotalCount()
        {
            return _uow.PolicyRepository.Count(x => !x.Archived);
        }

        public string ArchivedCount()
        {
            return _uow.PolicyRepository.Count(x => x.Archived);
        }

        public DtoActionResult UpdatePolicy(EntityPolicy policy)
        {
            var u = GetPolicy(policy.Id);
            if (u == null) return new DtoActionResult {ErrorMessage = "Policy Not Found", Id = 0};
            var activePolicy = GetActivePolicy(policy.Id);
            if (activePolicy != null) return new DtoActionResult(){ErrorMessage = "Active Policies Cannot Be Updated.  You Must Deactivate It First."};
            var validationResult = ValidatePolicy(policy, false);
            var actionResult = new DtoActionResult();
            if (validationResult.Success)
            {
                _uow.PolicyRepository.Update(policy, policy.Id);
                _uow.Save();
                actionResult.Success = true;
                actionResult.Id = policy.Id;
            }
            else
            {
                actionResult.ErrorMessage = validationResult.ErrorMessage;
            }

            return actionResult;
        }

        private DtoValidationResult ValidatePolicy(EntityPolicy policy, bool isNew)
        {
            var validationResult = new DtoValidationResult { Success = true };

            if (string.IsNullOrEmpty(policy.Name) || !policy.Name.All(c => char.IsLetterOrDigit(c) || c == '_' || c == '-' || c == ' ' || c == '.'))
            {
                validationResult.Success = false;
                validationResult.ErrorMessage = "Policy Name Is Not Valid";
                return validationResult;
            }

            if (isNew)
            {
                if (_uow.PolicyRepository.Exists(h => h.Name == policy.Name))
                {
                    validationResult.Success = false;
                    validationResult.ErrorMessage = "A Policy With This Name Already Exists";
                    return validationResult;
                }
            }
            else
            {
                var originalPolicy = _uow.PolicyRepository.GetById(policy.Id);
                if (originalPolicy.Name != policy.Name)
                {
                    if (_uow.PolicyRepository.Exists(h => h.Name == policy.Name))
                    {
                        validationResult.Success = false;
                        validationResult.ErrorMessage = "A Policy With This Name Already Exists";
                        return validationResult;
                    }
                }
            }

            return validationResult;
        }

        public DtoActionResult ClonePolicy(int policyId)
        {
            var u = GetPolicy(policyId);
            if (u == null) return new DtoActionResult {ErrorMessage = "Policy Not Found", Id = 0};


            string newName = string.Empty;
            for (var c = 1; c <= 100; c++)
            {
                if (c == 100)
                    return new DtoActionResult() { ErrorMessage = "Could Not Determine A Policy Name" };

                newName = u.Name + "_" + c;
                if (!_uow.PolicyRepository.Exists(h => h.Name == newName))
                {
                    break;
                }
            }

            var existingPolicySerialized = JsonConvert.SerializeObject(u);
            var clonedPolicy = JsonConvert.DeserializeObject<EntityPolicy>(existingPolicySerialized);
            clonedPolicy.Description += Environment.NewLine + "Cloned From " + u.Name;
            clonedPolicy.Name = newName;

            var addResult = AddPolicy(clonedPolicy);
            if(!addResult.Success)
                return new DtoActionResult {ErrorMessage = addResult.ErrorMessage, Id = 0};

            var searchFilter = new DtoModuleSearchFilter();
            searchFilter.IncludeCommand = true;
            searchFilter.IncludeFileCopy = true;
            searchFilter.IncludePrinter = true;
            searchFilter.IncludeScript = true;
            searchFilter.IncludeSoftware = true;
            searchFilter.IncludeMessage = true;
            searchFilter.IncludeWu = true;
            searchFilter.IncludeWinPe = true;
            searchFilter.IncludeWinget = true;

            foreach (var module in SearchAssignedPolicyModules(policyId, searchFilter))
            {
                module.PolicyId = clonedPolicy.Id;
                _uow.PolicyModulesRepository.Insert(module);
            }
            _uow.Save();

            var categories = GetPolicyCategories(u.Id);
            foreach(var cat in categories)
            {
                cat.PolicyId = clonedPolicy.Id;
            }
            new ServicePolicyCategory().AddOrUpdate(categories);


            if (u.PolicyComCondition == EnumPolicy.PolicyComCondition.Selective)
            {
                var comServers = GetPolicyComServers(u.Id);
                foreach(var com in comServers)
                {
                    com.PolicyId = clonedPolicy.Id;
                }

                new ServicePolicyComServer().AddOrUpdate(comServers);
            }
            return new DtoActionResult() {Id = clonedPolicy.Id, Success = true};
        }

        public List<DtoModule> AllAvailableModules(DtoModuleSearchFilter filter)
        {
            var catfilter = new DtoSearchFilterCategories();
            catfilter.SearchText = filter.Searchstring;
            catfilter.Limit = filter.Limit;

            var listModules = new List<DtoModule>();
            if (filter.Limit == 0) filter.Limit = int.MaxValue;
            if (filter.IncludePrinter)
            {
                listModules.AddRange(new ServicePrinterModule().SearchModules(catfilter).Select(printerModule => new DtoModule
                {
                    Id = printerModule.Id,
                    Name = printerModule.Name,
                    Description = printerModule.Description,
                    Guid =  printerModule.Guid,
                    ModuleType = EnumModule.ModuleType.Printer
                }).ToList());
            }
            if (filter.IncludeSoftware)
            {
                listModules.AddRange(new ServiceSoftwareModule().SearchModules(catfilter).Select(softwareModule => new DtoModule
                {
                    Id = softwareModule.Id,
                    Name = softwareModule.Name,
                    Description = softwareModule.Description,
                    Guid = softwareModule.Guid,
                    ModuleType = EnumModule.ModuleType.Software
                }).ToList());
            }

            if (filter.IncludeCommand)
            {
                listModules.AddRange(new ServiceCommandModule().SearchModules(catfilter).Select(commandModule => new DtoModule
                {
                    Id = commandModule.Id,
                    Name = commandModule.Name,
                    Description = commandModule.Description,
                    Guid = commandModule.Guid,
                    ModuleType = EnumModule.ModuleType.Command
                }).ToList());
            }

            if (filter.IncludeWinget)
            {
                listModules.AddRange(new ServiceWingetModule().SearchModules(catfilter).Select(wingetModule => new DtoModule
                {
                    Id = wingetModule.Id,
                    Name = wingetModule.Name,
                    Description = wingetModule.Description,
                    Guid = wingetModule.Guid,
                    ModuleType = EnumModule.ModuleType.Winget
                }).ToList());
            }

            if (filter.IncludeFileCopy)
            {
                listModules.AddRange(new ServiceFileCopyModule().SearchModules(catfilter).Select(fileCopyModule => new DtoModule
                {
                    Id = fileCopyModule.Id,
                    Name = fileCopyModule.Name,
                    Description = fileCopyModule.Description,
                    Guid = fileCopyModule.Guid,
                    ModuleType = EnumModule.ModuleType.FileCopy
                }).ToList());
            }

            if (filter.IncludeWinPe)
            {
                listModules.AddRange(new ServiceWinPeModule().SearchModules(catfilter).Select(winPeModule => new DtoModule
                {
                    Id = winPeModule.Id,
                    Name = winPeModule.Name,
                    Description = winPeModule.Description,
                    Guid = winPeModule.Guid,
                    ModuleType = EnumModule.ModuleType.WinPE
                }).ToList());
            }

            if (filter.IncludeMessage)
            {
                listModules.AddRange(new ServiceMessageModule().SearchModules(catfilter).Select(messageModule => new DtoModule
                {
                    Id = messageModule.Id,
                    Name = messageModule.Name,
                    Description = messageModule.Description,
                    Guid = messageModule.Guid,
                    ModuleType = EnumModule.ModuleType.Message
                }).ToList());
            }

            if (filter.IncludeScript)
            {
                listModules.AddRange(new ServiceScriptModule().SearchModules(catfilter).Select(scriptModule => new DtoModule
                {
                    Id = scriptModule.Id,
                    Name = scriptModule.Name,
                    Description = scriptModule.Description,
                    Guid = scriptModule.Guid,
                    ModuleType = EnumModule.ModuleType.Script
                }).ToList());
            }

            if (filter.IncludeWu)
            {
                listModules.AddRange(new ServiceWuModule().SearchModules(catfilter).Select(wuModule => new DtoModule
                {
                    Id = wuModule.Id,
                    Name = wuModule.Name,
                    Description = wuModule.Description,
                    Guid = wuModule.Guid,
                    ModuleType = EnumModule.ModuleType.Wupdate
                }).ToList());
            }

            if (filter.IncludeUnassigned) //actually is only unassigned
            {
                var modulesToRemove = new List<DtoModule>();
                var allPolicies = _uow.PolicyRepository.Get();
                foreach (var policy in allPolicies)
                {
                    var policyDetailed = GetDetailed(policy.Id);
                    foreach (var mod in policyDetailed.CommandModules)
                    {
                        modulesToRemove.AddRange(listModules.Where(module => mod.Guid == module.Guid));
                    }

                    foreach (var mod in policyDetailed.FileCopyModules)
                    {
                        modulesToRemove.AddRange(listModules.Where(module => mod.Guid == module.Guid));
                    }

                    foreach (var mod in policyDetailed.WinPeModules)
                    {
                        modulesToRemove.AddRange(listModules.Where(module => mod.Guid == module.Guid));
                    }

                    foreach (var mod in policyDetailed.SoftwareModules)
                    {
                        modulesToRemove.AddRange(listModules.Where(module => mod.Guid == module.Guid));
                    }
                    foreach (var mod in policyDetailed.PrinterModules)
                    {
                        modulesToRemove.AddRange(listModules.Where(module => mod.Guid == module.Guid));
                    }
                    foreach (var mod in policyDetailed.ScriptModules)
                    {
                        modulesToRemove.AddRange(listModules.Where(module => mod.Guid == module.Guid));
                    }
                    foreach (var mod in policyDetailed.MessageModules)
                    {
                        modulesToRemove.AddRange(listModules.Where(module => mod.Guid == module.Guid));
                    }
                    foreach (var mod in policyDetailed.WuModules)
                    {
                        modulesToRemove.AddRange(listModules.Where(module => mod.Guid == module.Guid));
                    }
                    foreach (var mod in policyDetailed.WingetModules)
                    {
                        modulesToRemove.AddRange(listModules.Where(module => mod.Guid == module.Guid));
                    }
                }

                foreach (var m in modulesToRemove)
                {
                    listModules.Remove(m);
                }

            }
            return listModules;
        }

        public List<EntityPolicyModules> SearchAssignedPolicyModules(int policyId, DtoModuleSearchFilter filter)
        {
            var list = new List<EntityPolicyModules>();
            if (filter.Limit == 0) filter.Limit = int.MaxValue;
          
            foreach (var module in _uow.PolicyModulesRepository.Get(x => x.PolicyId == policyId))
            {
                if (module.ModuleType == EnumModule.ModuleType.Printer)
                {
                    if (filter.IncludePrinter)
                    {
                        var pModule = new ServicePrinterModule().GetModule(module.ModuleId);
                        if (pModule != null)
                        {
                            module.Name = pModule.Name;
                            list.Add(module);
                        }
                    }
                }
                else if (module.ModuleType == EnumModule.ModuleType.Software)
                {
                    if (filter.IncludeSoftware)
                    {
                        var pModule = new ServiceSoftwareModule().GetModule(module.ModuleId);
                        if (pModule != null)
                        {
                            module.Name = pModule.Name;
                            list.Add(module);
                        }
                    }
                }
                else if (module.ModuleType == EnumModule.ModuleType.Command)
                {
                    if (filter.IncludeCommand)
                    {
                        var pModule = new ServiceCommandModule().GetModule(module.ModuleId);
                        if (pModule != null)
                        {
                            module.Name = pModule.Name;
                            list.Add(module);
                        }
                    }
                }
                else if (module.ModuleType == EnumModule.ModuleType.FileCopy)
                {
                    if (filter.IncludeFileCopy)
                    {
                        var pModule = new ServiceFileCopyModule().GetModule(module.ModuleId);
                        if (pModule != null)
                        {
                            module.Name = pModule.Name;
                            list.Add(module);
                        }
                    }
                }
                else if (module.ModuleType == EnumModule.ModuleType.Script)
                {
                    if (filter.IncludeScript)
                    {
                        var pModule = new ServiceScriptModule().GetModule(module.ModuleId);
                        if (pModule != null)
                        {
                            module.Name = pModule.Name;
                            list.Add(module);
                        }
                    }
                }
                else if (module.ModuleType == EnumModule.ModuleType.Wupdate)
                {
                    if (filter.IncludeWu)
                    {
                        var pModule = new ServiceWuModule().GetModule(module.ModuleId);
                        if (pModule != null)
                        {
                            module.Name = pModule.Name;
                            list.Add(module);
                        }
                    }
                }
                else if (module.ModuleType == EnumModule.ModuleType.Message)
                {
                    if (filter.IncludeMessage)
                    {
                        var pModule = new ServiceMessageModule().GetModule(module.ModuleId);
                        if (pModule != null)
                        {
                            module.Name = pModule.Name;
                            list.Add(module);
                        }
                    }
                }
                else if (module.ModuleType == EnumModule.ModuleType.WinPE)
                {
                    if (filter.IncludeWinPe)
                    {
                        var pModule = new ServiceWinPeModule().GetModule(module.ModuleId);
                        if (pModule != null)
                        {
                            module.Name = pModule.Name;
                            list.Add(module);
                        }
                    }
                }
                else if (module.ModuleType == EnumModule.ModuleType.Winget)
                {
                    if (filter.IncludeWinget)
                    {
                        var pModule = new ServiceWingetModule().GetModule(module.ModuleId);
                        if (pModule != null)
                        {
                            module.Name = pModule.Name;
                            list.Add(module);
                        }
                    }
                }
            }
          
            if (string.IsNullOrEmpty(filter.Searchstring))
                return list.OrderBy(x => x.Order).ThenBy(x => x.Name).Take(filter.Limit).ToList();
            else
                return list.Where(s => s.Name.Contains(filter.Searchstring)).OrderBy(x=> x.Order).ThenBy(x => x.Name).Take(filter.Limit).ToList();
        }

        public List<EntityPolicyComServer> GetPolicyComServers(int policyId)
        {
            return _uow.PolicyComServerRepository.Get(x => x.PolicyId == policyId);
        }

        public List<EntityGroup> GetPolicyGroups(int policyId)
        {
            return _uow.PolicyRepository.GetPolicyGroups(policyId);
        }

        public List<EntityPolicyHistory> GetHistoryWithComputer(int policyId,DtoSearchFilter filter)
        {
            return _uow.PolicyHistoryRepository.GetHistoryWithComputer(policyId,filter.Limit,filter.SearchText);
        }


        public List<EntityPolicyCategory> GetPolicyCategories(int policyId)
        {
            return _uow.PolicyCategoryRepository.Get(x => x.PolicyId == policyId);
        }

        public List<EntityComputer> GetPolicyComputers(int policyId)
        {
            return _uow.PolicyRepository.GetPolicyComputers(policyId);
        }

        public string AssignedModuleCount(int policyId)
        {
            return _uow.PolicyModulesRepository.Count(x => x.PolicyId == policyId);
        }

        public PolicyModules GetDetailed(int policyId)
        {
            return _uow.PolicyRepository.GetDetailed(policyId);
        }

        public EntityActiveClientPolicy GetActivePolicy(int policyId)
        {
            return _uow.ActiveClientPolicies.GetFirstOrDefault(x => x.PolicyId == policyId);
        }

        public List<EntityPolicyHashHistory> GetHashHistory(int policyId)
        {
            return
                _uow.PolicyHashHistoryRepository.Get(x => x.PolicyId == policyId)
                    .OrderByDescending(x => x.ModifyTime)
                    .ToList();
        }

        public DtoActionResult ValidatePolicyExport(DtoPolicyExportGeneral info)
        {
            var result = new DtoActionResult();
            if (string.IsNullOrEmpty(info.Name))
            {
                result.ErrorMessage = "Policy Export Must Have A Name.";
                return result;
            }

            if (string.IsNullOrEmpty(info.Description))
            {
                result.ErrorMessage = "Policy Export Must Have A Description";
                return result;
            }

            return new Workflows.ValidatePolicy().Validate(info.PolicyId);

        }

        public string GetHashDetail(int policyId, string hash)
        {
            var hashEntity = _uow.PolicyHashHistoryRepository.Get(x => x.PolicyId == policyId && x.Hash == hash).FirstOrDefault();
            if (hashEntity == null) return string.Empty;
            var a = JsonConvert.DeserializeObject(hashEntity.Json);
            return JsonConvert.SerializeObject(a, Formatting.Indented);
        }

        public string PolicyChangedSinceLastActivation(int policyId)
        {
            var policy = GetPolicy(policyId);
            if (policy == null) return "";
            if (string.IsNullOrEmpty(policy.Hash))
                return "new";
            var originalHash = policy.Hash;
            var json = JsonConvert.SerializeObject(new Workflows.ClientPolicyJson().Create(policyId));
#pragma warning disable CS0618 // Type or member is obsolete
            var newHash = FormsAuthentication.HashPasswordForStoringInConfigFile(json, "MD5");
#pragma warning restore CS0618 // Type or member is obsolete

            if (policy.Frequency != EnumPolicy.Frequency.OncePerComputer && policy.Frequency != EnumPolicy.Frequency.OncePerUserPerComputer)
                return "false"; //only oncepercomputer and onceperuserpercomputer policies need the option to run again

            if (originalHash.Equals(newHash))
            {
                //policy hasn't changed
                return "false";
            }
            return "true";
        }

        public DtoActionResult ActivatePolicy(int policyId, bool reRunExisting)
        {
            //Some extra verification is added here to ensure the active client policy accurately models the policy it was built from

            var policy = GetPolicy(policyId);
            if (policy == null) return new DtoActionResult { ErrorMessage = "Policy Not Found", Id = 0 };
            if (policy.Archived)
                return new DtoActionResult {ErrorMessage = "Archived Policies Cannot Be Activated", Id = 0};

            var validationResult = new ValidatePolicy().Validate(policyId);
            if (!validationResult.Success)
            {
                return new DtoActionResult()
                    {ErrorMessage = "Could Not Activate Policy. " + validationResult.ErrorMessage};
            }

            var originalHash = policy.Hash;
            //Update the existing policies hash
            var json = JsonConvert.SerializeObject(new Workflows.ClientPolicyJson().Create(policyId));
#pragma warning disable CS0618 // Type or member is obsolete
            var newHash = FormsAuthentication.HashPasswordForStoringInConfigFile(json, "MD5");
#pragma warning restore CS0618 // Type or member is obsolete
            policy.Hash = newHash;
            _uow.PolicyRepository.Update(policy, policy.Id);
            if (policy.Hash != originalHash)
            {
                var policyHashHistory = new EntityPolicyHashHistory();
                policyHashHistory.PolicyId = policy.Id;
                policyHashHistory.Hash = newHash;
                policyHashHistory.Json = json;
                policyHashHistory.ModifyTime = DateTime.UtcNow;
                _uow.PolicyHashHistoryRepository.Insert(policyHashHistory);
            }
            _uow.Save();

            //verify the new hash was saved correctly
            var updatedHashPolicy = GetPolicy(policyId);
            if (updatedHashPolicy == null) return new DtoActionResult { ErrorMessage = "Policy Not Found", Id = 0 };
            if (updatedHashPolicy.Hash != policy.Hash) return new DtoActionResult { ErrorMessage = "Could Not Update Policy Hash", Id = 0 };

            //Add active policy db entry after hash is added in
            var clientPolicy = JsonConvert.DeserializeObject<DtoClientPolicy>(json);
            clientPolicy.Hash = newHash;
            clientPolicy.ReRunExisting = reRunExisting;
            var jsonWithHash = JsonConvert.SerializeObject(clientPolicy);
            var activeClientPolicy = new EntityActiveClientPolicy();
            activeClientPolicy.PolicyId = policy.Id;
            activeClientPolicy.PolicyJson = jsonWithHash;
            new ServiceActiveClientPolicy().InsertOrUpdate(activeClientPolicy);

            //Verify hash one last time
            var finalActivePolicy = new ServiceActiveClientPolicy().Get(activeClientPolicy.Id);
            if (finalActivePolicy == null) return new DtoActionResult { ErrorMessage = "Could Not Activate Policy", Id = 0 };
            //verify deserialization
            try
            {
                var deserializedClientPolicy = JsonConvert.DeserializeObject<DtoClientPolicy>(finalActivePolicy.PolicyJson);
                if (deserializedClientPolicy.Hash != newHash)
                {
                    new ServiceActiveClientPolicy().Delete(finalActivePolicy.Id);
                    return new DtoActionResult {ErrorMessage = "Could Not Verify Hash", Id = 0};
                }
                else
                {
                    UpdateActiveGroups(policyId);
                    return new DtoActionResult { Success = true, Id = finalActivePolicy.Id };
                }
            }
            catch (Exception)
            {
                new ServiceActiveClientPolicy().Delete(finalActivePolicy.Id);
                return new DtoActionResult { ErrorMessage = "Could Not Verify Client Policy Deserialization", Id = 0 };
                //todo: add logging
            }
        }

        public DtoActionResult DeactivatePolicy(int policyId)
        {
            var policy = GetPolicy(policyId);
            if (policy == null) return new DtoActionResult { ErrorMessage = "Policy Not Found", Id = 0 };

            _uow.ActiveClientPolicies.DeleteRange(x => x.PolicyId == policyId);
            _uow.Save();
            //verify active policy was removed
            var activePolicy = GetActivePolicy(policyId);
            if (activePolicy == null)
            {
                UpdateActiveGroups(policyId);
                return new DtoActionResult() {Success = true, Id = policyId};
            }
            else
            {
                return new DtoActionResult() { Success = false, ErrorMessage = "Could Not Deactivate Policy"};
            }
        }

        private void UpdateActiveGroups(int policyId)
        {
            //find all groups that use this policy and update their active json
            var policyGroups = _uow.GroupPolicyRepository.Get(x => x.PolicyId == policyId);
            foreach (var policyGroup in policyGroups)
            {
                new Workflows.GenerateClientGroupPolicy().Execute(policyGroup.GroupId);
            }
        }

        public List<DtoPinnedPolicy> GetAllActiveStatus()
        {
            return _uow.PolicyRepository.GetActivePolicyStatus();
        }
    }
}