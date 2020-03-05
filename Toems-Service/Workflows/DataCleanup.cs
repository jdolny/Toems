using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using Toems_Common;
using Toems_Common.Enum;
using Toems_DataModel;
using Toems_Service.Entity;

namespace Toems_Service.Workflows
{
    public class DataCleanup
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private UnitOfWork _uow;
        private DateTime _localCutoff;
        private DateTime _utcCutoff;
        public DataCleanup()
        {
            _uow = new UnitOfWork();
            _localCutoff = DateTime.Now - TimeSpan.FromDays(10950); //30 years, to avoid dates of 01/01/0001
            _utcCutoff = DateTime.UtcNow - TimeSpan.FromDays(10950); //30 years, to avoid dates of 01/01/0001
        }

        public bool Run()
        {
            Logger.Info("Running Data Cleanup Job");
            AutoArchiveComputers();
            AutoArchivePolicies();
            AutoDeleteComputers();
            AutoDeleteAuditLogs();
            ComputerProcessHistory();
            PolicyHistory();
            UserLogins();
            ImagingLogs();
            Logger.Info("Completed Data Cleanup Job");
            return true;
        }

        private void AutoArchiveComputers()
        {
            Logger.Debug("Computer Archive Started");
            var serviceComputer = new ServiceComputer();
            var archiveDays = ServiceSetting.GetSettingValue(SettingStrings.ComputerAutoArchiveDays);
            int intArchiveDays;
            if (!int.TryParse(archiveDays, out intArchiveDays))
                return;

            if (intArchiveDays <= 0) return;

            Logger.Debug($"Archiving Computers Older Than {intArchiveDays} Days");

            var activeComputers =
                _uow.ComputerRepository.Get(x => x.ProvisionStatus == EnumProvisionStatus.Status.Provisioned);
            if (activeComputers.Count == 0) return;

            var dateCutOff = DateTime.Now - TimeSpan.FromDays(intArchiveDays);
            foreach (var computer in activeComputers)
            {
                if (computer.LastCheckinTime <= dateCutOff && computer.LastCheckinTime > _localCutoff)
                    serviceComputer.ArchiveComputer(computer.Id);
            }
        }

        private void AutoDeleteComputers()
        {
            Logger.Debug("Computer Delete Started");
            var serviceComputer = new ServiceComputer();
            var deleteDays = ServiceSetting.GetSettingValue(SettingStrings.ComputerAutoDelete);
            int intDays;
            if (!int.TryParse(deleteDays, out intDays))
                return;

            if (intDays <= 0) return;

            Logger.Debug($"Deleting Archived Computers Older Than {intDays} Days");
            var archivedComputers =
                _uow.ComputerRepository.Get(x => x.ProvisionStatus == EnumProvisionStatus.Status.Archived);
            if (archivedComputers.Count == 0) return;

            var dateCutOff = DateTime.Now - TimeSpan.FromDays(intDays);
            foreach (var computer in archivedComputers)
            {
                if (computer.ArchiveDateTime <= dateCutOff && computer.ArchiveDateTime > _localCutoff)
                    serviceComputer.DeleteComputer(computer.Id);
            }
        }

        private void AutoDeleteAuditLogs()
        {
            Logger.Debug("Audit Log Delete Started");
            var deleteDays = ServiceSetting.GetSettingValue(SettingStrings.AuditLogAutoDelete);
            int intDays;
            if (!int.TryParse(deleteDays, out intDays))
                return;
            if (intDays <= 0) return;
            Logger.Debug($"Deleting Audit Logs Older Than {intDays} Days");

            var dateCutOff = DateTime.Now - TimeSpan.FromDays(intDays);
            _uow.AuditLogRepository.DeleteRange(x => x.DateTime < dateCutOff && x.DateTime > _localCutoff);
            _uow.Save();
        }

        private void ComputerProcessHistory()
        {
            Logger.Debug("Computer Process History Delete Started");
            var deleteDays = ServiceSetting.GetSettingValue(SettingStrings.ComputerProcessAutoDelete);
            int intDays;
            if (!int.TryParse(deleteDays, out intDays))
                return;
            if (intDays <= 0) return;
            Logger.Debug($"Deleting Process History Older Than {intDays} Days");
            var dateCutOff = DateTime.UtcNow - TimeSpan.FromDays(intDays);
            _uow.ComputerProcessRepository.DeleteRange(x => x.StartTimeUtc < dateCutOff && x.StartTimeUtc > _utcCutoff);
            _uow.Save();
        }

