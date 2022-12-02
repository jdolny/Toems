using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;

namespace Toems_DataModel
{
    public class PolicyRepository : GenericRepository<EntityPolicy>
    {
        private readonly ToemsDbContext _context;

        public PolicyRepository(ToemsDbContext context)
            : base(context)
        {
            _context = context;
        }

        public List<string> GetPolicyComServerUrls(int policyId)
        {
            return (from h in _context.PolicyComServers
                join j in _context.ClientComServers on h.ComServerId equals j.Id
                where h.PolicyId == policyId
                select j.Url).ToList();
        }

        public List<EntityGroup> GetPolicyGroups(int policyId)
        {
            return (from h in _context.GroupPolicies
                    join j in _context.Groups on h.GroupId equals j.Id
                    where (h.PolicyId == policyId)
                    select j).ToList();
        }

        public List<EntityComputer> GetPolicyComputers(int policyId)
        {
            return (from h in _context.GroupPolicies
                    join x in _context.GroupMemberships on h.GroupId equals x.GroupId
                    join z in _context.Computers on x.ComputerId equals z.Id
                    where (h.PolicyId == policyId)
                    select z).ToList();
        }

        public PolicyModules GetDetailed(int policyId)
        {
            var policy = _context.Policies.Find(policyId);
            if (policy == null) return null;
            var policyDetailed = new PolicyModules();
            
            var printers = from p in _context.Policies
                where p.Id == policyId
                join policyModule in _context.PolicyModules on p.Id equals policyModule.PolicyId
                where policyModule.ModuleType == EnumModule.ModuleType.Printer
                join printerModule in _context.PrinterModules on policyModule.ModuleId equals printerModule.Id
                select printerModule;

            var software = from p in _context.Policies
                where p.Id == policyId
                join policyModule in _context.PolicyModules on p.Id equals policyModule.PolicyId
                where policyModule.ModuleType == EnumModule.ModuleType.Software
                join softwareModule in _context.SoftwareModules on policyModule.ModuleId equals softwareModule.Id
                select softwareModule;

            var files = from p in _context.Policies
                where p.Id == policyId
                join policyModule in _context.PolicyModules on p.Id equals policyModule.PolicyId
                where policyModule.ModuleType == EnumModule.ModuleType.FileCopy
                join fileModule in _context.FileCopyModules on policyModule.ModuleId equals fileModule.Id
                select fileModule;

            var winPe = from p in _context.Policies
                        where p.Id == policyId
                        join policyModule in _context.PolicyModules on p.Id equals policyModule.PolicyId
                        where policyModule.ModuleType == EnumModule.ModuleType.WinPE
                        join winPeModule in _context.WinPeModules on policyModule.ModuleId equals winPeModule.Id
                        select winPeModule;

            var scripts = from p in _context.Policies
                where p.Id == policyId
                join policyModule in _context.PolicyModules on p.Id equals policyModule.PolicyId
                where policyModule.ModuleType == EnumModule.ModuleType.Script
                join scriptModule in _context.ScriptModules on policyModule.ModuleId equals scriptModule.Id
                select scriptModule;

            var command = from p in _context.Policies
                where p.Id == policyId
                join policyModule in _context.PolicyModules on p.Id equals policyModule.PolicyId
                where policyModule.ModuleType == EnumModule.ModuleType.Command
                join commandModule in _context.CommandModules on policyModule.ModuleId equals commandModule.Id
                select commandModule;

            var wu = from p in _context.Policies
                where p.Id == policyId
                join policyModule in _context.PolicyModules on p.Id equals policyModule.PolicyId
                where policyModule.ModuleType == EnumModule.ModuleType.Wupdate
                join wuModule in _context.WindowsUpdateModules on policyModule.ModuleId equals wuModule.Id
                select wuModule;

            var message = from p in _context.Policies
                          where p.Id == policyId
                          join policyModule in _context.PolicyModules on p.Id equals policyModule.PolicyId
                          where policyModule.ModuleType == EnumModule.ModuleType.Message
                          join messageModule in _context.MessageModules on policyModule.ModuleId equals messageModule.Id
                          select messageModule;


            policyDetailed.PrinterModules = printers.ToList();
            policyDetailed.CommandModules = command.ToList();
            policyDetailed.FileCopyModules = files.ToList();
            policyDetailed.ScriptModules = scripts.ToList();
            policyDetailed.SoftwareModules = software.ToList();
            policyDetailed.MessageModules = message.ToList();
            policyDetailed.WuModules = wu.ToList();
            policyDetailed.WinPeModules = winPe.ToList();
            policyDetailed.Name = policy.Name;
            policyDetailed.CompletedAction = policy.CompletedAction;
            policyDetailed.Description = policy.Description;
            policyDetailed.ExecutionType = policy.ExecutionType;
            policyDetailed.Frequency = policy.Frequency;
            policyDetailed.Guid = policy.Guid;
            policyDetailed.Hash = policy.Hash;
            policyDetailed.Id = policy.Id;
            policyDetailed.RemoveInstallCache = policy.RemoveInstallCache;
            policyDetailed.RunInventory = policy.RunInventory;
            policyDetailed.Trigger = policy.Trigger;
            policyDetailed.SubFrequency = policy.SubFrequency;
            policyDetailed.StartDate = policy.StartDate;
            policyDetailed.RunLoginTracker = policy.RunLoginTracker;
            policyDetailed.ConditionId = policy.ConditionId;
            policyDetailed.ConditionFailedAction = policy.ConditionFailedAction;

            return policyDetailed;
        }

