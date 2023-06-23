using System;
using System.Linq;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Dto.client;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_Service.Entity;

namespace Toems_Service.Workflows
{
    public class ClientPolicyJson
    {
        private readonly ServicePolicy _policyService;
        private EntityPolicy _policy;
        private readonly DtoClientPolicy _clientPolicy;

        public ClientPolicyJson()
        {
            _policyService = new ServicePolicy();
            _clientPolicy = new DtoClientPolicy();
        }
        public DtoClientPolicy CreateInstantModule(DtoGuidTypeMapping moduleType)
        {
            _clientPolicy.Name = "Instant Module ";
            _clientPolicy.Guid = Guid.NewGuid().ToString();
            _clientPolicy.Frequency = EnumPolicy.Frequency.OncePerComputer;
            _clientPolicy.Trigger = EnumPolicy.Trigger.StartupOrCheckin;
            _clientPolicy.SubFrequency = 0;
            _clientPolicy.StartDate = DateTime.Now;
            _clientPolicy.CompletedAction = EnumPolicy.CompletedAction.DoNothing;
            _clientPolicy.RemoveInstallCache = true;
            _clientPolicy.ExecutionType = EnumPolicy.ExecutionType.Install;
            _clientPolicy.ErrorAction = EnumPolicy.ErrorAction.Continue;
            _clientPolicy.IsInventory = EnumPolicy.InventoryAction.Disabled;
            _clientPolicy.RemoteAccess = EnumPolicy.RemoteAccess.NotConfigured;
            _clientPolicy.IsLoginTracker = false;
            _clientPolicy.FrequencyMissedAction = EnumPolicy.FrequencyMissedAction.NextOpportunity;
            _clientPolicy.LogLevel = EnumPolicy.LogLevel.Full;
            _clientPolicy.SkipServerResult = false;
            _clientPolicy.IsApplicationMonitor = false;
            _clientPolicy.StartWindowScheduleId = -1;
            _clientPolicy.EndWindowScheduleId = -1;
            _clientPolicy.WuType = EnumPolicy.WuType.Disabled;
            _clientPolicy.PolicyComCondition = EnumPolicy.PolicyComCondition.Any;
            //_clientPolicy.Id = _policy.Id;
            _clientPolicy.ConditionFailedAction = EnumCondition.FailedAction.MarkFailed;

            EntityPolicyModules policyModule = new EntityPolicyModules();
            policyModule.ConditionId = -1;
            policyModule.ModuleId = moduleType.moduleId;
            policyModule.ModuleType = moduleType.moduleType;
            policyModule.Order = 0;

            if (policyModule.ModuleType == EnumModule.ModuleType.Command)
            {
                CommandModule(policyModule);
            }
            else if (policyModule.ModuleType == EnumModule.ModuleType.FileCopy)
            {
                FileCopyModule(policyModule);
            }
            else if (policyModule.ModuleType == EnumModule.ModuleType.Script)
            {
                ScriptModule(policyModule);
            }
            else if (policyModule.ModuleType == EnumModule.ModuleType.Printer)
            {
                PrinterModule(policyModule);
            }
            else if (policyModule.ModuleType == EnumModule.ModuleType.Software)
            {
                SoftwareModule(policyModule);
            }
            else if (policyModule.ModuleType == EnumModule.ModuleType.Wupdate)
            {
                WuModule(policyModule);
            }
            else if (policyModule.ModuleType == EnumModule.ModuleType.Message)
            {
                MessageModule(policyModule);
            }
            else if (policyModule.ModuleType == EnumModule.ModuleType.WinPE)
            {
                WinPeModule(policyModule);
            }


            return _clientPolicy;
        }
        public DtoClientPolicy Create(int policyId)
        {
            _policy = _policyService.GetPolicy(policyId);
            Policy();
            DtoModuleSearchFilter filter = new DtoModuleSearchFilter();
            filter.IncludeCommand = true;
            filter.IncludeFileCopy = true;
            filter.IncludePrinter = true;
            filter.IncludeScript = true;
            filter.IncludeSoftware = true;
            filter.IncludeWu = true;
            filter.IncludeMessage = true;
            filter.IncludeWinPe = true;
            filter.IncludeWinget = true;
            filter.Limit = Int32.MaxValue;
            var policyModules = _policyService.SearchAssignedPolicyModules(policyId,filter);
            foreach (var policyModule in policyModules.OrderBy(x => x.Name))
            {
                if (policyModule.ModuleType == EnumModule.ModuleType.Command)
                {
                    CommandModule(policyModule);
                }
                else if (policyModule.ModuleType == EnumModule.ModuleType.FileCopy)
                {
                    FileCopyModule(policyModule);
                }
                else if (policyModule.ModuleType == EnumModule.ModuleType.Script)
                {
                    ScriptModule(policyModule);
                }
                else if (policyModule.ModuleType == EnumModule.ModuleType.Printer)
                {
                    PrinterModule(policyModule);
                }
                else if (policyModule.ModuleType == EnumModule.ModuleType.Software)
                {
                    SoftwareModule(policyModule);
                }
                else if (policyModule.ModuleType == EnumModule.ModuleType.Wupdate)
                {
                    WuModule(policyModule);
                }
                else if (policyModule.ModuleType == EnumModule.ModuleType.Message)
                {
                    MessageModule(policyModule);
                }
                else if (policyModule.ModuleType == EnumModule.ModuleType.WinPE)
                {
                    WinPeModule(policyModule);
                }
            }

            return _clientPolicy;
        }