        private void PolicyHistory()
        {
            Logger.Debug("Policy History Delete Started");
            var deleteDays = ServiceSetting.GetSettingValue(SettingStrings.PolicyHistoryAutoDelete);
            int intDays;
            if (!int.TryParse(deleteDays, out intDays))
                return;
            if (intDays <= 0) return;
            Logger.Debug($"Deleting Policy History Older Than {intDays} Days");
            var dateCutOff = DateTime.UtcNow - TimeSpan.FromDays(intDays);
            _uow.PolicyHistoryRepository.DeleteRange(x => x.LastRunTime < dateCutOff && x.LastRunTime > _utcCutoff);
            _uow.Save();
        }

        private void UserLogins()
        {
            Logger.Debug("User Login Delete Started");
            var deleteDays = ServiceSetting.GetSettingValue(SettingStrings.UserLoginHistoryAutoDelete);
            int intDays;
            if (!int.TryParse(deleteDays, out intDays))
                return;
            if (intDays <= 0) return;
            Logger.Debug($"Deleting Policy History Older Than {intDays} Days");
            var dateCutOff = DateTime.UtcNow - TimeSpan.FromDays(intDays);
            _uow.UserLoginRepository.DeleteRange(x => x.LoginDateTime < dateCutOff && x.LoginDateTime > _utcCutoff);
            _uow.Save();
        }

        private void ImagingLogs()
        {
            Logger.Debug("Imaging Logs Delete Started");
            var deleteDays = ServiceSetting.GetSettingValue(SettingStrings.ImagingLogsAutoDeleteDays);
            int intDays;
            if (!int.TryParse(deleteDays, out intDays))
                return;
            if (intDays <= 0) return;
            Logger.Debug($"Deleting Imaging Logs Older Than {intDays} Days");
            var dateCutOff = DateTime.Now - TimeSpan.FromDays(intDays);
            _uow.ComputerLogRepository.DeleteRange(x => x.LogTime < dateCutOff && x.LogTime > _localCutoff && !x.SubType.Equals("ondupload") && !x.SubType.Equals("upload") && !x.SubType.Equals("unregupload")); //don't delete upload logs
            _uow.Save();
        }

        public void AutoArchivePolicies()
        {
            Logger.Debug("Policy Archive Started");
            var servicePolicy = new ServicePolicy();
            var allPolicies = _uow.PolicyRepository.Get(x => !x.Archived);
            foreach (var policy in allPolicies)
            {
                if (policy.AutoArchiveType == EnumPolicy.AutoArchiveType.AfterXdays)
                {
                    int intArchiveDays;
                    if (!int.TryParse(policy.AutoArchiveSub, out intArchiveDays))
                        continue;

                    if (intArchiveDays <= 0) continue;

                    var dateCutOff = DateTime.UtcNow - TimeSpan.FromDays(intArchiveDays);

                    if (policy.StartDate < dateCutOff && policy.StartDate > _utcCutoff)
                    {
                        servicePolicy.DeactivatePolicy(policy.Id);
                        servicePolicy.ArchivePolicy(policy.Id);
                    }
                }
                else if (policy.AutoArchiveType == EnumPolicy.AutoArchiveType.WhenComplete)
                {
                    var groupsWithThisPolicy = servicePolicy.GetPolicyGroups(policy.Id);
                    var memberIds = new List<int>();
                    foreach (var group in groupsWithThisPolicy)
                    {
                        var groupMembers = new ServiceGroup().GetGroupMembers(group.Id);
                        memberIds.AddRange(groupMembers.Select(member => member.Id));
                    }
                    var distinctMembers = memberIds.Distinct().ToList();
                    if (!distinctMembers.Any()) continue;
                    var successCount = _uow.PolicyHistoryRepository.Get(x => x.PolicyId == policy.Id && x.Result == EnumPolicyHistory.RunResult.Success).ToList().GroupBy(x => x.ComputerId).Count();
                    if (successCount == distinctMembers.Count)
                    {
                        servicePolicy.DeactivatePolicy(policy.Id);
                        servicePolicy.ArchivePolicy(policy.Id);
                    }
                }

            }
        }
    }
   
}
