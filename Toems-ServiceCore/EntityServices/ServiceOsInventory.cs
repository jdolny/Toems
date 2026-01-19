using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceOsInventory(EntityContext ectx)
    {
        public DtoActionResult AddOrUpdate(EntityOsInventory inventory, int computerId)
        {
            inventory.ComputerId = computerId;
            try
            {
                inventory.SpMajor = Convert.ToInt16(inventory.ServicePackMajorVersion);
                inventory.SpMinor = Convert.ToInt16(inventory.ServicePackMinorVersion);
            }
            catch
            {
                inventory.SpMajor = 0;
                inventory.SpMinor = 0;
            }

            if(string.IsNullOrEmpty(inventory.UpdateServer))
            inventory.UpdateServer = "Not Defined";
            var actionResult = new DtoActionResult();
            var existing = ectx.Uow.OsInventoryRepository.GetFirstOrDefault(x => x.ComputerId == inventory.ComputerId);
            if (existing == null)
            {
                ectx.Uow.OsInventoryRepository.Insert(inventory);
            }
            else
            {
                inventory.Id = existing.Id;
                ectx.Uow.OsInventoryRepository.Update(inventory, inventory.Id);
            }
                 
            ectx.Uow.Save();
            actionResult.Success = true;
            actionResult.Id = inventory.Id;

            return actionResult;
        }
    }
}