        private void Policy()
        {
            _clientPolicy.Name = _policy.Name;
            _clientPolicy.Guid = _policy.Guid;
            _clientPolicy.Frequency = _policy.Frequency;
            _clientPolicy.Trigger = _policy.Trigger;
            _clientPolicy.SubFrequency = _policy.SubFrequency;
            _clientPolicy.StartDate = _policy.StartDate;
            _clientPolicy.CompletedAction = _policy.CompletedAction;
            _clientPolicy.RemoveInstallCache = _policy.RemoveInstallCache;
            _clientPolicy.ExecutionType = _policy.ExecutionType;
            _clientPolicy.ErrorAction = _policy.ErrorAction;
            _clientPolicy.IsInventory = _policy.RunInventory;
            _clientPolicy.RemoteAccess = _policy.RemoteAccess;
            _clientPolicy.IsLoginTracker = _policy.RunLoginTracker;
            _clientPolicy.JoinDomain = _policy.JoinDomain;
            _clientPolicy.ImagePrepCleanup = _policy.ImagePrepCleanup;
            _clientPolicy.FrequencyMissedAction = _policy.MissedAction;
            _clientPolicy.LogLevel = _policy.LogLevel;
            _clientPolicy.SkipServerResult = _policy.SkipServerResult;
            _clientPolicy.IsApplicationMonitor = _policy.RunApplicationMonitor;
            _clientPolicy.StartWindowScheduleId = _policy.WindowStartScheduleId;
            _clientPolicy.EndWindowScheduleId = _policy.WindowEndScheduleId;
            _clientPolicy.WuType = _policy.WuType;
            _clientPolicy.PolicyComCondition = _policy.PolicyComCondition;
            _clientPolicy.Id = _policy.Id;
            _clientPolicy.ConditionFailedAction = _policy.ConditionFailedAction;
            if (_policy.ConditionId != -1)
            {
                var conditionScript = new ServiceScriptModule().GetModule(_policy.ConditionId);
                if (conditionScript != null)
                {
                    _clientPolicy.Condition.Arguments = conditionScript.Arguments;
                    _clientPolicy.Condition.DisplayName = conditionScript.Name;
                    _clientPolicy.Condition.Guid = conditionScript.Guid;
                    _clientPolicy.Condition.RedirectError = conditionScript.RedirectStdError;
                    _clientPolicy.Condition.RedirectOutput = conditionScript.RedirectStdOut;
                    if (conditionScript.ImpersonationId != -1)
                    {
                        var scriptImpersonationGuid = new ServiceImpersonationAccount().GetGuid(conditionScript.ImpersonationId);
                        if (!string.IsNullOrEmpty(scriptImpersonationGuid))
                            _clientPolicy.Condition.RunAs = scriptImpersonationGuid;
                    }
                    _clientPolicy.Condition.ScriptType = conditionScript.ScriptType;
                    foreach (var successCode in conditionScript.SuccessCodes.Split(','))
                        _clientPolicy.Condition.SuccessCodes.Add(successCode);
                    _clientPolicy.Condition.Timeout = conditionScript.Timeout;
                    _clientPolicy.Condition.WorkingDirectory = conditionScript.WorkingDirectory;
                }

            }
            if(_policy.JoinDomain)
            {
                _clientPolicy.DomainOU = _policy.DomainOU;
            }
        }

