using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceComputerSystemInventory(EntityContext ectx)
    {
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
            var existing = ectx.Uow.ComputerSystemInventoryRepository.GetFirstOrDefault(x => x.ComputerId == inventory.ComputerId);
            if (existing == null)
            {
                ectx.Uow.ComputerSystemInventoryRepository.Insert(inventory);
            }
            else
            {
                inventory.Id = existing.Id;
                ectx.Uow.ComputerSystemInventoryRepository.Update(inventory, inventory.Id);
            }
                 
            ectx.Uow.Save();
            actionResult.Success = true;
            actionResult.Id = inventory.Id;

            return actionResult;
        }
    }
}