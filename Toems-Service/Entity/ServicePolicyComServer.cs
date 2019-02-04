using System.Collections.Generic;
using System.Linq;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServicePolicyComServer
    {
        private readonly UnitOfWork _uow;

        public ServicePolicyComServer()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult AddOrUpdate(List<EntityPolicyComServer> policyComServers)
        {
            var first = policyComServers.FirstOrDefault();
            if (first == null) return new DtoActionResult { ErrorMessage = "No Policies Were In The List", Id = 0 };
            var allSame = policyComServers.All(x => x.PolicyId == first.PolicyId);
            if (!allSame) return new DtoActionResult { ErrorMessage = "The List Must Be For A Single Policy.", Id = 0 };
            var actionResult = new DtoActionResult();
            var pToRemove = _uow.PolicyComServerRepository.Get(x => x.PolicyId == first.PolicyId);
            foreach (var policyComServer in policyComServers)
            {
                var existing = _uow.PolicyComServerRepository.GetFirstOrDefault(x => x.PolicyId == policyComServer.PolicyId && x.ComServerId == policyComServer.ComServerId);
                if (existing == null)
                {
                    _uow.PolicyComServerRepository.Insert(policyComServer);
                }
                else
                {
                    pToRemove.Remove(existing);
                }

                actionResult.Id = 1;
            }

            //anything left in pToRemove does not exist anymore
            foreach (var p in pToRemove)
            {
                _uow.PolicyComServerRepository.Delete(p.Id);
            }

            _uow.Save();
            actionResult.Success = true;
            return actionResult;
        }

        public DtoActionResult DeleteAllForPolicy(int policyId)
        {
            _uow.PolicyComServerRepository.DeleteRange(x => x.PolicyId == policyId);
            _uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = 1;
            return actionResult;
        }
    }
}