using System;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceOsInventory
    {
        private readonly UnitOfWork _uow;

        public ServiceOsInventory()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult AddOrUpdate(EntityOsInventory inventory, int computerId)
        {
            inventory.ComputerId = computerId;
            try
            {
                inventory.SpMajor = Convert.ToInt16(inventory.ServicePackMajorVersion);
                inventory.SpMinor = Convert.ToInt16(inventory.ServicePackMinorVersion);
            }
            catch
            {
                inventory.SpMajor = 0;
                inventory.SpMinor = 0;
            }

            if(string.IsNullOrEmpty(inventory.UpdateServer))
            inventory.UpdateServer = "Not Defined";
            var actionResult = new DtoActionResult();
            var existing = _uow.OsInventoryRepository.GetFirstOrDefault(x => x.ComputerId == inventory.ComputerId);
            if (existing == null)
            {
                _uow.OsInventoryRepository.Insert(inventory);
            }
            else
            {
                inventory.Id = existing.Id;
                _uow.OsInventoryRepository.Update(inventory, inventory.Id);
            }
                 
            _uow.Save();
            actionResult.Success = true;
            actionResult.Id = inventory.Id;

            return actionResult;
        }
    }
}