using System.Collections.Generic;
using System.Linq;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_ServiceCore.Data;

namespace Toems_DataModel
{
    public class ToemsUserRepository(ToemsDbContext context) : GenericRepository<AppUser>(context)
    {
        public string GetUserName(int id)
        {
            return (from s in context.Users where s.UserId == id select s.UserName).FirstOrDefault();
        }

        public List<AppUser> Search(string searchString)
        {
            return (from s in context.Users
                where s.UserName.Contains(searchString)
                orderby s.UserName
                select new
                {
                    id = s.Id,
                    name = s.UserName,
                    membership = s.Membership,

                    email = s.Email,

                }).AsEnumerable().Select(x => new AppUser()
                {
                    Id = x.id,
                    UserName = x.name,
                    Membership = x.membership,

                    Email = x.email,

                }).ToList();
        }

        public List<DtoPinnedPolicy> GetUserPinnedPolicyCounts(int userId)
        {
            var usersPinnedPolicies = (from up in context.PinnedPolicies
                join p in context.Policies on up.PolicyId equals p.Id
                where up.UserId == userId
                orderby up.Id descending 
                select p).ToList();
            if(!usersPinnedPolicies.Any())
                return new List<DtoPinnedPolicy>();

            var result = new List<DtoPinnedPolicy>();
            foreach (var pinned in usersPinnedPolicies)
            {
                var history = (from h in context.Policies
                    join g in context.PolicyHistories on h.Id equals g.PolicyId
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
                        var policy = (from s in context.Policies where s.Id == id select s).FirstOrDefault();
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
            var usersPinnedGroups = (from up in context.PinnedGroups
                                       join p in context.Groups on up.GroupId equals p.Id
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
                dtoPinnedGroup.MemberCount = context.GroupMemberships.Count(x => x.GroupId == pinnedGroup.Id);
                list.Add(dtoPinnedGroup);
            }

            return list;
        }

        public List<AppUser> GetGroupMembers(int userGroupId)
        {
            return (from h in context.Users
                    join g in context.UserGroupMemberships on h.UserId equals g.ToemsUserId
                    where (g.UserGroupId == userGroupId)
                    orderby h.UserName
                    select h).ToList();
        }
    }
}