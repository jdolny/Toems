using System;
using System.Linq;
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
            _clientPolicy.IsLoginTracker = _policy.RunLoginTracker;
            _clientPolicy.FrequencyMissedAction = _policy.MissedAction;
            _clientPolicy.LogLevel = _policy.LogLevel;
            _clientPolicy.SkipServerResult = _policy.SkipServerResult;
            _clientPolicy.IsApplicationMonitor = _policy.RunApplicationMonitor;
            _clientPolicy.StartWindowScheduleId = _policy.WindowStartScheduleId;
            _clientPolicy.EndWindowScheduleId = _policy.WindowEndScheduleId;
            _clientPolicy.WuType = _policy.WuType;
            _clientPolicy.PolicyComCondition = _policy.PolicyComCondition;
            _clientPolicy.Id = _policy.Id;
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
            var moduleFiles = new ServiceModule().GetModuleFiles(fileCopyModule.Guid);
            foreach (var file in moduleFiles.OrderBy(x => x.FileName))
            {
                var clientFile = new DtoClientFileHash();
                clientFile.FileName = file.FileName;
                clientFile.FileHash = file.Md5Hash;
                clientFileCopyModule.Files.Add(clientFile);
            }
            _clientPolicy.FileCopyModules.Add(clientFileCopyModule);
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
