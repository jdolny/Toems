using System;
using System.Collections.Generic;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceComputerGpuInventory
    {
        private readonly UnitOfWork _uow;

        public ServiceComputerGpuInventory()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult AddOrUpdate(List<EntityComputerGpuInventory> inventory, int computerId)
        {
            var actionResult = new DtoActionResult();
            var pToRemove = _uow.ComputerGpuRepository.Get(x => x.ComputerId == computerId);
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
                var existing = _uow.ComputerGpuRepository.GetFirstOrDefault(x => x.ComputerId == gpu.ComputerId && x.Name == gpu.Name);
                if (existing == null)
                {
                    _uow.ComputerGpuRepository.Insert(gpu);
                }
                else
                {
                    pToRemove.Remove(existing);
                    gpu.Id = existing.Id;
                    _uow.ComputerGpuRepository.Update(gpu, gpu.Id);
                }
                actionResult.Id = gpu.Id;
            }

            //anything left in pToRemove does not exist on that computer anymore
            foreach (var p in pToRemove)
            {
                _uow.ComputerGpuRepository.Delete(p.Id);
            }

            _uow.Save();
            actionResult.Success = true;
            return actionResult;
        }
    }
}