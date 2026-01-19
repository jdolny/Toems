using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;
using Toems_Service.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceActiveClientPolicy(EntityContext ectx)
    {
        public DtoActionResult InsertOrUpdate(EntityActiveClientPolicy activePolicy)
        {
            var actionResult = new DtoActionResult();
            var p = new ServicePolicy().GetActivePolicy(activePolicy.PolicyId);
            if (p == null)
            {
                //insert
                ectx.Uow.ActiveClientPolicies.Insert(activePolicy);
                ectx.Uow.Save();
                actionResult.Success = true;
                actionResult.Id = activePolicy.Id;

            }
            else
            {
                //update
                activePolicy.Id = p.Id;
                ectx.Uow.ActiveClientPolicies.Update(activePolicy, activePolicy.Id);
                ectx.Uow.Save();
                actionResult.Success = true;
                actionResult.Id = activePolicy.Id;
            }

            return actionResult;
        }

        public EntityActiveClientPolicy Get(int activeClientPolicyId)
        {
            return ectx.Uow.ActiveClientPolicies.GetById(activeClientPolicyId);
        }

        public DtoActionResult Delete(int activeClientPolicyId)
        {
            var u = Get(activeClientPolicyId);
            if (u == null) return new DtoActionResult { ErrorMessage = "Active Client Policy Not Found", Id = 0 };
            ectx.Uow.ActiveClientPolicies.Delete(activeClientPolicyId);
            ectx.Uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.Id;
            return actionResult;
        }
    }
}