using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceActiveClientPolicy
    {
        private readonly UnitOfWork _uow;

        public ServiceActiveClientPolicy()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult InsertOrUpdate(EntityActiveClientPolicy activePolicy)
        {
            var actionResult = new DtoActionResult();
            var p = new ServicePolicy().GetActivePolicy(activePolicy.PolicyId);
            if (p == null)
            {
                //insert
                _uow.ActiveClientPolicies.Insert(activePolicy);
                _uow.Save();
                actionResult.Success = true;
                actionResult.Id = activePolicy.Id;

            }
            else
            {
                //update
                activePolicy.Id = p.Id;
                _uow.ActiveClientPolicies.Update(activePolicy, activePolicy.Id);
                _uow.Save();
                actionResult.Success = true;
                actionResult.Id = activePolicy.Id;
            }

            return actionResult;
        }

        public EntityActiveClientPolicy Get(int activeClientPolicyId)
        {
            return _uow.ActiveClientPolicies.GetById(activeClientPolicyId);
        }

        public DtoActionResult Delete(int activeClientPolicyId)
        {
            var u = Get(activeClientPolicyId);
            if (u == null) return new DtoActionResult { ErrorMessage = "Active Client Policy Not Found", Id = 0 };
            _uow.ActiveClientPolicies.Delete(activeClientPolicyId);
            _uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.Id;
            return actionResult;
        }
    }
}