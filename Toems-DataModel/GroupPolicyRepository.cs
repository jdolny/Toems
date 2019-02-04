using System.Linq;
using Toems_Common.Entity;

namespace Toems_DataModel
{
    public class GroupPolicyRepository : GenericRepository<EntityGroupPolicy>
    {
        private readonly ToemsDbContext _context;

        public GroupPolicyRepository(ToemsDbContext context)
            : base(context)
        {
            _context = context;
        }

        public GroupPolicyDetailed GetDetailed(int policyId, int groupPolicyId)
        {
            var a = (from s in _context.GroupPolicies
                join d in _context.Policies on s.PolicyId equals d.Id into joined
                from j in joined.DefaultIfEmpty()
                where s.PolicyId == policyId && s.Id == groupPolicyId
                orderby s.PolicyOrder
                select new
                {
                    id = s.Id,
                    groupId = s.GroupId,
                    policyId = s.PolicyId,
                    order = s.PolicyOrder,
                    policy = j
                }).AsEnumerable().Select(x => new GroupPolicyDetailed()
                {
                    Policy = x.policy,
                    Id = x.id,
                    PolicyId = x.policyId,
                    GroupId = x.groupId,
                    PolicyOrder = x.order
                }).First();

            return a;
        }

  
    }
}
