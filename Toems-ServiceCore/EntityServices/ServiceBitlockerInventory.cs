using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceBitlockerInventory(ServiceContext ctx)
    {
        public DtoActionResult AddOrUpdate(List<EntityBitlockerInventory> inventory, int computerId)
        {
            var actionResult = new DtoActionResult();
            var pToRemove = ctx.Uow.BitlockerRepository.Get(x => x.ComputerId == computerId);
            foreach (var bl in inventory)
            {
                bl.ComputerId = computerId;
                var localBl = bl;
                bl.Status = Convert.ToInt32(bl.ProtectionStatus);
                var existing = ctx.Uow.BitlockerRepository.GetFirstOrDefault(x => x.ComputerId == localBl.ComputerId && x.DriveLetter == localBl.DriveLetter);
                if (existing == null)
                {
                    ctx.Uow.BitlockerRepository.Insert(bl);
                }
                else
                {
                    pToRemove.Remove(existing);
                    bl.Id = existing.Id;
                    ctx.Uow.BitlockerRepository.Update(bl, bl.Id);
                }
                 actionResult.Id = bl.Id;
            }

            //anything left in pToRemove does not exist on that computer anymore
            foreach (var p in pToRemove)
            {
                ctx.Uow.BitlockerRepository.Delete(p.Id);
            }

            ctx.Uow.Save();
            actionResult.Success = true;
            return actionResult;
        }
    }
}