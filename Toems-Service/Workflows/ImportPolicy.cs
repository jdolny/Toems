using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Toems_Common.Dto;
using Toems_Common.Dto.exports;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_DataModel;
using Toems_Service.Entity;

namespace Toems_Service.Workflows
{
    public class ImportPolicy
    {
        private readonly ServicePolicy _policyService;
        private DtoPolicyExport _export;
        private UnitOfWork _uow;
        private EntityPolicy _policy;
        private bool _policyHasInternalFiles;
        private bool _policyHasExternalFiles;

        public ImportPolicy(DtoPolicyExport export)
        {
            _export = export;
            _policyService = new ServicePolicy();
            _policy = new EntityPolicy();
            _uow = new UnitOfWork();
        }
        public DtoImportResult Import()
        {
            if (_export == null)
            {
                return new DtoImportResult() {ErrorMessage = "Policy Was Null"};
            }

            var result = CreatePolicy();
            if (!result.Success)
                return new DtoImportResult() { ErrorMessage = result.ErrorMessage };

            result = CreateScripts();
            if (!result.Success)
            {
                _policyService.DeletePolicy(_policy.Id);
                return new DtoImportResult() {ErrorMessage = result.ErrorMessage};
            }

            result = CreatePrinters();
            if (!result.Success)
            {
                _policyService.DeletePolicy(_policy.Id);
                return new DtoImportResult() {ErrorMessage = result.ErrorMessage};
            }

            result = CreateCommands();
            if (!result.Success)
            {
                _policyService.DeletePolicy(_policy.Id);
                return new DtoImportResult() {ErrorMessage = result.ErrorMessage};
            }

            result = CreateFileCopy();
            if (!result.Success)
            {
                _policyService.DeletePolicy(_policy.Id);
                return new DtoImportResult() {ErrorMessage = result.ErrorMessage};
            }

            result = CreateSoftware();
            if (!result.Success)
            {
                _policyService.DeletePolicy(_policy.Id);
                return new DtoImportResult() {ErrorMessage = result.ErrorMessage};
            }

            result = CreateWindowsUpdate();
            if (!result.Success)
            {
                _policyService.DeletePolicy(_policy.Id);
                return new DtoImportResult() {ErrorMessage = result.ErrorMessage};
            }

            result = CreateMessages();
            if (!result.Success)
            {
                _policyService.DeletePolicy(_policy.Id);
                return new DtoImportResult() { ErrorMessage = result.ErrorMessage };
            }

            _uow.Save();

            if(_export.Instructions.Contains("[skip-policy-create]"))
            {
                _policyService.DeletePolicy(_policy.Id);
                _uow.Save();
            }
                

            QueueExternalDownloads();
         
            var importResult = new DtoImportResult();
            importResult.Success = true;
            importResult.HasExternalFiles = _policyHasExternalFiles;
            importResult.HasInternalFiles = _policyHasInternalFiles;
            return importResult;

        }

