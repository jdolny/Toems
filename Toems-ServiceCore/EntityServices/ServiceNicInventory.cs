using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceNicInventory(EntityContext ectx)
    {
        public DtoActionResult AddOrUpdate(List<EntityNicInventory> inventory, int computerId)
        {
            var actionResult = new DtoActionResult();
            var pToRemove = ectx.Uow.NicInventoryRepository.Get(x => x.ComputerId == computerId);
            foreach (var nic in inventory)
            {
                nic.ComputerId = computerId;
                var localNic = nic;
                var existing = ectx.Uow.NicInventoryRepository.GetFirstOrDefault(x => x.ComputerId == localNic.ComputerId && x.Name.Equals(localNic.Name));
                if (existing == null)
                {
                    ectx.Uow.NicInventoryRepository.Insert(nic);
                }
                else
                {
                    pToRemove.Remove(existing);
                    nic.Id = existing.Id;
                    ectx.Uow.NicInventoryRepository.Update(nic, nic.Id);
                }
                 actionResult.Id = nic.Id;
            }

            //anything left in pToRemove does not exist on that computer anymore
            foreach (var p in pToRemove)
            {
                ectx.Uow.NicInventoryRepository.Delete(p.Id);
            }

            ectx.Uow.Save();
            actionResult.Success = true;
            return actionResult;
        }
    }
}