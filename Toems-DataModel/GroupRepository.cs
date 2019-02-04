using System.Collections.Generic;
using System.Linq;
using Toems_Common.Entity;

namespace Toems_DataModel
{
    public class GroupRepository : GenericRepository<EntityGroup>
    {
        private readonly ToemsDbContext _context;

        public GroupRepository(ToemsDbContext context)
            : base(context)
        {
            _context = context;
        }

        public List<EntityComputer> GetGroupMembers(int searchGroupId, string searchString)
        {
            return (from h in _context.Computers
                    join g in _context.GroupMemberships on h.Id equals g.ComputerId
                    where (g.GroupId == searchGroupId) && (h.Name.Contains(searchString))
                    select h).ToList();
        }

        public List<EntityComputer> GetGroupMembers(int groupId)
        {
            return (from h in _context.Computers
                join g in _context.GroupMemberships on h.Id equals g.ComputerId
                where (g.GroupId == groupId)
                select h).ToList();
        }

      
    }
}
