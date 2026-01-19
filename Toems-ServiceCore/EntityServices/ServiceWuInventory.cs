using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceWuInventory(EntityContext ectx)
    {
        public DtoActionResult Add(List<EntityWindowsUpdateInventory> inventory)
        {
            var actionResult = new DtoActionResult();
            foreach (var update in inventory)
            {
                var localUpdate = update;
                var existing =
                    ectx.Uow.WuInventoryRepository.GetFirstOrDefault(
                        x => x.Title == localUpdate.Title);
                if (existing == null)
                {
                    ectx.Uow.WuInventoryRepository.Insert(update);
                }

                actionResult.Id = update.Id;
            }

            ectx.Uow.Save();
            actionResult.Success = true;
            return actionResult;
        }

    }
}