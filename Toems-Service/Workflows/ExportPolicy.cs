using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toems_Common.Dto;
using Toems_Common.Dto.client;
using Toems_Common.Dto.exports;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_Service.Entity;

namespace Toems_Service.Workflows
{
    public class ExportPolicy
    {
        private readonly ServicePolicy _policyService;
        private readonly DtoPolicyExport _policyExport;
        private EntityPolicy _policy;
        private readonly DtoModuleSearchFilter _filter;
        public ExportPolicy()
        {
            _policyService = new ServicePolicy();
            _filter = new DtoModuleSearchFilter();
            _policyExport = new DtoPolicyExport();
            _filter.IncludeCommand = true;
            _filter.IncludeFileCopy = true;
            _filter.IncludePrinter = true;
            _filter.IncludeScript = true;
            _filter.IncludeSoftware = true;
            _filter.IncludeWu = true;
            _filter.IncludeMessage = true;
            _filter.Limit = Int32.MaxValue;
        }

        public DtoPolicyExport Export(DtoPolicyExportGeneral exportInfo)
        {
            _policy = _policyService.GetPolicy(exportInfo.PolicyId);
            if (_policy == null) return null;

            var validationResult = new ValidatePolicy().Validate(exportInfo.PolicyId);
            if (!validationResult.Success)
            {
                return null;
            }

            CopyPolicy(exportInfo);

            var policyModules = _policyService.SearchAssignedPolicyModules(exportInfo.PolicyId, _filter);
            foreach (var policyModule in policyModules.OrderBy(x => x.Name))
            {
                if (policyModule.ModuleType == EnumModule.ModuleType.Command)
                {
                    CopyCommandModule(policyModule);
                }
                else if (policyModule.ModuleType == EnumModule.ModuleType.FileCopy)
                {
                    CopyFileCopyModule(policyModule);
                }
                else if (policyModule.ModuleType == EnumModule.ModuleType.Script)
                {
                    CopyScriptModule(policyModule);
                }
                else if (policyModule.ModuleType == EnumModule.ModuleType.Printer)
                {
                    CopyPrinterModule(policyModule);
                }
                else if (policyModule.ModuleType == EnumModule.ModuleType.Software)
                {
                    CopySoftwareModule(policyModule);
                }
                else if (policyModule.ModuleType == EnumModule.ModuleType.Wupdate)
                {
                    CopyWuModule(policyModule);
                }
                else if (policyModule.ModuleType == EnumModule.ModuleType.Message)
                {
                    CopyMessageModule(policyModule);
                }
            }

            return _policyExport;

        }

        private void CopyPolicy(DtoPolicyExportGeneral exportInfo)
        {
            _policyExport.Name = exportInfo.Name;
            _policyExport.Description = exportInfo.Description;
            _policyExport.Instructions = exportInfo.Instructions;
            _policyExport.Requirements = exportInfo.Requirements;
            _policyExport.Guid = _policy.Guid;
            _policyExport.Frequency = _policy.Frequency;
            _policyExport.Trigger = _policy.Trigger;
            _policyExport.SubFrequency = _policy.SubFrequency;
            _policyExport.CompletedAction = _policy.CompletedAction;
            _policyExport.RemoveInstallCache = _policy.RemoveInstallCache;
            _policyExport.ExecutionType = _policy.ExecutionType;
            _policyExport.ErrorAction = _policy.ErrorAction;
            _policyExport.IsInventory = _policy.RunInventory;
            _policyExport.IsLoginTracker = _policy.RunLoginTracker;
            _policyExport.FrequencyMissedAction = _policy.MissedAction;
            _policyExport.LogLevel = _policy.LogLevel;
            _policyExport.SkipServerResult = _policy.SkipServerResult;
            _policyExport.IsApplicationMonitor = _policy.RunApplicationMonitor;
            _policyExport.WuType = _policy.WuType;
            _policyExport.ConditionFailedAction = _policy.ConditionFailedAction;
            if(_policy.ConditionId != -1)
            {
                _policyExport.Condition = GetCondition(_policy.ConditionId);
            }
        }