        private void CommandModule(EntityPolicyModules policyModule)
        {
            var clientCommandModule = new DtoClientCommandModule();
            var commandModule = new ServiceCommandModule().GetModule(policyModule.ModuleId);
            clientCommandModule.Order = policyModule.Order;
            clientCommandModule.Guid = commandModule.Guid;
            clientCommandModule.Command = commandModule.Command;
            clientCommandModule.Arguments = commandModule.Arguments;
            clientCommandModule.DisplayName = commandModule.Name;
            clientCommandModule.Timeout = commandModule.Timeout;
            clientCommandModule.RedirectOutput = commandModule.RedirectStdOut;
            clientCommandModule.RedirectError = commandModule.RedirectStdError;
            clientCommandModule.WorkingDirectory = commandModule.WorkingDirectory;
            foreach(var successCode in commandModule.SuccessCodes.Split(','))
                clientCommandModule.SuccessCodes.Add(successCode);

            if (commandModule.ImpersonationId != -1)
            {
                var impersonationGuid = new ServiceImpersonationAccount().GetGuid(commandModule.ImpersonationId);
                if (!string.IsNullOrEmpty(impersonationGuid))
                    clientCommandModule.RunAs = impersonationGuid;
            }

            if (policyModule.ConditionId != -1)
            {
                var conditionScript = new ServiceScriptModule().GetModule(policyModule.ConditionId);
                if(conditionScript != null)
                {
                    clientCommandModule.ConditionFailedAction = policyModule.ConditionFailedAction;
                    clientCommandModule.ConditionNextOrder = policyModule.ConditionNextModule;
                    clientCommandModule.Condition = new DtoClientModuleCondition();
                    clientCommandModule.Condition.Arguments = conditionScript.Arguments;
                    clientCommandModule.Condition.DisplayName = conditionScript.Name;
                    clientCommandModule.Condition.Guid = conditionScript.Guid;
                    clientCommandModule.Condition.RedirectError = conditionScript.RedirectStdError;
                    clientCommandModule.Condition.RedirectOutput = conditionScript.RedirectStdOut;
                    if (conditionScript.ImpersonationId != -1)
                    {
                        var scriptImpersonationGuid = new ServiceImpersonationAccount().GetGuid(conditionScript.ImpersonationId);
                        if (!string.IsNullOrEmpty(scriptImpersonationGuid))
                            clientCommandModule.Condition.RunAs = scriptImpersonationGuid;
                    }
                    clientCommandModule.Condition.ScriptType = conditionScript.ScriptType;
                    foreach (var successCode in conditionScript.SuccessCodes.Split(','))
                        clientCommandModule.Condition.SuccessCodes.Add(successCode);
                    clientCommandModule.Condition.Timeout = conditionScript.Timeout;
                    clientCommandModule.Condition.WorkingDirectory = conditionScript.WorkingDirectory;
                }

            }

            var moduleFiles = new ServiceModule().GetModuleFiles(commandModule.Guid);
            foreach (var file in moduleFiles.OrderBy(x => x.FileName))
            {
                var clientFile = new DtoClientFileHash();
                clientFile.FileName = file.FileName;
                clientFile.FileHash = file.Md5Hash;
                clientCommandModule.Files.Add(clientFile);
            }

            _clientPolicy.CommandModules.Add(clientCommandModule);

        
        }

