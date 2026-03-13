using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServicePolicyModules(ServiceContext ctx)
    {
        public DtoActionResult AddPolicyModules(List<EntityPolicyModules> policyModules)
        {
            var actionResult = new DtoActionResult();
            foreach (var policyModule in policyModules)
            {
                var activePolicy = ctx.Policy.GetActivePolicy(policyModule.PolicyId);
                if (activePolicy != null) return new DtoActionResult() { ErrorMessage = "Active Policies Cannot Be Updated.  You Must Deactivate It First." };
                ctx.Uow.PolicyModulesRepository.Insert(policyModule);
            }
            ctx.Uow.Save();
            actionResult.Success = true;

            return actionResult;
        }

        public DtoActionResult AddPolicyModule(EntityPolicyModules policyModule)
        {
            var activePolicy = ctx.Policy.GetActivePolicy(policyModule.PolicyId);
            if (activePolicy != null) return new DtoActionResult() { ErrorMessage = "Active Policies Cannot Be Updated.  You Must Deactivate It First." };
            var actionResult = new DtoActionResult();

            ctx.Uow.PolicyModulesRepository.Insert(policyModule);
            ctx.Uow.Save();
            actionResult.Success = true;
            actionResult.Id = policyModule.Id;


            return actionResult;
        }

        public DtoActionResult DeletePolicyModule(int policyModuleId)
        {
            var u = GetPolicyModule(policyModuleId);
            if (u == null) return new DtoActionResult {ErrorMessage = "Policy Module Not Found", Id = 0};
            var activePolicy = ctx.Policy.GetActivePolicy(u.PolicyId);
            if (activePolicy != null) return new DtoActionResult() { ErrorMessage = "Active Policies Cannot Be Updated.  You Must Deactivate It First." };
            ctx.Uow.PolicyModulesRepository.Delete(policyModuleId);
            ctx.Uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.Id;
            return actionResult;
        }

        public EntityPolicyModules GetPolicyModule(int policyModuleId)
        {
            return ctx.Uow.PolicyModulesRepository.GetById(policyModuleId);
        }

        public DtoActionResult UpdatePolicyModule(EntityPolicyModules policy)
        {
            var u = GetPolicyModule(policy.Id);
            if (u == null) return new DtoActionResult {ErrorMessage = "Policy Module Not Found", Id = 0};
            var activePolicy = ctx.Policy.GetActivePolicy(u.PolicyId);
            if (activePolicy != null) return new DtoActionResult() { ErrorMessage = "Active Policies Cannot Be Updated.  You Must Deactivate It First." };
            var actionResult = new DtoActionResult();

            ctx.Uow.PolicyModulesRepository.Update(policy, policy.Id);
            ctx.Uow.Save();
            actionResult.Success = true;
            actionResult.Id = policy.Id;


            return actionResult;
        }

      


    }
}