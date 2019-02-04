using System.Linq;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServicePinnedPolicy
    {
        private readonly UnitOfWork _uow;

        public ServicePinnedPolicy()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult Add(EntityPinnedPolicy pinnedPolicy)
        {
            var actionResult = new DtoActionResult();
            var u = _uow.PinnedPolicyRepository.Get(x => x.PolicyId == pinnedPolicy.PolicyId && x.UserId == pinnedPolicy.UserId).FirstOrDefault();
            if (u != null) return new DtoActionResult() { ErrorMessage = "Policy Is Already Pinned" };
            _uow.PinnedPolicyRepository.Insert(pinnedPolicy);
            _uow.Save();
            actionResult.Success = true;
            actionResult.Id = pinnedPolicy.Id;

            return actionResult;
        }

        public DtoActionResult Delete(int policyId, int userId)
        {
            if (policyId == 0 || userId == 0) return new DtoActionResult() {ErrorMessage = "Policy Not Defined"};
            var u = _uow.PinnedPolicyRepository.Get(x => x.PolicyId == policyId && x.UserId == userId).FirstOrDefault();
            if (u == null) return new DtoActionResult() {ErrorMessage = "Pinned Policy Not Found"};
            _uow.PinnedPolicyRepository.Delete(u.Id);
            _uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.Id;
            return actionResult;
        }

        public EntityPinnedPolicy GetPinnedPolicy(int pinnedPolicyId)
        {
            return _uow.PinnedPolicyRepository.GetById(pinnedPolicyId);
        }
    }
}