        private void SoftwareModule(EntityPolicyModules policyModule)
        {
            var clientSoftwareModule = new DtoClientSoftwareModule();
            var softwareModule = new ServiceSoftwareModule().GetModule(policyModule.ModuleId);
            clientSoftwareModule.Guid = softwareModule.Guid;
            clientSoftwareModule.DisplayName = softwareModule.Name;
            clientSoftwareModule.Command = softwareModule.Command;
            clientSoftwareModule.Arguments = softwareModule.Arguments + " " + softwareModule.AdditionalArguments;
            clientSoftwareModule.Order = policyModule.Order;
            clientSoftwareModule.Timeout = softwareModule.Timeout;
            clientSoftwareModule.InstallType = softwareModule.InstallType;
            clientSoftwareModule.RedirectOutput = softwareModule.RedirectStdOut;
            clientSoftwareModule.RedirectError = softwareModule.RedirectStdError;
            foreach (var successCode in softwareModule.SuccessCodes.Split(','))
                clientSoftwareModule.SuccessCodes.Add(successCode);

            var moduleFiles = new ServiceModule().GetModuleFiles(softwareModule.Guid);
            foreach (var file in moduleFiles.OrderBy(x => x.FileName))
            {
                var clientFile = new DtoClientFileHash();
                clientFile.FileName = file.FileName;
                clientFile.FileHash = file.Md5Hash;
                clientSoftwareModule.Files.Add(clientFile);
            }

            if (softwareModule.ImpersonationId != -1)
            {
                var impersonationGuid = new ServiceImpersonationAccount().GetGuid(softwareModule.ImpersonationId);
                if (!string.IsNullOrEmpty(impersonationGuid))
                    clientSoftwareModule.RunAs = impersonationGuid;
            }

            if (policyModule.ConditionId != -1)
            {
                var conditionScript = new ServiceScriptModule().GetModule(policyModule.ConditionId);
                if (conditionScript != null)
                {
                    clientSoftwareModule.ConditionFailedAction = policyModule.ConditionFailedAction;
                    clientSoftwareModule.ConditionNextOrder = policyModule.ConditionNextModule;
                    clientSoftwareModule.Condition = new DtoClientModuleCondition();
                    clientSoftwareModule.Condition.Arguments = conditionScript.Arguments;
                    clientSoftwareModule.Condition.DisplayName = conditionScript.Name;
                    clientSoftwareModule.Condition.Guid = conditionScript.Guid;
                    clientSoftwareModule.Condition.RedirectError = conditionScript.RedirectStdError;
                    clientSoftwareModule.Condition.RedirectOutput = conditionScript.RedirectStdOut;
                    if (conditionScript.ImpersonationId != -1)
                    {
                        var scriptImpersonationGuid = new ServiceImpersonationAccount().GetGuid(conditionScript.ImpersonationId);
                        if (!string.IsNullOrEmpty(scriptImpersonationGuid))
                            clientSoftwareModule.Condition.RunAs = scriptImpersonationGuid;
                    }
                    clientSoftwareModule.Condition.ScriptType = conditionScript.ScriptType;
                    foreach (var successCode in conditionScript.SuccessCodes.Split(','))
                        clientSoftwareModule.Condition.SuccessCodes.Add(successCode);
                    clientSoftwareModule.Condition.Timeout = conditionScript.Timeout;
                    clientSoftwareModule.Condition.WorkingDirectory = conditionScript.WorkingDirectory;
                }

            }

            _clientPolicy.SoftwareModules.Add(clientSoftwareModule);
        }

