using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceCertificateInventory(ServiceContext ctx)
    {
        public DtoActionResult Add(List<EntityCertificateInventory> inventory)
        {
            var actionResult = new DtoActionResult();
            foreach (var cert in inventory)
            {
                var localCert = cert;
                var existing = ctx.Uow.CertificateInventoryRepository.GetFirstOrDefault(x => x.Thumbprint == localCert.Thumbprint && x.Serial == localCert.Serial);
                if (existing == null)
                {
                    ctx.Uow.CertificateInventoryRepository.Insert(cert);
                }
               
                 actionResult.Id = cert.Id;
            }

            ctx.Uow.Save();
            actionResult.Success = true;
            return actionResult;
        }

        public EntityCertificateInventory GetCertificate(int certificateId)
        {
            return ctx.Uow.CertificateInventoryRepository.GetById(certificateId);
        }

        public List<EntityCertificateInventory> Search(DtoSearchFilter filter)
        {
            return ctx.Uow.CertificateInventoryRepository.Get(x => x.Subject.Contains(filter.SearchText)).OrderBy(x => x.Subject).ToList();
        }

        public string TotalCount()
        {
            return ctx.Uow.CertificateInventoryRepository.Count();
        }
    }
}