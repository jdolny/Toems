using System;
using System.Collections.Generic;
using System.Linq;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceCustomBootMenu
    {
        private readonly UnitOfWork _uow;

        public ServiceCustomBootMenu()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult Add(EntityCustomBootMenu customBootMenu)
        {
            var actionResult = new DtoActionResult();

            var validationResult = Validate(customBootMenu,true);
            if (validationResult.Success)
            {
                _uow.CustomBootMenuRepository.Insert(customBootMenu);
                _uow.Save();
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

         

            _uow.CustomBootMenuRepository.Delete(customBootMenuId);
            _uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.Id;
            return actionResult;
        }

        public EntityCustomBootMenu GetCustomBootMenu(int customBootMenuId)
        {
            return _uow.CustomBootMenuRepository.GetById(customBootMenuId);
        }

        public List<EntityCustomBootMenu> Search(DtoSearchFilter filter)
        {
            if(filter.Limit == 0)
                filter.Limit = Int32.MaxValue;
            
            return _uow.CustomBootMenuRepository.Get(x => x.Name.Contains(filter.SearchText)).Take(filter.Limit).ToList();
        }

        public List<EntityCustomBootMenu> GetAll()
        {
            return _uow.CustomBootMenuRepository.Get();
        }

        public string TotalCount()
        {
            return _uow.CustomBootMenuRepository.Count();
        }

        public DtoActionResult Update(EntityCustomBootMenu customBootMenu)
        {
            var u = GetCustomBootMenu(customBootMenu.Id);
            if (u == null) return new DtoActionResult {ErrorMessage = "Custom Boot Menu Entry Not Found", Id = 0};

            var actionResult = new DtoActionResult();

               var validationResult = Validate(customBootMenu,false);
            if (validationResult.Success)
            {
                _uow.CustomBootMenuRepository.Update(customBootMenu, u.Id);
                _uow.Save();
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
                if (_uow.CustomBootMenuRepository.Exists(h => h.Name == customBootMenu.Name))
                {
                    validationResult.Success = false;
                    validationResult.ErrorMessage = "A Custom Boot Menu Entry With This Name Already Exists";
                    return validationResult;
                }
            }
            else
            {
                var original = _uow.CustomBootMenuRepository.GetById(customBootMenu.Id);
                if (original.Name != customBootMenu.Name)
                {
                    if (_uow.CustomBootMenuRepository.Exists(h => h.Name == customBootMenu.Name))
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