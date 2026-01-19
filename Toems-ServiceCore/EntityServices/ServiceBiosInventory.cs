using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceBiosInventory(EntityContext ectx)
    {
        public DtoActionResult AddOrUpdate(EntityBiosInventory inventory, int computerId)
        {
            inventory.ComputerId = computerId;
            var actionResult = new DtoActionResult();
            var existing = ectx.Uow.BiosInventoryRepository.GetFirstOrDefault(x => x.ComputerId == inventory.ComputerId);
            if (existing == null)
            {
                ectx.Uow.BiosInventoryRepository.Insert(inventory);
            }
            else
            {
                inventory.Id = existing.Id;
                ectx.Uow.BiosInventoryRepository.Update(inventory, inventory.Id);
            }
                 
            ectx.Uow.Save();
            actionResult.Success = true;
            actionResult.Id = inventory.Id;

            return actionResult;
        }
    }
}