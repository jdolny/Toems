using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceActiveGroupPolicy(ServiceContext ctx)
    {
        public DtoActionResult InsertOrUpdate(EntityActiveGroupPolicy activeGroupPolicy)
        {
            var actionResult = new DtoActionResult();
            var p = ctx.Group.GetActiveGroupPolicy(activeGroupPolicy.GroupId);
            if (p == null)
            {
                //insert
                ctx.Uow.ActiveGroupPoliciesRepository.Insert(activeGroupPolicy);
                ctx.Uow.Save();
                actionResult.Success = true;
                actionResult.Id = activeGroupPolicy.Id;

            }
            else
            {
                //update
                activeGroupPolicy.Id = p.Id;
                ctx.Uow.ActiveGroupPoliciesRepository.Update(activeGroupPolicy, activeGroupPolicy.Id);
                ctx.Uow.Save();
                actionResult.Success = true;
                actionResult.Id = activeGroupPolicy.Id;
            }

            return actionResult;
        }

        public EntityActiveGroupPolicy Get(int activeGroupPolicyId)
        {
            return ctx.Uow.ActiveGroupPoliciesRepository.GetById(activeGroupPolicyId);
        }

        public DtoActionResult Delete(int activeGroupPolicyId)
        {
            var u = Get(activeGroupPolicyId);
            if (u == null) return new DtoActionResult { ErrorMessage = "Group Policy Not Found", Id = 0 };
         
            ctx.Uow.ActiveGroupPoliciesRepository.Delete(activeGroupPolicyId);
            ctx.Uow.Save();
            var actionResult = new DtoActionResult();

                actionResult.Success = true;
                actionResult.Id = u.Id;
            return actionResult;
        }
    }
}