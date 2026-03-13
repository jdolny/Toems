using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServicePinnedPolicy(ServiceContext ctx)
    {
        public DtoActionResult Add(EntityPinnedPolicy pinnedPolicy)
        {
            var actionResult = new DtoActionResult();
            var u = ctx.Uow.PinnedPolicyRepository.Get(x => x.PolicyId == pinnedPolicy.PolicyId && x.UserId == pinnedPolicy.UserId).FirstOrDefault();
            if (u != null) return new DtoActionResult() { ErrorMessage = "Policy Is Already Pinned" };
            ctx.Uow.PinnedPolicyRepository.Insert(pinnedPolicy);
            ctx.Uow.Save();
            actionResult.Success = true;
            actionResult.Id = pinnedPolicy.Id;

            return actionResult;
        }

        public DtoActionResult Delete(int policyId, int userId)
        {
            if (policyId == 0 || userId == 0) return new DtoActionResult() {ErrorMessage = "Policy Not Defined"};
            var u = ctx.Uow.PinnedPolicyRepository.Get(x => x.PolicyId == policyId && x.UserId == userId).FirstOrDefault();
            if (u == null) return new DtoActionResult() {ErrorMessage = "Pinned Policy Not Found"};
            ctx.Uow.PinnedPolicyRepository.Delete(u.Id);
            ctx.Uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.Id;
            return actionResult;
        }

        public EntityPinnedPolicy GetPinnedPolicy(int pinnedPolicyId)
        {
            return ctx.Uow.PinnedPolicyRepository.GetById(pinnedPolicyId);
        }
    }
}