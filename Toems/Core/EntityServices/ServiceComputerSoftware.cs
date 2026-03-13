using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceComputerSoftware(ServiceContext ctx)
    {
        public DtoActionResult AddOrUpdate(List<EntitySoftwareInventory> inventory, int computerId)
        {
            var actionResult = new DtoActionResult();
            var pToRemove = ctx.Uow.ComputerSoftwareRepository.Get(x => x.ComputerId == computerId);
            foreach (var software in inventory)
            {
                var localSoftware = software;
                var s = ctx.Uow.SoftwareInventoryRepository.GetFirstOrDefault(x => x.Name == localSoftware.Name && x.Version == localSoftware.Version);
                //check if software exists before adding computer relationship
                if (s == null)
                    continue;

                var existingRelationship =
                    ctx.Uow.ComputerSoftwareRepository.GetFirstOrDefault(
                        x => x.ComputerId == computerId && x.SoftwareId == s.Id);

                if (existingRelationship == null)
                {
                    existingRelationship = new EntityComputerSoftware();
                    existingRelationship.ComputerId = computerId;
                    existingRelationship.SoftwareId = s.Id;
                    ctx.Uow.ComputerSoftwareRepository.Insert(existingRelationship);
                }
                else
                {
                    pToRemove.Remove(existingRelationship);           
                }
            }

            //anything left in pToRemove does not exist on that computer anymore
            foreach (var p in pToRemove)
            {
                ctx.Uow.ComputerSoftwareRepository.Delete(p.Id);
            }

            ctx.Uow.Save();
            actionResult.Success = true;
            return actionResult;
        }
    }
}