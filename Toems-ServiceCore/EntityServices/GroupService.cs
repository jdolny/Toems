using System.Data;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class GroupService(EntityContext ectx)
    {
        public DtoActionResult AddGroup(EntityGroup group)
        {
            var validationResult = ValidateGroup(group, true);
            var actionResult = new DtoActionResult();
            if (validationResult.Success)
            {
                ectx.Uow.GroupRepository.Insert(group);
                ectx.Uow.Save();
                actionResult.Success = true;
                actionResult.Id = group.Id;
            }
            else
            {
                actionResult.ErrorMessage = validationResult.ErrorMessage;
            }

            return actionResult;
        }

        public DtoActionResult ClearImagingIds(int groupId)
        {
            foreach(var computer in GetGroupMembers(groupId))
            {
                computer.ImagingClientId = string.Empty;
                computer.ImagingMac = string.Empty;
                ectx.Uow.ComputerRepository.Update(computer, computer.Id);
            }
            ectx.Uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = groupId;
            return actionResult;
        }

        public DtoActionResult DeleteGroup(int groupId)
        {
            var u = GetGroup(groupId);
            if (u == null) return new DtoActionResult {ErrorMessage = "Group Not Found", Id = 0};
            if (u.IsOu) return new DtoActionResult() {ErrorMessage = "Active Directory OU's Cannot Be Deleted."};
            if (u.Id == -1) return new DtoActionResult() { ErrorMessage = "The Built-In All Computers Group Cannot Be Deleted." };
            if (u.Id == -2) return new DtoActionResult() { ErrorMessage = "The Built-In Image First Run Group Cannot Be Deleted." };
            ectx.Uow.GroupRepository.Delete(groupId);
            ectx.Uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.Id;
            return actionResult;
        }

        public bool UpdateGroupList(List<EntityGroup> groups)
        {
            foreach (var group in groups)
            {
                ectx.Uow.GroupRepository.Update(group, group.Id);
            }
            ectx.Uow.Save();
            return true;
        }

        public bool AddGroupList(List<EntityGroup> groups)
        {
            foreach (var group in groups)
            {
                if (ectx.Uow.GroupRepository.Exists(h => h.Dn == group.Dn))
                {
                    continue;
                }
                ectx.Uow.GroupRepository.Insert(group);
            }
            ectx.Uow.Save();

            var allAdGroups = GetAllAdGroups();
            var toDelete = (from adGroup in allAdGroups
                let doesExist = groups.FirstOrDefault(x => x.Dn == adGroup.Dn)
                where doesExist == null
                select adGroup.Id).ToList();

            DeleteGroupList(toDelete);
            return true;
        }

        public bool DeleteGroupList(List<int> groupIds)
        {
            foreach (var id in groupIds)
            {
                ectx.Uow.GroupRepository.Delete(id);
            }
            ectx.Uow.Save();
            return true;
        }

        public EntityGroup GetGroupParentId(EntityGroup group)
        {
            return ectx.Uow.GroupRepository.GetFirstOrDefault(x => x.Dn == group.ParentOu);
            
        }
        public EntityGroup GetGroup(int groupId)
        {
            return ectx.Uow.GroupRepository.GetById(groupId);
        }

        public bool UpdateDynamicQuery(List<EntitySmartGroupQuery> queries)
        {

            var first = queries.FirstOrDefault();
            if (first == null) return false;
            var groupId = first.GroupId;
            ectx.Uow.SmartGroupQueryRepository.DeleteRange(x => x.GroupId == groupId);
            ectx.Uow.Save();
            var existing =ectx.Uow.SmartGroupQueryRepository.GetFirstOrDefault(x => x.GroupId == groupId);
            if (existing != null) return false;
            //Assume all entries were deleted, and nothing is being added
            if (first.Table == null) return new Workflows.UpdateDynamicMemberships().Single(groupId);
            foreach (var query in queries)
            {
                ectx.Uow.SmartGroupQueryRepository.Insert(query);
            }
            ectx.Uow.Save();

            return new Workflows.UpdateDynamicMemberships().Single(groupId);
        }

        public List<EntitySmartGroupQuery> GetDynamicQuery(int groupId)
        {
            return ectx.Uow.SmartGroupQueryRepository.Get(x => x.GroupId == groupId).OrderBy(x => x.Order).ToList();
        }

        public DataSet GetDynamicMembers(List<EntitySmartGroupQuery> queries)
        {
            var sql = new Workflows.BuildSqlQuery().Run(queries);
            if(sql == null) return null;
            return new RawSqlRepository().ExecuteReader(sql);
        }


        public bool SendMessage(int id, DtoMessage message)
        {
            var computerService = new ServiceComputer();
            foreach (var computer in GetGroupMembers(id))
                computerService.SendMessage(computer.Id, message);

            return true;
        }

        public bool ForceCheckin(int id)
        {
            var computerService = new ServiceComputer();
            foreach (var computer in GetGroupMembers(id))
                computerService.ForceCheckin(computer.Id);

            return true;
        }

        public bool CollectInventory(int id)
        {
            var computerService = new ServiceComputer();
            foreach (var computer in GetGroupMembers(id))
                computerService.CollectInventory(computer.Id);

            return true;
        }

        public bool Reboot(int id)
        {
            var group = GetGroup(id);
            var list = new List<EntityGroup>();
            list.Add(group);
            return new Workflows.PowerManagement().RebootGroups(list);
        }

        public bool Shutdown(int id)
        {
            var group = GetGroup(id);
            var list = new List<EntityGroup>();
            list.Add(group);
            return new Workflows.PowerManagement().ShutdownGroups(list);
        }

        public bool Wakeup(int id)
        {
            var group = GetGroup(id);
            var list = new List<EntityGroup>();
            list.Add(group);
            return new Workflows.PowerManagement().WakeupGroups(list);
        }

        public EntityGroup GetGroupByName(string name)
        {
            return ectx.Uow.GroupRepository.GetFirstOrDefault(x => x.Name == name);
        }

        public EntityGroup GetGroupByDn(string dn)
        {
            return ectx.Uow.GroupRepository.GetFirstOrDefault(x => x.Dn == dn);
        }

        public List<EntityGroup> GetAllAdGroups()
        {
            return ectx.Uow.GroupRepository.Get(x => x.IsOu).OrderBy(x => x.Name).ToList();
        }

        public List<EntityGroup> GetAllOuGroups()
        {
            return ectx.Uow.GroupRepository.Get(x => x.IsOu).Where(x=> !x.Dn.ToLower().Contains("cn=system")).OrderBy(x => x.ParentOu).ThenBy(x=> x.Dn).ToList();
        }

        public List<EntityGroup> GetAllAdSecurityGroups()
        {
            return ectx.Uow.GroupRepository.Get(x => x.IsSecurityGroup).OrderBy(x => x.Name).ToList();

        }
        public List<EntityGroup> GetAllDynamicGroups()
        {
            return ectx.Uow.GroupRepository.Get(x => x.Type == "Dynamic").OrderBy(x => x.Name).ToList();
        }

        public List<DtoGroupWithCount> SearchGroups(DtoSearchFilterCategories filter, int userId)
        {
            var returnList = new List<DtoGroupWithCount>();
            var list = new List<EntityGroup>();
            if(filter.IncludeOus)
                list = ectx.Uow.GroupRepository.Get(s => s.Name.Contains(filter.SearchText)).OrderBy(x => x.Name).ToList();
            else
                list = ectx.Uow.GroupRepository.Get(s => s.Name.Contains(filter.SearchText) && !s.IsOu && !s.IsHidden).OrderBy(x => x.Name).ToList();
            if (list.Count == 0) return returnList;

            var categoryFilterIds = new List<int>();
            foreach (var catName in filter.Categories)
            {
                var category = ectx.Uow.CategoryRepository.GetFirstOrDefault(x => x.Name.Equals(catName));
                if (category != null)
                    categoryFilterIds.Add(category.Id);
            }

            var toRemove = new List<EntityGroup>();
            if (filter.CategoryType.Equals("Any Category"))
            {
                //do nothing, keep all

            }
            else if (filter.CategoryType.Equals("And Category"))
            {
                foreach (var group in list)
                {
                    var gCategories = GetGroupCategories(group.Id);
                    if (gCategories == null) continue;

                    if (filter.Categories.Count == 0)
                    {
                        if (gCategories.Count > 0)
                        {
                            toRemove.Add(group);
                            continue;
                        }
                    }

                    foreach (var id in categoryFilterIds)
                    {
                        if (gCategories.Any(x => x.CategoryId == id)) continue;
                        toRemove.Add(group);
                        break;
                    }
                }
            }
            else if (filter.CategoryType.Equals("Or Category"))
            {
                foreach (var group in list)
                {
                    var pCategories = GetGroupCategories(group.Id);
                    if (pCategories == null) continue;
                    if (filter.Categories.Count == 0)
                    {
                        if (pCategories.Count > 0)
                        {
                            toRemove.Add(group);
                            continue;
                        }
                    }
                    var catFound = false;
                    foreach (var id in categoryFilterIds)
                    {
                        if (pCategories.Any(x => x.CategoryId == id))
                        {
                            catFound = true;
                            break;
                        }

                    }
                    if (!catFound)
                        toRemove.Add(group);
                }
            }

            foreach (var p in toRemove)
            {
                list.Remove(p);
            }

            foreach (var group in list)
            {
                var groupWithcount = new DtoGroupWithCount();
                groupWithcount.Id = group.Id;
                if (!group.IsOu)
                    groupWithcount.Name = group.Name;
                else
                    groupWithcount.Name = group.Dn;
                groupWithcount.Type = group.Type;
                groupWithcount.Description = group.Description;
                if (group.Id == -1)
                    groupWithcount.MemberCount = new ServiceComputer().TotalActiveCount();
                else
                    groupWithcount.MemberCount = ectx.Uow.GroupMembershipRepository.Count(x => x.GroupId == group.Id);
                if (groupWithcount.MemberCount == "0" && group.IsOu) continue;
                returnList.Add(groupWithcount);
            }

            var groupAcl = new ServiceUser().GetAllowedGroups(userId);
            if (!groupAcl.GroupManagementEnforced)
                return returnList.Take(filter.Limit).OrderBy(x => x.Name).ToList();
            else
            {
                var userGroups = new List<DtoGroupWithCount>();
                foreach (var group in returnList)
                {
                    if (groupAcl.AllowedGroupIds.Contains(group.Id))
                        userGroups.Add(group);
                }


                return userGroups.Take(filter.Limit).OrderBy(x => x.Name).ToList();
            }
        }

        public string TotalCount()
        {
            return ectx.Uow.GroupRepository.Count(x => !x.IsOu);
        }

        public DtoActionResult UpdateGroup(EntityGroup group)
        {
            var u = GetGroup(group.Id);
            if (u == null) return new DtoActionResult {ErrorMessage = "Group Not Found", Id = 0};
            var validationResult = ValidateGroup(group, false);
            var actionResult = new DtoActionResult();
            if (validationResult.Success)
            {
                ectx.Uow.GroupRepository.Update(group, group.Id);
                ectx.Uow.Save();
                actionResult.Success = true;
                actionResult.Id = group.Id;
               
            }
            else
            {
                actionResult.ErrorMessage = validationResult.ErrorMessage;
            }

            return actionResult;
        }

        private DtoValidationResult ValidateGroup(EntityGroup group, bool isNew)
        {
            var validationResult = new DtoValidationResult { Success = true };

            if (string.IsNullOrEmpty(group.Name) || !group.Name.All(c => char.IsLetterOrDigit(c) || c == '_' || c == '-' || c == ' '))
            {
                validationResult.Success = false;
                validationResult.ErrorMessage = "Group Name Is Not Valid";
                return validationResult;
            }

            if (isNew)
            {
                if (ectx.Uow.GroupRepository.Exists(h => h.Name == group.Name && h.IsOu == false))
                {
                    validationResult.Success = false;
                    validationResult.ErrorMessage = "A Group With This Name Already Exists";
                    return validationResult;
                }
            }
            else
            {
                var originalGroup = ectx.Uow.GroupRepository.GetById(group.Id);
                if (originalGroup.Name != group.Name)
                {
                    if (ectx.Uow.GroupRepository.Exists(h => h.Name == group.Name && h.IsOu == false))
                    {
                        validationResult.Success = false;
                        validationResult.ErrorMessage = "A Group With This Name Already Exists";
                        return validationResult;
                    }
                }
            }

            return validationResult;
        }

        public List<GroupPolicyDetailed> GetAssignedPolicies(int groupId, DtoSearchFilter filter)
        {
            var assignedPolices = ectx.Uow.GroupPolicyRepository.Get(x => x.GroupId == groupId);
            var list = assignedPolices.Select(groupPolicy => ectx.Uow.GroupPolicyRepository.GetDetailed(groupPolicy.PolicyId,groupPolicy.Id)).ToList();

            if (string.IsNullOrEmpty(filter.SearchText))
                return list.OrderBy(x => x.PolicyOrder).Take(filter.Limit).ToList();
            else
                return list.Where(s => s.Policy.Name.Contains(filter.SearchText)).OrderBy(x => x.PolicyOrder).Take(filter.Limit).ToList();
        }

        public bool DeleteAllMembershipsForGroup(int groupId)
        {
            ectx.Uow.GroupMembershipRepository.DeleteRange(x => x.GroupId == groupId);
            ectx.Uow.Save();
            return true;
        }

        public bool DeleteMembership(int computerId, int groupId)
        {
            ectx.Uow.GroupMembershipRepository.DeleteRange(
                g => g.ComputerId == computerId && g.GroupId == groupId);
            ectx.Uow.Save();
            return true;
        }

        public string GetGroupMemberCount(int groupId)
        {
            return ectx.Uow.GroupMembershipRepository.Count(g => g.GroupId == groupId);
        }

        public List<EntityComputer> SearchGroupMembers(int groupId, DtoSearchFilter filter)
        {
            return ectx.Uow.GroupRepository.GetGroupMembers(groupId, filter.SearchText);
        }

        public List<EntityComputer> GetGroupMembers(int groupId)
        {
            return ectx.Uow.GroupRepository.GetGroupMembers(groupId);
        }

        public EntityActiveGroupPolicy GetActiveGroupPolicy(int groupId)
        {
            return ectx.Uow.ActiveGroupPoliciesRepository.GetFirstOrDefault(x => x.GroupId == groupId);
        }

        public List<EntityGroupCategory> GetGroupCategories(int groupId)
        {
            return ectx.Uow.GroupCategoryRepository.Get(x => x.GroupId == groupId);
        }

        public List<DtoProcessWithTime> GetGroupProcessTimes(DateTime dateCutoff, int limit, int groupId)
        {
            return new ReportRepository().GetTopProcessTimesForGroup(dateCutoff, limit, groupId);
        }

        public List<DtoProcessWithCount> GetGroupProcessCounts(DateTime dateCutoff, int limit, int groupId)
        {
            return new ReportRepository().GetTopProcessCountsForGroup(dateCutoff, limit,groupId);
        }

        public int StartGroupUnicast(int groupId, int userId)
        {
            var count = 0;
            var members = ectx.Uow.GroupRepository.GetGroupMembersWithImages(groupId, "");
            foreach (var computer in members)
            {
                if (new Toems_Service.Workflows.Unicast(computer.Id, "deploy", userId,groupId).Start().Contains("Successfully"))
                    count++;
            }
            return count;
        }

        public bool StartGroupWinPeThread(int groupId, int userId)
        {
            Task.Run(() => StartGroupWinPe(groupId, userId));
            return true;
        }

        public async Task StartGroupWinPe(int groupId, int userId)
        {
            await Task.Delay(100);
            var members = ectx.Uow.GroupRepository.GetGroupMembers(groupId, "");
            foreach (var computer in members)
            {
                new ServiceComputer().DeployImageViaWindows(computer.Id, userId);
            }
        }
    }
}