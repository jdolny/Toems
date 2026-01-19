using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceUserLogins(EntityContext ectx)
    {
        public DtoActionResult AddOrUpdate(List<EntityUserLogin> inventory, int computerId)
        {
            DateTime nullTime = Convert.ToDateTime("0001-01-01 00:00:00");
            var actionResult = new DtoActionResult();
            foreach (var login in inventory)
            {
                login.ComputerId = computerId;
                login.ClientLoginId = login.Id;

                var existing = ectx.Uow.UserLoginRepository.GetFirstOrDefault(x => x.ComputerId == login.ComputerId && x.LoginDateTime == login.LoginDateTime && x.ClientLoginId == login.ClientLoginId);
                if (existing == null)
                {
                    ectx.Uow.UserLoginRepository.Insert(login);
                    actionResult.Id = login.Id;
                }
                else if (login.LogoutDateTime != nullTime)
                {
                    existing.LogoutDateTime = login.LogoutDateTime;
                    ectx.Uow.UserLoginRepository.Update(existing, existing.Id);
                    actionResult.Id = existing.Id;

                }


                ectx.Uow.Save();
            }

            actionResult.Success = true;
            return actionResult;
        }
    }
}