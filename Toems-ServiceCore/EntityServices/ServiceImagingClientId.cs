using System.Text.RegularExpressions;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceImagingClientId(EntityContext ectx)
    {
        public DtoActionResult AddOrUpdate(int computerId)
        {
            var actionResult = new DtoActionResult();
            var computer = ectx.Uow.ComputerRepository.GetById(computerId);
            if (computer == null)
                return actionResult;

            if (string.IsNullOrEmpty(computer.UUID))
                return actionResult;

            var biosInfo = ectx.Uow.BiosInventoryRepository.Get(x => x.ComputerId == computerId).FirstOrDefault();
            if (biosInfo == null)
                return actionResult;
            if (string.IsNullOrEmpty(biosInfo.SerialNumber))
                return actionResult;

            var computerMacs = ectx.Uow.NicInventoryRepository.Get(x => x.ComputerId == computerId && x.Type.Equals("Ethernet")).Select(x => x.Mac).ToList();
            if (computerMacs.Count == 0)
                return actionResult;

            var pToRemove = ectx.Uow.ClientImagingIdRepository.Get(x => x.ComputerId == computerId);
            foreach (var mac in computerMacs)
            {
                var macWithColon = Regex.Replace(mac, ".{2}", "$0:");
                macWithColon = macWithColon.Trim(':');
                var clientImagingId = $"{macWithColon.ToUpper()}.{biosInfo.SerialNumber.ToUpper()}.{computer.UUID.ToUpper()}";
                var existing = ectx.Uow.ClientImagingIdRepository.GetFirstOrDefault(x => x.ComputerId == computer.Id && x.ClientIdentifier.Equals(clientImagingId));
                if (existing == null)
                {
                    var entityClientImagingId = new EntityClientImagingId();
                    entityClientImagingId.ComputerId = computer.Id;
                    entityClientImagingId.ClientIdentifier = clientImagingId;
                    ectx.Uow.ClientImagingIdRepository.Insert(entityClientImagingId);
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
                var existingSingleIdPretty = ectx.Uow.ComputerRepository.Get(x => x.ImagingClientId.Equals(clientImagingId) && x.ProvisionStatus == Toems_Common.Enum.EnumProvisionStatus.Status.ImageOnly).FirstOrDefault();
                if (existingSingleIdPretty != null)
                {
                    if (!LinkComputers(existingSingleIdPretty, computer))
                    {
                        //check raw
                        var existingSingleIdRaw = ectx.Uow.ComputerRepository.Get(x => x.ImagingClientId.Equals(rawIdentifier) && x.ProvisionStatus == Toems_Common.Enum.EnumProvisionStatus.Status.ImageOnly).FirstOrDefault();
                        if (existingSingleIdRaw != null)
                            LinkComputers(existingSingleIdRaw, computer);
                    }
                }
            }

            //anything left in pToRemove does not exist on that computer anymore
            foreach (var p in pToRemove)
            {
                ectx.Uow.ClientImagingIdRepository.Delete(p.Id);
            }

            ectx.Uow.Save();
            actionResult.Id = 1;
            actionResult.Success = true;
            return actionResult;
        }

        private bool LinkComputers(EntityComputer from, EntityComputer to)
        {
            var provisionExists = ectx.Uow.ComputerRepository.Get(x => x.ImagingClientId.Equals(from.ImagingClientId) && x.ProvisionStatus == Toems_Common.Enum.EnumProvisionStatus.Status.Provisioned).FirstOrDefault();
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
            ectx.Uow.ComputerRepository.Update(to, to.Id);
            ectx.Uow.ComputerRepository.Delete(from.Id);
            ectx.Uow.Save();
            return true;
        }
            
    }
}