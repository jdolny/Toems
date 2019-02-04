using System.Collections.Generic;
using System.Linq;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;

namespace Toems_DataModel
{
    public class AssetRepository : GenericRepository<EntityAsset>
    {
        private readonly ToemsDbContext _context;

        public AssetRepository(ToemsDbContext context)
            : base(context)
        {
            _context = context;
        }

        public List<DtoAssetWithType> GetAssetWithType()
        {
            return (from a in _context.Assets
                join at in _context.CustomAssetTypes on a.AssetTypeId equals at.Id
                select new
                {
                    name = a.DisplayName,
                    id = a.Id,
                    type = at.Name,
                    isArchived = a.IsArchived
                }).AsEnumerable().Select(x => new DtoAssetWithType()
            {
                Name = x.name,
                AssetType = x.type,
                AssetId = x.id,
                IsArchived = x.isArchived
            }).ToList();
        }

        public List<EntityComputer> GetAssetSoftwareComputers(int assetId)
        {
            var sas = (from s in _context.SoftwareAssetSoftwares
                join d in _context.SoftwareInventory on s.SoftwareInventoryId equals d.Id
                where s.AssetId.Equals(assetId)
                select new
                {
                    name = d.Name,
                    version = d.Version,
                    matchType = s.MatchType,
                    major = d.Major,
                    minor = d.Minor,
                    build = d.Build
                }).AsEnumerable().Select(x => new DtoAssetSoftware()
            {
                Name = x.name,
                Version = x.version,
                MatchType = x.matchType,
                Major = x.major,
                Minor = x.minor,
                Build = x.build
            }).ToList();

            var listofComputers = new List<EntityComputer>();
            foreach (var sa in sas)
            {
                if (sa.MatchType == EnumSoftwareAsset.SoftwareMatchType.Name)
                {
                    listofComputers.AddRange(from c in _context.Computers
                        join cs in _context.ComputerSoftware on c.Id equals cs.ComputerId
                        join so in _context.SoftwareInventory on cs.SoftwareId equals so.Id
                        where so.Name.Equals(sa.Name) && c.ProvisionStatus == EnumProvisionStatus.Status.Provisioned
                        select c);
                }
                else if (sa.MatchType == EnumSoftwareAsset.SoftwareMatchType.Name_FullVersion)
                {
                    listofComputers.AddRange(from c in _context.Computers
                        join cs in _context.ComputerSoftware on c.Id equals cs.ComputerId
                        join so in _context.SoftwareInventory on cs.SoftwareId equals so.Id
                        where so.Name.Equals(sa.Name) && so.Version.Equals(sa.Version) && c.ProvisionStatus == EnumProvisionStatus.Status.Provisioned
                                             select c);
                }
                else if (sa.MatchType == EnumSoftwareAsset.SoftwareMatchType.Name_MajorVersion)
                {
                    listofComputers.AddRange(from c in _context.Computers
                        join cs in _context.ComputerSoftware on c.Id equals cs.ComputerId
                        join so in _context.SoftwareInventory on cs.SoftwareId equals so.Id
                        where so.Name.Equals(sa.Name) && so.Major == sa.Major && c.ProvisionStatus == EnumProvisionStatus.Status.Provisioned
                                             select c);
                }
                else if (sa.MatchType == EnumSoftwareAsset.SoftwareMatchType.Name_MajorVersion_MinorVersion)
                {
                    listofComputers.AddRange(from c in _context.Computers
                        join cs in _context.ComputerSoftware on c.Id equals cs.ComputerId
                        join so in _context.SoftwareInventory on cs.SoftwareId equals so.Id
                        where so.Name.Equals(sa.Name) && so.Major == sa.Major && so.Minor == sa.Minor && c.ProvisionStatus == EnumProvisionStatus.Status.Provisioned
                                             select c);
                }
                else if (sa.MatchType == EnumSoftwareAsset.SoftwareMatchType.Name_MajorVersion_MinorVersion_Build)
                {
                    listofComputers.AddRange(from c in _context.Computers
                        join cs in _context.ComputerSoftware on c.Id equals cs.ComputerId
                        join so in _context.SoftwareInventory on cs.SoftwareId equals so.Id
                        where so.Name.Equals(sa.Name) && so.Major == sa.Major && so.Minor == sa.Minor && so.Build == sa.Build && c.ProvisionStatus == EnumProvisionStatus.Status.Provisioned
                                             select c);
                }


            }

            return listofComputers.GroupBy(x => x.Name).Select(x => x.First()).ToList();
        }
    }
}