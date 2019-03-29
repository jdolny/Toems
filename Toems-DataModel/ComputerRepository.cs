using System;
using System.Collections.Generic;
using System.Linq;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;

namespace Toems_DataModel
{
    public class ComputerRepository : GenericRepository<EntityComputer>
    {
        private readonly ToemsDbContext _context;

        public ComputerRepository(ToemsDbContext context)
            : base(context)
        {
            _context = context;
        }

        public List<EntityGroup> GetComputerPreventShutdownGroups(int computerId)
        {
            return (from c in _context.Computers
                join gm in _context.GroupMemberships on c.Id equals gm.ComputerId
                join gr in _context.Groups on gm.GroupId equals gr.Id
                where c.Id == computerId && gr.PreventShutdown
                select gr).ToList();
        }

        public List<EntityComputer> GetPotentialWOLRelays(string gateway, DateTime dateCutoff)
        {
            return (from c in _context.Computers
                join g in _context.NicInventory on c.Id equals g.ComputerId
                where g.Gateways.Contains(gateway) && c.LastCheckinTime > dateCutoff
                select c).OrderByDescending(x => x.LastCheckinTime).Take(10).ToList();
        }

        public List<EntitySoftwareInventory> GetComputerSoftware(int computerId, string searchString)
        {
            return (from h in _context.Computers
                    join g in _context.ComputerSoftware on h.Id equals g.ComputerId
                    join z in _context.SoftwareInventory on g.SoftwareId equals z.Id
                    where (h.Id == computerId && z.Name.Contains(searchString))
                    select z).OrderBy(x => x.Name).ToList();
        }

        public List<EntityCertificateInventory> GetComputerCertificates(int computerId, string searchString)
        {
            return (from h in _context.Computers
                    join g in _context.ComputerCertificates on h.Id equals g.ComputerId
                    join z in _context.CertificateInventory on g.CertificateId equals z.Id
                    where (h.Id == computerId && z.Subject.Contains(searchString))
                    select z).OrderBy(x => x.Subject).ToList();
        }


        public List<DtoComputerUpdates> GetWindowsUpdates(int computerId, string searchString)
        {
            return (from h in _context.Computers
                join g in _context.ComputerUpdates on h.Id equals g.ComputerId
                join z in _context.WuInventory on g.UpdateId equals z.Id
                where (h.Id == computerId && z.Title.Contains(searchString))
                orderby g.IsInstalled
                select new
                {
                    title = z.Title,
                    installDate = g.LastDeploymentChangeTime,
                    isInstalled = g.IsInstalled,
                    category = z.Category,
                    updateId = z.UpdateId
                }).AsEnumerable().Select(x => new DtoComputerUpdates()
                {
                    Title = x.title,
                    InstallDate = x.installDate,
                    IsInstalled = x.isInstalled,
                    Category = x.category,
                    UpdateId = x.updateId
                }).ToList();
        }

        public List<DtoComputerPolicyHistory> GetPolicyHistory(int computerId)
        {
            return (from s in _context.PolicyHistories
                    join d in _context.Policies on s.PolicyId equals d.Id
                    where s.ComputerId == computerId
                    orderby s.LastRunTime descending
                    select new
                    {
                        policyId = s.PolicyId,
                        policyName = d.Name,
                        result = s.Result,
                        hash = s.Hash,
                        runtime = s.LastRunTime
                     
                    }).AsEnumerable().Select(x => new DtoComputerPolicyHistory()
                    {
                       PolicyId = x.policyId,
                       PolicyName = x.policyName,
                       PolicyHash = x.hash,
                       Result = x.result,
                       RunTime = x.runtime

                    }).ToList();
        }

        public List<DtoCustomComputerInventory> GetCustomComputerInventory(int computerId)
        {
            return (from s in _context.CustomInventories
                    join d in _context.Computers on s.ComputerId equals d.Id
                    join e in _context.ScriptModules on s.ScriptId equals e.Id into joined
                    from j in joined.DefaultIfEmpty()
                    where s.ComputerId == computerId
                    orderby d.Name
                    select new
                    {
                        computerId = d.Id,
                        ModuleName = j.Name,
                        ModuleValue = s.Value
                    }).AsEnumerable().Select(x => new DtoCustomComputerInventory()
                    {
                        ComputerId = x.computerId,
                        ModuleName = x.ModuleName,
                        Value = x.ModuleValue

                    }).ToList();
        }

        public List<EntityGroup> GetComputerAdGroups(int computerId)
        {
            return (from h in _context.GroupMemberships
                    join g in _context.Groups on h.GroupId equals g.Id
                    where h.ComputerId == computerId && g.IsOu
                    select g).ToList();

        }

        public List<EntityGroup> GetAllComputerGroups(int computerId)
        {
            return (from h in _context.GroupMemberships
                    join g in _context.Groups on h.GroupId equals g.Id
                    where h.ComputerId == computerId
                    select g).ToList();
        }

        public List<DtoModule> GetComputerModules(int computerId)
        {
            var policies = (from h in _context.GroupMemberships
                    join i in _context.GroupPolicies on h.GroupId equals i.GroupId
                    join j in _context.Policies on i.PolicyId equals j.Id
                    where h.ComputerId == computerId
                    select j).ToList();

            var policyModules = policies.Select(policy => new PolicyRepository(_context).GetDetailed(policy.Id)).ToList();

            var list = new List<DtoModule>();
            foreach (var policyModule in policyModules)
            {
                foreach (var module in policyModule.CommandModules)
                {
                    var dto = new DtoModule();
                    dto.ModuleType = EnumModule.ModuleType.Command;
                    dto.Guid = module.Guid;
                    dto.Name = module.Name;
                    dto.Id = module.Id;
                    list.Add(dto);
                }
                foreach (var module in policyModule.FileCopyModules)
                {
                    var dto = new DtoModule();
                    dto.ModuleType = EnumModule.ModuleType.FileCopy;
                    dto.Guid = module.Guid;
                    dto.Name = module.Name;
                    dto.Id = module.Id;
                    list.Add(dto);
                }
                foreach (var module in policyModule.PrinterModules)
                {
                    var dto = new DtoModule();
                    dto.ModuleType = EnumModule.ModuleType.Printer;
                    dto.Guid = module.Guid;
                    dto.Name = module.Name;
                    dto.Id = module.Id;
                    list.Add(dto);
                }
                foreach (var module in policyModule.ScriptModules)
                {
                    var dto = new DtoModule();
                    dto.ModuleType = EnumModule.ModuleType.Script;
                    dto.Guid = module.Guid;
                    dto.Name = module.Name;
                    dto.Id = module.Id;
                    list.Add(dto);
                }
                foreach (var module in policyModule.SoftwareModules)
                {
                    var dto = new DtoModule();
                    dto.ModuleType = EnumModule.ModuleType.Software;
                    dto.Guid = module.Guid;
                    dto.Name = module.Name;
                    dto.Id = module.Id;
                    list.Add(dto);
                }

                foreach (var module in policyModule.WuModules)
                {
                    var dto = new DtoModule();
                    dto.ModuleType = EnumModule.ModuleType.Wupdate;
                    dto.Guid = module.Guid;
                    dto.Name = module.Name;
                    dto.Id = module.Id;
                    list.Add(dto);
                }
               
            }
            return list;
        }

        public List<EntityPolicy> GetComputerPolicies(int computerId)
        {
            return (from h in _context.GroupMemberships
                    join i in _context.GroupPolicies on h.GroupId equals i.GroupId
                    join j in _context.Policies on i.PolicyId equals  j.Id
                    where h.ComputerId == computerId
                    select j).ToList();
        }

      
    }
}
