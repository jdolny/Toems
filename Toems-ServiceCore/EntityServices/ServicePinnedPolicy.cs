using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServicePinnedPolicy(EntityContext ectx)
    {
        public DtoActionResult Add(EntityPinnedPolicy pinnedPolicy)
        {
            var actionResult = new DtoActionResult();
            var u = ectx.Uow.PinnedPolicyRepository.Get(x => x.PolicyId == pinnedPolicy.PolicyId && x.UserId == pinnedPolicy.UserId).FirstOrDefault();
            if (u != null) return new DtoActionResult() { ErrorMessage = "Policy Is Already Pinned" };
            ectx.Uow.PinnedPolicyRepository.Insert(pinnedPolicy);
            ectx.Uow.Save();
            actionResult.Success = true;
            actionResult.Id = pinnedPolicy.Id;

            return actionResult;
        }

        public DtoActionResult Delete(int policyId, int userId)
        {
            if (policyId == 0 || userId == 0) return new DtoActionResult() {ErrorMessage = "Policy Not Defined"};
            var u = ectx.Uow.PinnedPolicyRepository.Get(x => x.PolicyId == policyId && x.UserId == userId).FirstOrDefault();
            if (u == null) return new DtoActionResult() {ErrorMessage = "Pinned Policy Not Found"};
            ectx.Uow.PinnedPolicyRepository.Delete(u.Id);
            ectx.Uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.Id;
            return actionResult;
        }

        public EntityPinnedPolicy GetPinnedPolicy(int pinnedPolicyId)
        {
            return ectx.Uow.PinnedPolicyRepository.GetById(pinnedPolicyId);
        }
    }
}