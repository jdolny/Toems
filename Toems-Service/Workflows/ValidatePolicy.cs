using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Dto.exports;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_Service.Entity;

namespace Toems_Service.Workflows
{
    public class ValidatePolicy
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private DtoActionResult _result;
        private readonly ServicePolicy _policyService;
        private EntityPolicy _policy;
        private readonly DtoModuleSearchFilter _filter;

        public ValidatePolicy()
        {
            _result = new DtoActionResult();
            _policyService = new ServicePolicy();
            _filter = new DtoModuleSearchFilter();
            _filter.IncludeCommand = true;
            _filter.IncludeFileCopy = true;
            _filter.IncludePrinter = true;
            _filter.IncludeScript = true;
            _filter.IncludeSoftware = true;
            _filter.IncludeWu = true;
            _filter.IncludeMessage = true;
            _filter.IncludeWinPe = true;
            _filter.Limit = Int32.MaxValue;
        }

        public DtoActionResult Validate(int policyId)
        {
            _policy = _policyService.GetPolicy(policyId);
            if (_policy == null)
            {
                _result.Success = false;
                _result.ErrorMessage = "Could Not Find A Policy With Id: " + policyId;
                return _result;
            }

            string verifyResult;
            verifyResult = VerifyPolicy();
            if (verifyResult != null)
            {
                _result.Success = false;
                _result.ErrorMessage = verifyResult;
                return _result;
            }

            var policyModules = _policyService.SearchAssignedPolicyModules(policyId, _filter);
            foreach (var policyModule in policyModules.OrderBy(x => x.Name))
            {
                verifyResult = VerifyConditions(policyModule, policyModules);
                if (verifyResult != null)
                {
                    _result.Success = false;
                    _result.ErrorMessage = verifyResult;
                    return _result;
                }

                if (policyModule.ModuleType == EnumModule.ModuleType.Command)
                {
                    verifyResult = VerifyCommand(policyModule);
                    if (verifyResult != null)
                    {
                        _result.Success = false;
                        _result.ErrorMessage = verifyResult;
                        return _result;
                    }
                }
                else if (policyModule.ModuleType == EnumModule.ModuleType.FileCopy)
                {
                    verifyResult = VerifyFileCopy(policyModule);
                    if (verifyResult != null)
                    {
                        _result.Success = false;
                        _result.ErrorMessage = verifyResult;
                        return _result;
                    }
                }
                else if (policyModule.ModuleType == EnumModule.ModuleType.WinPE)
                {
                    verifyResult = VerifyWinPe(policyModule);
                    if (verifyResult != null)
                    {
                        _result.Success = false;
                        _result.ErrorMessage = verifyResult;
                        return _result;
                    }
                }
                else if (policyModule.ModuleType == EnumModule.ModuleType.Message)
                {
                    verifyResult = VerifyMessage(policyModule);
                    if (verifyResult != null)
                    {
                        _result.Success = false;
                        _result.ErrorMessage = verifyResult;
                        return _result;
                    }
                }
                else if (policyModule.ModuleType == EnumModule.ModuleType.Script)
                {
                    verifyResult = VerifyScript(policyModule);
                    if (verifyResult != null)
                    {
                        _result.Success = false;
                        _result.ErrorMessage = verifyResult;
                        return _result;
                    }
                }
                else if (policyModule.ModuleType == EnumModule.ModuleType.Printer)
                {
                    verifyResult = VerifyPrinter(policyModule);
                    if (verifyResult != null)
                    {
                        _result.Success = false;
                        _result.ErrorMessage = verifyResult;
                        return _result;
                    }
                }
                else if (policyModule.ModuleType == EnumModule.ModuleType.Software)
                {
                    verifyResult = VerifySoftware(policyModule);
                    if (verifyResult != null)
                    {
                        _result.Success = false;
                        _result.ErrorMessage = verifyResult;
                        return _result;
                    }
                }
                else if (policyModule.ModuleType == EnumModule.ModuleType.Wupdate)
                {
                    verifyResult = VerifyWindowsUpdate(policyModule);
                    if (verifyResult != null)
                    {
                        _result.Success = false;
                        _result.ErrorMessage = verifyResult;
                        return _result;
                    }
                }
            }

            _result.Success = true;
            return _result;
        }

