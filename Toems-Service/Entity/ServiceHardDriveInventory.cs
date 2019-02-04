using System;
using System.Collections.Generic;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceHardDriveInventory
    {
        private readonly UnitOfWork _uow;

        public ServiceHardDriveInventory()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult AddOrUpdate(List<EntityHardDriveInventory> inventory, int computerId)
        {

            var actionResult = new DtoActionResult();
            var pToRemove = _uow.HardDriveInventoryRepository.Get(x => x.ComputerId == computerId);
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
                var existing = _uow.HardDriveInventoryRepository.GetFirstOrDefault(x => x.ComputerId == hd.ComputerId && x.Model == hd.Model && x.SerialNumber == hd.SerialNumber);
                if (existing == null)
                {
                    _uow.HardDriveInventoryRepository.Insert(hd);
                }
                else
                {
                    pToRemove.Remove(existing);
                    hd.Id = existing.Id;
                    _uow.HardDriveInventoryRepository.Update(hd, hd.Id);
                }
                 actionResult.Id = hd.Id;
            }

            //anything left in pToRemove does not exist on that computer anymore
            foreach (var p in pToRemove)
            {
                _uow.HardDriveInventoryRepository.Delete(p.Id);
            }

            _uow.Save();
            actionResult.Success = true;
            return actionResult;
        }
    }
}