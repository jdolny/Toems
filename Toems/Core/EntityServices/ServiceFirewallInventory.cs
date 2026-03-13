using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceFirewallInventory(ServiceContext ctx)
    {
        public DtoActionResult AddOrUpdate(EntityFirewallInventory inventory, int computerId)
        {
            inventory.ComputerId = computerId;
            var actionResult = new DtoActionResult();
            var existing = ctx.Uow.FirewallRepository.GetFirstOrDefault(x => x.ComputerId == inventory.ComputerId);
            if (existing == null)
            {
                ctx.Uow.FirewallRepository.Insert(inventory);
            }
            else
            {
                inventory.Id = existing.Id;
                ctx.Uow.FirewallRepository.Update(inventory, inventory.Id);
            }
                 
            ctx.Uow.Save();
            actionResult.Success = true;
            actionResult.Id = inventory.Id;

            return actionResult;
        }
    }
}