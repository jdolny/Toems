using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceComputerCertificate(ServiceContext ctx)
    {
        public DtoActionResult AddOrUpdate(List<EntityCertificateInventory> inventory, int computerId)
        {
            var actionResult = new DtoActionResult();
            var pToRemove = ctx.Uow.ComputerCertificateRepository.Get(x => x.ComputerId == computerId);
            foreach (var cert in inventory)
            {
                var localCert = cert;
                var s = ctx.Uow.CertificateInventoryRepository.GetFirstOrDefault(x => x.Thumbprint == localCert.Thumbprint && x.Serial == localCert.Serial);
                //check if certificate exists before adding computer relationship
                if (s == null)
                    continue;

                var existingRelationship =
                    ctx.Uow.ComputerCertificateRepository.GetFirstOrDefault(
                        x => x.ComputerId == computerId && x.CertificateId == s.Id);

                if (existingRelationship == null)
                {
                    existingRelationship = new EntityComputerCertificate();
                    existingRelationship.ComputerId = computerId;
                    existingRelationship.CertificateId = s.Id;
                    ctx.Uow.ComputerCertificateRepository.Insert(existingRelationship);
                }
                else
                {
                    pToRemove.Remove(existingRelationship);           
                }
            }

            //anything left in pToRemove does not exist on that computer anymore
            foreach (var p in pToRemove)
            {
                ctx.Uow.ComputerCertificateRepository.Delete(p.Id);
            }

            ctx.Uow.Save();
            actionResult.Success = true;
            return actionResult;
        }
    }
}