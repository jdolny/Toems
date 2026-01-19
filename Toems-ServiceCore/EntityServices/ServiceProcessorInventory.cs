using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceProcessorInventory(EntityContext ectx)
    {
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
            var existing = ectx.Uow.ProcessorInventoryRepository.GetFirstOrDefault(x => x.ComputerId == inventory.ComputerId);
            if (existing == null)
            {
                ectx.Uow.ProcessorInventoryRepository.Insert(inventory);
            }
            else
            {
                inventory.Id = existing.Id;
                ectx.Uow.ProcessorInventoryRepository.Update(inventory, inventory.Id);
            }
                 
            ectx.Uow.Save();
            actionResult.Success = true;
            actionResult.Id = inventory.Id;

            return actionResult;
        }
    }
}