        private void FileCopyModule(EntityPolicyModules policyModule)
        {
            var clientFileCopyModule = new DtoClientFileCopyModule();
            var fileCopyModule = new ServiceFileCopyModule().GetModule(policyModule.ModuleId);
            clientFileCopyModule.Guid = fileCopyModule.Guid;
            clientFileCopyModule.DisplayName = fileCopyModule.Name;
            clientFileCopyModule.Destination = fileCopyModule.Destination;
            clientFileCopyModule.Order = policyModule.Order;
            clientFileCopyModule.Unzip = fileCopyModule.DecompressAfterCopy;
            clientFileCopyModule.Overwrite = fileCopyModule.OverwriteExisting;
            var moduleFiles = new ServiceModule().GetModuleFiles(fileCopyModule.Guid);
            foreach (var file in moduleFiles.OrderBy(x => x.FileName))
            {
                var clientFile = new DtoClientFileHash();
                clientFile.FileName = file.FileName;
                clientFile.FileHash = file.Md5Hash;
                clientFileCopyModule.Files.Add(clientFile);
            }

            if (policyModule.ConditionId != -1)
            {
                var conditionScript = new ServiceScriptModule().GetModule(policyModule.ConditionId);
                if (conditionScript != null)
                {
                    clientFileCopyModule.ConditionFailedAction = policyModule.ConditionFailedAction;
                    clientFileCopyModule.ConditionNextOrder = policyModule.ConditionNextModule;
                    clientFileCopyModule.Condition = new DtoClientModuleCondition();
                    clientFileCopyModule.Condition.Arguments = conditionScript.Arguments;
                    clientFileCopyModule.Condition.DisplayName = conditionScript.Name;
                    clientFileCopyModule.Condition.Guid = conditionScript.Guid;
                    clientFileCopyModule.Condition.RedirectError = conditionScript.RedirectStdError;
                    clientFileCopyModule.Condition.RedirectOutput = conditionScript.RedirectStdOut;
                    if (conditionScript.ImpersonationId != -1)
                    {
                        var scriptImpersonationGuid = new ServiceImpersonationAccount().GetGuid(conditionScript.ImpersonationId);
                        if (!string.IsNullOrEmpty(scriptImpersonationGuid))
                            clientFileCopyModule.Condition.RunAs = scriptImpersonationGuid;
                    }
                    clientFileCopyModule.Condition.ScriptType = conditionScript.ScriptType;
                    foreach (var successCode in conditionScript.SuccessCodes.Split(','))
                        clientFileCopyModule.Condition.SuccessCodes.Add(successCode);
                    clientFileCopyModule.Condition.Timeout = conditionScript.Timeout;
                    clientFileCopyModule.Condition.WorkingDirectory = conditionScript.WorkingDirectory;
                }

            }
            _clientPolicy.FileCopyModules.Add(clientFileCopyModule);
        }

        private void WinPeModule(EntityPolicyModules policyModule)
        {
            var clientWinPeModule = new DtoClientWinPeModule();
            var winPeModule = new ServiceWinPeModule().GetModule(policyModule.ModuleId);
            clientWinPeModule.Guid = winPeModule.Guid;
            clientWinPeModule.DisplayName = winPeModule.Name;
            clientWinPeModule.Order = policyModule.Order;
            var moduleFiles = new ServiceModule().GetModuleFiles(winPeModule.Guid);
            foreach (var file in moduleFiles.OrderBy(x => x.FileName))
            {
                var clientFile = new DtoClientFileHash();
                clientFile.FileName = file.FileName;
                clientFile.FileHash = file.Md5Hash;
                clientWinPeModule.Files.Add(clientFile);
            }

            if (policyModule.ConditionId != -1)
            {
                var conditionScript = new ServiceScriptModule().GetModule(policyModule.ConditionId);
                if (conditionScript != null)
                {
                    clientWinPeModule.ConditionFailedAction = policyModule.ConditionFailedAction;
                    clientWinPeModule.ConditionNextOrder = policyModule.ConditionNextModule;
                    clientWinPeModule.Condition = new DtoClientModuleCondition();
                    clientWinPeModule.Condition.Arguments = conditionScript.Arguments;
                    clientWinPeModule.Condition.DisplayName = conditionScript.Name;
                    clientWinPeModule.Condition.Guid = conditionScript.Guid;
                    clientWinPeModule.Condition.RedirectError = conditionScript.RedirectStdError;
                    clientWinPeModule.Condition.RedirectOutput = conditionScript.RedirectStdOut;
                    if (conditionScript.ImpersonationId != -1)
                    {
                        var scriptImpersonationGuid = new ServiceImpersonationAccount().GetGuid(conditionScript.ImpersonationId);
                        if (!string.IsNullOrEmpty(scriptImpersonationGuid))
                            clientWinPeModule.Condition.RunAs = scriptImpersonationGuid;
                    }
                    clientWinPeModule.Condition.ScriptType = conditionScript.ScriptType;
                    foreach (var successCode in conditionScript.SuccessCodes.Split(','))
                        clientWinPeModule.Condition.SuccessCodes.Add(successCode);
                    clientWinPeModule.Condition.Timeout = conditionScript.Timeout;
                    clientWinPeModule.Condition.WorkingDirectory = conditionScript.WorkingDirectory;
                }

            }
            _clientPolicy.WinPeModules.Add(clientWinPeModule);
        }