        private DtoActionResult CreatePolicy()
        {
            if (_export.Instructions.Contains("[generate-policy-guid]"))
            {
                _export.Guid = Guid.NewGuid().ToString();
            }

            if (_uow.PolicyRepository.Exists(h => h.Guid.Equals(_export.Guid)))
            {
                return new DtoActionResult() { ErrorMessage = "A Policy With This Guid Already Exists.  " + _export.Guid };
            }

            _policy.Name = _export.Name;
            _policy.Description = "Added Via Policy Template " + _export.Name + " On " + DateTime.Now + "\r\n" + _export.Description;
            _policy.Guid = _export.Guid;
            _policy.CompletedAction = _export.CompletedAction;
            _policy.ErrorAction = _export.ErrorAction;
            _policy.ExecutionType = _export.ExecutionType;
            _policy.Frequency = _export.Frequency;
            _policy.LogLevel = _export.LogLevel;
            _policy.MissedAction = _export.FrequencyMissedAction;
            _policy.RemoveInstallCache = _export.RemoveInstallCache;
            _policy.RunApplicationMonitor = _export.IsApplicationMonitor;
            _policy.RunInventory = _export.IsInventory;
            _policy.RunLoginTracker = _export.IsLoginTracker;
            _policy.SkipServerResult = _export.SkipServerResult;
            _policy.SubFrequency = _export.SubFrequency;
            _policy.Trigger = _export.Trigger;
            _policy.WuType = _export.WuType;
            _policy.StartDate = Convert.ToDateTime(DateTime.Now.ToShortDateString());
            _policy.PolicyComCondition = EnumPolicy.PolicyComCondition.Any;
            _policy.WindowEndScheduleId = -1;
            _policy.WindowStartScheduleId = -1;
            _policy.ConditionFailedAction = _export.ConditionFailedAction;

            var conditionId = CreateCondition(_export.Condition);
            if (conditionId != 0)
                _policy.ConditionId = conditionId;
            else
                _policy.ConditionId = -1;

            if (_uow.PolicyRepository.Exists(h => h.Name.Equals(_policy.Name)))
            {
                for (var c = 1; c <= 100; c++)
                {
                    if (c == 100)
                        return new DtoActionResult() { ErrorMessage = "Could Not Determine A Policy Name" };

                    var newName = _policy.Name + "_" + c;
                    if (!_uow.PolicyRepository.Exists(h => h.Name == newName))
                    {
                        _policy.Name = newName;
                        break;
                    }
                }
            }

            _uow.PolicyRepository.Insert(_policy);
            _uow.Save();

            return new DtoActionResult() {Success = true, Id = _policy.Id};
        }

        private DtoActionResult CreateScripts()
        {
            foreach (var scriptModule in _export.ScriptModules)
            {
                if (_export.Instructions.Contains("[generate-module-guid]"))
                {
                    scriptModule.Guid = Guid.NewGuid().ToString();
                }

                if (_uow.ScriptModuleRepository.Exists(h => h.Guid.Equals(scriptModule.Guid)))
                {
                    return new DtoActionResult() { ErrorMessage = "A Script Module With This Guid Already Exists.  " + scriptModule.Guid };
                }

                var script = new EntityScriptModule();
                script.AddInventoryCollection = scriptModule.AddToInventory;
                script.Name = scriptModule.DisplayName;
                script.Description = "Added Via Policy Template " + _export.Name + "  On " + DateTime.Now +
                                     "\r\n" + script.Description;
                script.Guid = scriptModule.Guid;
                script.Arguments = scriptModule.Arguments;
                script.IsCondition = scriptModule.IsCondition;
                script.RedirectStdError = scriptModule.RedirectError;
                script.RedirectStdOut = scriptModule.RedirectOutput;
                script.ScriptContents = scriptModule.ScriptContents;
                script.ScriptType = scriptModule.ScriptType;
                script.SuccessCodes = scriptModule.SuccessCodes;
                script.Timeout = scriptModule.Timeout;
                script.WorkingDirectory = scriptModule.WorkingDirectory;
                script.ImpersonationId = -1;
                
                if (_uow.ScriptModuleRepository.Exists(h => h.Name.Equals(script.Name)))
                {
                    for (var c = 1; c <= 100; c++)
                    {
                        if (c == 100)
                            return new DtoActionResult() { ErrorMessage = "Could Not Determine A Script Name" };

                        var newName = script.Name + "_" + c;
                        if (!_uow.ScriptModuleRepository.Exists(h => h.Name == newName))
                        {
                            script.Name = newName;
                            break;
                        }
                    }
                }

                var addResult = new ServiceScriptModule().AddModule(script);
                if (!addResult.Success) return addResult;

                var policyModule = new EntityPolicyModules();
                policyModule.Guid = script.Guid;
                policyModule.ModuleId = addResult.Id;
                policyModule.ModuleType = EnumModule.ModuleType.Script;
                policyModule.Name = script.Name;
                policyModule.Order = scriptModule.Order;
                policyModule.PolicyId = _policy.Id;
                policyModule.ConditionFailedAction = scriptModule.ConditionFailedAction;
                policyModule.ConditionNextModule = scriptModule.ConditionNextOrder;


                var conditionId = CreateCondition(scriptModule.Condition);
                if (conditionId != 0)
                    policyModule.ConditionId = conditionId;
                else
                    policyModule.ConditionId = -1;

                _uow.PolicyModulesRepository.Insert(policyModule);

            }

            return new DtoActionResult() {Success = true};
        }

