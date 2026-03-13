using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceOsInventory(ServiceContext ctx)
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
            var existing = ctx.Uow.OsInventoryRepository.GetFirstOrDefault(x => x.ComputerId == inventory.ComputerId);
            if (existing == null)
            {
                ctx.Uow.OsInventoryRepository.Insert(inventory);
            }
            else
            {
                inventory.Id = existing.Id;
                ctx.Uow.OsInventoryRepository.Update(inventory, inventory.Id);
            }
                 
            ctx.Uow.Save();
            actionResult.Success = true;
            actionResult.Id = inventory.Id;

            return actionResult;
        }
    }
}