using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServicePrinterInventory(EntityContext ectx)
    {
        public DtoActionResult AddOrUpdate(List<EntityPrinterInventory> inventory, int computerId)
        {
            var actionResult = new DtoActionResult();
            var pToRemove = ectx.Uow.PrinterInventoryRepository.Get(x => x.ComputerId == computerId);
            foreach (var printer in inventory)
            {
                printer.ComputerId = computerId;
                var existing = ectx.Uow.PrinterInventoryRepository.GetFirstOrDefault(x => x.ComputerId == printer.ComputerId && x.SystemName == printer.SystemName && x.Name == printer.Name);
                if (existing == null)
                {
                    ectx.Uow.PrinterInventoryRepository.Insert(printer);
                }
                else
                {
                    pToRemove.Remove(existing);
                    printer.Id = existing.Id;
                    ectx.Uow.PrinterInventoryRepository.Update(printer, printer.Id);
                }
                 actionResult.Id = printer.Id;
            }

            //anything left in pToRemove does not exist on that computer anymore
            foreach (var p in pToRemove)
            {
                ectx.Uow.PrinterInventoryRepository.Delete(p.Id);
            }

            ectx.Uow.Save();
            actionResult.Success = true;
            return actionResult;
        }
    }
}