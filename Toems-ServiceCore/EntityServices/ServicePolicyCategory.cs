using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServicePolicyCategory(EntityContext ectx)
    {
        public DtoActionResult AddOrUpdate(List<EntityPolicyCategory> policyCategories)
        {
            var first = policyCategories.FirstOrDefault();
            if (first == null) return new DtoActionResult { ErrorMessage = "No Policies Were In The List", Id = 0 };
            var allSame = policyCategories.All(x => x.PolicyId == first.PolicyId);
            if (!allSame) return new DtoActionResult { ErrorMessage = "The List Must Be For A Single Policy.", Id = 0 };
            var actionResult = new DtoActionResult();
            var pToRemove = ectx.Uow.PolicyCategoryRepository.Get(x => x.PolicyId == first.PolicyId);
            foreach (var policyCategory in policyCategories)
            {
                var existing = ectx.Uow.PolicyCategoryRepository.GetFirstOrDefault(x => x.PolicyId == policyCategory.PolicyId && x.CategoryId == policyCategory.CategoryId);
                if (existing == null)
                {
                    ectx.Uow.PolicyCategoryRepository.Insert(policyCategory);
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
                ectx.Uow.PolicyCategoryRepository.Delete(p.Id);
            }

            ectx.Uow.Save();
            actionResult.Success = true;
            return actionResult;
        }

        public DtoActionResult DeleteAllForPolicy(int policyId)
        {
            ectx.Uow.PolicyCategoryRepository.DeleteRange(x => x.PolicyId == policyId);
            ectx.Uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = 1;
            return actionResult;
        }
    }
}