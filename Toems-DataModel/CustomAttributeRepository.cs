using System.Collections.Generic;
using System.Linq;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;

namespace Toems_DataModel
{
    public class CustomAttributeRepository : GenericRepository<EntityCustomAttribute>
    {
        private readonly ToemsDbContext _context;

        public CustomAttributeRepository(ToemsDbContext context)
            : base(context)
        {
            _context = context;
        }

        public List<DtoCustomAttributeWithType> GetAttributeWithType()
        {
            return (from a in _context.CustomAttributes
                join at in _context.CustomAssetTypes on a.UsageType equals at.Id
                select new
                {
                    name = a.Name,
                    id = a.Id,
                    usageType = at.Name,
                    textMode = a.TextMode
                }).AsEnumerable().Select(x => new DtoCustomAttributeWithType()
            {
                Name = x.name,
                AttributeId = x.id,
                TextMode = x.textMode,
                UsageType = x.usageType
            }).ToList();
        }

     
    }
}