        private string VerifyPolicy()
        {
            if (string.IsNullOrEmpty(_policy.Name))
                return "The Policy's Name Is Not Valid.";
            if (string.IsNullOrEmpty(_policy.Guid))
                return "The Policy's Guid Is Not Valid.";
            
            if (_policy.Frequency != EnumPolicy.Frequency.OncePerComputer &&
                _policy.Frequency != EnumPolicy.Frequency.OncePerDay
                && _policy.Frequency != EnumPolicy.Frequency.OncePerMonth &&
                _policy.Frequency != EnumPolicy.Frequency.OncePerUserPerComputer
                && _policy.Frequency != EnumPolicy.Frequency.OncePerWeek &&
                _policy.Frequency != EnumPolicy.Frequency.Ongoing &&
                _policy.Frequency != EnumPolicy.Frequency.EveryXdays &&
                _policy.Frequency != EnumPolicy.Frequency.EveryXhours)
                return "The Policy's Frequency Is Not Valid.";
            if (_policy.Trigger != EnumPolicy.Trigger.Checkin && _policy.Trigger != EnumPolicy.Trigger.Login &&
                _policy.Trigger != EnumPolicy.Trigger.Startup &&
                _policy.Trigger != EnumPolicy.Trigger.StartupOrCheckin)
                return "The Policy's Trigger Is Not Valid.";
            if (_policy.RunInventory != EnumPolicy.InventoryAction.After &&
                _policy.RunInventory != EnumPolicy.InventoryAction.Before &&
                _policy.RunInventory != EnumPolicy.InventoryAction.Both &&
                _policy.RunInventory != EnumPolicy.InventoryAction.Disabled)
                return "The Policy's Run Inventory Is Not Valid.";
            int value;
            if (!int.TryParse(_policy.SubFrequency.ToString(), out value))
                return "The Policy's Sub Frequency Is Not Valid.";
            if (_policy.CompletedAction != EnumPolicy.CompletedAction.DoNothing &&
                _policy.CompletedAction != EnumPolicy.CompletedAction.Reboot && _policy.CompletedAction != EnumPolicy.CompletedAction.RebootIfNoLogins)
                return "The Policy's Completed Action Is Not Valid.";
            if (_policy.ExecutionType != EnumPolicy.ExecutionType.Cache &&
                _policy.ExecutionType != EnumPolicy.ExecutionType.Install)
                return "The Policy's Execution Type Is Not Valid.";
            if (_policy.ErrorAction != EnumPolicy.ErrorAction.AbortCurrentPolicy &&
                _policy.ErrorAction != EnumPolicy.ErrorAction.AbortRemainingPolicies &&
                _policy.ErrorAction != EnumPolicy.ErrorAction.Continue)
                return "The Policy's Error Action Is Not Valid";
            if (_policy.MissedAction != EnumPolicy.FrequencyMissedAction.NextOpportunity &&
                _policy.MissedAction != EnumPolicy.FrequencyMissedAction.ScheduleDayOnly)
                return "The Policy's Frequency Missed Action Is Not Valid.";
            if (_policy.LogLevel != EnumPolicy.LogLevel.Full &&
                _policy.LogLevel != EnumPolicy.LogLevel.HiddenArguments &&
                _policy.LogLevel != EnumPolicy.LogLevel.None)
                return "The Policy's Log Level Is Not Valid.";
            if (_policy.WuType != EnumPolicy.WuType.Disabled && _policy.WuType != EnumPolicy.WuType.Microsoft &&
                _policy.WuType != EnumPolicy.WuType.MicrosoftSkipUpgrades &&
                _policy.WuType != EnumPolicy.WuType.Wsus && _policy.WuType != EnumPolicy.WuType.WsusSkipUpgrades)
                return "The Policy's Windows Update Type Is Not Valid.";
            if (_policy.AutoArchiveType != EnumPolicy.AutoArchiveType.AfterXdays &&
                _policy.AutoArchiveType != EnumPolicy.AutoArchiveType.None &&
                _policy.AutoArchiveType != EnumPolicy.AutoArchiveType.WhenComplete)
                return "The Policy's Auto Archive Type Is Not Valid.";

            if (_policy.AutoArchiveType == EnumPolicy.AutoArchiveType.AfterXdays)
            {
                int autoArchiveSub;
                if (!int.TryParse(_policy.AutoArchiveSub, out autoArchiveSub))
                    return "The Policy's Auto Archive Sub Is Not Valid.";
            }

            if (_policy.WindowStartScheduleId != -1)
            {
                var schedule = new ServiceSchedule().GetSchedule(_policy.WindowStartScheduleId);
                if(schedule == null) return "The Policy's Start Schedule Id Is Not Valid.";
            }
            if (_policy.WindowEndScheduleId != -1)
            {
                var schedule = new ServiceSchedule().GetSchedule(_policy.WindowEndScheduleId);
                if (schedule == null) return "The Policy's End Schedule Id Is Not Valid.";
            }

            if (_policy.PolicyComCondition != EnumPolicy.PolicyComCondition.Any &&
                _policy.PolicyComCondition != EnumPolicy.PolicyComCondition.Selective)
                return "The Policy's Com Server Condition Is Not Valid";
            if (_policy.PolicyComCondition == EnumPolicy.PolicyComCondition.Selective)
            {
                var policyComServers = _policyService.GetPolicyComServers(_policy.Id);
                if(policyComServers == null) return "The Policy's Selected Com Servers Are Not Valid";
                if (policyComServers.Count == 0) return "The Policy's Selected Com Servers Are Not Valid.  At Least One Server Must Be Selected.";
                foreach (var policyComServer in policyComServers)
                {
                    var comServer = new ServiceClientComServer().GetServer(policyComServer.ComServerId);
                    if(comServer == null) return "The Policy's Selected Com Servers Are Not Valid.  A Specified Com Server Does Not Exist";
                }
            }

            if(_policy.JoinDomain)
            {
                //get domain join creds
                var domainUser = ServiceSetting.GetSettingValue(SettingStrings.DomainJoinUser);
                if (string.IsNullOrEmpty(domainUser))
                    return $"The Policy {_policy.Name} Cannot Use Join Domain.  The Domain User Is Not Set.";
                if (!domainUser.Contains("\\"))
                    return $"The Policy {_policy.Name} Cannot Use Join Domain.  The Domain User Must Be in NetBIOS Format.  Domain\\User";

                var domainName = ServiceSetting.GetSettingValue(SettingStrings.DomainJoinName);
                if (string.IsNullOrEmpty(domainName))
                    return $"The Policy {_policy.Name} Cannot Use Join Domain.  The Domain Name Is Not Set.";

                try
                {
                    var domainPassword = new EncryptionServices().DecryptText(ServiceSetting.GetSettingValue(SettingStrings.DomainJoinPasswordEncrypted));
                    if(string.IsNullOrEmpty(domainPassword))
                        return $"The Policy {_policy.Name} Cannot Use Join Domain.  The Domain User Password Is Not Set.";
                }
                catch(Exception ex)
                {
                    Logger.Error(ex.Message);
                    return $"The Policy {_policy.Name} Cannot Use Join Domain.  The Domain User Password Could Not Be Decrypted.";
                }
            }

            if (_policy.ConditionId != -1) // -1 = disabled
            {
                var conditionScript = new ServiceScriptModule().GetModule(_policy.ConditionId);
                if (conditionScript == null)
                    return $"Condition Script For {_policy.Name} Does Not Exist";

                if (!conditionScript.IsCondition)
                    return $"The Condition Script For {_policy.Name} Is Not Currently Set As A Condition";

                if (string.IsNullOrEmpty(conditionScript.Name))
                    return $"A Condition Script For {_policy.Name} Has An Invalid Name";

                if (conditionScript.Archived)
                    return $"A Condition Script For {_policy.Name} Is Archived";

                if (string.IsNullOrEmpty(conditionScript.ScriptContents))
                    return "Condition Script: " + conditionScript.Name + " Has An Invalid Script.  It Cannot Be Empty.";

                if (string.IsNullOrEmpty(conditionScript.Guid))
                    return "Condition Script: " + conditionScript.Name + " Has An Invalid GUID";

                if (conditionScript.ScriptType != EnumScriptModule.ScriptType.Batch &&
                    conditionScript.ScriptType != EnumScriptModule.ScriptType.Powershell &&
                    conditionScript.ScriptType != EnumScriptModule.ScriptType.VbScript)
                    return "Condition Script: " + conditionScript.Name + " Has An Invalid Type";

                if (!int.TryParse(conditionScript.Timeout.ToString(), out value))
                    return "Condition Script: " + conditionScript.Name + " Has An Invalid Timeout";

                List<string> successCodes = new List<string>();
                foreach (var successCode in conditionScript.SuccessCodes.Split(','))
                    successCodes.Add(successCode);

                if (successCodes.Count == 0)
                    return "Condition Script: " + conditionScript.Name + " Has An Invalid Success Code";

                if (successCodes.Any(code => !int.TryParse(code, out value)))
                {
                    return "Condition Script: " + conditionScript.Name + " Has An Invalid Success Code";
                }

                if (!string.IsNullOrEmpty(conditionScript.WorkingDirectory))
                {
                    try
                    {
                        Path.GetFullPath(conditionScript.WorkingDirectory);
                    }
                    catch
                    {
                        return "Condition Script: " + conditionScript.Name + " Has An Invalid Working Directory";
                    }
                }

                if (conditionScript.ImpersonationId != -1)
                {
                    var impAccount = new ServiceImpersonationAccount().GetAccount(conditionScript.ImpersonationId);
                    if (impAccount == null) return "Condition Script: " + conditionScript.Name + " Has An Invalid Impersonation Account";
                }


                if (_policy.ConditionFailedAction != EnumCondition.FailedAction.MarkFailed
                    && _policy.ConditionFailedAction != EnumCondition.FailedAction.MarkNotApplicable && _policy.ConditionFailedAction != EnumCondition.FailedAction.MarkSkipped
                    && _policy.ConditionFailedAction != EnumCondition.FailedAction.MarkSuccess)
                    return $"The Condition Failed Action For {_policy.Name} Is Not Valid";
            }

                return null;
        }

