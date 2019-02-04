using System.Collections.Generic;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceUserLogins
    {
        private readonly UnitOfWork _uow;

        public ServiceUserLogins()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult AddOrUpdate(List<EntityUserLogin> inventory, int computerId)
        {
            var actionResult = new DtoActionResult();
            foreach (var login in inventory)
            {
                login.ComputerId = computerId;
                var existing = _uow.UserLoginRepository.GetFirstOrDefault(x => x.ComputerId == login.ComputerId && x.LoginDateTime == login.LoginDateTime && x.LogoutDateTime == login.LogoutDateTime);
                if (existing == null)
                {
                    _uow.UserLoginRepository.Insert(login);
                }
             
                 actionResult.Id = login.Id;
                 _uow.Save();
            }
           
            actionResult.Success = true;
            return actionResult;
        }
    }
}