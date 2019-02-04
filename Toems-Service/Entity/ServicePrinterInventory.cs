using System.Collections.Generic;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServicePrinterInventory
    {
        private readonly UnitOfWork _uow;

        public ServicePrinterInventory()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult AddOrUpdate(List<EntityPrinterInventory> inventory, int computerId)
        {
            var actionResult = new DtoActionResult();
            var pToRemove = _uow.PrinterInventoryRepository.Get(x => x.ComputerId == computerId);
            foreach (var printer in inventory)
            {
                printer.ComputerId = computerId;
                var existing = _uow.PrinterInventoryRepository.GetFirstOrDefault(x => x.ComputerId == printer.ComputerId && x.SystemName == printer.SystemName && x.Name == printer.Name);
                if (existing == null)
                {
                    _uow.PrinterInventoryRepository.Insert(printer);
                }
                else
                {
                    pToRemove.Remove(existing);
                    printer.Id = existing.Id;
                    _uow.PrinterInventoryRepository.Update(printer, printer.Id);
                }
                 actionResult.Id = printer.Id;
            }

            //anything left in pToRemove does not exist on that computer anymore
            foreach (var p in pToRemove)
            {
                _uow.PrinterInventoryRepository.Delete(p.Id);
            }

            _uow.Save();
            actionResult.Success = true;
            return actionResult;
        }
    }
}