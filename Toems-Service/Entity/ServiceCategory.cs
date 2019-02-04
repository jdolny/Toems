using System;
using System.Collections.Generic;
using System.Linq;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceCategory
    {
        private readonly UnitOfWork _uow;

        public ServiceCategory()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult Add(EntityCategory category)
        {
            var actionResult = new DtoActionResult();

            var validationResult = Validate(category,true);
            if (validationResult.Success)
            {
                _uow.CategoryRepository.Insert(category);
                _uow.Save();
                actionResult.Success = true;
                actionResult.Id = category.Id;
            }
            else
            {
                return new DtoActionResult() {ErrorMessage = validationResult.ErrorMessage};
            }

            return actionResult;
        }

        public DtoActionResult Delete(int categoryId)
        {
            var u = GetCategory(categoryId);
            if (u == null) return new DtoActionResult { ErrorMessage = "Category Not Found", Id = 0 };

         

            _uow.CategoryRepository.Delete(categoryId);
            _uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.Id;
            return actionResult;
        }

        public EntityCategory GetCategory(int categoryId)
        {
            return _uow.CategoryRepository.GetById(categoryId);
        }

        public List<EntityCategory> Search(DtoSearchFilter filter)
        {
            if(filter.Limit == 0)
                filter.Limit = Int32.MaxValue;
            
            return _uow.CategoryRepository.Get(x => x.Name.Contains(filter.SearchText)).Take(filter.Limit).ToList();
        }

        public List<EntityCategory> GetAll()
        {
            return _uow.CategoryRepository.Get();
        }

        public string TotalCount()
        {
            return _uow.CategoryRepository.Count();
        }

        public DtoActionResult Update(EntityCategory category)
        {
            var u = GetCategory(category.Id);
            if (u == null) return new DtoActionResult {ErrorMessage = "Category Not Found", Id = 0};

            var actionResult = new DtoActionResult();

               var validationResult = Validate(category,false);
            if (validationResult.Success)
            {
                _uow.CategoryRepository.Update(category, u.Id);
                _uow.Save();
                actionResult.Success = true;
                actionResult.Id = category.Id;
            }
            else
            {
                return new DtoActionResult() {ErrorMessage = validationResult.ErrorMessage};
            }
            return actionResult;
        }

        public DtoValidationResult Validate(EntityCategory category, bool isNew)
        {
            var validationResult = new DtoValidationResult { Success = true };

            if (string.IsNullOrEmpty(category.Name) || !category.Name.All(c => char.IsLetterOrDigit(c) || c == '_' || c == '-' || c == ' '))
            {
                validationResult.Success = false;
                validationResult.ErrorMessage = "Category Name Is Not Valid";
                return validationResult;
            }

            if (isNew)
            {
                if (_uow.CategoryRepository.Exists(h => h.Name == category.Name))
                {
                    validationResult.Success = false;
                    validationResult.ErrorMessage = "A Category With This Name Already Exists";
                    return validationResult;
                }
            }
            else
            {
                var original = _uow.CategoryRepository.GetById(category.Id);
                if (original.Name != category.Name)
                {
                    if (_uow.CategoryRepository.Exists(h => h.Name == category.Name))
                    {
                        validationResult.Success = false;
                        validationResult.ErrorMessage = "A Category With This Name Already Exists";
                        return validationResult;
                    }
                }
            }

            return validationResult;
        }

       
        


    }
}