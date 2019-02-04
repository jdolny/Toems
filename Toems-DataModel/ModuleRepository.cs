using System.Collections.Generic;
using System.Linq;
using Toems_Common.Entity;

namespace Toems_DataModel
{
    public class ModuleRepository : GenericRepository<EntityModule>
    {
        private readonly ToemsDbContext _context;

        public ModuleRepository(ToemsDbContext context)
            : base(context)
        {
            _context = context;
        }

        public List<EntityGroup> GetModuleGroups(string moduleGuid)
        {
            return (from h in _context.PolicyModules
                    join g in _context.Policies on h.PolicyId equals g.Id
                    join i in _context.GroupPolicies on g.Id equals i.PolicyId
                    join j in _context.Groups on i.GroupId equals j.Id
                    where (h.Guid == moduleGuid)
                    select j).ToList();
        }

        public List<EntityComputer> GetModuleComputers(string moduleGuid)
        {
            return (from h in _context.PolicyModules
                    join g in _context.Policies on h.PolicyId equals g.Id
                    join i in _context.GroupPolicies on g.Id equals i.PolicyId
                    join x in _context.GroupMemberships on i.GroupId equals x.GroupId
                    join z in _context.Computers on x.ComputerId equals z.Id
                    where (h.Guid == moduleGuid)
                    select z).ToList();
        }

        public List<EntityPolicy> GetModulePolicies(string moduleGuid)
        {
            return (from h in _context.PolicyModules
                    join g in _context.Policies on h.PolicyId equals g.Id               
                    where (h.Guid == moduleGuid)
                    select g).ToList();
        }
    }
}
