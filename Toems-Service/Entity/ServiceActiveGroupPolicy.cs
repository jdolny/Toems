using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceActiveGroupPolicy
    {
        private readonly UnitOfWork _uow;

        public ServiceActiveGroupPolicy()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult InsertOrUpdate(EntityActiveGroupPolicy activeGroupPolicy)
        {
            var actionResult = new DtoActionResult();
            var p = new ServiceGroup().GetActiveGroupPolicy(activeGroupPolicy.GroupId);
            if (p == null)
            {
                //insert
                _uow.ActiveGroupPoliciesRepository.Insert(activeGroupPolicy);
                _uow.Save();
                actionResult.Success = true;
                actionResult.Id = activeGroupPolicy.Id;

            }
            else
            {
                //update
                activeGroupPolicy.Id = p.Id;
                _uow.ActiveGroupPoliciesRepository.Update(activeGroupPolicy, activeGroupPolicy.Id);
                _uow.Save();
                actionResult.Success = true;
                actionResult.Id = activeGroupPolicy.Id;
            }

            return actionResult;
        }

        public EntityActiveGroupPolicy Get(int activeGroupPolicyId)
        {
            return _uow.ActiveGroupPoliciesRepository.GetById(activeGroupPolicyId);
        }

        public DtoActionResult Delete(int activeGroupPolicyId)
        {
            var u = Get(activeGroupPolicyId);
            if (u == null) return new DtoActionResult { ErrorMessage = "Group Policy Not Found", Id = 0 };
         
            _uow.ActiveGroupPoliciesRepository.Delete(activeGroupPolicyId);
            _uow.Save();
            var actionResult = new DtoActionResult();

                actionResult.Success = true;
                actionResult.Id = u.Id;
            return actionResult;
        }
    }
}