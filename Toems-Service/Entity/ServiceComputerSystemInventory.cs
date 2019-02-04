using System;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceComputerSystemInventory
    {
        private readonly UnitOfWork _uow;

        public ServiceComputerSystemInventory()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult AddOrUpdate(EntityComputerSystemInventory inventory, int computerId)
        {
            inventory.ComputerId = computerId;
            try
            {
                var m = Convert.ToInt64(inventory.TotalPhysicalMemory);
                inventory.Memory = Convert.ToInt32(m / 1024 / 1024);
            }
            catch
            {
                inventory.Memory = 0;
            }
          

            if (!string.IsNullOrEmpty(inventory.Domain))
            {
                if (inventory.Domain.ToLower().Equals("workgroup"))
                    inventory.Domain = string.Empty;
            }

            var actionResult = new DtoActionResult();
            var existing = _uow.ComputerSystemInventoryRepository.GetFirstOrDefault(x => x.ComputerId == inventory.ComputerId);
            if (existing == null)
            {
                _uow.ComputerSystemInventoryRepository.Insert(inventory);
            }
            else
            {
                inventory.Id = existing.Id;
                _uow.ComputerSystemInventoryRepository.Update(inventory, inventory.Id);
            }
                 
            _uow.Save();
            actionResult.Success = true;
            actionResult.Id = inventory.Id;

            return actionResult;
        }
    }
}