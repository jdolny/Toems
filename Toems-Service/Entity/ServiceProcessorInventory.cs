using System;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceProcessorInventory
    {
        private readonly UnitOfWork _uow;

        public ServiceProcessorInventory()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult AddOrUpdate(EntityProcessorInventory inventory, int computerId)
        {
            inventory.ComputerId = computerId;
            try
            {
                inventory.Speed = Convert.ToInt32(inventory.MaxClockSpeed);
                inventory.Cores = Convert.ToInt32(inventory.NumberOfCores);
            }
            catch
            {
                inventory.Speed = 0;
                inventory.Cores = 0;
            }
            var actionResult = new DtoActionResult();
            var existing = _uow.ProcessorInventoryRepository.GetFirstOrDefault(x => x.ComputerId == inventory.ComputerId);
            if (existing == null)
            {
                _uow.ProcessorInventoryRepository.Insert(inventory);
            }
            else
            {
                inventory.Id = existing.Id;
                _uow.ProcessorInventoryRepository.Update(inventory, inventory.Id);
            }
                 
            _uow.Save();
            actionResult.Success = true;
            actionResult.Id = inventory.Id;

            return actionResult;
        }
    }
}