        private DtoActionResult CreatePrinters()
        {
            foreach (var printerModule in _export.PrinterModules)
            {
                if (_export.Instructions.Contains("[generate-module-guid]"))
                {
                    printerModule.Guid = Guid.NewGuid().ToString();
                }

                if (_uow.PrinterModuleRepository.Exists(h => h.Guid.Equals(printerModule.Guid)))
                {
                    return new DtoActionResult() { ErrorMessage = "A Printer Module With This Guid Already Exists.  " + printerModule.Guid };
                }

                var printer = new EntityPrinterModule();
                printer.Name = printerModule.DisplayName;
                printer.Description = "Added Via Policy Template " + _export.Name + "  On " + DateTime.Now +
                                     "\r\n" + printerModule.Description;
                printer.Guid = printerModule.Guid;
                printer.Action = printerModule.PrinterAction;
                printer.IsDefault = printerModule.IsDefault;
                printer.NetworkPath = printerModule.PrinterPath;
                printer.RestartSpooler = printerModule.RestartSpooler;
                printer.WaitForEnumeration = printerModule.WaitForEnumeration;

                if (_uow.PrinterModuleRepository.Exists(h => h.Name.Equals(printer.Name)))
                {
                    for (var c = 1; c <= 100; c++)
                    {
                        if (c == 100)
                            return new DtoActionResult() { ErrorMessage = "Could Not Determine A Printer Name" };

                        var newName = printer.Name + "_" + c;
                        if (!_uow.PrinterModuleRepository.Exists(h => h.Name == newName))
                        {
                            printer.Name = newName;
                            break;
                        }
                    }
                }

                var addResult = new ServicePrinterModule().AddModule(printer);
                if (!addResult.Success) return addResult;

                var policyModule = new EntityPolicyModules();
                policyModule.Guid = printer.Guid;
                policyModule.ModuleId = addResult.Id;
                policyModule.ModuleType = EnumModule.ModuleType.Printer;
                policyModule.Name = printer.Name;
                policyModule.Order = printerModule.Order;
                policyModule.PolicyId = _policy.Id;
                policyModule.ConditionFailedAction = printerModule.ConditionFailedAction;
                policyModule.ConditionNextModule = printerModule.ConditionNextOrder;


                var conditionId = CreateCondition(printerModule.Condition);
                if (conditionId != 0)
                    policyModule.ConditionId = conditionId;
                else
                    policyModule.ConditionId = -1;

                _uow.PolicyModulesRepository.Insert(policyModule);

            }

            return new DtoActionResult() { Success = true };
        }

        private DtoActionResult CreateMessages()
        {
            foreach (var messageModule in _export.MessageModules)
            {
                if (_export.Instructions.Contains("[generate-module-guid]"))
                {
                    messageModule.Guid = Guid.NewGuid().ToString();
                }

                if (_uow.MessageModuleRepository.Exists(h => h.Guid.Equals(messageModule.Guid)))
                {
                    return new DtoActionResult() { ErrorMessage = "A Message Module With This Guid Already Exists.  " + messageModule.Guid };
                }

                var message = new EntityMessageModule();
                message.Name = messageModule.DisplayName;
                message.Description = "Added Via Policy Template " + _export.Name + "  On " + DateTime.Now +
                                     "\r\n" + messageModule.Description;
                message.Guid = messageModule.Guid;
                message.Title = messageModule.Title;
                message.Message = messageModule.Message;
                message.Timeout = messageModule.Timeout;

                if (_uow.MessageModuleRepository.Exists(h => h.Name.Equals(message.Name)))
                {
                    for (var c = 1; c <= 100; c++)
                    {
                        if (c == 100)
                            return new DtoActionResult() { ErrorMessage = "Could Not Determine A Message Name" };

                        var newName = message.Name + "_" + c;
                        if (!_uow.MessageModuleRepository.Exists(h => h.Name == newName))
                        {
                            message.Name = newName;
                            break;
                        }
                    }
                }

                var addResult = new ServiceMessageModule().AddModule(message);
                if (!addResult.Success) return addResult;

                var policyModule = new EntityPolicyModules();
                policyModule.Guid = message.Guid;
                policyModule.ModuleId = addResult.Id;
                policyModule.ModuleType = EnumModule.ModuleType.Message;
                policyModule.Name = message.Name;
                policyModule.Order = messageModule.Order;
                policyModule.PolicyId = _policy.Id;
                policyModule.ConditionFailedAction = messageModule.ConditionFailedAction;
                policyModule.ConditionNextModule = messageModule.ConditionNextOrder;


                var conditionId = CreateCondition(messageModule.Condition);
                if (conditionId != 0)
                    policyModule.ConditionId = conditionId;
                else
                    policyModule.ConditionId = -1;
                _uow.PolicyModulesRepository.Insert(policyModule);

            }

            return new DtoActionResult() { Success = true };
        }

