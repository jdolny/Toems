using System.Collections.Generic;
using System.Linq;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;

namespace Toems_DataModel
{
    public class ToemsUserRepository : GenericRepository<EntityToemsUser>
    {
        private readonly ToemsDbContext _context;

        public ToemsUserRepository(ToemsDbContext context)
            : base(context)
        {
            _context = context;
        }

        public string GetUserName(int id)
        {
            return (from s in _context.Users where s.Id == id select s.Name).FirstOrDefault();
        }

        public List<EntityToemsUser> Search(string searchString)
        {
            return (from s in _context.Users
                where s.Name.Contains(searchString)
                orderby s.Name
                select new
                {
                    id = s.Id,
                    name = s.Name,
                    membership = s.Membership,

                    email = s.Email,

                }).AsEnumerable().Select(x => new EntityToemsUser
                {
                    Id = x.id,
                    Name = x.name,
                    Membership = x.membership,

                    Email = x.email,

                }).ToList();
        }

        public List<DtoPinnedPolicy> GetUserPinnedPolicyCounts(int userId)
        {
            var usersPinnedPolicies = (from up in _context.PinnedPolicies
                join p in _context.Policies on up.PolicyId equals p.Id
                where up.UserId == userId
                orderby up.Id descending 
                select p).ToList();
            if(!usersPinnedPolicies.Any())
                return new List<DtoPinnedPolicy>();

            var result = new List<DtoPinnedPolicy>();
            foreach (var pinned in usersPinnedPolicies)
            {
                var history = (from h in _context.Policies
                    join g in _context.PolicyHistories on h.Id equals g.PolicyId
                    where (h.Id == pinned.Id)
                    select g).ToList();

                if (!history.Any())
                {
                    var p = new DtoPinnedPolicy();
                    p.PolicyId = pinned.Id;
                    p.PolicyName = pinned.Name;
                    p.Description = pinned.Description;
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

        public List<DtoPinnedGroup> GetUserPinnedGroupCounts(int userId)
        {
            var usersPinnedGroups = (from up in _context.PinnedGroups
                                       join p in _context.Groups on up.GroupId equals p.Id
                                       where up.UserId == userId
                                       orderby up.Id descending 
                                       select p).ToList();
            if (!usersPinnedGroups.Any())
                return new List<DtoPinnedGroup>();

            var list = new List<DtoPinnedGroup>();
            foreach (var pinnedGroup in usersPinnedGroups)
            {
                var dtoPinnedGroup = new DtoPinnedGroup();
                dtoPinnedGroup.GroupId = pinnedGroup.Id;
                dtoPinnedGroup.GroupName = pinnedGroup.Name;
                dtoPinnedGroup.Description = pinnedGroup.Description;
                dtoPinnedGroup.MemberCount = _context.GroupMemberships.Count(x => x.GroupId == pinnedGroup.Id);
                list.Add(dtoPinnedGroup);
            }

            return list;
        }

        public List<EntityToemsUser> GetGroupMembers(int userGroupId)
        {
            return (from h in _context.Users
                    join g in _context.UserGroupMemberships on h.Id equals g.ToemsUserId
                    where (g.UserGroupId == userGroupId)
                    orderby h.Name
                    select h).ToList();
        }
    }
}