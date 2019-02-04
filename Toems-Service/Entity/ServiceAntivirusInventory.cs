using System.Collections.Generic;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceAntivirusInventory
    {
        private readonly UnitOfWork _uow;

        public ServiceAntivirusInventory()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult AddOrUpdate(List<EntityAntivirusInventory> inventory, int computerId)
        {
            var actionResult = new DtoActionResult();
            var pToRemove = _uow.AntivirusRepository.Get(x => x.ComputerId == computerId);
            foreach (var av in inventory)
            {
                av.ComputerId = computerId;
                var localAv = av;
                var existing = _uow.AntivirusRepository.GetFirstOrDefault(x => x.ComputerId == localAv.ComputerId && x.DisplayName == localAv.DisplayName);
                if (existing == null)
                {
                    _uow.AntivirusRepository.Insert(av);
                }
                else
                {
                    pToRemove.Remove(existing);
                    av.Id = existing.Id;
                    _uow.AntivirusRepository.Update(av, av.Id);
                }
                 actionResult.Id = av.Id;
            }

            //anything left in pToRemove does not exist on that computer anymore
            foreach (var p in pToRemove)
            {
                _uow.AntivirusRepository.Delete(p.Id);
            }

            _uow.Save();
            actionResult.Success = true;
            return actionResult;
        }
    }
}