        private DtoActionResult CreateCommands()
        {
            foreach (var commandModule in _export.CommandModules)
            {
                if (_export.Instructions.Contains("[generate-module-guid]"))
                {
                    commandModule.Guid = Guid.NewGuid().ToString();
                }

                if (_uow.CommandModuleRepository.Exists(h => h.Guid.Equals(commandModule.Guid)))
                {
                    return new DtoActionResult() { ErrorMessage = "A Command Module With This Guid Already Exists.  " + commandModule.Guid };
                }

                var command = new EntityCommandModule();
                command.Name = commandModule.DisplayName;
                command.Description = "Added Via Policy Template " + _export.Name + "  On " + DateTime.Now +
                                     "\r\n" + commandModule.Description;
                command.Guid = commandModule.Guid;
                command.Arguments = commandModule.Arguments;
                command.Command = commandModule.Command;
                command.RedirectStdError = commandModule.RedirectError;
                command.RedirectStdOut = commandModule.RedirectOutput;
                command.SuccessCodes = commandModule.SuccessCodes;
                command.Timeout = commandModule.Timeout;
                command.WorkingDirectory = commandModule.WorkingDirectory;
                command.ImpersonationId = -1;

                if (commandModule.UploadedFiles.Any())
                    _policyHasInternalFiles = true;

                if (commandModule.ExternalFiles.Any())
                    _policyHasExternalFiles = true;

                if (_uow.CommandModuleRepository.Exists(h => h.Name.Equals(command.Name)))
                {
                    for (var c = 1; c <= 100; c++)
                    {
                        if (c == 100)
                            return new DtoActionResult() { ErrorMessage = "Could Not Determine A Command Name" };

                        var newName = command.Name + "_" + c;
                        if (!_uow.CommandModuleRepository.Exists(h => h.Name == newName))
                        {
                            command.Name = newName;
                            break;
                        }
                    }
                }

                var addResult = new ServiceCommandModule().AddModule(command);
                if (!addResult.Success) return addResult;

                var policyModule = new EntityPolicyModules();
                policyModule.Guid = command.Guid;
                policyModule.ModuleId = addResult.Id;
                policyModule.ModuleType = EnumModule.ModuleType.Command;
                policyModule.Name = command.Name;
                policyModule.Order = commandModule.Order;
                policyModule.PolicyId = _policy.Id;
                policyModule.ConditionFailedAction = commandModule.ConditionFailedAction;
                policyModule.ConditionNextModule = commandModule.ConditionNextOrder;


                var conditionId = CreateCondition(commandModule.Condition);
                if (conditionId != 0)
                    policyModule.ConditionId = conditionId;
                else
                    policyModule.ConditionId = -1;

                _uow.PolicyModulesRepository.Insert(policyModule);

            }

            return new DtoActionResult() { Success = true };
        }

