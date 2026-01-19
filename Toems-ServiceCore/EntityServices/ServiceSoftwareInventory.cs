using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceSoftwareInventory(EntityContext ectx)
    {
        public DtoActionResult Add(List<EntitySoftwareInventory> inventory)
        {
            var actionResult = new DtoActionResult();
            foreach (var software in inventory)
            {
                var localSoftware = software;
                var existing = ectx.Uow.SoftwareInventoryRepository.GetFirstOrDefault(x => x.Name == localSoftware.Name && x.Version == localSoftware.Version);
                if (existing == null)
                {
                    ectx.Uow.SoftwareInventoryRepository.Insert(software);
                    actionResult.Id = software.Id;
                }
                else if (string.IsNullOrEmpty(existing.UninstallString) && !string.IsNullOrEmpty(software.UninstallString))
                {
                    //update to add uninstall string as of version 1.4.8
                    existing.UninstallString = software.UninstallString;
                    ectx.Uow.SoftwareInventoryRepository.Update(existing, existing.Id);
                    actionResult.Id = existing.Id;
                }
            }

            ectx.Uow.Save();
            actionResult.Success = true;
            return actionResult;
        }

        public EntitySoftwareInventory GetSoftware(int softwareId)
        {
            return ectx.Uow.SoftwareInventoryRepository.GetById(softwareId);
        }

        public List<EntitySoftwareInventory> Search(DtoSearchFilter filter)
        {
            return ectx.Uow.SoftwareInventoryRepository.Get(x => x.Name.Contains(filter.SearchText)).OrderBy(x => x.Name).ToList();
        }

        public string TotalCount()
        {
            return ectx.Uow.SoftwareInventoryRepository.Count();
        }
    }
}