        private void MessageModule(EntityPolicyModules policyModule)
        {
            var clientMessageModule = new DtoClientMessageModule();
            var messageModule = new ServiceMessageModule().GetModule(policyModule.ModuleId);
            clientMessageModule.Guid = messageModule.Guid;
            clientMessageModule.DisplayName = messageModule.Name;    
            clientMessageModule.Order = policyModule.Order;
            clientMessageModule.Title = messageModule.Title;
            clientMessageModule.Message = messageModule.Message;
            clientMessageModule.Timeout = messageModule.Timeout;
            if (policyModule.ConditionId != -1)
            {
                var conditionScript = new ServiceScriptModule().GetModule(policyModule.ConditionId);
                if (conditionScript != null)
                {
                    clientMessageModule.ConditionFailedAction = policyModule.ConditionFailedAction;
                    clientMessageModule.ConditionNextOrder = policyModule.ConditionNextModule;
                    clientMessageModule.Condition = new DtoClientModuleCondition();
                    clientMessageModule.Condition.Arguments = conditionScript.Arguments;
                    clientMessageModule.Condition.DisplayName = conditionScript.Name;
                    clientMessageModule.Condition.Guid = conditionScript.Guid;
                    clientMessageModule.Condition.RedirectError = conditionScript.RedirectStdError;
                    clientMessageModule.Condition.RedirectOutput = conditionScript.RedirectStdOut;
                    if (conditionScript.ImpersonationId != -1)
                    {
                        var scriptImpersonationGuid = new ServiceImpersonationAccount().GetGuid(conditionScript.ImpersonationId);
                        if (!string.IsNullOrEmpty(scriptImpersonationGuid))
                            clientMessageModule.Condition.RunAs = scriptImpersonationGuid;
                    }
                    clientMessageModule.Condition.ScriptType = conditionScript.ScriptType;
                    foreach (var successCode in conditionScript.SuccessCodes.Split(','))
                        clientMessageModule.Condition.SuccessCodes.Add(successCode);
                    clientMessageModule.Condition.Timeout = conditionScript.Timeout;
                    clientMessageModule.Condition.WorkingDirectory = conditionScript.WorkingDirectory;
                }

            }
            _clientPolicy.MessageModules.Add(clientMessageModule);
        }

