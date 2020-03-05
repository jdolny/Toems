using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceImagingClientId
    {
        private readonly UnitOfWork _uow;

        public ServiceImagingClientId()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult AddOrUpdate(int computerId)
        {
            var actionResult = new DtoActionResult();
            var computer = _uow.ComputerRepository.GetById(computerId);
            if (computer == null)
                return actionResult;

            if (string.IsNullOrEmpty(computer.UUID))
                return actionResult;

            var biosInfo = _uow.BiosInventoryRepository.Get(x => x.ComputerId == computerId).FirstOrDefault();
            if (biosInfo == null)
                return actionResult;
            if (string.IsNullOrEmpty(biosInfo.SerialNumber))
                return actionResult;

            var computerMacs = _uow.NicInventoryRepository.Get(x => x.ComputerId == computerId && x.Type.Equals("Ethernet")).Select(x => x.Mac).ToList();
            if (computerMacs.Count == 0)
                return actionResult;

            var pToRemove = _uow.ClientImagingIdRepository.Get(x => x.ComputerId == computerId);
            foreach (var mac in computerMacs)
            {
                var macWithColon = Regex.Replace(mac, ".{2}", "$0:");
                macWithColon = macWithColon.Trim(':');
                var clientImagingId = $"{macWithColon.ToUpper()}.{biosInfo.SerialNumber.ToUpper()}.{computer.UUID.ToUpper()}";
                var existing = _uow.ClientImagingIdRepository.GetFirstOrDefault(x => x.ComputerId == computer.Id && x.ClientIdentifier.Equals(clientImagingId));
                if (existing == null)
                {
                    var entityClientImagingId = new EntityClientImagingId();
                    entityClientImagingId.ComputerId = computer.Id;
                    entityClientImagingId.ClientIdentifier = clientImagingId;
                    _uow.ClientImagingIdRepository.Insert(entityClientImagingId);
                }
                else
                {
                    pToRemove.Remove(existing);
                }

                var rawIdentifier = "";
                try
                {
                    var uuid = clientImagingId.Substring(clientImagingId.LastIndexOf('.') + 1);
                    var clientIdFirst = clientImagingId.Replace(uuid, string.Empty);
                    var uuidGuid = new Guid(uuid);
                    var uuidBytes = uuidGuid.ToByteArray();
                    var strReverseUuid = "";
                    foreach (var b in uuidBytes)
                    {
                        strReverseUuid += b.ToString("X2");
                    }
                    var reverseUuid = new Guid(strReverseUuid);
                    rawIdentifier = (clientIdFirst + reverseUuid).ToUpper();
                }
                catch
                { //ignored
                }

                //look for existing imageonly computers that can be linked to this computer
                var existingSingleIdPretty = _uow.ComputerRepository.Get(x => x.ImagingClientId.Equals(clientImagingId) && x.ProvisionStatus == Toems_Common.Enum.EnumProvisionStatus.Status.ImageOnly).FirstOrDefault();
                if (existingSingleIdPretty != null)
                {
                    if (!LinkComputers(existingSingleIdPretty, computer))
                    {
                        //check raw
                        var existingSingleIdRaw = _uow.ComputerRepository.Get(x => x.ImagingClientId.Equals(rawIdentifier) && x.ProvisionStatus == Toems_Common.Enum.EnumProvisionStatus.Status.ImageOnly).FirstOrDefault();
                        if (existingSingleIdRaw != null)
                            LinkComputers(existingSingleIdRaw, computer);
                    }
                }
            }

            //anything left in pToRemove does not exist on that computer anymore
            foreach (var p in pToRemove)
            {
                _uow.ClientImagingIdRepository.Delete(p.Id);
            }

            _uow.Save();
            actionResult.Id = 1;
            actionResult.Success = true;
            return actionResult;
        }

        private bool LinkComputers(EntityComputer from, EntityComputer to)
        {
            var provisionExists = _uow.ComputerRepository.Get(x => x.ImagingClientId.Equals(from.ImagingClientId) && x.ProvisionStatus == Toems_Common.Enum.EnumProvisionStatus.Status.Provisioned).FirstOrDefault();
            if(provisionExists != null)
            {
                //another provisioned computer already has this single imaging client identifier
                return false;
            }
            if (!from.Name.Split(':').First().ToUpper().Equals(to.Name.ToUpper()))
            {
                //client ids matched, but names did not, don't link
                return false;
            }
            to.ImagingClientId = from.ImagingClientId;
            to.ImagingMac = from.ImagingMac;

            //todo: copy other properties from old to new, such as custom attributes, imaging logs, etc
            _uow.ComputerRepository.Update(to, to.Id);
            _uow.ComputerRepository.Delete(from.Id);
            _uow.Save();
            return true;
        }
            
    }
}