using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceCertificateInventory(EntityContext ectx)
    {
        public DtoActionResult Add(List<EntityCertificateInventory> inventory)
        {
            var actionResult = new DtoActionResult();
            foreach (var cert in inventory)
            {
                var localCert = cert;
                var existing = ectx.Uow.CertificateInventoryRepository.GetFirstOrDefault(x => x.Thumbprint == localCert.Thumbprint && x.Serial == localCert.Serial);
                if (existing == null)
                {
                    ectx.Uow.CertificateInventoryRepository.Insert(cert);
                }
               
                 actionResult.Id = cert.Id;
            }

            ectx.Uow.Save();
            actionResult.Success = true;
            return actionResult;
        }

        public EntityCertificateInventory GetCertificate(int certificateId)
        {
            return ectx.Uow.CertificateInventoryRepository.GetById(certificateId);
        }

        public List<EntityCertificateInventory> Search(DtoSearchFilter filter)
        {
            return ectx.Uow.CertificateInventoryRepository.Get(x => x.Subject.Contains(filter.SearchText)).OrderBy(x => x.Subject).ToList();
        }

        public string TotalCount()
        {
            return ectx.Uow.CertificateInventoryRepository.Count();
        }
    }
}