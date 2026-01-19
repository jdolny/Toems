using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceLogicalVolumeInventory(EntityContext ectx)
    {
        public DtoActionResult AddOrUpdate(List<EntityLogicalVolumeInventory> inventory, int computerId)
        {
            var actionResult = new DtoActionResult();
            var pToRemove = ectx.Uow.LogicalVolumeRepository.Get(x => x.ComputerId == computerId);
            foreach (var lv in inventory)
            {
                lv.ComputerId = computerId;
                var localLv = lv;
                var existing = ectx.Uow.LogicalVolumeRepository.GetFirstOrDefault(x => x.ComputerId == localLv.ComputerId && x.Drive == localLv.Drive);
                if (existing == null)
                {
                    ectx.Uow.LogicalVolumeRepository.Insert(lv);
                }
                else
                {
                    pToRemove.Remove(existing);
                    lv.Id = existing.Id;
                    ectx.Uow.LogicalVolumeRepository.Update(lv, lv.Id);
                }
                 actionResult.Id = lv.Id;
            }

            //anything left in pToRemove does not exist on that computer anymore
            foreach (var p in pToRemove)
            {
                ectx.Uow.LogicalVolumeRepository.Delete(p.Id);
            }

            ectx.Uow.Save();
            actionResult.Success = true;
            return actionResult;
        }
    }
}