        private void ScriptModule(EntityPolicyModules policyModule)
        {
            var clientScriptModule = new DtoClientScriptModule();
            var scriptModule = new ServiceScriptModule().GetModule(policyModule.ModuleId);
            clientScriptModule.Guid = scriptModule.Guid;
            clientScriptModule.DisplayName = scriptModule.Name;
            clientScriptModule.Arguments = scriptModule.Arguments;
            clientScriptModule.Order = policyModule.Order;
            clientScriptModule.Timeout = scriptModule.Timeout;
            clientScriptModule.ScriptType = scriptModule.ScriptType;
            clientScriptModule.RedirectOutput = scriptModule.RedirectStdOut;
            clientScriptModule.RedirectError = scriptModule.RedirectStdError;
            clientScriptModule.AddToInventory = scriptModule.AddInventoryCollection;
            clientScriptModule.WorkingDirectory = scriptModule.WorkingDirectory;
            clientScriptModule.IsCondition = scriptModule.IsCondition;
            foreach (var successCode in scriptModule.SuccessCodes.Split(','))
                clientScriptModule.SuccessCodes.Add(successCode);

            if (scriptModule.ImpersonationId != -1)
            {
                var impersonationGuid = new ServiceImpersonationAccount().GetGuid(scriptModule.ImpersonationId);
                if (!string.IsNullOrEmpty(impersonationGuid))
                    clientScriptModule.RunAs = impersonationGuid;
            }

            if (policyModule.ConditionId != -1)
            {
                var conditionScript = new ServiceScriptModule().GetModule(policyModule.ConditionId);
                if (conditionScript != null)
                {
                    clientScriptModule.ConditionFailedAction = policyModule.ConditionFailedAction;
                    clientScriptModule.ConditionNextOrder = policyModule.ConditionNextModule;
                    clientScriptModule.Condition = new DtoClientModuleCondition();
                    clientScriptModule.Condition.Arguments = conditionScript.Arguments;
                    clientScriptModule.Condition.DisplayName = conditionScript.Name;
                    clientScriptModule.Condition.Guid = conditionScript.Guid;
                    clientScriptModule.Condition.RedirectError = conditionScript.RedirectStdError;
                    clientScriptModule.Condition.RedirectOutput = conditionScript.RedirectStdOut;
                    if (conditionScript.ImpersonationId != -1)
                    {
                        var scriptImpersonationGuid = new ServiceImpersonationAccount().GetGuid(conditionScript.ImpersonationId);
                        if (!string.IsNullOrEmpty(scriptImpersonationGuid))
                            clientScriptModule.Condition.RunAs = scriptImpersonationGuid;
                    }
                    clientScriptModule.Condition.ScriptType = conditionScript.ScriptType;
                    foreach (var successCode in conditionScript.SuccessCodes.Split(','))
                        clientScriptModule.Condition.SuccessCodes.Add(successCode);
                    clientScriptModule.Condition.Timeout = conditionScript.Timeout;
                    clientScriptModule.Condition.WorkingDirectory = conditionScript.WorkingDirectory;
                }

            }
            _clientPolicy.ScriptModules.Add(clientScriptModule);
        }

        private void PrinterModule(EntityPolicyModules policyModule)
        {
            var clientPrinterModule = new DtoClientPrinterModule();
            var printerModule = new ServicePrinterModule().GetModule(policyModule.ModuleId);
            clientPrinterModule.Guid = printerModule.Guid;
            clientPrinterModule.DisplayName = printerModule.Name;
            clientPrinterModule.PrinterPath = printerModule.NetworkPath;
            clientPrinterModule.Order = policyModule.Order;
            clientPrinterModule.IsDefault = printerModule.IsDefault;
            clientPrinterModule.RestartSpooler = printerModule.RestartSpooler;
            clientPrinterModule.PrinterAction = printerModule.Action;
            clientPrinterModule.WaitForEnumeration = printerModule.WaitForEnumeration;

            if (policyModule.ConditionId != -1)
            {
                var conditionScript = new ServiceScriptModule().GetModule(policyModule.ConditionId);
                if (conditionScript != null)
                {
                    clientPrinterModule.ConditionFailedAction = policyModule.ConditionFailedAction;
                    clientPrinterModule.ConditionNextOrder = policyModule.ConditionNextModule;
                    clientPrinterModule.Condition = new DtoClientModuleCondition();
                    clientPrinterModule.Condition.Arguments = conditionScript.Arguments;
                    clientPrinterModule.Condition.DisplayName = conditionScript.Name;
                    clientPrinterModule.Condition.Guid = conditionScript.Guid;
                    clientPrinterModule.Condition.RedirectError = conditionScript.RedirectStdError;
                    clientPrinterModule.Condition.RedirectOutput = conditionScript.RedirectStdOut;
                    if (conditionScript.ImpersonationId != -1)
                    {
                        var scriptImpersonationGuid = new ServiceImpersonationAccount().GetGuid(conditionScript.ImpersonationId);
                        if (!string.IsNullOrEmpty(scriptImpersonationGuid))
                            clientPrinterModule.Condition.RunAs = scriptImpersonationGuid;
                    }
                    clientPrinterModule.Condition.ScriptType = conditionScript.ScriptType;
                    foreach (var successCode in conditionScript.SuccessCodes.Split(','))
                        clientPrinterModule.Condition.SuccessCodes.Add(successCode);
                    clientPrinterModule.Condition.Timeout = conditionScript.Timeout;
                    clientPrinterModule.Condition.WorkingDirectory = conditionScript.WorkingDirectory;
                }

            }

            _clientPolicy.PrinterModules.Add(clientPrinterModule);
        }

