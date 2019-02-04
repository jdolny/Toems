using System.Collections.Generic;
using System.Linq;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceSoftwareInventory
    {
        private readonly UnitOfWork _uow;

        public ServiceSoftwareInventory()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult Add(List<EntitySoftwareInventory> inventory)
        {
            var actionResult = new DtoActionResult();
            foreach (var software in inventory)
            {
                var localSoftware = software;
                var existing = _uow.SoftwareInventoryRepository.GetFirstOrDefault(x => x.Name == localSoftware.Name && x.Version == localSoftware.Version);
                if (existing == null)
                {
                    _uow.SoftwareInventoryRepository.Insert(software);
                }
               
                 actionResult.Id = software.Id;
            }

            _uow.Save();
            actionResult.Success = true;
            return actionResult;
        }

        public EntitySoftwareInventory GetSoftware(int softwareId)
        {
            return _uow.SoftwareInventoryRepository.GetById(softwareId);
        }

        public List<EntitySoftwareInventory> Search(DtoSearchFilter filter)
        {
            return _uow.SoftwareInventoryRepository.Get(x => x.Name.Contains(filter.SearchText)).OrderBy(x => x.Name).ToList();
        }

        public string TotalCount()
        {
            return _uow.SoftwareInventoryRepository.Count();
        }
    }
}