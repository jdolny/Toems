using System.Collections.Generic;
using System.Net;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceWolRelay
    {
        private readonly UnitOfWork _uow;

        public ServiceWolRelay()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult Add(EntityWolRelay relay)
        {
            var actionResult = new DtoActionResult();

            var validationResult = Validate(relay,true);
            if (validationResult.Success)
            {
                _uow.WolRelayRepository.Insert(relay);
                _uow.Save();
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

            _uow.WolRelayRepository.Delete(relayId);
            _uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.Id;
            return actionResult;
        }

        public EntityWolRelay GetWolRelay(int relayId)
        {
            return _uow.WolRelayRepository.GetById(relayId);
        }

        public List<EntityWolRelay> Search(DtoSearchFilter filter)
        {
            return _uow.WolRelayRepository.Get(x => x.Gateway.Contains(filter.SearchText));
        }

        public string TotalCount()
        {
            return _uow.WolRelayRepository.Count();
        }

        public DtoActionResult Update(EntityWolRelay relay)
        {
            var u = GetWolRelay(relay.Id);
            if (u == null) return new DtoActionResult {ErrorMessage = "Wol Relay Not Found", Id = 0};

            var actionResult = new DtoActionResult();

               var validationResult = Validate(relay,false);
            if (validationResult.Success)
            {
                _uow.WolRelayRepository.Update(relay, u.Id);
                _uow.Save();
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
                if (_uow.WolRelayRepository.Exists(h => h.Gateway == relay.Gateway))
                {
                    validationResult.Success = false;
                    validationResult.ErrorMessage = "A Relay With This Gateway Already Exists";
                    return validationResult;
                }
            }
            else
            {
                var originalRelay = _uow.WolRelayRepository.GetById(relay.Id);
                if (originalRelay.Gateway != relay.Gateway)
                {
                    if (_uow.WolRelayRepository.Exists(h => h.Gateway == relay.Gateway))
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