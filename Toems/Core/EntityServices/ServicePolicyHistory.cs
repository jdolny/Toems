using Toems_Common.Dto;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServicePolicyHistory(ServiceContext ctx)
    {
        public DtoActionResult AddHistories(DtoPolicyResults results, string clientIdentity)
        {
            var client = ctx.Uow.ComputerRepository.GetFirstOrDefault(x => x.Guid == clientIdentity);
            if (client == null) return new DtoActionResult() {ErrorMessage = "Client Not Found",Success = false};

            var actionResult = new DtoActionResult();

            foreach (var history in results.Histories)
            {
                var policy = ctx.Uow.PolicyRepository.GetFirstOrDefault(x => x.Guid == history.PolicyGuid);
                if (policy == null) continue;
                history.PolicyId = policy.Id;
                history.ComputerId = client.Id;
                ctx.Uow.PolicyHistoryRepository.Insert(history);
            }
           
            ctx.Uow.Save();

            foreach (var ci in results.CustomInventories)
            {
                var module = ctx.Uow.ScriptModuleRepository.GetFirstOrDefault(x => x.Guid == ci.ModuleGuid);
                if (module == null) continue;
                var existing =
                    ctx.Uow.CustomInventoryRepository.GetFirstOrDefault(
                        x => x.ComputerId == client.Id && x.ScriptId == module.Id);
                if (existing == null)
                {
                    ci.ScriptId = module.Id;
                    ci.ComputerId = client.Id;
                    ctx.Uow.CustomInventoryRepository.Insert(ci);
                }
                else
                {
                    existing.Value = ci.Value;
                    ctx.Uow.CustomInventoryRepository.Update(existing,existing.Id);
                }
            }

            ctx.Uow.Save();
            actionResult.Success = true;

            return actionResult;
        }

       
    }
}
