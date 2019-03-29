using System.Collections.Generic;
using System.Linq;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceCertificateInventory
    {
        private readonly UnitOfWork _uow;

        public ServiceCertificateInventory()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult Add(List<EntityCertificateInventory> inventory)
        {
            var actionResult = new DtoActionResult();
            foreach (var cert in inventory)
            {
                var localCert = cert;
                var existing = _uow.CertificateInventoryRepository.GetFirstOrDefault(x => x.Thumbprint == localCert.Thumbprint && x.Serial == localCert.Serial);
                if (existing == null)
                {
                    _uow.CertificateInventoryRepository.Insert(cert);
                }
               
                 actionResult.Id = cert.Id;
            }

            _uow.Save();
            actionResult.Success = true;
            return actionResult;
        }

        public EntityCertificateInventory GetCertificate(int certificateId)
        {
            return _uow.CertificateInventoryRepository.GetById(certificateId);
        }

        public List<EntityCertificateInventory> Search(DtoSearchFilter filter)
        {
            return _uow.CertificateInventoryRepository.Get(x => x.Subject.Contains(filter.SearchText)).OrderBy(x => x.Subject).ToList();
        }

        public string TotalCount()
        {
            return _uow.CertificateInventoryRepository.Count();
        }
    }
}