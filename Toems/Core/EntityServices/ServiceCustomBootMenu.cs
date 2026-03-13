using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceCustomBootMenu(ServiceContext ctx)
    {
        public DtoActionResult Add(EntityCustomBootMenu customBootMenu)
        {
            var actionResult = new DtoActionResult();

            var validationResult = Validate(customBootMenu,true);
            if (validationResult.Success)
            {
                ctx.Uow.CustomBootMenuRepository.Insert(customBootMenu);
                ctx.Uow.Save();
                actionResult.Success = true;
                actionResult.Id = customBootMenu.Id;
            }
            else
            {
                return new DtoActionResult() {ErrorMessage = validationResult.ErrorMessage};
            }

            return actionResult;
        }

        public DtoActionResult Delete(int customBootMenuId)
        {
            var u = GetCustomBootMenu(customBootMenuId);
            if (u == null) return new DtoActionResult { ErrorMessage = "Custom Boot Menu Entry Not Found", Id = 0 };

         

            ctx.Uow.CustomBootMenuRepository.Delete(customBootMenuId);
            ctx.Uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.Id;
            return actionResult;
        }

        public EntityCustomBootMenu GetCustomBootMenu(int customBootMenuId)
        {
            return ctx.Uow.CustomBootMenuRepository.GetById(customBootMenuId);
        }

        public List<EntityCustomBootMenu> Search(DtoSearchFilter filter)
        {
            if(filter.Limit == 0)
                filter.Limit = Int32.MaxValue;
            
            return ctx.Uow.CustomBootMenuRepository.Get(x => x.Name.Contains(filter.SearchText)).Take(filter.Limit).ToList();
        }

        public List<EntityCustomBootMenu> GetAll()
        {
            return ctx.Uow.CustomBootMenuRepository.Get();
        }

        public string TotalCount()
        {
            return ctx.Uow.CustomBootMenuRepository.Count();
        }

        public DtoActionResult Update(EntityCustomBootMenu customBootMenu)
        {
            var u = GetCustomBootMenu(customBootMenu.Id);
            if (u == null) return new DtoActionResult {ErrorMessage = "Custom Boot Menu Entry Not Found", Id = 0};

            var actionResult = new DtoActionResult();

               var validationResult = Validate(customBootMenu,false);
            if (validationResult.Success)
            {
                ctx.Uow.CustomBootMenuRepository.Update(customBootMenu, u.Id);
                ctx.Uow.Save();
                actionResult.Success = true;
                actionResult.Id = customBootMenu.Id;
            }
            else
            {
                return new DtoActionResult() {ErrorMessage = validationResult.ErrorMessage};
            }
            return actionResult;
        }

        public DtoValidationResult Validate(EntityCustomBootMenu customBootMenu, bool isNew)
        {
            var validationResult = new DtoValidationResult { Success = true };

            if (string.IsNullOrEmpty(customBootMenu.Name))
            {
                validationResult.Success = false;
                validationResult.ErrorMessage = "Custom Boot Menu Entry Name Is Not Valid";
                return validationResult;
            }

            if (isNew)
            {
                if (ctx.Uow.CustomBootMenuRepository.Exists(h => h.Name == customBootMenu.Name))
                {
                    validationResult.Success = false;
                    validationResult.ErrorMessage = "A Custom Boot Menu Entry With This Name Already Exists";
                    return validationResult;
                }
            }
            else
            {
                var original = ctx.Uow.CustomBootMenuRepository.GetById(customBootMenu.Id);
                if (original.Name != customBootMenu.Name)
                {
                    if (ctx.Uow.CustomBootMenuRepository.Exists(h => h.Name == customBootMenu.Name))
                    {
                        validationResult.Success = false;
                        validationResult.ErrorMessage = "A Custom Boot Menu Entry With This Name Already Exists";
                        return validationResult;
                    }
                }
            }

            return validationResult;
        }

       
        


    }
}