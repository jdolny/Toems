using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceFirewallInventory
    {
        private readonly UnitOfWork _uow;

        public ServiceFirewallInventory()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult AddOrUpdate(EntityFirewallInventory inventory, int computerId)
        {
            inventory.ComputerId = computerId;
            var actionResult = new DtoActionResult();
            var existing = _uow.FirewallRepository.GetFirstOrDefault(x => x.ComputerId == inventory.ComputerId);
            if (existing == null)
            {
                _uow.FirewallRepository.Insert(inventory);
            }
            else
            {
                inventory.Id = existing.Id;
                _uow.FirewallRepository.Update(inventory, inventory.Id);
            }
                 
            _uow.Save();
            actionResult.Success = true;
            actionResult.Id = inventory.Id;

            return actionResult;
        }
    }
}