        private void CopyCommandModule(EntityPolicyModules policyModule)
        {
            var commandModuleExport = new DtoCommandModuleExport();
            var commandModule = new ServiceCommandModule().GetModule(policyModule.ModuleId);
            commandModuleExport.Description = commandModule.Description;
            commandModuleExport.Order = policyModule.Order;
            commandModuleExport.Command = commandModule.Command;
            commandModuleExport.Arguments = commandModule.Arguments;
            commandModuleExport.DisplayName = commandModule.Name;
            commandModuleExport.Timeout = commandModule.Timeout;
            commandModuleExport.RedirectOutput = commandModule.RedirectStdOut;
            commandModuleExport.RedirectError = commandModule.RedirectStdError;
            commandModuleExport.WorkingDirectory = commandModule.WorkingDirectory;
            commandModuleExport.SuccessCodes = commandModule.SuccessCodes;
            commandModuleExport.Guid = commandModule.Guid;
            commandModuleExport.ConditionFailedAction = policyModule.ConditionFailedAction;
            commandModuleExport.ConditionNextOrder = policyModule.ConditionNextModule;

            var uploadedFiles = new ServiceUploadedFile().GetFilesForModule(commandModule.Guid);
            foreach (var file in uploadedFiles.OrderBy(x => x.Name))
            {
                var uploadedFile = new DtoUploadedFileExport();
                uploadedFile.FileName = file.Name;
                uploadedFile.Md5Hash = file.Hash;
                uploadedFile.ModuleGuid = file.Guid;
                commandModuleExport.UploadedFiles.Add(uploadedFile);
            }

            var externalFiles = new ServiceExternalDownload().GetForModule(commandModule.Guid);
            foreach (var file in externalFiles.OrderBy(x => x.FileName))
            {
                var externalFile = new DtoExternalFileExport();
                externalFile.FileName = file.FileName;
                externalFile.Sha256Hash = file.Sha256Hash;
                externalFile.Url = file.Url;
                externalFile.ModuleGuid = file.ModuleGuid;
                commandModuleExport.ExternalFiles.Add(externalFile);
            }

            if(policyModule.ConditionId != -1)
            {
                commandModuleExport.Condition = GetCondition(policyModule.ConditionId);
            }


            _policyExport.CommandModules.Add(commandModuleExport);
        }

        private void CopySoftwareModule(EntityPolicyModules policyModule)
        {
            var softwareModuleExport = new DtoSoftwareModuleExport();
            var softwareModule = new ServiceSoftwareModule().GetModule(policyModule.ModuleId);
            softwareModuleExport.DisplayName = softwareModule.Name;
            softwareModuleExport.Command = softwareModule.Command;
            softwareModuleExport.Description = softwareModule.Description;
            softwareModuleExport.Arguments = softwareModule.Arguments;
            softwareModuleExport.AdditionalArguments = softwareModule.AdditionalArguments;
            softwareModuleExport.Order = policyModule.Order;
            softwareModuleExport.Timeout = softwareModule.Timeout;
            softwareModuleExport.InstallType = softwareModule.InstallType;
            softwareModuleExport.RedirectOutput = softwareModule.RedirectStdOut;
            softwareModuleExport.RedirectError = softwareModule.RedirectStdError;
            softwareModuleExport.SuccessCodes = softwareModule.SuccessCodes;
            softwareModuleExport.Guid = softwareModule.Guid;
            softwareModuleExport.ConditionFailedAction = policyModule.ConditionFailedAction;
            softwareModuleExport.ConditionNextOrder = policyModule.ConditionNextModule;


            var uploadedFiles = new ServiceUploadedFile().GetFilesForModule(softwareModule.Guid);
            foreach (var file in uploadedFiles.OrderBy(x => x.Name))
            {
                var uploadedFile = new DtoUploadedFileExport();
                uploadedFile.FileName = file.Name;
                uploadedFile.Md5Hash = file.Hash;
                uploadedFile.ModuleGuid = file.Guid;
                softwareModuleExport.UploadedFiles.Add(uploadedFile);
            }

            var externalFiles = new ServiceExternalDownload().GetForModule(softwareModule.Guid);
            foreach (var file in externalFiles.OrderBy(x => x.FileName))
            {
                var externalFile = new DtoExternalFileExport();
                externalFile.FileName = file.FileName;
                externalFile.Sha256Hash = file.Sha256Hash;
                externalFile.Url = file.Url;
                externalFile.ModuleGuid = file.ModuleGuid;
                softwareModuleExport.ExternalFiles.Add(externalFile);
            }

            if (policyModule.ConditionId != -1)
            {
                softwareModuleExport.Condition = GetCondition(policyModule.ConditionId);
            }

            _policyExport.SoftwareModules.Add(softwareModuleExport);
        }

