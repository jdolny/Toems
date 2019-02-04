using System.Collections.Generic;
using System.Linq;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServicePolicyCategory
    {
        private readonly UnitOfWork _uow;

        public ServicePolicyCategory()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult AddOrUpdate(List<EntityPolicyCategory> policyCategories)
        {
            var first = policyCategories.FirstOrDefault();
            if (first == null) return new DtoActionResult { ErrorMessage = "No Policies Were In The List", Id = 0 };
            var allSame = policyCategories.All(x => x.PolicyId == first.PolicyId);
            if (!allSame) return new DtoActionResult { ErrorMessage = "The List Must Be For A Single Policy.", Id = 0 };
            var actionResult = new DtoActionResult();
            var pToRemove = _uow.PolicyCategoryRepository.Get(x => x.PolicyId == first.PolicyId);
            foreach (var policyCategory in policyCategories)
            {
                var existing = _uow.PolicyCategoryRepository.GetFirstOrDefault(x => x.PolicyId == policyCategory.PolicyId && x.CategoryId == policyCategory.CategoryId);
                if (existing == null)
                {
                    _uow.PolicyCategoryRepository.Insert(policyCategory);
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
                _uow.PolicyCategoryRepository.Delete(p.Id);
            }

            _uow.Save();
            actionResult.Success = true;
            return actionResult;
        }

        public DtoActionResult DeleteAllForPolicy(int policyId)
        {
            _uow.PolicyCategoryRepository.DeleteRange(x => x.PolicyId == policyId);
            _uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = 1;
            return actionResult;
        }
    }
}