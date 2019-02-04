using System.Collections.Generic;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceComputerSoftware
    {
        private readonly UnitOfWork _uow;

        public ServiceComputerSoftware()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult AddOrUpdate(List<EntitySoftwareInventory> inventory, int computerId)
        {
            var actionResult = new DtoActionResult();
            var pToRemove = _uow.ComputerSoftwareRepository.Get(x => x.ComputerId == computerId);
            foreach (var software in inventory)
            {
                var localSoftware = software;
                var s = _uow.SoftwareInventoryRepository.GetFirstOrDefault(x => x.Name == localSoftware.Name && x.Version == localSoftware.Version);
                //check if software exists before adding computer relationship
                if (s == null)
                    continue;

                var existingRelationship =
                    _uow.ComputerSoftwareRepository.GetFirstOrDefault(
                        x => x.ComputerId == computerId && x.SoftwareId == s.Id);

                if (existingRelationship == null)
                {
                    existingRelationship = new EntityComputerSoftware();
                    existingRelationship.ComputerId = computerId;
                    existingRelationship.SoftwareId = s.Id;
                    _uow.ComputerSoftwareRepository.Insert(existingRelationship);
                }
                else
                {
                    pToRemove.Remove(existingRelationship);           
                }
            }

            //anything left in pToRemove does not exist on that computer anymore
            foreach (var p in pToRemove)
            {
                _uow.ComputerSoftwareRepository.Delete(p.Id);
            }

            _uow.Save();
            actionResult.Success = true;
            return actionResult;
        }
    }
}