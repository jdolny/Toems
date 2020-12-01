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

        public List<EntityImage> GetModuleImagesFileCopy(string moduleGuid)
        {
            return (from h in _context.ImageProfileFileCopy
                    join g in _context.ImageProfiles on h.ProfileId equals g.Id
                    join i in _context.Images on g.ImageId equals i.Id
                    join k in _context.FileCopyModules on h.FileCopyModuleId equals k.Id
                    where k.Guid == moduleGuid
                    select i).ToList();
        }
        public List<EntityImage> GetModuleImagesScript(string moduleGuid)
        {
            return (from h in _context.ImageProfileScripts
                    join g in _context.ImageProfiles on h.ProfileId equals g.Id
                    join i in _context.Images on g.ImageId equals i.Id
                    join k in _context.ScriptModules on h.ScriptModuleId equals k.Id
                    where k.Guid == moduleGuid
                    select i).ToList();
        }
        public List<EntityImage> GetModuleImagesSysprep(string moduleGuid)
        {
            return (from h in _context.ImageProfileSyspreps
                    join g in _context.ImageProfiles on h.ProfileId equals g.Id
                    join i in _context.Images on g.ImageId equals i.Id
                    join k in _context.SysprepModules on h.SysprepModuleId equals k.Id
                    where k.Guid == moduleGuid
                    select i).ToList();
        }

    }
}
