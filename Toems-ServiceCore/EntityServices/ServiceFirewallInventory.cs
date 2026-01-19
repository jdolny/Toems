using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceFirewallInventory(EntityContext ectx)
    {
        public DtoActionResult AddOrUpdate(EntityFirewallInventory inventory, int computerId)
        {
            inventory.ComputerId = computerId;
            var actionResult = new DtoActionResult();
            var existing = ectx.Uow.FirewallRepository.GetFirstOrDefault(x => x.ComputerId == inventory.ComputerId);
            if (existing == null)
            {
                ectx.Uow.FirewallRepository.Insert(inventory);
            }
            else
            {
                inventory.Id = existing.Id;
                ectx.Uow.FirewallRepository.Update(inventory, inventory.Id);
            }
                 
            ectx.Uow.Save();
            actionResult.Success = true;
            actionResult.Id = inventory.Id;

            return actionResult;
        }
    }
}