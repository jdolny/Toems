using System.Collections.Generic;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceLogicalVolumeInventory
    {
        private readonly UnitOfWork _uow;

        public ServiceLogicalVolumeInventory()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult AddOrUpdate(List<EntityLogicalVolumeInventory> inventory, int computerId)
        {
            var actionResult = new DtoActionResult();
            var pToRemove = _uow.LogicalVolumeRepository.Get(x => x.ComputerId == computerId);
            foreach (var lv in inventory)
            {
                lv.ComputerId = computerId;
                var localLv = lv;
                var existing = _uow.LogicalVolumeRepository.GetFirstOrDefault(x => x.ComputerId == localLv.ComputerId && x.Drive == localLv.Drive);
                if (existing == null)
                {
                    _uow.LogicalVolumeRepository.Insert(lv);
                }
                else
                {
                    pToRemove.Remove(existing);
                    lv.Id = existing.Id;
                    _uow.LogicalVolumeRepository.Update(lv, lv.Id);
                }
                 actionResult.Id = lv.Id;
            }

            //anything left in pToRemove does not exist on that computer anymore
            foreach (var p in pToRemove)
            {
                _uow.LogicalVolumeRepository.Delete(p.Id);
            }

            _uow.Save();
            actionResult.Success = true;
            return actionResult;
        }
    }
}