using System.Collections.Generic;
using System.Linq;
using Toems_Common.Entity;

namespace Toems_DataModel
{
    public class PolicyHistoryRepository : GenericRepository<EntityPolicyHistory>
    {
        private readonly ToemsDbContext _context;

        public PolicyHistoryRepository(ToemsDbContext context)
            : base(context)
        {
            _context = context;
        }

        public List<EntityPolicyHistory> GetHistoryWithComputer(int policyId,int limit,string searchstring)
        {
            if (!string.IsNullOrEmpty(searchstring))
            {
                return (from s in _context.PolicyHistories
                    join d in _context.Computers on s.ComputerId equals d.Id into joined
                    from j in joined.DefaultIfEmpty()
                    where s.PolicyId == policyId && (j.Name.Contains(searchstring) || s.Hash.Contains(searchstring) || s.User.Contains(searchstring) )
                    orderby s.LastRunTime descending
                    select new
                    {
                        computerId = s.ComputerId,
                        policyId = s.PolicyId,
                        result = s.Result,
                        hash = s.Hash,
                        runTime = s.LastRunTime,
                        username = s.User,
                        computerName = j.Name,
                    }).AsEnumerable().Select(x => new EntityPolicyHistory()
                    {
                        PolicyId = x.policyId,
                        ComputerId = x.computerId,
                        Result = x.result,
                        Hash = x.hash,
                        LastRunTime = x.runTime,
                        ComputerName = x.computerName,
                        User = x.username
                    }).Take(limit).ToList();
            }
            else
            {
                return (from s in _context.PolicyHistories
                        join d in _context.Computers on s.ComputerId equals d.Id into joined
                        from j in joined.DefaultIfEmpty()
                        where s.PolicyId == policyId
                        orderby s.LastRunTime descending
                        select new
                        {
                            computerId = s.ComputerId,
                            policyId = s.PolicyId,
                            result = s.Result,
                            hash = s.Hash,
                            runTime = s.LastRunTime,
                            username = s.User,
                            computerName = j.Name,
                        }).AsEnumerable().Select(x => new EntityPolicyHistory()
                        {
                            PolicyId = x.policyId,
                            ComputerId = x.computerId,
                            Result = x.result,
                            Hash = x.hash,
                            LastRunTime = x.runTime,
                            User = x.username,
                            ComputerName = x.computerName
                        }).Take(limit).ToList();
            }
        }

  
    }
}