        private DtoActionResult CreateFileCopy()
        {
            foreach (var fileCopyModule in _export.FileCopyModules)
            {
                if (_export.Instructions.Contains("[generate-module-guid]"))
                {
                    fileCopyModule.Guid = Guid.NewGuid().ToString();
                }

                if (_uow.FileCopyModuleRepository.Exists(h => h.Guid.Equals(fileCopyModule.Guid)))
                {
                    return new DtoActionResult() { ErrorMessage = "A File Copy Module With This Guid Already Exists. " + fileCopyModule.Guid };
                }

                var fileCopy = new EntityFileCopyModule();

                fileCopy.Name = fileCopyModule.DisplayName;
                fileCopy.Description = "Added Via Policy Template " + _export.Name + "  On " + DateTime.Now +
                                     "\r\n" + fileCopyModule.Description;
                fileCopy.Guid = fileCopyModule.Guid;
                fileCopy.DecompressAfterCopy = fileCopyModule.Unzip;
                fileCopy.OverwriteExisting = fileCopyModule.Overwrite;
                fileCopy.Destination = fileCopyModule.Destination;
                
                if (fileCopyModule.UploadedFiles.Any())
                    _policyHasInternalFiles = true;

                if (fileCopyModule.ExternalFiles.Any())
                    _policyHasExternalFiles = true;

                if (_uow.FileCopyModuleRepository.Exists(h => h.Name.Equals(fileCopy.Name)))
                {
                    for (var c = 1; c <= 100; c++)
                    {
                        if (c == 100)
                            return new DtoActionResult() { ErrorMessage = "Could Not Determine A File Copy Name" };

                        var newName = fileCopy.Name + "_" + c;
                        if (!_uow.FileCopyModuleRepository.Exists(h => h.Name == newName))
                        {
                            fileCopy.Name = newName;
                            break;
                        }
                    }
                }

                var addResult = new ServiceFileCopyModule().AddModule(fileCopy);
                if (!addResult.Success) return addResult;

                var policyModule = new EntityPolicyModules();
                policyModule.Guid = fileCopy.Guid;
                policyModule.ModuleId = addResult.Id;
                policyModule.ModuleType = EnumModule.ModuleType.FileCopy;
                policyModule.Name = fileCopy.Name;
                policyModule.Order = fileCopyModule.Order;
                policyModule.PolicyId = _policy.Id;
                policyModule.ConditionFailedAction = fileCopyModule.ConditionFailedAction;
                policyModule.ConditionNextModule = fileCopyModule.ConditionNextOrder;


                var conditionId = CreateCondition(fileCopyModule.Condition);
                if (conditionId != 0)
                    policyModule.ConditionId = conditionId;
                else
                    policyModule.ConditionId = -1;
                _uow.PolicyModulesRepository.Insert(policyModule);

            }

            return new DtoActionResult() { Success = true };
        }

        private DtoActionResult CreateSoftware()
        {
            foreach (var softwareModule in _export.SoftwareModules)
            {
                if (_export.Instructions.Contains("[generate-module-guid]"))
                {
                    softwareModule.Guid = Guid.NewGuid().ToString();
                }

                if (_uow.SoftwareModuleRepository.Exists(h => h.Guid.Equals(softwareModule.Guid)))
                {
                    return new DtoActionResult() { ErrorMessage = "A Software Module With This Guid Already Exists.  " + softwareModule.Guid };
                }

                var software = new EntitySoftwareModule();
                software.Name = softwareModule.DisplayName;
                software.Description = "Added Via Policy Template " + _export.Name + "  On " + DateTime.Now +
                                     "\r\n" + software.Description;
                software.Guid = softwareModule.Guid;
                software.Arguments = softwareModule.Arguments;
                software.RedirectStdError = softwareModule.RedirectError;
                software.RedirectStdOut = softwareModule.RedirectOutput;
                software.SuccessCodes = softwareModule.SuccessCodes;
                software.Timeout = softwareModule.Timeout;
                software.AdditionalArguments = softwareModule.AdditionalArguments;
                software.Command = softwareModule.Command;
                software.InstallType = softwareModule.InstallType;
                software.ImpersonationId = -1;
                if (softwareModule.UploadedFiles.Any())
                    _policyHasInternalFiles = true;

                if (softwareModule.ExternalFiles.Any())
                    _policyHasExternalFiles = true;

                if (_uow.SoftwareModuleRepository.Exists(h => h.Name.Equals(software.Name)))
                {
                    for (var c = 1; c <= 100; c++)
                    {
                        if (c == 100)
                            return new DtoActionResult() { ErrorMessage = "Could Not Determine A Software Name" };

                        var newName = software.Name + "_" + c;
                        if (!_uow.SoftwareModuleRepository.Exists(h => h.Name == newName))
                        {
                            software.Name = newName;
                            break;
                        }
                    }
                }

                var addResult = new ServiceSoftwareModule().AddModule(software);
                if (!addResult.Success) return addResult;

                var policyModule = new EntityPolicyModules();
                policyModule.Guid = software.Guid;
                policyModule.ModuleId = addResult.Id;
                policyModule.ModuleType = EnumModule.ModuleType.Software;
                policyModule.Name = software.Name;
                policyModule.Order = softwareModule.Order;
                policyModule.PolicyId = _policy.Id;
                policyModule.ConditionFailedAction = softwareModule.ConditionFailedAction;
                policyModule.ConditionNextModule = softwareModule.ConditionNextOrder;


                var conditionId = CreateCondition(softwareModule.Condition);
                if (conditionId != 0)
                    policyModule.ConditionId = conditionId;
                else
                    policyModule.ConditionId = -1;
                _uow.PolicyModulesRepository.Insert(policyModule);

            }

            return new DtoActionResult() { Success = true };
        }