        private string VerifyCommand(EntityPolicyModules policyModule)
        {
            var commandModule = new ServiceCommandModule().GetModule(policyModule.ModuleId);
            if (commandModule == null) return "An Assigned Command Module No Longer Exists";

            if (string.IsNullOrEmpty(commandModule.Name))
                return "A Command Module Has An Invalid Name";

            if (commandModule.Archived)
                return "Command Module: " + commandModule.Name + " Is Archived";

            if (string.IsNullOrEmpty(commandModule.Guid))
                return "Command Module: " + commandModule.Name + " Has An Invalid GUID";

            if (string.IsNullOrEmpty(commandModule.Command))
                return "Command Module: " + commandModule.Name + " Has An Invalid Command";

            int value;
            if (!int.TryParse(commandModule.Timeout.ToString(), out value))
                return "Command Module: " + commandModule.Name + " Has An Invalid Timeout";

            List<string> successCodes = new List<string>();
            foreach(var successCode in commandModule.SuccessCodes.Split(','))
            successCodes.Add(successCode);

            if (successCodes.Count == 0)
                return "Command Module: " + commandModule.Name + " Has An Invalid Success Code";

            if (successCodes.Any(code => !int.TryParse(code, out value)))
            {
                return "Command Module: " + commandModule.Name + " Has An Invalid Success Code";
            }

            if (!string.IsNullOrEmpty(commandModule.WorkingDirectory))
            {
                try
                {
                    Path.GetFullPath(commandModule.WorkingDirectory);
                }
                catch
                {
                    return "Command Module: " + commandModule.Name + " Has An Invalid Working Directory";
                }
            }

            if (commandModule.ImpersonationId != -1)
            {
                var impAccount = new ServiceImpersonationAccount().GetAccount(commandModule.ImpersonationId);
                if(impAccount == null) return "Command Module: " + commandModule.Name + " Has An Invalid Impersonation Account";
            }

            if (!int.TryParse(policyModule.Order.ToString(), out value))
                return "Command Module: " + commandModule.Name + " Has An Invalid Order";

            var uploadedFiles = new ServiceUploadedFile().GetFilesForModule(commandModule.Guid);
            var externalFiles = new ServiceExternalDownload().GetForModule(commandModule.Guid);

            var basePath = Path.Combine(ServiceSetting.GetSettingValue(SettingStrings.StoragePath), "software_uploads");
            using (var unc = new UncServices())
            {
                if (unc.NetUseWithCredentials() || unc.LastError == 1219)
                {
                    try
                    {
                        foreach (var file in uploadedFiles.OrderBy(x => x.Name))
                        {
                            if (string.IsNullOrEmpty(file.Hash))
                                return "Command Module: " + commandModule.Name + " " + file.Name + " Does Not Have An MD5 Hash";
                            var fullPath = Path.Combine(basePath, file.Guid, file.Name);
                            if (!File.Exists(fullPath))
                                return "Command Module: " + commandModule.Name + " " + fullPath + " Does Not Exist";
                        }


                        foreach (var file in externalFiles.OrderBy(x => x.FileName))
                        {
                            if (file.Status != EnumFileDownloader.DownloadStatus.Complete)
                                return "Command Module: " + commandModule.Name + " " + file.FileName + " Has Not Finished Downloading Or Is In An Error State";
                            if (string.IsNullOrEmpty(file.Md5Hash))
                                return "Command Module: " + commandModule.Name + " " + file.FileName + " Does Not Have An MD5 Hash";
                            var fullPath = Path.Combine(basePath, file.ModuleGuid, file.FileName);
                            if (!File.Exists(fullPath))
                                return "Command Module: " + commandModule.Name + " " + fullPath + " Does Not Exist";
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Debug(ex.Message);
                        return "Command Module: " + commandModule.Name + " Unknown Error Trying To Verify Files";
                    }
                }
                else
                {
                    return "Could Not Reach Storage Path";
                }
            }

            return null;
        }

        private string VerifyConditions(EntityPolicyModules policyModule, List<EntityPolicyModules> policyModules)
        {
            if (policyModule.ConditionId != -1) // -1 = disabled
            {
                var conditionScript = new ServiceScriptModule().GetModule(policyModule.ConditionId);
                if(conditionScript == null)
                    return $"Condition Script For {policyModule.Name} Does Not Exist";

                if (!conditionScript.IsCondition)
                    return $"The Condition Script For {policyModule.Name} Is Not Currently Set As A Condition";

                if (string.IsNullOrEmpty(conditionScript.Name))
                    return $"A Condition Script For {policyModule.Name} Has An Invalid Name";

                if (conditionScript.Archived)
                    return $"A Condition Script For {policyModule.Name} Is Archived";

                if (string.IsNullOrEmpty(conditionScript.ScriptContents))
                    return "Condition Script: " + conditionScript.Name + " Has An Invalid Script.  It Cannot Be Empty.";

                if (string.IsNullOrEmpty(conditionScript.Guid))
                    return "Condition Script: " + conditionScript.Name + " Has An Invalid GUID";

                if (conditionScript.ScriptType != EnumScriptModule.ScriptType.Batch &&
                    conditionScript.ScriptType != EnumScriptModule.ScriptType.Powershell &&
                    conditionScript.ScriptType != EnumScriptModule.ScriptType.VbScript)
                    return "Condition Script: " + conditionScript.Name + " Has An Invalid Type";
                int value;
                if (!int.TryParse(conditionScript.Timeout.ToString(), out value))
                    return "Condition Script: " + conditionScript.Name + " Has An Invalid Timeout";

                List<string> successCodes = new List<string>();
                foreach (var successCode in conditionScript.SuccessCodes.Split(','))
                    successCodes.Add(successCode);

                if (successCodes.Count == 0)
                    return "Condition Script: " + conditionScript.Name + " Has An Invalid Success Code";

                if (successCodes.Any(code => !int.TryParse(code, out value)))
                {
                    return "Condition Script: " + conditionScript.Name + " Has An Invalid Success Code";
                }

                if (!string.IsNullOrEmpty(conditionScript.WorkingDirectory))
                {
                    try
                    {
                        Path.GetFullPath(conditionScript.WorkingDirectory);
                    }
                    catch
                    {
                        return "Condition Script: " + conditionScript.Name + " Has An Invalid Working Directory";
                    }
                }

                if (conditionScript.ImpersonationId != -1)
                {
                    var impAccount = new ServiceImpersonationAccount().GetAccount(conditionScript.ImpersonationId);
                    if (impAccount == null) return "Condition Script: " + conditionScript.Name + " Has An Invalid Impersonation Account";
                }


                if (policyModule.ConditionFailedAction != EnumCondition.FailedAction.GotoModule && policyModule.ConditionFailedAction != EnumCondition.FailedAction.MarkFailed
                    && policyModule.ConditionFailedAction != EnumCondition.FailedAction.MarkNotApplicable && policyModule.ConditionFailedAction != EnumCondition.FailedAction.MarkSkipped
                    && policyModule.ConditionFailedAction != EnumCondition.FailedAction.MarkSuccess)
                    return $"The Condition Failed Action For {policyModule.Name} Is Not Valid";

                if (policyModule.ConditionFailedAction == EnumCondition.FailedAction.GotoModule)
                {

                    if (!int.TryParse(policyModule.ConditionNextModule.ToString(), out value))
                        return "Module: " + policyModule.Name + " Has An Invalid Next Order Number";

                    if(policyModule.ConditionNextModule <= policyModule.Order)
                        return "Module: " + policyModule.Name + " The Goto Order Number Must Be Greater Than Itself";

                    if(!policyModules.Any(x => x.Order == policyModule.ConditionNextModule))
                        return "Module: " + policyModule.Name + " The Goto Order Number Is Not Assigned To Any Modules.";

                }

            }
            return null;
        }

        private string VerifyFileCopy(EntityPolicyModules policyModule)
        {
            var fileCopyModule = new ServiceFileCopyModule().GetModule(policyModule.ModuleId);

            if (string.IsNullOrEmpty(fileCopyModule.Name))
                return "A File Copy Module Has An Invalid Name";

            if (fileCopyModule.Archived)
                return "File Copy Module: " + fileCopyModule.Name + " Is Archived";

            if (string.IsNullOrEmpty(fileCopyModule.Guid))
                return "File Copy Module: " + fileCopyModule.Name + " Has An Invalid GUID";

            if (string.IsNullOrEmpty(fileCopyModule.Destination))
                return "File Copy Module: " + fileCopyModule.Name + " Has An Invalid Destination";

           
            int value;
            if (!int.TryParse(policyModule.Order.ToString(), out value))
                return "File Copy Module: " + fileCopyModule.Name + " Has An Invalid Order";

            var uploadedFiles = new ServiceUploadedFile().GetFilesForModule(fileCopyModule.Guid);
            var externalFiles = new ServiceExternalDownload().GetForModule(fileCopyModule.Guid);

            if(uploadedFiles == null && externalFiles == null)
                return "File Copy Module: " + fileCopyModule.Name + " Does Not Have Any Associated Files";

            try
            {
                if (uploadedFiles.Count == 0 && externalFiles.Count == 0)
                    return "File Copy Module: " + fileCopyModule.Name + " Does Not Have Any Associated Files";
            }
            catch
            {
                return "File Copy Module: " + fileCopyModule.Name + " Error While Determining Associated Files";
            }

            var basePath = Path.Combine(ServiceSetting.GetSettingValue(SettingStrings.StoragePath), "software_uploads");         
            using (var unc = new UncServices())
            {
                if (unc.NetUseWithCredentials() || unc.LastError == 1219)
                {
                    try
                    {
                        foreach (var file in uploadedFiles.OrderBy(x => x.Name))
                        {
                            if(string.IsNullOrEmpty(file.Hash))
                                return "File Copy Module: " + fileCopyModule.Name + " " + file.Name + " Does Not Have An MD5 Hash";
                            var fullPath = Path.Combine(basePath, file.Guid, file.Name);
                            if(!File.Exists(fullPath))
                                return "File Copy Module: " + fileCopyModule.Name + " " + fullPath + " Does Not Exist";
                        }


                        foreach (var file in externalFiles.OrderBy(x => x.FileName))
                        {
                            if(file.Status != EnumFileDownloader.DownloadStatus.Complete)
                                return "File Copy Module: " + fileCopyModule.Name + " " + file.FileName + " Has Not Finished Downloading Or Is In An Error State";
                            if (string.IsNullOrEmpty(file.Md5Hash))
                                return "File Copy Module: " + fileCopyModule.Name + " " + file.FileName + " Does Not Have An MD5 Hash";
                            var fullPath = Path.Combine(basePath, file.ModuleGuid, file.FileName);
                            if (!File.Exists(fullPath))
                                return "File Copy Module: " + fileCopyModule.Name + " " + fullPath + " Does Not Exist";
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Debug(ex.Message);
                        return "File Copy Module: " + fileCopyModule.Name + " Unknown Error Trying To Verify Files";
                    }
                }
                else
                {
                    return "Could Not Reach Storage Path";
                }
            }
        
            return null;
        }

        private string VerifyWinPe(EntityPolicyModules policyModule)
        {
            var winPeModule = new ServiceWinPeModule().GetModule(policyModule.ModuleId);

            if (string.IsNullOrEmpty(winPeModule.Name))
                return "A WinPE Module Has An Invalid Name";

            if (winPeModule.Archived)
                return "WinPE Module: " + winPeModule.Name + " Is Archived";

            if (string.IsNullOrEmpty(winPeModule.Guid))
                return "WinPE Module: " + winPeModule.Name + " Has An Invalid GUID";

            int value;
            if (!int.TryParse(policyModule.Order.ToString(), out value))
                return "WinPE Module: " + winPeModule.Name + " Has An Invalid Order";

            var uploadedFiles = new ServiceUploadedFile().GetFilesForModule(winPeModule.Guid);

            if (uploadedFiles == null)
                return "WinPE Module: " + winPeModule.Name + " Does Not Have Any Associated Files";

            try
            {
                if (uploadedFiles.Count == 0)
                    return "WinPE Module: " + winPeModule.Name + " Does Not Have Any Associated Files";
            }
            catch
            {
                return "WinPE Module: " + winPeModule.Name + " Error While Determining Associated Files";
            }

            var basePath = Path.Combine(ServiceSetting.GetSettingValue(SettingStrings.StoragePath), "software_uploads");
            using (var unc = new UncServices())
            {
                if (unc.NetUseWithCredentials() || unc.LastError == 1219)
                {
                    try
                    {
                        foreach (var file in uploadedFiles.OrderBy(x => x.Name))
                        {
                            if (string.IsNullOrEmpty(file.Hash))
                                return "WinPE Module: " + winPeModule.Name + " " + file.Name + " Does Not Have An MD5 Hash";
                            var fullPath = Path.Combine(basePath, file.Guid, file.Name);
                            if (!File.Exists(fullPath))
                                return "WinPE Module: " + winPeModule.Name + " " + fullPath + " Does Not Exist";
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Debug(ex.Message);
                        return "File Copy Module: " + winPeModule.Name + " Unknown Error Trying To Verify Files";
                    }
                }
                else
                {
                    return "Could Not Reach Storage Path";
                }
            }

            return null;
        }
        private string VerifyMessage(EntityPolicyModules policyModule)
        {
            var messageModule = new ServiceMessageModule().GetModule(policyModule.ModuleId);

            if (string.IsNullOrEmpty(messageModule.Name))
                return "A Message Module Has An Invalid Name";

            if (messageModule.Archived)
                return "Message Module: " + messageModule.Name + " Is Archived";

            if (string.IsNullOrEmpty(messageModule.Guid))
                return "Message Module: " + messageModule.Name + " Has An Invalid GUID";

            if (string.IsNullOrEmpty(messageModule.Title))
                return "Message Module: " + messageModule.Name + " Has An Invalid Title";

            if (string.IsNullOrEmpty(messageModule.Message))
                return "Message Module: " + messageModule.Name + " Has An Invalid Message";


            int value;
            if (!int.TryParse(policyModule.Order.ToString(), out value))
                return "Message Module: " + messageModule.Name + " Has An Invalid Order";

            int tValue;
            if (!int.TryParse(messageModule.Timeout.ToString(), out tValue))
                return "Message Module: " + messageModule.Name + " Has An Invalid Timeout";

            return null;
        }

        private string VerifyScript(EntityPolicyModules policyModule)
        {
            var scriptModule = new ServiceScriptModule().GetModule(policyModule.ModuleId);
            if (scriptModule == null) return "An Assigned Script Module No Longer Exists";

            if (string.IsNullOrEmpty(scriptModule.Name))
                return "A Script Module Has An Invalid Name";

            if (scriptModule.Archived)
                return "Script Module: " + scriptModule.Name + " Is Archived";

            if (string.IsNullOrEmpty(scriptModule.ScriptContents))
                return "Script Module: " + scriptModule.Name + " Has An Invalid Script.  It Cannot Be Empty.";

            if (string.IsNullOrEmpty(scriptModule.Guid))
                return "Script Module: " + scriptModule.Name + " Has An Invalid GUID";

            if (scriptModule.ScriptType != EnumScriptModule.ScriptType.Batch &&
                scriptModule.ScriptType != EnumScriptModule.ScriptType.Powershell &&
                scriptModule.ScriptType != EnumScriptModule.ScriptType.VbScript)
                return "Script Module: " + scriptModule.Name + " Has An Invalid Type";
            int value;
            if (!int.TryParse(scriptModule.Timeout.ToString(), out value))
                return "Script Module: " + scriptModule.Name + " Has An Invalid Timeout";

            List<string> successCodes = new List<string>();
            foreach (var successCode in scriptModule.SuccessCodes.Split(','))
                successCodes.Add(successCode);

            if (successCodes.Count == 0)
                return "Script Module: " + scriptModule.Name + " Has An Invalid Success Code";

            if (successCodes.Any(code => !int.TryParse(code, out value)))
            {
                return "Script Module: " + scriptModule.Name + " Has An Invalid Success Code";
            }

            if (!string.IsNullOrEmpty(scriptModule.WorkingDirectory))
            {
                try
                {
                    Path.GetFullPath(scriptModule.WorkingDirectory);
                }
                catch
                {
                    return "Script Module: " + scriptModule.Name + " Has An Invalid Working Directory";
                }
            }

            if (scriptModule.ImpersonationId != -1)
            {
                var impAccount = new ServiceImpersonationAccount().GetAccount(scriptModule.ImpersonationId);
                if (impAccount == null) return "Script Module: " + scriptModule.Name + " Has An Invalid Impersonation Account";
            }

            if (!int.TryParse(policyModule.Order.ToString(), out value))
                return "Script Module: " + scriptModule.Name + " Has An Invalid Order";
            return null;

        }

        private string VerifyPrinter(EntityPolicyModules policyModule)
        {
            var printerModule = new ServicePrinterModule().GetModule(policyModule.ModuleId);

            if (printerModule == null) return "An Assigned Printer Module No Longer Exists";

            if (string.IsNullOrEmpty(printerModule.Name))
                return "A Printer Module Has An Invalid Name";

            if (printerModule.Archived)
                return "Printer Module: " + printerModule.Name + " Is Archived";

            if (string.IsNullOrEmpty(printerModule.NetworkPath))
                return "Printer Module: " + printerModule.Name + " Has An Invalid Network Path";

            if (string.IsNullOrEmpty(printerModule.Guid))
                return "Printer Module: " + printerModule.Name + " Has An Invalid GUID";

            if (printerModule.Action != EnumPrinterModule.ActionType.Delete &&
                printerModule.Action != EnumPrinterModule.ActionType.Install &&
                printerModule.Action != EnumPrinterModule.ActionType.InstallPowershell &&
                printerModule.Action != EnumPrinterModule.ActionType.None)
                return "Printer Module: " + printerModule.Name + " Has An Invalid Action";
            int value;
            if (!int.TryParse(policyModule.Order.ToString(), out value))
                return "Printer Module: " + printerModule.Name + " Has An Invalid Order";

            return null;
        }

        private string VerifySoftware(EntityPolicyModules policyModule)
        {
            var softwareModule = new ServiceSoftwareModule().GetModule(policyModule.ModuleId);
            if (string.IsNullOrEmpty(softwareModule.Name))
                return "A Software Module Has An Invalid Name";

            if (softwareModule.Archived)
                return "Software Module: " + softwareModule.Name + " Is Archived";

            if (string.IsNullOrEmpty(softwareModule.Guid))
                return "Software Module: " + softwareModule.Name + " Has An Invalid GUID";

            int value;
            if (!int.TryParse(softwareModule.Timeout.ToString(), out value))
                return "Software Module: " + softwareModule.Name + " Has An Invalid Timeout";

            if (string.IsNullOrEmpty(softwareModule.Command))
                return "Software Module: " + softwareModule.Name + " Has An Invalid Command";

            if (!int.TryParse(policyModule.Order.ToString(), out value))
                return "Software Module: " + softwareModule.Name + " Has An Invalid Order";

            List<string> successCodes = new List<string>();
            foreach (var successCode in softwareModule.SuccessCodes.Split(','))
                successCodes.Add(successCode);

            if (successCodes.Count == 0)
                return "Software Module: " + softwareModule.Name + " Has An Invalid Success Code";

            if (successCodes.Any(code => !int.TryParse(code, out value)))
            {
                return "Software Module: " + softwareModule.Name + " Has An Invalid Success Code";
            }

            var uploadedFiles = new ServiceUploadedFile().GetFilesForModule(softwareModule.Guid);
            var externalFiles = new ServiceExternalDownload().GetForModule(softwareModule.Guid);

            if (uploadedFiles == null && externalFiles == null)
                return "Software Module: " + softwareModule.Name + " Does Not Have Any Associated Files";

            try
            {
                if (uploadedFiles.Count == 0 && externalFiles.Count == 0)
                    return "Software Module: " + softwareModule.Name + " Does Not Have Any Associated Files";
            }
            catch
            {
                return "Software Module: " + softwareModule.Name + " Error While Determining Associated Files";
            }

            var basePath = Path.Combine(ServiceSetting.GetSettingValue(SettingStrings.StoragePath), "software_uploads");
            using (var unc = new UncServices())
            {
                if (unc.NetUseWithCredentials() || unc.LastError == 1219)
                {
                    try
                    {
                        foreach (var file in uploadedFiles.OrderBy(x => x.Name))
                        {
                            if (string.IsNullOrEmpty(file.Hash))
                                return "Software Module: " + softwareModule.Name + " " + file.Name + " Does Not Have An MD5 Hash";
                            var fullPath = Path.Combine(basePath, file.Guid, file.Name);
                            if (!File.Exists(fullPath))
                                return "Software Module: " + softwareModule.Name + " " + fullPath + " Does Not Exist";
                        }


                        foreach (var file in externalFiles.OrderBy(x => x.FileName))
                        {
                            if (file.Status != EnumFileDownloader.DownloadStatus.Complete)
                                return "Software Module: " + softwareModule.Name + " " + file.FileName + " Has Not Finished Downloading Or Is In An Error State";
                            if (string.IsNullOrEmpty(file.Md5Hash))
                                return "Software Module: " + softwareModule.Name + " " + file.FileName + " Does Not Have An MD5 Hash";
                            var fullPath = Path.Combine(basePath, file.ModuleGuid, file.FileName);
                            if (!File.Exists(fullPath))
                                return "Software Module: " + softwareModule.Name + " " + fullPath + " Does Not Exist";
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Debug(ex.Message);
                        return "Software Module: " + softwareModule.Name + " Unknown Error Trying To Verify Files";
                    }
                }
                else
                {
                    return "Could Not Reach Storage Path";
                }
            }


            if (softwareModule.ImpersonationId != -1)
            {
                var impAccount = new ServiceImpersonationAccount().GetAccount(softwareModule.ImpersonationId);
                if (impAccount == null) return "Software Module: " + softwareModule.Name + " Has An Invalid Impersonation Account";
            }

            return null;

        }

        private string VerifyWindowsUpdate(EntityPolicyModules policyModule)
        {
            var wuModule = new ServiceWuModule().GetModule(policyModule.ModuleId);
            if (string.IsNullOrEmpty(wuModule.Name))
                return "A Windows Update Module Has An Invalid Name";
            if (wuModule.Archived)
                return "Windows Update Module: " + wuModule.Name + " Is Archived";
            if (string.IsNullOrEmpty(wuModule.Guid))
                return "Windows Update Module: " + wuModule.Name + " Has An Invalid GUID";

            int value;
            if (!int.TryParse(wuModule.Timeout.ToString(), out value))
                return "Windows Update Module: " + wuModule.Name + " Has An Invalid Timeout";

            if (!int.TryParse(policyModule.Order.ToString(), out value))
                return "Windows Update Module: " + wuModule.Name + " Has An Invalid Order";

            List<string> successCodes = new List<string>();
            foreach (var successCode in wuModule.SuccessCodes.Split(','))
                successCodes.Add(successCode);

            if (successCodes.Count == 0)
                return "Windows Update Module: " + wuModule.Name + " Has An Invalid Success Code";

            if (successCodes.Any(code => !int.TryParse(code, out value)))
            {
                return "Windows Update Module: " + wuModule.Name + " Has An Invalid Success Code";
            }

            var uploadedFiles = new ServiceUploadedFile().GetFilesForModule(wuModule.Guid);
            var externalFiles = new ServiceExternalDownload().GetForModule(wuModule.Guid);

            if (uploadedFiles == null && externalFiles == null)
                return "Windows Update Module: " + wuModule.Name + " Does Not Have Any Associated Files";

            try
            {
                if (uploadedFiles.Count == 0 && externalFiles.Count == 0)
                    return "Windows Update Module: " + wuModule.Name + " Does Not Have Any Associated Files";
            }
            catch
            {
                return "Windows Update Module: " + wuModule.Name + " Error While Determining Associated Files";
            }

            var firstExtension = "";
            int uploadFileCounter = 0;
            foreach (var file in uploadedFiles)
            {
                var ext = Path.GetExtension(file.Name);
                if(ext == null)
                    return "Windows Update Module: " + wuModule.Name + " Has An Invalid File Extension In The Uploaded Files List";
                if(!ext.ToLower().Equals(".cab") && !ext.ToLower().Equals(".msu"))
                    return "Windows Update Module: " + wuModule.Name + " Has An Invalid File Extension In The Uploaded Files List";
                if (uploadFileCounter == 0)
                    firstExtension = ext;
                else
                {
                    if(!firstExtension.Equals(ext))
                        return "Windows Update Module: " + wuModule.Name + " All Files Per Windows Update Module Must Be The Same Type.  IE. All .msu Or All .cab";
                }

                uploadFileCounter++;
            }

            int externalFileCounter = 0;
            foreach (var file in externalFiles)
            {
                var ext = Path.GetExtension(file.FileName);
                if (ext == null)
                    return "Windows Update Module: " + wuModule.Name + " Has An Invalid File Extension In The External Files List";
                if (!ext.ToLower().Equals(".cab") && !ext.ToLower().Equals(".msu"))
                    return "Windows Update Module: " + wuModule.Name + " Has An Invalid File Extension In The External Files List";
                if (uploadFileCounter == 0 && string.IsNullOrEmpty(firstExtension)) //don't overwrite extension that may have been set in uploaded files
                    firstExtension = ext;
                else
                {
                    if (!firstExtension.Equals(ext))
                        return "Windows Update Module: " + wuModule.Name + " All Files Per Windows Update Module Must Be The Same Type.  IE. All .msu Or All .cab";
                }

                externalFileCounter++;
            }



            var basePath = Path.Combine(ServiceSetting.GetSettingValue(SettingStrings.StoragePath), "software_uploads");
            using (var unc = new UncServices())
            {
                if (unc.NetUseWithCredentials() || unc.LastError == 1219)
                {
                    try
                    {
                        foreach (var file in uploadedFiles.OrderBy(x => x.Name))
                        {
                            if (string.IsNullOrEmpty(file.Hash))
                                return "Windows Update Module: " + wuModule.Name + " " + file.Name + " Does Not Have An MD5 Hash";
                            var fullPath = Path.Combine(basePath, file.Guid, file.Name);
                            if (!File.Exists(fullPath))
                                return "Windows Update Module: " + wuModule.Name + " " + fullPath + " Does Not Exist";
                        }


                        foreach (var file in externalFiles.OrderBy(x => x.FileName))
                        {
                            if (file.Status != EnumFileDownloader.DownloadStatus.Complete)
                                return "Windows Update Module: " + wuModule.Name + " " + file.FileName + " Has Not Finished Downloading Or Is In An Error State";
                            if (string.IsNullOrEmpty(file.Md5Hash))
                                return "Windows Update Module: " + wuModule.Name + " " + file.FileName + " Does Not Have An MD5 Hash";
                            var fullPath = Path.Combine(basePath, file.ModuleGuid, file.FileName);
                            if (!File.Exists(fullPath))
                                return "Windows Update Module: " + wuModule.Name + " " + fullPath + " Does Not Exist";
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Debug(ex.Message);
                        return "Windows Update Module: " + wuModule.Name + " Unknown Error Trying To Verify Files";
                    }
                }
                else
                {
                    return "Could Not Reach Storage Path";
                }
            }

            return null;
        }
    }
}
