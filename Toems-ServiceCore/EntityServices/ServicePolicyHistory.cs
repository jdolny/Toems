using Toems_Common.Dto;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServicePolicyHistory(EntityContext ectx)
    {
        public DtoActionResult AddHistories(DtoPolicyResults results, string clientIdentity)
        {
            var client = ectx.Uow.ComputerRepository.GetFirstOrDefault(x => x.Guid == clientIdentity);
            if (client == null) return new DtoActionResult() {ErrorMessage = "Client Not Found",Success = false};

            var actionResult = new DtoActionResult();

            foreach (var history in results.Histories)
            {
                var policy = ectx.Uow.PolicyRepository.GetFirstOrDefault(x => x.Guid == history.PolicyGuid);
                if (policy == null) continue;
                history.PolicyId = policy.Id;
                history.ComputerId = client.Id;
                ectx.Uow.PolicyHistoryRepository.Insert(history);
            }
           
            ectx.Uow.Save();

            foreach (var ci in results.CustomInventories)
            {
                var module = ectx.Uow.ScriptModuleRepository.GetFirstOrDefault(x => x.Guid == ci.ModuleGuid);
                if (module == null) continue;
                var existing =
                    ectx.Uow.CustomInventoryRepository.GetFirstOrDefault(
                        x => x.ComputerId == client.Id && x.ScriptId == module.Id);
                if (existing == null)
                {
                    ci.ScriptId = module.Id;
                    ci.ComputerId = client.Id;
                    ectx.Uow.CustomInventoryRepository.Insert(ci);
                }
                else
                {
                    existing.Value = ci.Value;
                    ectx.Uow.CustomInventoryRepository.Update(existing,existing.Id);
                }
            }

            ectx.Uow.Save();
            actionResult.Success = true;

            return actionResult;
        }

       
    }
}
