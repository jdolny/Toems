using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceBitlockerInventory(EntityContext ectx)
    {
        public DtoActionResult AddOrUpdate(List<EntityBitlockerInventory> inventory, int computerId)
        {
            var actionResult = new DtoActionResult();
            var pToRemove = ectx.Uow.BitlockerRepository.Get(x => x.ComputerId == computerId);
            foreach (var bl in inventory)
            {
                bl.ComputerId = computerId;
                var localBl = bl;
                bl.Status = Convert.ToInt32(bl.ProtectionStatus);
                var existing = ectx.Uow.BitlockerRepository.GetFirstOrDefault(x => x.ComputerId == localBl.ComputerId && x.DriveLetter == localBl.DriveLetter);
                if (existing == null)
                {
                    ectx.Uow.BitlockerRepository.Insert(bl);
                }
                else
                {
                    pToRemove.Remove(existing);
                    bl.Id = existing.Id;
                    ectx.Uow.BitlockerRepository.Update(bl, bl.Id);
                }
                 actionResult.Id = bl.Id;
            }

            //anything left in pToRemove does not exist on that computer anymore
            foreach (var p in pToRemove)
            {
                ectx.Uow.BitlockerRepository.Delete(p.Id);
            }

            ectx.Uow.Save();
            actionResult.Success = true;
            return actionResult;
        }
    }
}