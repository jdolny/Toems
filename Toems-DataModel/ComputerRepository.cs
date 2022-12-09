using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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

        public List<EntityGroup> GetComputerPreventShutdownGroups(int computerId)
        {
            return (from c in _context.Computers
                join gm in _context.GroupMemberships on c.Id equals gm.ComputerId
                join gr in _context.Groups on gm.GroupId equals gr.Id
                where c.Id == computerId && gr.PreventShutdown
                select gr).ToList();
        }

        public List<EntityComputer> SearchActiveComputers(DtoSearchFilterCategories filter, int userId)
        {
            var user = _context.Users.Where(x => x.Id == userId).FirstOrDefault();
            var sortMode = "";
            if (user.ComputerSortMode.Equals("Default"))
            {
                var sortModeSetting = _context.Settings.Where(x => x.Name == SettingStrings.ComputerSortMode).FirstOrDefault();
                sortMode = sortModeSetting == null ? "Last Checkin" : sortModeSetting.Value.ToString();
            }
            else
                sortMode = user.ComputerSortMode;
            if (string.IsNullOrEmpty(sortMode))
                sortMode = "Last Checkin";

            if (sortMode.Equals("Last Checkin"))
            {
                return (from c in _context.Computers
                                 from u in _context.UserLogins.Where(x => x.ComputerId == c.Id).OrderByDescending(x => x.Id).Take(1).DefaultIfEmpty()
                                 from b in _context.BiosInventory.Where(x => x.ComputerId == c.Id).DefaultIfEmpty()
                                 where
                                 (
                                 b.SerialNumber.Contains(filter.SearchText) || u.UserName.Contains(filter.SearchText) ||
                                 c.Name.Contains(filter.SearchText) || c.Guid.Contains(filter.SearchText) || c.InstallationId.Contains(filter.SearchText) ||
                                 c.UUID.Contains(filter.SearchText) || c.ImagingClientId.Contains(filter.SearchText) || c.LastIp.Contains(filter.SearchText))

                                 && c.ProvisionStatus != EnumProvisionStatus.Status.PreProvisioned && c.ProvisionStatus != EnumProvisionStatus.Status.Archived
                                 && c.ProvisionStatus != EnumProvisionStatus.Status.ProvisionApproved
                                 && c.ProvisionStatus != EnumProvisionStatus.Status.ImageOnly
                                 orderby c.LastCheckinTime descending
                                 select new
                                 {
                                     id = c.Id,
                                     name = c.Name,
                                     lastCheckinTime = c.LastCheckinTime,
                                     lastIp = c.LastIp,
                                     clientVersion = c.ClientVersion,
                                     lastUser = u.UserName,
                                     provision = c.ProvisionedTime,
                                 }).AsEnumerable().Select(x => new EntityComputer()
                                 {
                                     Id = x.id,
                                     Name = x.name,
                                     LastCheckinTime = x.lastCheckinTime,
                                     LastIp = x.lastIp,
                                     ClientVersion = x.clientVersion,
                                     LastLoggedInUser = x.lastUser,
                                     ProvisionedTime = x.provision
                                 }).Take(filter.Limit).ToList();
            }
            else
            {
                return (from c in _context.Computers
                        from u in _context.UserLogins.Where(x => x.ComputerId == c.Id).OrderByDescending(x => x.Id).Take(1).DefaultIfEmpty()
                        from b in _context.BiosInventory.Where(x => x.ComputerId == c.Id).DefaultIfEmpty()
                        where
                        (
                        b.SerialNumber.Contains(filter.SearchText) || u.UserName.Contains(filter.SearchText) ||
                        c.Name.Contains(filter.SearchText) || c.Guid.Contains(filter.SearchText) || c.InstallationId.Contains(filter.SearchText) ||
                        c.UUID.Contains(filter.SearchText) || c.ImagingClientId.Contains(filter.SearchText) || c.LastIp.Contains(filter.SearchText))

                        && c.ProvisionStatus != EnumProvisionStatus.Status.PreProvisioned && c.ProvisionStatus != EnumProvisionStatus.Status.Archived
                        && c.ProvisionStatus != EnumProvisionStatus.Status.ProvisionApproved
                        && c.ProvisionStatus != EnumProvisionStatus.Status.ImageOnly
                        orderby c.Name
                        select new
                        {
                            id = c.Id,
                            name = c.Name,
                            lastCheckinTime = c.LastCheckinTime,
                            lastIp = c.LastIp,
                            clientVersion = c.ClientVersion,
                            lastUser = u.UserName,
                            provision = c.ProvisionedTime,
                        }).AsEnumerable().Select(x => new EntityComputer()
                        {
                            Id = x.id,
                            Name = x.name,
                            LastCheckinTime = x.lastCheckinTime,
                            LastIp = x.lastIp,
                            ClientVersion = x.clientVersion,
                            LastLoggedInUser = x.lastUser,
                            ProvisionedTime = x.provision
                        }).Take(filter.Limit).ToList();
            }

           

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

      
    }
}
