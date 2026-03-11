using System.Data;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;
using Toems_ServiceCore.Infrastructure;
using Toems_ServiceCore.Workflows;

namespace Toems_ServiceCore.EntityServices
{
    public class GroupService(ServiceContext ctx)
    {
        public DtoActionResult AddGroup(EntityGroup group)
        {
            var validationResult = ValidateGroup(group, true);
            var actionResult = new DtoActionResult();
            if (validationResult.Success)
            {
                ctx.Uow.GroupRepository.Insert(group);
                ctx.Uow.Save();
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
                ctx.Uow.ComputerRepository.Update(computer, computer.Id);
            }
            ctx.Uow.Save();
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
            ctx.Uow.GroupRepository.Delete(groupId);
            ctx.Uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.Id;
            return actionResult;
        }

        public bool UpdateGroupList(List<EntityGroup> groups)
        {
            foreach (var group in groups)
            {
                ctx.Uow.GroupRepository.Update(group, group.Id);
            }
            ctx.Uow.Save();
            return true;
        }

        public bool AddGroupList(List<EntityGroup> groups)
        {
            foreach (var group in groups)
            {
                if (ctx.Uow.GroupRepository.Exists(h => h.Dn == group.Dn))
                {
                    continue;
                }
                ctx.Uow.GroupRepository.Insert(group);
            }
            ctx.Uow.Save();

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
                ctx.Uow.GroupRepository.Delete(id);
            }
            ctx.Uow.Save();
            return true;
        }

        public EntityGroup GetGroupParentId(EntityGroup group)
        {
            return ctx.Uow.GroupRepository.GetFirstOrDefault(x => x.Dn == group.ParentOu);
            
        }
        public EntityGroup GetGroup(int groupId)
        {
            return ctx.Uow.GroupRepository.GetById(groupId);
        }

        public bool UpdateDynamicQuery(List<EntitySmartGroupQuery> queries)
        {

            var first = queries.FirstOrDefault();
            if (first == null) return false;
            var groupId = first.GroupId;
            ctx.Uow.SmartGroupQueryRepository.DeleteRange(x => x.GroupId == groupId);
            ctx.Uow.Save();
            var existing =ctx.Uow.SmartGroupQueryRepository.GetFirstOrDefault(x => x.GroupId == groupId);
            if (existing != null) return false;
            //Assume all entries were deleted, and nothing is being added
            if (first.Table == null) return UpdateGroupMemberships(groupId);
            foreach (var query in queries)
            {
                ctx.Uow.SmartGroupQueryRepository.Insert(query);
            }
            ctx.Uow.Save();

            return UpdateGroupMemberships(groupId);
        }

        public List<EntitySmartGroupQuery> GetDynamicQuery(int groupId)
        {
            return ctx.Uow.SmartGroupQueryRepository.Get(x => x.GroupId == groupId).OrderBy(x => x.Order).ToList();
        }

        public DataSet GetDynamicMembers(List<EntitySmartGroupQuery> queries)
        {
            var sql = ctx.BuildSqlQuery.Run(queries);
            if(sql == null) return null;
            return new RawSqlRepository().ExecuteReader(sql);
        }


        public bool SendMessage(int id, DtoMessage message)
        {
            foreach (var computer in GetGroupMembers(id))
                ctx.Computer.SendMessage(computer.Id, message);

            return true;
        }

        public bool ForceCheckin(int id)
        {
            foreach (var computer in GetGroupMembers(id))
                ctx.Computer.ForceCheckin(computer.Id);

            return true;
        }

        public bool CollectInventory(int id)
        {
            foreach (var computer in GetGroupMembers(id))
                ctx.Computer.CollectInventory(computer.Id);

            return true;
        }

        public bool Reboot(int id)
        {
            var group = GetGroup(id);
            var list = new List<EntityGroup>();
            list.Add(group);
            return ctx.PowerManagement.RebootGroups(list);
        }

        public bool Shutdown(int id)
        {
            var group = GetGroup(id);
            var list = new List<EntityGroup>();
            list.Add(group);
            return ctx.PowerManagement.ShutdownGroups(list);
        }

