using System.Collections.Generic;
using System.Linq;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceGroupPolicy
    {
        private readonly UnitOfWork _uow;

        public ServiceGroupPolicy()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult AddGroupPolicies(List<EntityGroupPolicy> groupPolicies)
        {
            var firstGroup = groupPolicies.FirstOrDefault();
            if(firstGroup == null) return new DtoActionResult { ErrorMessage = "No Groups Were In The List", Id = 0 };
            var allSameGroup = groupPolicies.All(x => x.GroupId == firstGroup.GroupId);
            if (!allSameGroup) return new DtoActionResult { ErrorMessage = "The List Of Policies Must Only Be For A Single Group", Id = 0 };
            
            foreach (var groupPolicy in groupPolicies)
            {
                _uow.GroupPolicyRepository.Insert(groupPolicy);
            }
            _uow.Save();

            return new Workflows.GenerateClientGroupPolicy().Execute(firstGroup.GroupId);
        }

        public DtoActionResult AddGroupPolicy(EntityGroupPolicy groupPolicy)
        {
            var actionResult = new DtoActionResult();

            _uow.GroupPolicyRepository.Insert(groupPolicy);
            _uow.Save();
            actionResult.Success = true;
            actionResult.Id = groupPolicy.Id;


            return actionResult;
        }

        public DtoActionResult DeleteGroupPolicy(int groupPolicyId)
        {
            var u = GetGroupPolicy(groupPolicyId);
            if (u == null) return new DtoActionResult { ErrorMessage = "Group Policy Not Found", Id = 0 };
            _uow.GroupPolicyRepository.Delete(groupPolicyId);
            _uow.Save();
            //recalculate Active group policies
            new Workflows.GenerateClientGroupPolicy().Execute(u.GroupId);
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.Id;
            return actionResult;
        }

        public EntityGroupPolicy GetGroupPolicy(int groupPolicyId)
        {
            return _uow.GroupPolicyRepository.GetById(groupPolicyId);
        }

        public DtoActionResult UpdateGroupPolicy(EntityGroupPolicy groupPolicy)
        {
            var u = GetGroupPolicy(groupPolicy.Id);
            if (u == null) return new DtoActionResult { ErrorMessage = "Group Policy Not Found", Id = 0 };
            var actionResult = new DtoActionResult();

            _uow.GroupPolicyRepository.Update(groupPolicy, groupPolicy.Id);
            _uow.Save();
            //recalculate Active group policies
            new Workflows.GenerateClientGroupPolicy().Execute(u.GroupId);
            actionResult.Success = true;
            actionResult.Id = groupPolicy.Id;


            return actionResult;
        }
    }
}