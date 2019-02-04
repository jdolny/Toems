using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;
using Toems_Service.Entity;

namespace Toems_Service.Workflows
{
    public class UpdateDynamicMemberships
    {
        private ServiceGroup _groupService;
        private readonly UnitOfWork _uow;
        public UpdateDynamicMemberships()
        {
            _groupService = new ServiceGroup();
            _uow = new UnitOfWork();
        }
        public bool All()
        {
            var groups = _groupService.GetAllDynamicGroups();
            if (groups == null) return false;
            foreach (var group in groups)
            {
                Update(group.Id);
            }

            return true;
        }

        public bool Single(int groupId)
        {
            return Update(groupId);
        }

        private bool Update(int groupId)
        {
            var queries = _groupService.GetDynamicQuery(groupId);
            var members = _groupService.GetDynamicMembers(queries);
            if (members == null)
            {
                _uow.GroupMembershipRepository.DeleteRange(x => x.GroupId == groupId);
                _uow.Save();
                return true;
            }

            var membershipList = new List<EntityGroupMembership>();
            foreach (DataTable table in members.Tables)
            {
                if (table.Rows.Count == 0)
                {
                    _uow.GroupMembershipRepository.DeleteRange(x => x.GroupId == groupId);
                    _uow.Save();
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
            var existingMembers = _groupService.GetGroupMembers(groupId);
            foreach (var existingMember in existingMembers)
            {
                if(membershipList.All(x => x.ComputerId != existingMember.Id))
                {
                    _uow.GroupMembershipRepository.DeleteRange(x => x.ComputerId == existingMember.Id && x.GroupId == groupId);
                }
                _uow.Save();
            }

            //add the new members
            var result = new ServiceGroupMembership().AddMembership(membershipList);
            if(result != null)
                if (result.Success)
                    return true;

            return false;
        }
    }
}