        public bool Wakeup(int id)
        {
            var group = GetGroup(id);
            var list = new List<EntityGroup>();
            list.Add(group);
            return ctx.PowerManagement.WakeupGroups(list);
        }

        public EntityGroup GetGroupByName(string name)
        {
            return ctx.Uow.GroupRepository.GetFirstOrDefault(x => x.Name == name);
        }

        public EntityGroup GetGroupByDn(string dn)
        {
            return ctx.Uow.GroupRepository.GetFirstOrDefault(x => x.Dn == dn);
        }

        public List<EntityGroup> GetAllAdGroups()
        {
            return ctx.Uow.GroupRepository.Get(x => x.IsOu).OrderBy(x => x.Name).ToList();
        }

        public List<EntityGroup> GetAllOuGroups()
        {
            return ctx.Uow.GroupRepository.Get(x => x.IsOu).Where(x=> !x.Dn.ToLower().Contains("cn=system")).OrderBy(x => x.ParentOu).ThenBy(x=> x.Dn).ToList();
        }

        public List<EntityGroup> GetAllAdSecurityGroups()
        {
            return ctx.Uow.GroupRepository.Get(x => x.IsSecurityGroup).OrderBy(x => x.Name).ToList();

        }
        public List<EntityGroup> GetAllDynamicGroups()
        {
            return ctx.Uow.GroupRepository.Get(x => x.Type == "Dynamic").OrderBy(x => x.Name).ToList();
        }

        public List<DtoGroupWithCount> SearchGroups(DtoSearchFilterCategories filter, int userId)
        {
            var returnList = new List<DtoGroupWithCount>();
            var list = new List<EntityGroup>();
            if(filter.IncludeOus)
                list = ctx.Uow.GroupRepository.Get(s => s.Name.Contains(filter.SearchText)).OrderBy(x => x.Name).ToList();
            else
                list = ctx.Uow.GroupRepository.Get(s => s.Name.Contains(filter.SearchText) && !s.IsOu && !s.IsHidden).OrderBy(x => x.Name).ToList();
            if (list.Count == 0) return returnList;

            var categoryFilterIds = new List<int>();
            foreach (var catName in filter.Categories)
            {
                var category = ctx.Uow.CategoryRepository.GetFirstOrDefault(x => x.Name.Equals(catName));
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
                    groupWithcount.MemberCount = ctx.Computer.TotalActiveCount();
                else
                    groupWithcount.MemberCount = ctx.Uow.GroupMembershipRepository.Count(x => x.GroupId == group.Id);
                if (groupWithcount.MemberCount == "0" && group.IsOu) continue;
                returnList.Add(groupWithcount);
            }

            var groupAcl = ctx.User.GetAllowedGroups(userId);
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
            return ctx.Uow.GroupRepository.Count(x => !x.IsOu);
        }

