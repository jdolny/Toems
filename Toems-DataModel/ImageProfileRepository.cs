﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toems_Common.Entity;

namespace Toems_DataModel
{
    public class ImageProfileRepository : GenericRepository<EntityImageProfile>
    {
        private readonly ToemsDbContext _context;

        public ImageProfileRepository(ToemsDbContext context)
            : base(context)
        {
            _context = context;
        }

        public List<ImageProfileWithImage> GetImageProfilesWithImages()
        {
            return (from h in _context.ImageProfiles
                    join g in _context.Images on h.ImageId equals g.Id into joined
                    from p in joined.DefaultIfEmpty()
                    select new
                    {
                        profile = h,
                        image = p
                    }).AsEnumerable().Select(x => new ImageProfileWithImage
                    {
                        Id = x.profile.Id,
                        Name = x.profile.Name,
                        Description = x.profile.Description,
                        ImageId = x.profile.ImageId,
                        Kernel = x.profile.Kernel,
                        BootImage = x.profile.BootImage,
                        KernelArguments = x.profile.KernelArguments,
                        SkipCore = x.profile.SkipCore,
                        TaskCompletedAction = x.profile.TaskCompletedAction,
                        RemoveGPT = x.profile.RemoveGPT,
                        SkipShrinkVolumes = x.profile.SkipShrinkVolumes,
                        SkipShrinkLvm = x.profile.SkipShrinkLvm,
                        SkipExpandVolumes = x.profile.SkipExpandVolumes,
                        FixBcd = x.profile.FixBcd,
                        FixBootloader = x.profile.FixBootloader,
                        PartitionMethod = x.profile.PartitionMethod,
                        ForceDynamicPartitions = x.profile.ForceDynamicPartitions,
                        CustomPartitionScript = x.profile.CustomPartitionScript,
                        Compression = x.profile.Compression,
                        CompressionLevel = x.profile.CompressionLevel,
                        CustomSchema = x.profile.CustomSchema,
                        CustomUploadSchema = x.profile.CustomUploadSchema,
                        SenderArguments = x.profile.SenderArguments,
                        ReceiverArguments = x.profile.ReceiverArguments,
                        WebCancel = x.profile.WebCancel,
                        ChangeName = x.profile.ChangeName,
                        SkipNvramUpdate = x.profile.SkipNvramUpdate,
                        RandomizeGuids = x.profile.RandomizeGuids,
                        ForceStandardEfi = x.profile.ForceStandardEfi,
                        ForceStandardLegacy = x.profile.ForceStandardLegacy,
                        SimpleUploadSchema = x.profile.SimpleUploadSchema,
                        Image = x.image,
                        ModelMatch = x.profile.ModelMatch,
                        ModelMatchType = x.profile.ModelMatchType,
                        SkipBitlockerCheck = x.profile.SkipBitlockerCheck,
                        SkipHibernationCheck = x.profile.SkipHibernationCheck,
                        SetBootmgrFirst = x.profile.SetBootmgrFirst
                    }).OrderBy(x => x.Name).ToList();
        }

        public ImageProfileWithImage GetImageProfileWithImage(int profileId)
        {
            return (from h in _context.ImageProfiles
                    join g in _context.Images on h.ImageId equals g.Id into joined
                    from p in joined.DefaultIfEmpty()
                    where h.Id == profileId
                    select new
                    {
                        profile = h,
                        image = p
                    }).AsEnumerable().Select(x => new ImageProfileWithImage
                    {
                        Id = x.profile.Id,
                        Name = x.profile.Name,
                        Description = x.profile.Description,
                        ImageId = x.profile.ImageId,
                        Kernel = x.profile.Kernel,
                        BootImage = x.profile.BootImage,
                        KernelArguments = x.profile.KernelArguments,
                        SkipCore = x.profile.SkipCore,
                        TaskCompletedAction = x.profile.TaskCompletedAction,
                        RemoveGPT = x.profile.RemoveGPT,
                        SkipShrinkVolumes = x.profile.SkipShrinkVolumes,
                        SkipShrinkLvm = x.profile.SkipShrinkLvm,
                        SkipExpandVolumes = x.profile.SkipExpandVolumes,
                        FixBcd = x.profile.FixBcd,
                        FixBootloader = x.profile.FixBootloader,
                        PartitionMethod = x.profile.PartitionMethod,
                        ForceDynamicPartitions = x.profile.ForceDynamicPartitions,
                        CustomPartitionScript = x.profile.CustomPartitionScript,
                        Compression = x.profile.Compression,
                        CompressionLevel = x.profile.CompressionLevel,
                        CustomSchema = x.profile.CustomSchema,
                        CustomUploadSchema = x.profile.CustomUploadSchema,
                        SenderArguments = x.profile.SenderArguments,
                        ReceiverArguments = x.profile.ReceiverArguments,
                        WebCancel = x.profile.WebCancel,
                        ChangeName = x.profile.ChangeName,
                        SkipNvramUpdate = x.profile.SkipNvramUpdate,
                        RandomizeGuids = x.profile.RandomizeGuids,
                        ForceStandardEfi = x.profile.ForceStandardEfi,
                        ForceStandardLegacy = x.profile.ForceStandardLegacy,
                        SimpleUploadSchema = x.profile.SimpleUploadSchema,
                        Image = x.image,
                        ModelMatch = x.profile.ModelMatch,
                        ModelMatchType = x.profile.ModelMatchType,
                        SkipBitlockerCheck = x.profile.SkipBitlockerCheck,
                        SkipHibernationCheck = x.profile.SkipHibernationCheck,
                        SetBootmgrFirst = x.profile.SetBootmgrFirst
                    }).FirstOrDefault();
        }
    }
}
