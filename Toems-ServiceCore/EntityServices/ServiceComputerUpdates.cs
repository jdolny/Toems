using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceComputerUpdates(EntityContext ectx)
    {
        public DtoActionResult AddOrUpdate(List<EntityWindowsUpdateInventory> inventory, int computerId)
        {
            var actionResult = new DtoActionResult();
            var pToRemove = ectx.Uow.ComputerUpdatesRepository.Get(x => x.ComputerId == computerId);
            foreach (var update in inventory)
            {
                var localUpdate = update;
                var s = ectx.Uow.WuInventoryRepository.GetFirstOrDefault(x => x.Title == localUpdate.Title);
                //check if update exists before adding computer relationship
                if (s == null)
                    continue;

                var existingRelationship =
                    ectx.Uow.ComputerUpdatesRepository.GetFirstOrDefault(
                        x => x.ComputerId == computerId && x.UpdateId == s.Id);

                if (existingRelationship == null)
                {
                    existingRelationship = new EntityComputerUpdates();
                    existingRelationship.ComputerId = computerId;
                    existingRelationship.UpdateId = s.Id;
                    existingRelationship.IsInstalled = update.IsInstalled;
                    existingRelationship.LastDeploymentChangeTime = update.LastDeploymentChangeTime;
                    ectx.Uow.ComputerUpdatesRepository.Insert(existingRelationship);
                }
                else
                {
                    pToRemove.Remove(existingRelationship);
                    existingRelationship.IsInstalled = update.IsInstalled;
                    existingRelationship.LastDeploymentChangeTime = update.LastDeploymentChangeTime;
                    ectx.Uow.ComputerUpdatesRepository.Update(existingRelationship,existingRelationship.Id);
                }
            }

            //anything left in pToRemove does not exist on that computer anymore
            foreach (var p in pToRemove)
            {
                ectx.Uow.ComputerUpdatesRepository.Delete(p.Id);
            }

            ectx.Uow.Save();
            actionResult.Success = true;
            return actionResult;
        }
    }
}