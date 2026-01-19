using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceComputerSoftware(EntityContext ectx)
    {
        public DtoActionResult AddOrUpdate(List<EntitySoftwareInventory> inventory, int computerId)
        {
            var actionResult = new DtoActionResult();
            var pToRemove = ectx.Uow.ComputerSoftwareRepository.Get(x => x.ComputerId == computerId);
            foreach (var software in inventory)
            {
                var localSoftware = software;
                var s = ectx.Uow.SoftwareInventoryRepository.GetFirstOrDefault(x => x.Name == localSoftware.Name && x.Version == localSoftware.Version);
                //check if software exists before adding computer relationship
                if (s == null)
                    continue;

                var existingRelationship =
                    ectx.Uow.ComputerSoftwareRepository.GetFirstOrDefault(
                        x => x.ComputerId == computerId && x.SoftwareId == s.Id);

                if (existingRelationship == null)
                {
                    existingRelationship = new EntityComputerSoftware();
                    existingRelationship.ComputerId = computerId;
                    existingRelationship.SoftwareId = s.Id;
                    ectx.Uow.ComputerSoftwareRepository.Insert(existingRelationship);
                }
                else
                {
                    pToRemove.Remove(existingRelationship);           
                }
            }

            //anything left in pToRemove does not exist on that computer anymore
            foreach (var p in pToRemove)
            {
                ectx.Uow.ComputerSoftwareRepository.Delete(p.Id);
            }

            ectx.Uow.Save();
            actionResult.Success = true;
            return actionResult;
        }
    }
}