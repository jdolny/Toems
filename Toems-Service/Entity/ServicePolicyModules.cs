using System.Collections.Generic;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServicePolicyModules
    {
        private readonly UnitOfWork _uow;

        public ServicePolicyModules()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult AddPolicyModules(List<EntityPolicyModules> policyModules)
        {
            var actionResult = new DtoActionResult();
            foreach (var policyModule in policyModules)
            {
                var activePolicy = new ServicePolicy().GetActivePolicy(policyModule.PolicyId);
                if (activePolicy != null) return new DtoActionResult() { ErrorMessage = "Active Policies Cannot Be Updated.  You Must Deactivate It First." };
                _uow.PolicyModulesRepository.Insert(policyModule);
            }
            _uow.Save();
            actionResult.Success = true;

            return actionResult;
        }

        public DtoActionResult AddPolicyModule(EntityPolicyModules policyModule)
        {
            var activePolicy = new ServicePolicy().GetActivePolicy(policyModule.PolicyId);
            if (activePolicy != null) return new DtoActionResult() { ErrorMessage = "Active Policies Cannot Be Updated.  You Must Deactivate It First." };
            var actionResult = new DtoActionResult();

            _uow.PolicyModulesRepository.Insert(policyModule);
            _uow.Save();
            actionResult.Success = true;
            actionResult.Id = policyModule.Id;


            return actionResult;
        }

        public DtoActionResult DeletePolicyModule(int policyModuleId)
        {
            var u = GetPolicyModule(policyModuleId);
            if (u == null) return new DtoActionResult {ErrorMessage = "Policy Module Not Found", Id = 0};
            var activePolicy = new ServicePolicy().GetActivePolicy(u.PolicyId);
            if (activePolicy != null) return new DtoActionResult() { ErrorMessage = "Active Policies Cannot Be Updated.  You Must Deactivate It First." };
            _uow.PolicyModulesRepository.Delete(policyModuleId);
            _uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.Id;
            return actionResult;
        }

        public EntityPolicyModules GetPolicyModule(int policyModuleId)
        {
            return _uow.PolicyModulesRepository.GetById(policyModuleId);
        }

        public DtoActionResult UpdatePolicyModule(EntityPolicyModules policy)
        {
            var u = GetPolicyModule(policy.Id);
            if (u == null) return new DtoActionResult {ErrorMessage = "Policy Module Not Found", Id = 0};
            var activePolicy = new ServicePolicy().GetActivePolicy(u.PolicyId);
            if (activePolicy != null) return new DtoActionResult() { ErrorMessage = "Active Policies Cannot Be Updated.  You Must Deactivate It First." };
            var actionResult = new DtoActionResult();

            _uow.PolicyModulesRepository.Update(policy, policy.Id);
            _uow.Save();
            actionResult.Success = true;
            actionResult.Id = policy.Id;


            return actionResult;
        }

      


    }
}