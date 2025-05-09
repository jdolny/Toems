using System;
using System.Collections.Generic;
using System.Linq;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;

namespace Toems_DataModel
{
    public class ComputerRepository : GenericRepository<EntityComputer>
    {
        private readonly ToemsDbContext _context;

        public ComputerRepository(ToemsDbContext context)
            : base(context)
        {
            _context = context;
        }

         public List<EntityComputer> SearchAllComputers(DtoComputerFilter filter, int userId, List<int> categoryIds)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == userId);
            var sortMode = user?.ComputerSortMode == "Default"
                ? _context.Settings.FirstOrDefault(x => x.Name == SettingStrings.ComputerSortMode)?.Value?.ToString() ?? "Last Checkin"
                : user?.ComputerSortMode ?? "Last Checkin";

            var searchText = filter.SearchText ?? "";
            var customAttribSearch = string.IsNullOrEmpty(searchText) ? "zzkjfjekrhwlhhw" : searchText;

            var allowedStatuses = new List<EnumProvisionStatus.Status>();
            if (filter.IsActive)
            {
                allowedStatuses.Add(EnumProvisionStatus.Status.Provisioned);
            }
            else if (filter.IsUnmanaged)
            {
                allowedStatuses.Add(EnumProvisionStatus.Status.ImageOnly);
            }
            else
            { 
                allowedStatuses.Add(EnumProvisionStatus.Status.IntermediateInstalled);
                allowedStatuses.Add(EnumProvisionStatus.Status.NotStarted);
                allowedStatuses.Add(EnumProvisionStatus.Status.PendingPreProvision);
                allowedStatuses.Add(EnumProvisionStatus.Status.PendingProvisionApproval);
                allowedStatuses.Add(EnumProvisionStatus.Status.PendingConfirmation);
                allowedStatuses.Add(EnumProvisionStatus.Status.PendingReset);
                allowedStatuses.Add(EnumProvisionStatus.Status.PreProvisioned);
                allowedStatuses.Add(EnumProvisionStatus.Status.ProvisionApproved);
                allowedStatuses.Add(EnumProvisionStatus.Status.Reset);
                allowedStatuses.Add(EnumProvisionStatus.Status.Error);
                allowedStatuses.Add(EnumProvisionStatus.Status.Archived);
                allowedStatuses.Add(EnumProvisionStatus.Status.FullReset);
            }


            var query = from c in _context.Computers
                        from u in _context.UserLogins.Where(x => x.ComputerId == c.Id).OrderByDescending(x => x.Id).Take(1).DefaultIfEmpty()
                        from b in _context.BiosInventory.Where(x => x.ComputerId == c.Id).DefaultIfEmpty()
                        from cu in _context.CustomComputerAttributes.Where(x => x.ComputerId == c.Id && x.Value.Contains(customAttribSearch)).DefaultIfEmpty()
                        from sc in _context.ActiveSockets.Where(x => x.ComputerId == c.Id).DefaultIfEmpty()
                        from cs in _context.ComputerSystemInventory.Where(x => x.ComputerId == c.Id).DefaultIfEmpty()
                        from os in _context.OsInventory.Where(x => x.ComputerId == c.Id).DefaultIfEmpty()
                        where (b.SerialNumber.Contains(searchText) || u.UserName.Contains(searchText) ||
                               c.Name.Contains(searchText) || c.Guid.Contains(searchText) || c.InstallationId.Contains(searchText) ||
                               c.UUID.Contains(searchText) || c.ImagingClientId.Contains(searchText) || c.LastIp.Contains(searchText) ||
                               cu.Value.Contains(customAttribSearch))
                              && allowedStatuses.Contains(c.ProvisionStatus)
                              && (
                                  categoryIds.Count == 0 || filter.CategoryType == "Any" || // No filter or "Any": Include all computers
                                  (filter.CategoryType == "Or" &&
                                   _context.ComputerCategories.Any(cat => cat.ComputerId == c.Id && categoryIds.Contains(cat.CategoryId))) || // Or: At least one category matches
                                  (filter.CategoryType == "And" &&
                                   categoryIds.All(catId =>
                                       _context.ComputerCategories.Any(cc => cc.ComputerId == c.Id && cc.CategoryId == catId))) // And: All categories must match
                              )
                        select new { c, u, b, cu, sc, cs, os };

