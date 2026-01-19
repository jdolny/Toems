using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceGroupPolicy(EntityContext ectx)
    {
        public DtoActionResult AddGroupPolicies(List<EntityGroupPolicy> groupPolicies)
        {
            var firstGroup = groupPolicies.FirstOrDefault();
            if(firstGroup == null) return new DtoActionResult { ErrorMessage = "No Groups Were In The List", Id = 0 };
            var allSameGroup = groupPolicies.All(x => x.GroupId == firstGroup.GroupId);
            if (!allSameGroup) return new DtoActionResult { ErrorMessage = "The List Of Policies Must Only Be For A Single Group", Id = 0 };
            
            foreach (var groupPolicy in groupPolicies)
            {
                ectx.Uow.GroupPolicyRepository.Insert(groupPolicy);
            }
            ectx.Uow.Save();

            return new Toems_Service.Workflows.GenerateClientGroupPolicy().Execute(firstGroup.GroupId);
        }

        public DtoActionResult AddGroupPolicy(EntityGroupPolicy groupPolicy)
        {
            var actionResult = new DtoActionResult();

            ectx.Uow.GroupPolicyRepository.Insert(groupPolicy);
            ectx.Uow.Save();
            actionResult.Success = true;
            actionResult.Id = groupPolicy.Id;


            return actionResult;
        }

        public DtoActionResult DeleteGroupPolicy(int groupPolicyId)
        {
            var u = GetGroupPolicy(groupPolicyId);
            if (u == null) return new DtoActionResult { ErrorMessage = "Group Policy Not Found", Id = 0 };
            ectx.Uow.GroupPolicyRepository.Delete(groupPolicyId);
            ectx.Uow.Save();
            //recalculate Active group policies
            new Toems_Service.Workflows.GenerateClientGroupPolicy().Execute(u.GroupId);
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.Id;
            return actionResult;
        }

        public EntityGroupPolicy GetGroupPolicy(int groupPolicyId)
        {
            return ectx.Uow.GroupPolicyRepository.GetById(groupPolicyId);
        }

        public DtoActionResult UpdateGroupPolicy(EntityGroupPolicy groupPolicy)
        {
            var u = GetGroupPolicy(groupPolicy.Id);
            if (u == null) return new DtoActionResult { ErrorMessage = "Group Policy Not Found", Id = 0 };
            var actionResult = new DtoActionResult();

            ectx.Uow.GroupPolicyRepository.Update(groupPolicy, groupPolicy.Id);
            ectx.Uow.Save();
            //recalculate Active group policies
            new Toems_Service.Workflows.GenerateClientGroupPolicy().Execute(u.GroupId);
            actionResult.Success = true;
            actionResult.Id = groupPolicy.Id;


            return actionResult;
        }
    }
}