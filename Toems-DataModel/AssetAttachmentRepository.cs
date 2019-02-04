using System.Collections.Generic;
using System.Linq;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;

namespace Toems_DataModel
{
    public class AssetAttachmentRepository : GenericRepository<EntityAssetAttachment>
    {
        private readonly ToemsDbContext _context;

        public AssetAttachmentRepository(ToemsDbContext context)
            : base(context)
        {
            _context = context;
        }

      
        public List<EntityAttachment> GetAssetAttachments(int assetId)
        {
            return (from s in _context.Attachments
                join d in _context.AssetAttachments on s.Id equals d.AttachmentId into joined
                from j in joined.DefaultIfEmpty()
                where j.AssetId == assetId
                select s).ToList();

        }


        public List<EntityAttachment> GetComputerAttachments(int computerId)
        {
            return (from s in _context.Attachments
                join d in _context.ComputerAttachments on s.Id equals d.AttachmentId into joined
                from j in joined.DefaultIfEmpty()
                where j.ComputerId == computerId
                select s).ToList();

        }


    }
}