            query = sortMode == "Last Checkin"
                ? query.OrderByDescending(x => x.c.LastCheckinTime)
                : query.OrderBy(x => x.c.Name);

            return query.AsEnumerable().Select(x => new EntityComputer
            {
                Id = x.c.Id,
                Name = x.c.Name,
                LastCheckinTime = x.c.LastCheckinTime,
                LastIp = x.c.LastIp,
                ClientVersion = x.c.ClientVersion,
                LastLoggedInUser = x.u?.UserName,
                ProvisionedTime = x.c.ProvisionedTime,
                Status = x.sc?.ConnectionId != null ? "Connected" : "Disconnected",
                Manufacturer = x.cs?.Manufacturer,
                Description = x.c.Description,
                Model = x.cs?.Model,
                Domain = x.cs?.Domain,
                OsName = x.os?.Caption,
                OsVersion = x.os?.Version,
                OsBuild = x.os?.BuildNumber,
                ProvisionStatus = x.c.ProvisionStatus,
                IsAdSync = x.c.IsAdSync,
                AdDisabled = x.c.AdDisabled
            }).Take(filter.Limit).ToList();
        }
        public List<EntityGroup> GetComputerPreventShutdownGroups(int computerId)
        {
            return (from c in _context.Computers
                join gm in _context.GroupMemberships on c.Id equals gm.ComputerId
                join gr in _context.Groups on gm.GroupId equals gr.Id
                where c.Id == computerId && gr.PreventShutdown
                select gr).ToList();
        }

      

        public List<EntityComputer> GetPotentialWOLRelays(string gateway, DateTime dateCutoff)
        {
            return (from c in _context.Computers
                join g in _context.NicInventory on c.Id equals g.ComputerId
                where g.Gateways.Contains(gateway) && c.LastCheckinTime > dateCutoff
                select c).OrderByDescending(x => x.LastCheckinTime).Take(10).ToList();
        }

        public List<EntitySoftwareInventory> GetComputerSoftware(int computerId, string searchString)
        {
            return (from h in _context.Computers
                    join g in _context.ComputerSoftware on h.Id equals g.ComputerId
                    join z in _context.SoftwareInventory on g.SoftwareId equals z.Id
                    where (h.Id == computerId && z.Name.Contains(searchString))
                    select z).OrderBy(x => x.Name).ToList();
        }

        public List<EntityCertificateInventory> GetComputerCertificates(int computerId, string searchString)
        {
            return (from h in _context.Computers
                    join g in _context.ComputerCertificates on h.Id equals g.ComputerId
                    join z in _context.CertificateInventory on g.CertificateId equals z.Id
                    where (h.Id == computerId && z.Subject.Contains(searchString))
                    select z).OrderBy(x => x.Subject).ToList();
        }


        public List<DtoComputerUpdates> GetWindowsUpdates(int computerId, string searchString)
        {
            return (from h in _context.Computers
                join g in _context.ComputerUpdates on h.Id equals g.ComputerId
                join z in _context.WuInventory on g.UpdateId equals z.Id
                where (h.Id == computerId && z.Title.Contains(searchString))
                orderby g.IsInstalled
                select new
                {
                    title = z.Title,
                    installDate = g.LastDeploymentChangeTime,
                    isInstalled = g.IsInstalled,
                    category = z.Category,
                    updateId = z.UpdateId
                }).AsEnumerable().Select(x => new DtoComputerUpdates()
                {
                    Title = x.title,
                    InstallDate = x.installDate,
                    IsInstalled = x.isInstalled,
                    Category = x.category,
                    UpdateId = x.updateId
                }).ToList();
        }

        public List<DtoComputerPolicyHistory> GetPolicyHistory(int computerId)
        {
            return (from s in _context.PolicyHistories
                    join d in _context.Policies on s.PolicyId equals d.Id
                    where s.ComputerId == computerId
                    orderby s.LastRunTime descending
                    select new
                    {
                        policyId = s.PolicyId,
                        policyName = d.Name,
                        result = s.Result,
                        hash = s.Hash,
                        runtime = s.LastRunTime
                     
                    }).AsEnumerable().Select(x => new DtoComputerPolicyHistory()
                    {
                       PolicyId = x.policyId,
                       PolicyName = x.policyName,
                       PolicyHash = x.hash,
                       Result = x.result,
                       RunTime = x.runtime

                    }).ToList();
        }

        public List<DtoCustomComputerInventory> GetCustomComputerInventory(int computerId)
        {
            return (from s in _context.CustomInventories
                    join d in _context.Computers on s.ComputerId equals d.Id
                    join e in _context.ScriptModules on s.ScriptId equals e.Id into joined
                    from j in joined.DefaultIfEmpty()
                    where s.ComputerId == computerId
                    orderby d.Name
                    select new
                    {
                        computerId = d.Id,
                        ModuleName = j.Name,
                        ModuleValue = s.Value
                    }).AsEnumerable().Select(x => new DtoCustomComputerInventory()
                    {
                        ComputerId = x.computerId,
                        ModuleName = x.ModuleName,
                        Value = x.ModuleValue

                    }).ToList();
        }

        public List<EntityGroup> GetComputerAdGroups(int computerId)
        {
            return (from h in _context.GroupMemberships
                    join g in _context.Groups on h.GroupId equals g.Id
                    where h.ComputerId == computerId && g.IsOu
                    select g).ToList();

        }

        public List<EntityGroup> GetComputerAdSecurityGroups(int computerId)
        {
            return (from h in _context.GroupMemberships
                    join g in _context.Groups on h.GroupId equals g.Id
                    where h.ComputerId == computerId && g.IsSecurityGroup
                    select g).ToList();

        }

        public List<EntityGroup> GetAllComputerGroups(int computerId)
        {
            var groups = (from h in _context.GroupMemberships
                    join g in _context.Groups on h.GroupId equals g.Id
                    where h.ComputerId == computerId
                    select g).ToList();

            var allComputersGroup = _context.Groups.Where(x => x.Id == -1).FirstOrDefault();
            if (allComputersGroup != null)
                groups.Add(allComputersGroup);

            return groups;
        }

        public List<DtoGroupImage> GetAllComputerGroupsWithImage(int computerId)
        {
            var groups = (from h in _context.GroupMemberships
                    join g in _context.Groups on h.GroupId equals g.Id
                    join i in _context.ImageProfiles on g.ImageProfileId equals i.Id into gi
                    from joinedProfile in gi.DefaultIfEmpty()
                    join img in _context.Images on g.ImageId equals img.Id into gimage
                    from joinedImage in gimage.DefaultIfEmpty()
                    where h.ComputerId == computerId
                    select new
                    {
                        groupId = g.Id,
                        groupName = g.Name,
                        groupDn = g.Dn,
                        ImagePriority = g.ImagingPriority,
                        EmPriority = g.EmPriority,
                        ImageName = joinedImage.Name,
                        ProfileName = joinedProfile.Name,
                    }).AsEnumerable().Select(x => new DtoGroupImage()
                    {
                       GroupId = x.groupId,
                       GroupName = x.groupName,
                       GroupDn = x.groupDn,
                       ImagePriority = x.ImagePriority,
                       EmPriority = x.EmPriority,
                       ImageName = x.ImageName,
                       ProfileName = x.ProfileName

                    }).ToList();

            var allComputersGroup = _context.Groups.Where(x => x.Id == -1).FirstOrDefault();
            if (allComputersGroup == null) return groups;
            var allComputersGroupImage = _context.Images.Where(x => x.Id == allComputersGroup.ImageId).FirstOrDefault();
            var allComputersImageProfile = _context.ImageProfiles.Where(x => x.Id == allComputersGroup.ImageProfileId).FirstOrDefault();

            var groupWithImage = new DtoGroupImage();
            groupWithImage.GroupId = allComputersGroup.Id;
            groupWithImage.GroupName = allComputersGroup.Name;
            groupWithImage.GroupDn = allComputersGroup.Dn;
            groupWithImage.ImagePriority = allComputersGroup.ImagingPriority;
            groupWithImage.EmPriority = allComputersGroup.EmPriority;
            if (allComputersGroupImage != null)
                groupWithImage.ImageName = allComputersGroupImage.Name;
            if (allComputersImageProfile != null)
                groupWithImage.ProfileName = allComputersImageProfile.Name;

            groups.Add(groupWithImage);
            return groups;

        }

        public List<DtoModule> GetComputerModules(int computerId)
        {
            var policies = (from h in _context.GroupMemberships
                    join i in _context.GroupPolicies on h.GroupId equals i.GroupId
                    join j in _context.Policies on i.PolicyId equals j.Id
                    where h.ComputerId == computerId
                    select j).ToList();

            var allComputersGroupPolicies = (from i in _context.GroupPolicies
                                             join j in _context.Policies on i.PolicyId equals j.Id
                                             where i.GroupId == -1
                                             select j).ToList();
            policies.AddRange(allComputersGroupPolicies);

            var policyModules = policies.Select(policy => new PolicyRepository(_context).GetDetailed(policy.Id)).ToList();

            var list = new List<DtoModule>();
            foreach (var policyModule in policyModules)
            {
                foreach (var module in policyModule.CommandModules)
                {
                    var dto = new DtoModule();
                    dto.ModuleType = EnumModule.ModuleType.Command;
                    dto.Guid = module.Guid;
                    dto.Name = module.Name;
                    dto.Id = module.Id;
                    list.Add(dto);
                }
                foreach (var module in policyModule.FileCopyModules)
                {
                    var dto = new DtoModule();
                    dto.ModuleType = EnumModule.ModuleType.FileCopy;
                    dto.Guid = module.Guid;
                    dto.Name = module.Name;
                    dto.Id = module.Id;
                    list.Add(dto);
                }
                foreach (var module in policyModule.MessageModules)
                {
                    var dto = new DtoModule();
                    dto.ModuleType = EnumModule.ModuleType.Message;
                    dto.Guid = module.Guid;
                    dto.Name = module.Name;
                    dto.Id = module.Id;
                    list.Add(dto);
                }
                foreach (var module in policyModule.PrinterModules)
                {
                    var dto = new DtoModule();
                    dto.ModuleType = EnumModule.ModuleType.Printer;
                    dto.Guid = module.Guid;
                    dto.Name = module.Name;
                    dto.Id = module.Id;
                    list.Add(dto);
                }
                foreach (var module in policyModule.ScriptModules)
                {
                    var dto = new DtoModule();
                    dto.ModuleType = EnumModule.ModuleType.Script;
                    dto.Guid = module.Guid;
                    dto.Name = module.Name;
                    dto.Id = module.Id;
                    list.Add(dto);
                }
                foreach (var module in policyModule.SoftwareModules)
                {
                    var dto = new DtoModule();
                    dto.ModuleType = EnumModule.ModuleType.Software;
                    dto.Guid = module.Guid;
                    dto.Name = module.Name;
                    dto.Id = module.Id;
                    list.Add(dto);
                }

                foreach (var module in policyModule.WuModules)
                {
                    var dto = new DtoModule();
                    dto.ModuleType = EnumModule.ModuleType.Wupdate;
                    dto.Guid = module.Guid;
                    dto.Name = module.Name;
                    dto.Id = module.Id;
                    list.Add(dto);
                }
                foreach (var module in policyModule.WingetModules)
                {
                    var dto = new DtoModule();
                    dto.ModuleType = EnumModule.ModuleType.Winget;
                    dto.Guid = module.Guid;
                    dto.Name = module.Name;
                    dto.Id = module.Id;
                    list.Add(dto);
                }

            }
            return list;
        }
        public List<EntityWingetModule> GetComputerWingetUpdateModules(int computerId)
        {
            var policies = (from h in _context.GroupMemberships
                            join i in _context.GroupPolicies on h.GroupId equals i.GroupId
                            join j in _context.Policies on i.PolicyId equals j.Id
                            join a in _context.ActiveClientPolicies on i.PolicyId equals a.PolicyId
                            where h.ComputerId == computerId
                            select j).ToList();


            var allComputersGroupPolicies = (from i in _context.GroupPolicies
                                             join j in _context.Policies on i.PolicyId equals j.Id
                                             where i.GroupId == -1
                                             select j).ToList();
            policies.AddRange(allComputersGroupPolicies);

            var policyModules = policies.Select(policy => new PolicyRepository(_context).GetDetailed(policy.Id)).ToList();

            var list = new List<EntityWingetModule>();
            foreach (var policyModule in policyModules)
            {
                foreach (var module in policyModule.WingetModules.Where(x => x.InstallType == EnumWingetInstallType.WingetInstallType.Install && x.KeepUpdated))
                {
                    list.Add(module);
                }

            }
            return list;
        }

        public List<EntityPolicy> GetComputerPolicies(int computerId)
        {
            var policies = (from h in _context.GroupMemberships
                    join i in _context.GroupPolicies on h.GroupId equals i.GroupId
                    join j in _context.Policies on i.PolicyId equals  j.Id
                    where h.ComputerId == computerId
                    select j).ToList();

            var allComputersGroupPolicies = (from i in _context.GroupPolicies
                                             join j in _context.Policies on i.PolicyId equals j.Id
                                             where i.GroupId == -1
                                             select j).ToList();
            policies.AddRange(allComputersGroupPolicies);

            return policies;
        }

      //todo: remove this method
      //no longer used since Blazor UI
        public List<EntityComputer> SearchActiveComputers(DtoSearchFilterCategories filter, int userId)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == userId);
            var sortMode = user?.ComputerSortMode == "Default"
                ? _context.Settings.FirstOrDefault(x => x.Name == SettingStrings.ComputerSortMode)?.Value?.ToString() ?? "Last Checkin"
                : user?.ComputerSortMode ?? "Last Checkin";

            var searchText = filter.SearchText ?? "";
            var customAttribSearch = string.IsNullOrEmpty(searchText) ? "zzkjfjekrhwlhhw" : searchText;

            var query = from c in _context.Computers
                from u in _context.UserLogins.Where(x => x.ComputerId == c.Id).OrderByDescending(x => x.Id).Take(1).DefaultIfEmpty()
                from b in _context.BiosInventory.Where(x => x.ComputerId == c.Id).DefaultIfEmpty()
                from cu in _context.CustomComputerAttributes.Where(x => x.ComputerId == c.Id && x.Value.Contains(customAttribSearch)).DefaultIfEmpty()
                from sc in _context.ActiveSockets.Where(x => x.ComputerId == c.Id).DefaultIfEmpty()
                from cs in _context.ComputerSystemInventory.Where(x => x.ComputerId == c.Id).DefaultIfEmpty()
                from os in _context.OsInventory.Where(x => x.ComputerId == c.Id).DefaultIfEmpty()
                where (b.SerialNumber.Contains(searchText) || u.UserName.Contains(searchText) ||
                       c.Name.Contains(searchText) || c.Guid.Contains(searchText) || c.InstallationId.Contains(searchText) ||
                       c.UUID.Contains(searchText) || c.ImagingClientId.Contains(searchText) || c.LastIp.Contains(searchText) ||
                       cu.Value.Contains(customAttribSearch))
                      && c.ProvisionStatus != EnumProvisionStatus.Status.PreProvisioned
                      && c.ProvisionStatus != EnumProvisionStatus.Status.Archived
                      && c.ProvisionStatus != EnumProvisionStatus.Status.ProvisionApproved
                      && c.ProvisionStatus != EnumProvisionStatus.Status.ImageOnly
                select new { c, u, b, cu, sc, cs, os };

            query = sortMode == "Last Checkin"
                ? query.OrderByDescending(x => x.c.LastCheckinTime)
                : query.OrderBy(x => x.c.Name);

            return query.AsEnumerable().Select(x => new EntityComputer
            {
                Id = x.c.Id,
                Name = x.c.Name,
                LastCheckinTime = x.c.LastCheckinTime,
                LastIp = x.c.LastIp,
                ClientVersion = x.c.ClientVersion,
                LastLoggedInUser = x.u?.UserName,
                ProvisionedTime = x.c.ProvisionedTime,
                Status = x.sc?.ConnectionId != null ? "Connected" : "Disconnected",
                Manufacturer = x.cs?.Manufacturer,
                Description = x.c.Description,
                Model = x.cs?.Model,
                Domain = x.cs?.Domain,
                OsName = x.os?.Caption,
                OsVersion = x.os?.Version,
                OsBuild = x.os?.BuildNumber,
            }).Take(filter.Limit).ToList();
        }
    }
}