        public List<EntityPolicy> ActivePoliciesWithModule(int moduleId, EnumModule.ModuleType moduleType)
        {
            return (from pm in _context.PolicyModules
                    join a in _context.ActiveClientPolicies on pm.PolicyId equals a.PolicyId
                    join p in _context.Policies on a.PolicyId equals p.Id 
                    where (pm.ModuleId == moduleId) && (pm.ModuleType == moduleType)
                    select p).ToList();
        }

        public List<DtoPinnedPolicy> GetActivePolicyStatus()
        {
            var activePolicies = (from p in _context.Policies
                                  join a in _context.ActiveClientPolicies on p.Id equals a.PolicyId
                                       select p).ToList();
            if (!activePolicies.Any())
                return new List<DtoPinnedPolicy>();

            var result = new List<DtoPinnedPolicy>();
            foreach (var activePolicy in activePolicies)
            {
                var history = (from h in _context.Policies
                               join g in _context.PolicyHistories on h.Id equals g.PolicyId
                               where (h.Id == activePolicy.Id)
                               select g).ToList();

                if (!history.Any())
                {
                    var p = new DtoPinnedPolicy();
                    p.PolicyId = activePolicy.Id;
                    p.PolicyName = activePolicy.Name;
                    p.Description = activePolicy.Description;
                    p.FailedCount = 0;
                    p.SkippedCount = 0;
                    p.SuccessCount = 0;
                    result.Add(p);
                }
                else
                {
                    var policyIds = history.GroupBy(x => x.PolicyId).Select(x => x.Key);
                    foreach (var id in policyIds)
                    {
                        var dtoPinnedHistory = new DtoPinnedPolicy();
                        dtoPinnedHistory.PolicyId = id;
                        var policy = (from s in _context.Policies where s.Id == id select s).FirstOrDefault();
                        if (policy != null)
                        {
                            dtoPinnedHistory.PolicyName = policy.Name;
                            dtoPinnedHistory.Description = policy.Description;
                        }
                        else
                        {
                            continue;
                        }

                        dtoPinnedHistory.SuccessCount =
                            history.Where(x => x.PolicyId == id && x.Result == EnumPolicyHistory.RunResult.Success).ToList().GroupBy(x => x.ComputerId).Count();
                        dtoPinnedHistory.FailedCount =
                            history.Where(x => x.PolicyId == id && x.Result == EnumPolicyHistory.RunResult.Failed).ToList().GroupBy(x => x.ComputerId).Count();
                        dtoPinnedHistory.SkippedCount =
                            history.Where(x => x.PolicyId == id && x.Result == EnumPolicyHistory.RunResult.Skipped).ToList().GroupBy(x => x.ComputerId).Count();

                        result.Add(dtoPinnedHistory);
                    }
                }
            }

            return result;
        }


    }
}
