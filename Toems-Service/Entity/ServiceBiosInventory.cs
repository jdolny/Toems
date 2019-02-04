using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceBiosInventory
    {
        private readonly UnitOfWork _uow;

        public ServiceBiosInventory()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult AddOrUpdate(EntityBiosInventory inventory, int computerId)
        {
            inventory.ComputerId = computerId;
            var actionResult = new DtoActionResult();
            var existing = _uow.BiosInventoryRepository.GetFirstOrDefault(x => x.ComputerId == inventory.ComputerId);
            if (existing == null)
            {
                _uow.BiosInventoryRepository.Insert(inventory);
            }
            else
            {
                inventory.Id = existing.Id;
                _uow.BiosInventoryRepository.Update(inventory, inventory.Id);
            }
                 
            _uow.Save();
            actionResult.Success = true;
            actionResult.Id = inventory.Id;

            return actionResult;
        }
    }
}