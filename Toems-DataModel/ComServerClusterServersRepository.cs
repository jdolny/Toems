using System.Collections.Generic;
using System.Linq;
using Toems_Common.Dto;
using Toems_Common.Dto.client;
using Toems_Common.Entity;

namespace Toems_DataModel
{
    public class ComServerClusterServersRepository : GenericRepository<EntityComServerClusterServer>
    {
        private readonly ToemsDbContext _context;

        public ComServerClusterServersRepository(ToemsDbContext context)
            : base(context)
        {
            _context = context;
        }

        public List<DtoClientComServers> GetClusterServers(int clusterId)
        {
            return (from s in _context.ComServerClusterServers
                join d in _context.ClientComServers on s.ComServerId equals d.Id into joined
                from j in joined.DefaultIfEmpty()
                where s.ComServerClusterId.Equals(clusterId)
                select new
                {
                    url = j.Url,
                    role = s.Role
                }).AsEnumerable().Select(x => new DtoClientComServers()
                {
                    Url = x.url,
                    Role = x.role
                }).ToList();
        }


    }
}