        public DtoActionResult UpdateGroup(EntityGroup group)
        {
            var u = GetGroup(group.Id);
            if (u == null) return new DtoActionResult {ErrorMessage = "Group Not Found", Id = 0};
            var validationResult = ValidateGroup(group, false);
            var actionResult = new DtoActionResult();
            if (validationResult.Success)
            {
                ctx.Uow.GroupRepository.Update(group, group.Id);
                ctx.Uow.Save();
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
                if (ctx.Uow.GroupRepository.Exists(h => h.Name == group.Name && h.IsOu == false))
                {
                    validationResult.Success = false;
                    validationResult.ErrorMessage = "A Group With This Name Already Exists";
                    return validationResult;
                }
            }
            else
            {
                var originalGroup = ctx.Uow.GroupRepository.GetById(group.Id);
                if (originalGroup.Name != group.Name)
                {
                    if (ctx.Uow.GroupRepository.Exists(h => h.Name == group.Name && h.IsOu == false))
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
            var assignedPolices = ctx.Uow.GroupPolicyRepository.Get(x => x.GroupId == groupId);
            var list = assignedPolices.Select(groupPolicy => ctx.Uow.GroupPolicyRepository.GetDetailed(groupPolicy.PolicyId,groupPolicy.Id)).ToList();

            if (string.IsNullOrEmpty(filter.SearchText))
                return list.OrderBy(x => x.PolicyOrder).Take(filter.Limit).ToList();
            else
                return list.Where(s => s.Policy.Name.Contains(filter.SearchText)).OrderBy(x => x.PolicyOrder).Take(filter.Limit).ToList();
        }

        public bool DeleteAllMembershipsForGroup(int groupId)
        {
            ctx.Uow.GroupMembershipRepository.DeleteRange(x => x.GroupId == groupId);
            ctx.Uow.Save();
            return true;
        }

        public bool DeleteMembership(int computerId, int groupId)
        {
            ctx.Uow.GroupMembershipRepository.DeleteRange(
                g => g.ComputerId == computerId && g.GroupId == groupId);
            ctx.Uow.Save();
            return true;
        }

        public string GetGroupMemberCount(int groupId)
        {
            return ctx.Uow.GroupMembershipRepository.Count(g => g.GroupId == groupId);
        }

        public List<EntityComputer> SearchGroupMembers(int groupId, DtoSearchFilter filter)
        {
            return ctx.Uow.GroupRepository.GetGroupMembers(groupId, filter.SearchText);
        }

        public List<EntityComputer> GetGroupMembers(int groupId)
        {
            return ctx.Uow.GroupRepository.GetGroupMembers(groupId);
        }

        public EntityActiveGroupPolicy GetActiveGroupPolicy(int groupId)
        {
            return ctx.Uow.ActiveGroupPoliciesRepository.GetFirstOrDefault(x => x.GroupId == groupId);
        }

        public List<EntityGroupCategory> GetGroupCategories(int groupId)
        {
            return ctx.Uow.GroupCategoryRepository.Get(x => x.GroupId == groupId);
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
            var members = ctx.Uow.GroupRepository.GetGroupMembersWithImages(groupId, "");
            foreach (var computer in members)
            {
                ctx.Unicast.InitGroup(computer.Id, "deploy", userId,groupId);
                if (ctx.Unicast.Start().Contains("Successfully"))
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
            var members = ctx.Uow.GroupRepository.GetGroupMembers(groupId, "");
            foreach (var computer in members)
            {
                ctx.Computer.DeployImageViaWindows(computer.Id, userId);
            }
        }
        
         public bool UpdateAllGroupMemberships()
        {
            var groups = GetAllDynamicGroups();
            if (groups == null) return false;
            foreach (var group in groups)
            {
                UpdateDynamicMemberships(group.Id);
            }

            return true;
        }

        public bool UpdateGroupMemberships(int groupId)
        {
            return UpdateDynamicMemberships(groupId);
        }

        private bool UpdateDynamicMemberships(int groupId)
        {
            var queries = GetDynamicQuery(groupId);
            var members = GetDynamicMembers(queries);
            if (members == null)
            {
                ctx.Uow.GroupMembershipRepository.DeleteRange(x => x.GroupId == groupId);
                ctx.Uow.Save();
                return true;
            }

            var membershipList = new List<EntityGroupMembership>();
            foreach (DataTable table in members.Tables)
            {
                if (table.Rows.Count == 0)
                {
                    ctx.Uow.GroupMembershipRepository.DeleteRange(x => x.GroupId == groupId);
                    ctx.Uow.Save();
                    return true;
                }
                foreach (DataRow row in table.Rows)
                {
                    var membership = new EntityGroupMembership();
                    var computerId = row["computer_id"];
                    membership.ComputerId = Convert.ToInt32(computerId);
                    membership.GroupId = groupId;
                    membershipList.Add(membership);
                }
            }

            //Delete members that no longer belong
            var existingMembers = GetGroupMembers(groupId);
            foreach (var existingMember in existingMembers)
            {
                if(membershipList.All(x => x.ComputerId != existingMember.Id))
                {
                    ctx.Uow.GroupMembershipRepository.DeleteRange(x => x.ComputerId == existingMember.Id && x.GroupId == groupId);
                }
                ctx.Uow.Save();
            }

            //add the new members
            var result = ctx.GroupMembership.AddMembership(membershipList);
            if(result != null)
                if (result.Success)
                    return true;

            return false;
        }
    }
}