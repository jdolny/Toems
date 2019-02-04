using System.Collections.Generic;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceProcessInventory
    {
        private readonly UnitOfWork _uow;

        public ServiceProcessInventory()
        {
            _uow = new UnitOfWork();
        }

        public EntityProcessInventory GetProcess(int processId)
        {
            return _uow.ProcessInventoryRepository.GetById(processId);
        }

        public List<EntityProcessInventory> Search(DtoSearchFilter filter)
        {
            return _uow.ProcessInventoryRepository.Get(x => x.Name.Contains(filter.SearchText));
        }

        public string TotalCount()
        {
            return _uow.ProcessInventoryRepository.Count();
        }

        


    }
}