        private void WuModule(EntityPolicyModules policyModule)
        {
            var clientWuModule = new DtoClientWuModule();
            var wuModule = new ServiceWuModule().GetModule(policyModule.ModuleId);
            clientWuModule.Guid = wuModule.Guid;
            clientWuModule.DisplayName = wuModule.Name;
            clientWuModule.Arguments = wuModule.AdditionalArguments;
            clientWuModule.Order = policyModule.Order;
            clientWuModule.Timeout = wuModule.Timeout;
            clientWuModule.RedirectOutput = wuModule.RedirectStdOut;
            clientWuModule.RedirectError = wuModule.RedirectStdError;
            foreach (var successCode in wuModule.SuccessCodes.Split(','))
                clientWuModule.SuccessCodes.Add(successCode);

            if (policyModule.ConditionId != -1)
            {
                var conditionScript = new ServiceScriptModule().GetModule(policyModule.ConditionId);
                if (conditionScript != null)
                {
                    clientWuModule.ConditionFailedAction = policyModule.ConditionFailedAction;
                    clientWuModule.ConditionNextOrder = policyModule.ConditionNextModule;
                    clientWuModule.Condition = new DtoClientModuleCondition();
                    clientWuModule.Condition.Arguments = conditionScript.Arguments;
                    clientWuModule.Condition.DisplayName = conditionScript.Name;
                    clientWuModule.Condition.Guid = conditionScript.Guid;
                    clientWuModule.Condition.RedirectError = conditionScript.RedirectStdError;
                    clientWuModule.Condition.RedirectOutput = conditionScript.RedirectStdOut;
                    if (conditionScript.ImpersonationId != -1)
                    {
                        var scriptImpersonationGuid = new ServiceImpersonationAccount().GetGuid(conditionScript.ImpersonationId);
                        if (!string.IsNullOrEmpty(scriptImpersonationGuid))
                            clientWuModule.Condition.RunAs = scriptImpersonationGuid;
                    }
                    clientWuModule.Condition.ScriptType = conditionScript.ScriptType;
                    foreach (var successCode in conditionScript.SuccessCodes.Split(','))
                        clientWuModule.Condition.SuccessCodes.Add(successCode);
                    clientWuModule.Condition.Timeout = conditionScript.Timeout;
                    clientWuModule.Condition.WorkingDirectory = conditionScript.WorkingDirectory;
                }

            }

            var moduleFiles = new ServiceModule().GetModuleFiles(wuModule.Guid);
            foreach (var file in moduleFiles.OrderBy(x => x.FileName))
            {
                var clientFile = new DtoClientFileHash();
                clientFile.FileName = file.FileName;
                clientFile.FileHash = file.Md5Hash;
                clientWuModule.Files.Add(clientFile);
            }

            _clientPolicy.WuModules.Add(clientWuModule);
        }
    }
}
