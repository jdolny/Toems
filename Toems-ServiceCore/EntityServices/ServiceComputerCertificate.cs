using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceComputerCertificate(EntityContext ectx)
    {
        public DtoActionResult AddOrUpdate(List<EntityCertificateInventory> inventory, int computerId)
        {
            var actionResult = new DtoActionResult();
            var pToRemove = ectx.Uow.ComputerCertificateRepository.Get(x => x.ComputerId == computerId);
            foreach (var cert in inventory)
            {
                var localCert = cert;
                var s = ectx.Uow.CertificateInventoryRepository.GetFirstOrDefault(x => x.Thumbprint == localCert.Thumbprint && x.Serial == localCert.Serial);
                //check if certificate exists before adding computer relationship
                if (s == null)
                    continue;

                var existingRelationship =
                    ectx.Uow.ComputerCertificateRepository.GetFirstOrDefault(
                        x => x.ComputerId == computerId && x.CertificateId == s.Id);

                if (existingRelationship == null)
                {
                    existingRelationship = new EntityComputerCertificate();
                    existingRelationship.ComputerId = computerId;
                    existingRelationship.CertificateId = s.Id;
                    ectx.Uow.ComputerCertificateRepository.Insert(existingRelationship);
                }
                else
                {
                    pToRemove.Remove(existingRelationship);           
                }
            }

            //anything left in pToRemove does not exist on that computer anymore
            foreach (var p in pToRemove)
            {
                ectx.Uow.ComputerCertificateRepository.Delete(p.Id);
            }

            ectx.Uow.Save();
            actionResult.Success = true;
            return actionResult;
        }
    }
}