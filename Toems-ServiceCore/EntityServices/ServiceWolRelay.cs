using System.Net;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceWolRelay(ServiceContext ctx)
    {
        public DtoActionResult Add(EntityWolRelay relay)
        {
            var actionResult = new DtoActionResult();

            var validationResult = Validate(relay,true);
            if (validationResult.Success)
            {
                ctx.Uow.WolRelayRepository.Insert(relay);
                ctx.Uow.Save();
                actionResult.Success = true;
                actionResult.Id = relay.Id;
            }
            else
            {
                return new DtoActionResult() {ErrorMessage = validationResult.ErrorMessage};
            }

            return actionResult;
        }

        public DtoActionResult Delete(int relayId)
        {
            var u = GetWolRelay(relayId);
            if (u == null) return new DtoActionResult { ErrorMessage = "Wol Relay Not Found", Id = 0 };

            ctx.Uow.WolRelayRepository.Delete(relayId);
            ctx.Uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.Id;
            return actionResult;
        }

        public EntityWolRelay GetWolRelay(int relayId)
        {
            return ctx.Uow.WolRelayRepository.GetById(relayId);
        }

        public List<EntityWolRelay> Search(DtoSearchFilter filter)
        {
            return ctx.Uow.WolRelayRepository.Get(x => x.Gateway.Contains(filter.SearchText));
        }

        public string TotalCount()
        {
            return ctx.Uow.WolRelayRepository.Count();
        }

        public DtoActionResult Update(EntityWolRelay relay)
        {
            var u = GetWolRelay(relay.Id);
            if (u == null) return new DtoActionResult {ErrorMessage = "Wol Relay Not Found", Id = 0};

            var actionResult = new DtoActionResult();

               var validationResult = Validate(relay,false);
            if (validationResult.Success)
            {
                ctx.Uow.WolRelayRepository.Update(relay, u.Id);
                ctx.Uow.Save();
                actionResult.Success = true;
                actionResult.Id = relay.Id;
            }
            else
            {
                return new DtoActionResult() {ErrorMessage = validationResult.ErrorMessage};
            }
            return actionResult;
        }

        public DtoValidationResult Validate(EntityWolRelay relay, bool isNew)
        {
            IPAddress gateway;
            if(!IPAddress.TryParse(relay.Gateway,out gateway))
                return new DtoValidationResult() { ErrorMessage = "Invalid Gateway Address", Success = false };
           
               
            var validationResult = new DtoValidationResult();
            if (isNew)
            {
                if (ctx.Uow.WolRelayRepository.Exists(h => h.Gateway == relay.Gateway))
                {
                    validationResult.Success = false;
                    validationResult.ErrorMessage = "A Relay With This Gateway Already Exists";
                    return validationResult;
                }
            }
            else
            {
                var originalRelay = ctx.Uow.WolRelayRepository.GetById(relay.Id);
                if (originalRelay.Gateway != relay.Gateway)
                {
                    if (ctx.Uow.WolRelayRepository.Exists(h => h.Gateway == relay.Gateway))
                    {
                        validationResult.Success = false;
                        validationResult.ErrorMessage = "A Relay With This Gateway Already Exists";
                        return validationResult;
                    }
                }
            }

            return new DtoValidationResult() {Success = true};
        }


    }
}