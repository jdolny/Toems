using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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


            policyDetailed.PrinterModules = printers.ToList();
            policyDetailed.CommandModules = command.ToList();
            policyDetailed.FileCopyModules = files.ToList();
            policyDetailed.ScriptModules = scripts.ToList();
            policyDetailed.SoftwareModules = software.ToList();
            policyDetailed.WuModules = wu.ToList();
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

       
    }
}
