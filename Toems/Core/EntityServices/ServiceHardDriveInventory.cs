using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceHardDriveInventory(ServiceContext ctx)
    {
        public DtoActionResult AddOrUpdate(List<EntityHardDriveInventory> inventory, int computerId)
        {

            var actionResult = new DtoActionResult();
            var pToRemove = ctx.Uow.HardDriveInventoryRepository.Get(x => x.ComputerId == computerId);
            foreach (var hd in inventory)
            {
                try
                {
                    var h = Convert.ToInt64(hd.Size);
                    hd.SizeMb = Convert.ToInt32(h / 1024 / 1024);
                }
                catch
                {
                    hd.SizeMb = 0;
                }


                hd.ComputerId = computerId;
                var existing = ctx.Uow.HardDriveInventoryRepository.GetFirstOrDefault(x => x.ComputerId == hd.ComputerId && x.Model == hd.Model && x.SerialNumber == hd.SerialNumber);
                if (existing == null)
                {
                    ctx.Uow.HardDriveInventoryRepository.Insert(hd);
                }
                else
                {
                    pToRemove.Remove(existing);
                    hd.Id = existing.Id;
                    ctx.Uow.HardDriveInventoryRepository.Update(hd, hd.Id);
                }

                actionResult.Id = hd.Id;
            }

            //anything left in pToRemove does not exist on that computer anymore
            foreach (var p in pToRemove)
            {
                ctx.Uow.HardDriveInventoryRepository.Delete(p.Id);
            }

            ctx.Uow.Save();
            actionResult.Success = true;
            return actionResult;
        }
    }
}