using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceAntivirusInventory(ServiceContext ctx)
    {
        public DtoActionResult AddOrUpdate(List<EntityAntivirusInventory> inventory, int computerId)
        {
            var actionResult = new DtoActionResult();
            var pToRemove = ctx.Uow.AntivirusRepository.Get(x => x.ComputerId == computerId);
            foreach (var av in inventory)
            {
                av.ComputerId = computerId;
                var localAv = av;
                var existing = ctx.Uow.AntivirusRepository.GetFirstOrDefault(x => x.ComputerId == localAv.ComputerId && x.DisplayName == localAv.DisplayName);
                if (existing == null)
                {
                    ctx.Uow.AntivirusRepository.Insert(av);
                }
                else
                {
                    pToRemove.Remove(existing);
                    av.Id = existing.Id;
                    ctx.Uow.AntivirusRepository.Update(av, av.Id);
                }
                 actionResult.Id = av.Id;
            }

            //anything left in pToRemove does not exist on that computer anymore
            foreach (var p in pToRemove)
            {
                ctx.Uow.AntivirusRepository.Delete(p.Id);
            }

            ctx.Uow.Save();
            actionResult.Success = true;
            return actionResult;
        }
    }
}