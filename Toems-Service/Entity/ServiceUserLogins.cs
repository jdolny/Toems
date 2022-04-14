using System;
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
            DateTime nullTime = Convert.ToDateTime("0001-01-01 00:00:00");
            var actionResult = new DtoActionResult();
            foreach (var login in inventory)
            {
                login.ComputerId = computerId;
                login.ClientLoginId = login.Id;

                var existing = _uow.UserLoginRepository.GetFirstOrDefault(x => x.ComputerId == login.ComputerId && x.LoginDateTime == login.LoginDateTime && x.ClientLoginId == login.ClientLoginId);
                if (existing == null)
                {
                    _uow.UserLoginRepository.Insert(login);
                    actionResult.Id = login.Id;
                }
                else if (login.LogoutDateTime != nullTime)
                {
                    existing.LogoutDateTime = login.LogoutDateTime;
                    _uow.UserLoginRepository.Update(existing, existing.Id);
                    actionResult.Id = existing.Id;

                }
             
                
                 _uow.Save();
            }
           
            actionResult.Success = true;
            return actionResult;
        }
    }
}