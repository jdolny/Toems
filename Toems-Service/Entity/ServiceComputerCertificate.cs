using System.Collections.Generic;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceComputerCertificate
    {
        private readonly UnitOfWork _uow;

        public ServiceComputerCertificate()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult AddOrUpdate(List<EntityCertificateInventory> inventory, int computerId)
        {
            var actionResult = new DtoActionResult();
            var pToRemove = _uow.ComputerCertificateRepository.Get(x => x.ComputerId == computerId);
            foreach (var cert in inventory)
            {
                var localCert = cert;
                var s = _uow.CertificateInventoryRepository.GetFirstOrDefault(x => x.Thumbprint == localCert.Thumbprint && x.Serial == localCert.Serial);
                //check if certificate exists before adding computer relationship
                if (s == null)
                    continue;

                var existingRelationship =
                    _uow.ComputerCertificateRepository.GetFirstOrDefault(
                        x => x.ComputerId == computerId && x.CertificateId == s.Id);

                if (existingRelationship == null)
                {
                    existingRelationship = new EntityComputerCertificate();
                    existingRelationship.ComputerId = computerId;
                    existingRelationship.CertificateId = s.Id;
                    _uow.ComputerCertificateRepository.Insert(existingRelationship);
                }
                else
                {
                    pToRemove.Remove(existingRelationship);           
                }
            }

            //anything left in pToRemove does not exist on that computer anymore
            foreach (var p in pToRemove)
            {
                _uow.ComputerCertificateRepository.Delete(p.Id);
            }

            _uow.Save();
            actionResult.Success = true;
            return actionResult;
        }
    }
}