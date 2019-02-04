using System;
using System.Collections.Generic;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceBitlockerInventory
    {
        private readonly UnitOfWork _uow;

        public ServiceBitlockerInventory()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult AddOrUpdate(List<EntityBitlockerInventory> inventory, int computerId)
        {
            var actionResult = new DtoActionResult();
            var pToRemove = _uow.BitlockerRepository.Get(x => x.ComputerId == computerId);
            foreach (var bl in inventory)
            {
                bl.ComputerId = computerId;
                var localBl = bl;
                bl.Status = Convert.ToInt32(bl.ProtectionStatus);
                var existing = _uow.BitlockerRepository.GetFirstOrDefault(x => x.ComputerId == localBl.ComputerId && x.DriveLetter == localBl.DriveLetter);
                if (existing == null)
                {
                    _uow.BitlockerRepository.Insert(bl);
                }
                else
                {
                    pToRemove.Remove(existing);
                    bl.Id = existing.Id;
                    _uow.BitlockerRepository.Update(bl, bl.Id);
                }
                 actionResult.Id = bl.Id;
            }

            //anything left in pToRemove does not exist on that computer anymore
            foreach (var p in pToRemove)
            {
                _uow.BitlockerRepository.Delete(p.Id);
            }

            _uow.Save();
            actionResult.Success = true;
            return actionResult;
        }
    }
}