        private void CopyFileCopyModule(EntityPolicyModules policyModule)
        {
            var fileCopyModuleExport = new DtoFileCopyModuleExport();
            var fileCopyModule = new ServiceFileCopyModule().GetModule(policyModule.ModuleId);
            fileCopyModuleExport.DisplayName = fileCopyModule.Name;
            fileCopyModuleExport.Description = fileCopyModule.Description;
            fileCopyModuleExport.Destination = fileCopyModule.Destination;
            fileCopyModuleExport.Order = policyModule.Order;
            fileCopyModuleExport.Unzip = fileCopyModule.DecompressAfterCopy;
            fileCopyModuleExport.Guid = fileCopyModule.Guid;
            fileCopyModuleExport.ConditionFailedAction = policyModule.ConditionFailedAction;
            fileCopyModuleExport.ConditionNextOrder = policyModule.ConditionNextModule;

            var uploadedFiles = new ServiceUploadedFile().GetFilesForModule(fileCopyModule.Guid);
            foreach (var file in uploadedFiles.OrderBy(x => x.Name))
            {
                var uploadedFile = new DtoUploadedFileExport();
                uploadedFile.FileName = file.Name;
                uploadedFile.Md5Hash = file.Hash;
                uploadedFile.ModuleGuid = file.Guid;
                fileCopyModuleExport.UploadedFiles.Add(uploadedFile);
            }

            var externalFiles = new ServiceExternalDownload().GetForModule(fileCopyModule.Guid);
            foreach (var file in externalFiles.OrderBy(x => x.FileName))
            {
                var externalFile = new DtoExternalFileExport();
                externalFile.FileName = file.FileName;
                externalFile.Sha256Hash = file.Sha256Hash;
                externalFile.Url = file.Url;
                externalFile.ModuleGuid = file.ModuleGuid;
                fileCopyModuleExport.ExternalFiles.Add(externalFile);
            }

            if (policyModule.ConditionId != -1)
            {
                fileCopyModuleExport.Condition = GetCondition(policyModule.ConditionId);
            }

            _policyExport.FileCopyModules.Add(fileCopyModuleExport);
        }

        private void CopyScriptModule(EntityPolicyModules policyModule)
        {
            var scriptModuleExport = new DtoScriptModuleExport();
            var scriptModule = new ServiceScriptModule().GetModule(policyModule.ModuleId);

            scriptModuleExport.ScriptContents = scriptModule.ScriptContents;
            scriptModuleExport.Description = scriptModule.Description;
            scriptModuleExport.DisplayName = scriptModule.Name;
            scriptModuleExport.Arguments = scriptModule.Arguments;
            scriptModuleExport.Order = policyModule.Order;
            scriptModuleExport.Timeout = scriptModule.Timeout;
            scriptModuleExport.ScriptType = scriptModule.ScriptType;
            scriptModuleExport.RedirectOutput = scriptModule.RedirectStdOut;
            scriptModuleExport.RedirectError = scriptModule.RedirectStdError;
            scriptModuleExport.AddToInventory = scriptModule.AddInventoryCollection;
            scriptModuleExport.WorkingDirectory = scriptModule.WorkingDirectory;
            scriptModuleExport.IsCondition = scriptModule.IsCondition;
            scriptModuleExport.SuccessCodes = scriptModule.SuccessCodes;
            scriptModuleExport.Guid = scriptModule.Guid;
            scriptModuleExport.ConditionFailedAction = policyModule.ConditionFailedAction;
            scriptModuleExport.ConditionNextOrder = policyModule.ConditionNextModule;

            if (policyModule.ConditionId != -1)
            {
                scriptModuleExport.Condition = GetCondition(policyModule.ConditionId);
            }

            _policyExport.ScriptModules.Add(scriptModuleExport);
        }

        private void CopyMessageModule(EntityPolicyModules policyModule)
        {
            var messageModuleExport = new DtoMessageModuleExport();
            var messageModule = new ServiceMessageModule().GetModule(policyModule.ModuleId);

         
            messageModuleExport.Description = messageModule.Description;
            messageModuleExport.DisplayName = messageModule.Name;       
            messageModuleExport.Order = policyModule.Order;
            messageModuleExport.Timeout = messageModule.Timeout;    
            messageModuleExport.Guid = messageModule.Guid;
            messageModuleExport.Title = messageModule.Title;
            messageModuleExport.Message = messageModule.Message;
            messageModuleExport.ConditionFailedAction = policyModule.ConditionFailedAction;
            messageModuleExport.ConditionNextOrder = policyModule.ConditionNextModule;

            if (policyModule.ConditionId != -1)
            {
                messageModuleExport.Condition = GetCondition(policyModule.ConditionId);
            }

            _policyExport.MessageModules.Add(messageModuleExport);
        }