        private DtoActionResult CreateWindowsUpdate()
        {
            foreach (var wuModuleModule in _export.WuModules)
            {
                if (_export.Instructions.Contains("[generate-module-guid]"))
                {
                    wuModuleModule.Guid = Guid.NewGuid().ToString();
                }

                if (_uow.WindowsUpdateModuleRepository.Exists(h => h.Guid.Equals(wuModuleModule.Guid)))
                {
                    return new DtoActionResult() { ErrorMessage = "A Windows Update Module With This Guid Already Exists.  " + wuModuleModule.Guid };
                }

                var wu = new EntityWuModule();
                wu.Name = wuModuleModule.DisplayName;
                wu.Description = "Added Via Policy Template " + _export.Name + "  On " + DateTime.Now +
                                     "\r\n" + wu.Description;
                wu.Guid = wuModuleModule.Guid;
                wu.AdditionalArguments = wuModuleModule.Arguments;
                wu.RedirectStdError = wuModuleModule.RedirectError;
                wu.RedirectStdOut = wuModuleModule.RedirectOutput;
                wu.SuccessCodes = wuModuleModule.SuccessCodes;
                wu.Timeout = wuModuleModule.Timeout;
                
                if (wuModuleModule.UploadedFiles.Any())
                    _policyHasInternalFiles = true;

                if (wuModuleModule.ExternalFiles.Any())
                    _policyHasExternalFiles = true;

                if (_uow.WindowsUpdateModuleRepository.Exists(h => h.Name.Equals(wu.Name)))
                {
                    for (var c = 1; c <= 100; c++)
                    {
                        if (c == 100)
                            return new DtoActionResult() { ErrorMessage = "Could Not Determine A Windows Update Name" };

                        var newName = wu.Name + "_" + c;
                        if (!_uow.WindowsUpdateModuleRepository.Exists(h => h.Name == newName))
                        {
                            wu.Name = newName;
                            break;
                        }
                    }
                }

                var addResult = new ServiceWuModule().AddModule(wu);
                if (!addResult.Success) return addResult;

                var policyModule = new EntityPolicyModules();
                policyModule.Guid = wu.Guid;
                policyModule.ModuleId = addResult.Id;
                policyModule.ModuleType = EnumModule.ModuleType.Wupdate;
                policyModule.Name = wu.Name;
                policyModule.Order = wuModuleModule.Order;
                policyModule.PolicyId = _policy.Id;
                policyModule.ConditionFailedAction = wuModuleModule.ConditionFailedAction;
                policyModule.ConditionNextModule = wuModuleModule.ConditionNextOrder;


                var conditionId = CreateCondition(wuModuleModule.Condition);
                if (conditionId != 0)
                    policyModule.ConditionId = conditionId;
                else
                    policyModule.ConditionId = -1;
                _uow.PolicyModulesRepository.Insert(policyModule);

            }

            return new DtoActionResult() { Success = true };
        }

