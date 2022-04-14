using System.Collections.Generic;
using System.Linq;
using Toems_Common.Dto;
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

        public List<DtoOrderedGroup> GetMembershipsForClientPolicy(int computerId)
        {
            var allComputersGroup = (from a in _context.Groups where a.Id == -1 select a).FirstOrDefault();

            var groups = (from h in _context.Groups
                    join g in _context.GroupMemberships on h.Id equals g.GroupId
                    where g.ComputerId == computerId
                    select new
                    {
                        groupId = h.Id,
                        priority = h.EmPriority,
                    }).AsEnumerable().Select(x => new DtoOrderedGroup
                    {
                        GroupId = x.groupId,
                        Priority = x.priority
                    }).ToList();

            if (allComputersGroup != null)
                groups.Add(new DtoOrderedGroup { GroupId = allComputersGroup.Id, Priority = allComputersGroup.EmPriority });

            return groups.OrderBy(x => x.Priority).ToList();
                   
        }
        public List<EntityComputer> GetGroupMembers(int searchGroupId, string searchString)
        {
            return (from h in _context.Computers
                    join g in _context.GroupMemberships on h.Id equals g.ComputerId
                    where (g.GroupId == searchGroupId) && (h.Name.Contains(searchString))
                    orderby h.Name
                    select h).ToList();
        }

        public List<EntityComputer> GetGroupMembers(int groupId)
        {
            return (from h in _context.Computers
                join g in _context.GroupMemberships on h.Id equals g.ComputerId
                where (g.GroupId == groupId)
                orderby h.Name
                select h).ToList();
        }

        public List<ComputerWithImage> GetGroupMembersWithImages(int searchGroupId, string searchString)
        {
            return (from h in _context.Computers
                    join g in _context.GroupMemberships on h.Id equals g.ComputerId
                    join t in _context.Images on h.ImageId equals t.Id into joined
                    from p in joined.DefaultIfEmpty()
                    where (g.GroupId == searchGroupId) && (h.Name.Contains(searchString) || h.ImagingMac.Contains(searchString))
                    select new
                    {
                        id = h.Id,
                        name = h.Name,
                        mac = h.ImagingMac,
                        image = p,
                        profileId = h.ImageProfileId,
                    
                    }).AsEnumerable().Select(x => new ComputerWithImage
                    {
                        Id = x.id,
                        Name = x.name,
                        ImagingMac = x.mac,
                        Image = x.image,
                        ImageProfileId = x.profileId,
                        
                    }).OrderBy(x => x.Name).ToList();
        }


    }
}
