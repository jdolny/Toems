using System.Collections.Generic;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceNicInventory
    {
        private readonly UnitOfWork _uow;

        public ServiceNicInventory()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult AddOrUpdate(List<EntityNicInventory> inventory, int computerId)
        {
            var actionResult = new DtoActionResult();
            var pToRemove = _uow.NicInventoryRepository.Get(x => x.ComputerId == computerId);
            foreach (var nic in inventory)
            {
                nic.ComputerId = computerId;
                var localNic = nic;
                var existing = _uow.NicInventoryRepository.GetFirstOrDefault(x => x.ComputerId == localNic.ComputerId && x.Name.Equals(localNic.Name));
                if (existing == null)
                {
                    _uow.NicInventoryRepository.Insert(nic);
                }
                else
                {
                    pToRemove.Remove(existing);
                    nic.Id = existing.Id;
                    _uow.NicInventoryRepository.Update(nic, nic.Id);
                }
                 actionResult.Id = nic.Id;
            }

            //anything left in pToRemove does not exist on that computer anymore
            foreach (var p in pToRemove)
            {
                _uow.NicInventoryRepository.Delete(p.Id);
            }

            _uow.Save();
            actionResult.Success = true;
            return actionResult;
        }
    }
}