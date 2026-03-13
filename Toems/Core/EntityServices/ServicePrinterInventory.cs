using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServicePrinterInventory(ServiceContext ctx)
    {
        public DtoActionResult AddOrUpdate(List<EntityPrinterInventory> inventory, int computerId)
        {
            var actionResult = new DtoActionResult();
            var pToRemove = ctx.Uow.PrinterInventoryRepository.Get(x => x.ComputerId == computerId);
            foreach (var printer in inventory)
            {
                printer.ComputerId = computerId;
                var existing = ctx.Uow.PrinterInventoryRepository.GetFirstOrDefault(x => x.ComputerId == printer.ComputerId && x.SystemName == printer.SystemName && x.Name == printer.Name);
                if (existing == null)
                {
                    ctx.Uow.PrinterInventoryRepository.Insert(printer);
                }
                else
                {
                    pToRemove.Remove(existing);
                    printer.Id = existing.Id;
                    ctx.Uow.PrinterInventoryRepository.Update(printer, printer.Id);
                }
                 actionResult.Id = printer.Id;
            }

            //anything left in pToRemove does not exist on that computer anymore
            foreach (var p in pToRemove)
            {
                ctx.Uow.PrinterInventoryRepository.Delete(p.Id);
            }

            ctx.Uow.Save();
            actionResult.Success = true;
            return actionResult;
        }
    }
}