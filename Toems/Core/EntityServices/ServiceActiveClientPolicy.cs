using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceActiveClientPolicy(ServiceContext ctx)
    {
        public DtoActionResult InsertOrUpdate(EntityActiveClientPolicy activePolicy)
        {
            var actionResult = new DtoActionResult();
            var p = ctx.Policy.GetActivePolicy(activePolicy.PolicyId);
            if (p == null)
            {
                //insert
                ctx.Uow.ActiveClientPolicies.Insert(activePolicy);
                ctx.Uow.Save();
                actionResult.Success = true;
                actionResult.Id = activePolicy.Id;

            }
            else
            {
                //update
                activePolicy.Id = p.Id;
                ctx.Uow.ActiveClientPolicies.Update(activePolicy, activePolicy.Id);
                ctx.Uow.Save();
                actionResult.Success = true;
                actionResult.Id = activePolicy.Id;
            }

            return actionResult;
        }

        public EntityActiveClientPolicy Get(int activeClientPolicyId)
        {
            return ctx.Uow.ActiveClientPolicies.GetById(activeClientPolicyId);
        }

        public DtoActionResult Delete(int activeClientPolicyId)
        {
            var u = Get(activeClientPolicyId);
            if (u == null) return new DtoActionResult { ErrorMessage = "Active Client Policy Not Found", Id = 0 };
            ctx.Uow.ActiveClientPolicies.Delete(activeClientPolicyId);
            ctx.Uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.Id;
            return actionResult;
        }
    }
}