        private void CopyPrinterModule(EntityPolicyModules policyModule)
        {
            var printerModuleExport = new DtoPrinterModuleExport();
            var printerModule = new ServicePrinterModule().GetModule(policyModule.ModuleId);
            printerModuleExport.DisplayName = printerModule.Name;
            printerModuleExport.PrinterPath = printerModule.NetworkPath;
            printerModuleExport.Description = printerModule.Description;
            printerModuleExport.Order = policyModule.Order;
            printerModuleExport.IsDefault = printerModule.IsDefault;
            printerModuleExport.RestartSpooler = printerModule.RestartSpooler;
            printerModuleExport.PrinterAction = printerModule.Action;
            printerModuleExport.WaitForEnumeration = printerModule.WaitForEnumeration;
            printerModuleExport.Guid = printerModule.Guid;
            printerModuleExport.ConditionFailedAction = policyModule.ConditionFailedAction;
            printerModuleExport.ConditionNextOrder = policyModule.ConditionNextModule;

            if (policyModule.ConditionId != -1)
            {
                printerModuleExport.Condition = GetCondition(policyModule.ConditionId);
            }
            _policyExport.PrinterModules.Add(printerModuleExport);
        }

        private void CopyWuModule(EntityPolicyModules policyModule)
        {
            var wuModuleExport = new DtoWuModuleExport();
            var wuModule = new ServiceWuModule().GetModule(policyModule.ModuleId);
            wuModuleExport.DisplayName = wuModule.Name;
            wuModuleExport.Description = wuModule.Description;
            wuModuleExport.Arguments = wuModule.AdditionalArguments;
            wuModuleExport.Order = policyModule.Order;
            wuModuleExport.Timeout = wuModule.Timeout;
            wuModuleExport.RedirectOutput = wuModule.RedirectStdOut;
            wuModuleExport.RedirectError = wuModule.RedirectStdError;
            wuModuleExport.SuccessCodes = wuModule.SuccessCodes;
            wuModuleExport.Guid = wuModule.Guid;
            wuModuleExport.ConditionFailedAction = policyModule.ConditionFailedAction;
            wuModuleExport.ConditionNextOrder = policyModule.ConditionNextModule;

            var uploadedFiles = new ServiceUploadedFile().GetFilesForModule(wuModule.Guid);
            foreach (var file in uploadedFiles.OrderBy(x => x.Name))
            {
                var uploadedFile = new DtoUploadedFileExport();
                uploadedFile.FileName = file.Name;
                uploadedFile.Md5Hash = file.Hash;
                uploadedFile.ModuleGuid = file.Guid;
                wuModuleExport.UploadedFiles.Add(uploadedFile);
            }

            var externalFiles = new ServiceExternalDownload().GetForModule(wuModule.Guid);
            foreach (var file in externalFiles.OrderBy(x => x.FileName))
            {
                var externalFile = new DtoExternalFileExport();
                externalFile.FileName = file.FileName;
                externalFile.Sha256Hash = file.Sha256Hash;
                externalFile.Url = file.Url;
                externalFile.ModuleGuid = file.ModuleGuid;
                wuModuleExport.ExternalFiles.Add(externalFile);
            }

            if (policyModule.ConditionId != -1)
            {
                wuModuleExport.Condition = GetCondition(policyModule.ConditionId);
            }

            _policyExport.WuModules.Add(wuModuleExport);
        }

        private DtoScriptModuleExport GetCondition(int conditionScriptId)
        {
            var condition = new ServiceScriptModule().GetModule(conditionScriptId);
            if (condition == null) return null;

            var scriptModuleExport = new DtoScriptModuleExport();

            scriptModuleExport.ScriptContents = condition.ScriptContents;
            scriptModuleExport.Description = condition.Description;
            scriptModuleExport.DisplayName = condition.Name;
            scriptModuleExport.Arguments = condition.Arguments;
            scriptModuleExport.Timeout = condition.Timeout;
            scriptModuleExport.ScriptType = condition.ScriptType;
            scriptModuleExport.RedirectOutput = condition.RedirectStdOut;
            scriptModuleExport.RedirectError = condition.RedirectStdError;
            scriptModuleExport.AddToInventory = condition.AddInventoryCollection;
            scriptModuleExport.WorkingDirectory = condition.WorkingDirectory;
            scriptModuleExport.IsCondition = condition.IsCondition;
            scriptModuleExport.SuccessCodes = condition.SuccessCodes;
            scriptModuleExport.Guid = condition.Guid;

            return scriptModuleExport;
        }
    }
}
