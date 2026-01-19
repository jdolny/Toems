using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceActiveGroupPolicy(EntityContext ectx, GroupService groupService)
    {
        public DtoActionResult InsertOrUpdate(EntityActiveGroupPolicy activeGroupPolicy)
        {
            var actionResult = new DtoActionResult();
            var p = groupService.GetActiveGroupPolicy(activeGroupPolicy.GroupId);
            if (p == null)
            {
                //insert
                ectx.Uow.ActiveGroupPoliciesRepository.Insert(activeGroupPolicy);
                ectx.Uow.Save();
                actionResult.Success = true;
                actionResult.Id = activeGroupPolicy.Id;

            }
            else
            {
                //update
                activeGroupPolicy.Id = p.Id;
                ectx.Uow.ActiveGroupPoliciesRepository.Update(activeGroupPolicy, activeGroupPolicy.Id);
                ectx.Uow.Save();
                actionResult.Success = true;
                actionResult.Id = activeGroupPolicy.Id;
            }

            return actionResult;
        }

        public EntityActiveGroupPolicy Get(int activeGroupPolicyId)
        {
            return ectx.Uow.ActiveGroupPoliciesRepository.GetById(activeGroupPolicyId);
        }

        public DtoActionResult Delete(int activeGroupPolicyId)
        {
            var u = Get(activeGroupPolicyId);
            if (u == null) return new DtoActionResult { ErrorMessage = "Group Policy Not Found", Id = 0 };
         
            ectx.Uow.ActiveGroupPoliciesRepository.Delete(activeGroupPolicyId);
            ectx.Uow.Save();
            var actionResult = new DtoActionResult();

                actionResult.Success = true;
                actionResult.Id = u.Id;
            return actionResult;
        }
    }
}