        private void QueueExternalDownloads()
        {
            var listToDownload = new List<DtoFileDownload>();

            foreach (var fileCopyModule in _export.FileCopyModules)
            {
                foreach (var file in fileCopyModule.ExternalFiles)
                {
                    var dtoFile = new DtoFileDownload(){SyncWhenDone = true};
                    dtoFile.FileName = file.FileName;
                    dtoFile.ModuleGuid = file.ModuleGuid;
                    dtoFile.Sha256 = file.Sha256Hash;
                    dtoFile.Url = file.Url;
                    listToDownload.Add(dtoFile);
                }
            }

            foreach (var softwareModule in _export.SoftwareModules)
            {
                foreach (var file in softwareModule.ExternalFiles)
                {
                    var dtoFile = new DtoFileDownload(){SyncWhenDone = true};
                    dtoFile.FileName = file.FileName;
                    dtoFile.ModuleGuid = file.ModuleGuid;
                    dtoFile.Sha256 = file.Sha256Hash;
                    dtoFile.Url = file.Url;
                    listToDownload.Add(dtoFile);
                }
            }

            foreach (var wuModule in _export.WuModules)
            {
                foreach (var file in wuModule.ExternalFiles)
                {
                    var dtoFile = new DtoFileDownload(){SyncWhenDone = true};
                    dtoFile.FileName = file.FileName;
                    dtoFile.ModuleGuid = file.ModuleGuid;
                    dtoFile.Sha256 = file.Sha256Hash;
                    dtoFile.Url = file.Url;
                    listToDownload.Add(dtoFile);
                }
            }

            foreach (var commandModule in _export.CommandModules)
            {
                foreach (var file in commandModule.ExternalFiles)
                {
                    var dtoFile = new DtoFileDownload() { SyncWhenDone = true };
                    dtoFile.FileName = file.FileName;
                    dtoFile.ModuleGuid = file.ModuleGuid;
                    dtoFile.Sha256 = file.Sha256Hash;
                    dtoFile.Url = file.Url;
                    listToDownload.Add(dtoFile);
                }
            }


            if (listToDownload.Count > 0)
            {
                _policyHasExternalFiles = true;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                var t = new Thread(() => new ServiceExternalDownload().BatchDownload(listToDownload));
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                t.Start();
                
            }
        }

        private int CreateCondition(DtoScriptModuleExport condition)
        {
            if (condition == null) return 0;

            var scriptModule = new ServiceScriptModule().GetModuleByGuid(condition.Guid);
            if(scriptModule == null)
            {
                var script = new EntityScriptModule();
                script.AddInventoryCollection = condition.AddToInventory;
                script.Name = condition.DisplayName;
                script.Description = "Added Via Policy Template " + _export.Name + "  On " + DateTime.Now +
                                     "\r\n" + script.Description;
                script.Guid = condition.Guid;
                script.Arguments = condition.Arguments;
                script.IsCondition = condition.IsCondition;
                script.RedirectStdError = condition.RedirectError;
                script.RedirectStdOut = condition.RedirectOutput;
                script.ScriptContents = condition.ScriptContents;
                script.ScriptType = condition.ScriptType;
                script.SuccessCodes = condition.SuccessCodes;
                script.Timeout = condition.Timeout;
                script.WorkingDirectory = condition.WorkingDirectory;
                script.ImpersonationId = -1;

                if (_uow.ScriptModuleRepository.Exists(h => h.Name.Equals(script.Name)))
                {
                    for (var c = 1; c <= 100; c++)
                    {
                        if (c == 100)
                            return 0;

                        var newName = script.Name + "_" + c;
                        if (!_uow.ScriptModuleRepository.Exists(h => h.Name == newName))
                        {
                            script.Name = newName;
                            break;
                        }
                    }
                }

                var addResult = new ServiceScriptModule().AddModule(script);
                if (!addResult.Success) return 0;
                return script.Id;
            }
            else
            {
                return scriptModule.Id;
            }
            
        }
    }
}
