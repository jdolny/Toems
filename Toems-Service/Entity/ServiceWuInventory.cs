using System.Collections.Generic;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceWuInventory
    {
        private readonly UnitOfWork _uow;

        public ServiceWuInventory()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult Add(List<EntityWindowsUpdateInventory> inventory)
        {
            var actionResult = new DtoActionResult();
            foreach (var update in inventory)
            {
                var localUpdate = update;
                var existing =
                    _uow.WuInventoryRepository.GetFirstOrDefault(
                        x => x.Title == localUpdate.Title);
                if (existing == null)
                {
                    _uow.WuInventoryRepository.Insert(update);
                }

                actionResult.Id = update.Id;
            }

            _uow.Save();
            actionResult.Success = true;
            return actionResult;
        }

    }
}