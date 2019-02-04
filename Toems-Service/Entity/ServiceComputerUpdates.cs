using System.Collections.Generic;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceComputerUpdates
    {
        private readonly UnitOfWork _uow;

        public ServiceComputerUpdates()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult AddOrUpdate(List<EntityWindowsUpdateInventory> inventory, int computerId)
        {
            var actionResult = new DtoActionResult();
            var pToRemove = _uow.ComputerUpdatesRepository.Get(x => x.ComputerId == computerId);
            foreach (var update in inventory)
            {
                var localUpdate = update;
                var s = _uow.WuInventoryRepository.GetFirstOrDefault(x => x.Title == localUpdate.Title);
                //check if update exists before adding computer relationship
                if (s == null)
                    continue;

                var existingRelationship =
                    _uow.ComputerUpdatesRepository.GetFirstOrDefault(
                        x => x.ComputerId == computerId && x.UpdateId == s.Id);

                if (existingRelationship == null)
                {
                    existingRelationship = new EntityComputerUpdates();
                    existingRelationship.ComputerId = computerId;
                    existingRelationship.UpdateId = s.Id;
                    existingRelationship.IsInstalled = update.IsInstalled;
                    existingRelationship.LastDeploymentChangeTime = update.LastDeploymentChangeTime;
                    _uow.ComputerUpdatesRepository.Insert(existingRelationship);
                }
                else
                {
                    pToRemove.Remove(existingRelationship);
                    existingRelationship.IsInstalled = update.IsInstalled;
                    existingRelationship.LastDeploymentChangeTime = update.LastDeploymentChangeTime;
                    _uow.ComputerUpdatesRepository.Update(existingRelationship,existingRelationship.Id);
                }
            }

            //anything left in pToRemove does not exist on that computer anymore
            foreach (var p in pToRemove)
            {
                _uow.ComputerUpdatesRepository.Delete(p.Id);
            }

            _uow.Save();
            actionResult.Success = true;
            return actionResult;
        }
    }
}