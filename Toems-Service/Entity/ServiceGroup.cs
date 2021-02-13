using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceGroup
    {
        private readonly UnitOfWork _uow;

        public ServiceGroup()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult AddGroup(EntityGroup group)
        {
            var validationResult = ValidateGroup(group, true);
            var actionResult = new DtoActionResult();
            if (validationResult.Success)
            {
                _uow.GroupRepository.Insert(group);
                _uow.Save();
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
                _uow.ComputerRepository.Update(computer, computer.Id);
            }
            _uow.Save();
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
            _uow.GroupRepository.Delete(groupId);
            _uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.Id;
            return actionResult;
        }

        public bool UpdateGroupList(List<EntityGroup> groups)
        {
            foreach (var group in groups)
            {
                _uow.GroupRepository.Update(group, group.Id);
            }
            _uow.Save();
            return true;
        }

        public bool AddGroupList(List<EntityGroup> groups)
        {
            foreach (var group in groups)
            {
                if (_uow.GroupRepository.Exists(h => h.Dn == group.Dn))
                {
                    continue;
                }
                _uow.GroupRepository.Insert(group);
            }
            _uow.Save();

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
                _uow.GroupRepository.Delete(id);
            }
            _uow.Save();
            return true;
        }

        public EntityGroup GetGroupParentId(EntityGroup group)
        {
            return _uow.GroupRepository.GetFirstOrDefault(x => x.Dn == group.ParentOu);
            
        }
        public EntityGroup GetGroup(int groupId)
        {
            return _uow.GroupRepository.GetById(groupId);
        }

        public bool UpdateDynamicQuery(List<EntitySmartGroupQuery> queries)
        {

            var first = queries.FirstOrDefault();
            if (first == null) return false;
            var groupId = first.GroupId;
            _uow.SmartGroupQueryRepository.DeleteRange(x => x.GroupId == groupId);
            _uow.Save();
            var existing =_uow.SmartGroupQueryRepository.GetFirstOrDefault(x => x.GroupId == groupId);
            if (existing != null) return false;
            //Assume all entries were deleted, and nothing is being added
            if (first.Table == null) return new Workflows.UpdateDynamicMemberships().Single(groupId);
            foreach (var query in queries)
            {
                _uow.SmartGroupQueryRepository.Insert(query);
            }
            _uow.Save();

            return new Workflows.UpdateDynamicMemberships().Single(groupId);
        }

        public List<EntitySmartGroupQuery> GetDynamicQuery(int groupId)
        {
            return _uow.SmartGroupQueryRepository.Get(x => x.GroupId == groupId).OrderBy(x => x.Order).ToList();
        }

        public DataSet GetDynamicMembers(List<EntitySmartGroupQuery> queries)
        {
            var sql = new Workflows.BuildSqlQuery().Run(queries);
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
            return _uow.GroupRepository.GetFirstOrDefault(x => x.Name == name);
        }

        public EntityGroup GetGroupByDn(string dn)
        {
            return _uow.GroupRepository.GetFirstOrDefault(x => x.Dn == dn);
        }

        public List<EntityGroup> GetAllAdGroups()
        {
            return _uow.GroupRepository.Get(x => x.IsOu).OrderBy(x => x.Name).ToList();
        }

        public List<EntityGroup> GetAllDynamicGroups()
        {
            return _uow.GroupRepository.Get(x => x.Type == "Dynamic").OrderBy(x => x.Name).ToList();
        }

        public List<DtoGroupWithCount> SearchGroups(DtoSearchFilterCategories filter)
        {
            var returnList = new List<DtoGroupWithCount>();
            var list = new List<EntityGroup>();
            if(filter.IncludeOus)
                list = _uow.GroupRepository.Get(s => s.Name.Contains(filter.SearchText)).OrderBy(x => x.Name).ToList();
            else
                list = _uow.GroupRepository.Get(s => s.Name.Contains(filter.SearchText) && !s.IsOu).OrderBy(x => x.Name).ToList();
            if (list.Count == 0) return returnList;

            var categoryFilterIds = new List<int>();
            foreach (var catName in filter.Categories)
            {
                var category = _uow.CategoryRepository.GetFirstOrDefault(x => x.Name.Equals(catName));
                if (category != null)
                    categoryFilterIds.Add(category.Id);
            }

            var toRemove = new List<EntityGroup>();
            if (filter.CategoryType.Equals("Any Category"))
            {
                foreach(var group in list)
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
                        groupWithcount.MemberCount = _uow.GroupMembershipRepository.Count(x => x.GroupId == group.Id);
                    if (groupWithcount.MemberCount == "0" && group.IsOu) continue;
                    returnList.Add(groupWithcount);
                }
                return returnList.Take(filter.Limit).OrderBy(x => x.Name).ToList();
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
                    groupWithcount.MemberCount = _uow.GroupMembershipRepository.Count(x => x.GroupId == group.Id);
                if (groupWithcount.MemberCount == "0" && group.IsOu) continue;
                returnList.Add(groupWithcount);
            }
            return returnList.Take(filter.Limit).OrderBy(x => x.Name).ToList();
        }

        public string TotalCount()
        {
            return _uow.GroupRepository.Count(x => !x.IsOu);
        }

        public DtoActionResult UpdateGroup(EntityGroup group)
        {
            var u = GetGroup(group.Id);
            if (u == null) return new DtoActionResult {ErrorMessage = "Group Not Found", Id = 0};
            var validationResult = ValidateGroup(group, false);
            var actionResult = new DtoActionResult();
            if (validationResult.Success)
            {
                _uow.GroupRepository.Update(group, group.Id);
                _uow.Save();
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
                if (_uow.GroupRepository.Exists(h => h.Name == group.Name && h.IsOu == false))
                {
                    validationResult.Success = false;
                    validationResult.ErrorMessage = "A Group With This Name Already Exists";
                    return validationResult;
                }
            }
            else
            {
                var originalGroup = _uow.GroupRepository.GetById(group.Id);
                if (originalGroup.Name != group.Name)
                {
                    if (_uow.GroupRepository.Exists(h => h.Name == group.Name && h.IsOu == false))
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
            var assignedPolices = _uow.GroupPolicyRepository.Get(x => x.GroupId == groupId);
            var list = assignedPolices.Select(groupPolicy => _uow.GroupPolicyRepository.GetDetailed(groupPolicy.PolicyId,groupPolicy.Id)).ToList();

            if (string.IsNullOrEmpty(filter.SearchText))
                return list.OrderBy(x => x.PolicyOrder).Take(filter.Limit).ToList();
            else
                return list.Where(s => s.Policy.Name.Contains(filter.SearchText)).OrderBy(x => x.PolicyOrder).Take(filter.Limit).ToList();
        }

        public bool DeleteAllMembershipsForGroup(int groupId)
        {
            _uow.GroupMembershipRepository.DeleteRange(x => x.GroupId == groupId);
            _uow.Save();
            return true;
        }

        public bool DeleteMembership(int computerId, int groupId)
        {
            _uow.GroupMembershipRepository.DeleteRange(
                g => g.ComputerId == computerId && g.GroupId == groupId);
            _uow.Save();
            return true;
        }

        public string GetGroupMemberCount(int groupId)
        {
            return _uow.GroupMembershipRepository.Count(g => g.GroupId == groupId);
        }

        public List<EntityComputer> SearchGroupMembers(int groupId, DtoSearchFilter filter)
        {
            return _uow.GroupRepository.GetGroupMembers(groupId, filter.SearchText);
        }

        public List<EntityComputer> GetGroupMembers(int groupId)
        {
            return _uow.GroupRepository.GetGroupMembers(groupId);
        }

        public EntityActiveGroupPolicy GetActiveGroupPolicy(int groupId)
        {
            return _uow.ActiveGroupPoliciesRepository.GetFirstOrDefault(x => x.GroupId == groupId);
        }

        public List<EntityGroupCategory> GetGroupCategories(int groupId)
        {
            return _uow.GroupCategoryRepository.Get(x => x.GroupId == groupId);
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
            var members = _uow.GroupRepository.GetGroupMembersWithImages(groupId, "");
            foreach (var computer in members)
            {
                if (new Toems_Service.Workflows.Unicast(computer.Id, "deploy", userId,groupId).Start().Contains("Successfully"))
                    count++;
            }
            return count;
        }
    }
}