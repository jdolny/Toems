using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceComputerGpuInventory(EntityContext ectx)
    {
        public DtoActionResult AddOrUpdate(List<EntityComputerGpuInventory> inventory, int computerId)
        {
            var actionResult = new DtoActionResult();
            var pToRemove = ectx.Uow.ComputerGpuRepository.Get(x => x.ComputerId == computerId);
            foreach (var gpu in inventory)
            {
                try
                {
                    var h = Convert.ToInt64(gpu.AdapterRam);
                    gpu.Memory = Convert.ToInt32(h / 1024 / 1024);
                }
                catch
                {
                    gpu.Memory = 0;
                }


                gpu.ComputerId = computerId;
                var existing = ectx.Uow.ComputerGpuRepository.GetFirstOrDefault(x => x.ComputerId == gpu.ComputerId && x.Name == gpu.Name);
                if (existing == null)
                {
                    ectx.Uow.ComputerGpuRepository.Insert(gpu);
                }
                else
                {
                    pToRemove.Remove(existing);
                    gpu.Id = existing.Id;
                    ectx.Uow.ComputerGpuRepository.Update(gpu, gpu.Id);
                }
                actionResult.Id = gpu.Id;
            }

            //anything left in pToRemove does not exist on that computer anymore
            foreach (var p in pToRemove)
            {
                ectx.Uow.ComputerGpuRepository.Delete(p.Id);
            }

            ectx.Uow.Save();
            actionResult.Success = true;
            return actionResult;
        }
    }
}