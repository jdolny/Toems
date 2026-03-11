using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceCategory(ServiceContext ctx)
    {
        public DtoActionResult Add(EntityCategory category)
        {
            var actionResult = new DtoActionResult();

            var validationResult = Validate(category,true);
            if (validationResult.Success)
            {
                ctx.Uow.CategoryRepository.Insert(category);
                ctx.Uow.Save();
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

         

            ctx.Uow.CategoryRepository.Delete(categoryId);
            ctx.Uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.Id;
            return actionResult;
        }

        public EntityCategory GetCategory(int categoryId)
        {
            return ctx.Uow.CategoryRepository.GetById(categoryId);
        }

        public List<EntityCategory> Search(DtoSearchFilter filter)
        {
            if(filter.Limit == 0)
                filter.Limit = Int32.MaxValue;
            
            return ctx.Uow.CategoryRepository.Get(x => x.Name.Contains(filter.SearchText)).Take(filter.Limit).OrderBy(x => x.Name).ToList();
        }

        public List<EntityCategory> GetAll()
        {
            return ctx.Uow.CategoryRepository.Get().OrderBy(x => x.Name).ToList(); ;
        }

        public string TotalCount()
        {
            return ctx.Uow.CategoryRepository.Count();
        }

        public DtoActionResult Update(EntityCategory category)
        {
            var u = GetCategory(category.Id);
            if (u == null) return new DtoActionResult {ErrorMessage = "Category Not Found", Id = 0};

            var actionResult = new DtoActionResult();

               var validationResult = Validate(category,false);
            if (validationResult.Success)
            {
                ctx.Uow.CategoryRepository.Update(category, u.Id);
                ctx.Uow.Save();
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
                if (ctx.Uow.CategoryRepository.Exists(h => h.Name == category.Name))
                {
                    validationResult.Success = false;
                    validationResult.ErrorMessage = "A Category With This Name Already Exists";
                    return validationResult;
                }
            }
            else
            {
                var original = ctx.Uow.CategoryRepository.GetById(category.Id);
                if (original.Name != category.Name)
                {
                    if (ctx.Uow.CategoryRepository.Exists(h => h.Name == category.Name))
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