using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServicePolicyCategory(ServiceContext ctx)
    {
        public DtoActionResult AddOrUpdate(List<EntityPolicyCategory> policyCategories)
        {
            var first = policyCategories.FirstOrDefault();
            if (first == null) return new DtoActionResult { ErrorMessage = "No Policies Were In The List", Id = 0 };
            var allSame = policyCategories.All(x => x.PolicyId == first.PolicyId);
            if (!allSame) return new DtoActionResult { ErrorMessage = "The List Must Be For A Single Policy.", Id = 0 };
            var actionResult = new DtoActionResult();
            var pToRemove = ctx.Uow.PolicyCategoryRepository.Get(x => x.PolicyId == first.PolicyId);
            foreach (var policyCategory in policyCategories)
            {
                var existing = ctx.Uow.PolicyCategoryRepository.GetFirstOrDefault(x => x.PolicyId == policyCategory.PolicyId && x.CategoryId == policyCategory.CategoryId);
                if (existing == null)
                {
                    ctx.Uow.PolicyCategoryRepository.Insert(policyCategory);
                }
                else
                {
                    pToRemove.Remove(existing);
                }

                actionResult.Id = 1;
            }

            //anything left in pToRemove does not exist anymore
            foreach (var p in pToRemove)
            {
                ctx.Uow.PolicyCategoryRepository.Delete(p.Id);
            }

            ctx.Uow.Save();
            actionResult.Success = true;
            return actionResult;
        }

        public DtoActionResult DeleteAllForPolicy(int policyId)
        {
            ctx.Uow.PolicyCategoryRepository.DeleteRange(x => x.PolicyId == policyId);
            ctx.Uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = 1;
            return actionResult;
        }
    }
}