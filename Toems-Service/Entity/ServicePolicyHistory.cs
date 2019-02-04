using Toems_Common.Dto;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServicePolicyHistory
    {
        private readonly UnitOfWork _uow;

        public ServicePolicyHistory()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult AddHistories(DtoPolicyResults results, string clientIdentity)
        {
            var client = _uow.ComputerRepository.GetFirstOrDefault(x => x.Guid == clientIdentity);
            if (client == null) return new DtoActionResult() {ErrorMessage = "Client Not Found",Success = false};

            var actionResult = new DtoActionResult();

            foreach (var history in results.Histories)
            {
                var policy = _uow.PolicyRepository.GetFirstOrDefault(x => x.Guid == history.PolicyGuid);
                if (policy == null) continue;
                history.PolicyId = policy.Id;
                history.ComputerId = client.Id;
                _uow.PolicyHistoryRepository.Insert(history);
            }
           
            _uow.Save();

            foreach (var ci in results.CustomInventories)
            {
                var module = _uow.ScriptModuleRepository.GetFirstOrDefault(x => x.Guid == ci.ModuleGuid);
                if (module == null) continue;
                var existing =
                    _uow.CustomInventoryRepository.GetFirstOrDefault(
                        x => x.ComputerId == client.Id && x.ScriptId == module.Id);
                if (existing == null)
                {
                    ci.ScriptId = module.Id;
                    ci.ComputerId = client.Id;
                    _uow.CustomInventoryRepository.Insert(ci);
                }
                else
                {
                    existing.Value = ci.Value;
                    _uow.CustomInventoryRepository.Update(existing,existing.Id);
                }
            }

            _uow.Save();
            actionResult.Success = true;

            return actionResult;
        }

       
    }
}
