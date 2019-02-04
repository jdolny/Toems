using System.Collections.Generic;
using System.Linq;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;

namespace Toems_DataModel
{
    public class AssetGroupRepository : GenericRepository<EntityAssetGroup>
    {
        private readonly ToemsDbContext _context;

        public AssetGroupRepository(ToemsDbContext context)
            : base(context)
        {
            _context = context;
        }

        public List<DtoAssetWithType> GetAssetGroupMembers(int assetGroupId)
        {
            return (from a in _context.Assets
                join at in _context.CustomAssetTypes on a.AssetTypeId equals at.Id
                join g in _context.AssetGroupMembers on a.Id equals g.AssetId
                    where g.AssetGroupId == assetGroupId
                select new
                {
                    name = a.DisplayName,
                    id = a.Id,
                    type = at.Name
                }).AsEnumerable().Select(x => new DtoAssetWithType()
            {
                Name = x.name,
                AssetType = x.type,
                AssetId = x.id
            }).ToList();
        }
    }
}