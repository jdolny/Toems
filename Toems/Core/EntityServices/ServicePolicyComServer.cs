using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServicePolicyComServer(ServiceContext ctx)
    {
        public DtoActionResult AddOrUpdate(List<EntityPolicyComServer> policyComServers)
        {
            var first = policyComServers.FirstOrDefault();
            if (first == null) return new DtoActionResult { ErrorMessage = "No Policies Were In The List", Id = 0 };
            var allSame = policyComServers.All(x => x.PolicyId == first.PolicyId);
            if (!allSame) return new DtoActionResult { ErrorMessage = "The List Must Be For A Single Policy.", Id = 0 };
            var actionResult = new DtoActionResult();
            var pToRemove = ctx.Uow.PolicyComServerRepository.Get(x => x.PolicyId == first.PolicyId);
            foreach (var policyComServer in policyComServers)
            {
                var existing = ctx.Uow.PolicyComServerRepository.GetFirstOrDefault(x => x.PolicyId == policyComServer.PolicyId && x.ComServerId == policyComServer.ComServerId);
                if (existing == null)
                {
                    ctx.Uow.PolicyComServerRepository.Insert(policyComServer);
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
                ctx.Uow.PolicyComServerRepository.Delete(p.Id);
            }

            ctx.Uow.Save();
            actionResult.Success = true;
            return actionResult;
        }

        public DtoActionResult DeleteAllForPolicy(int policyId)
        {
            ctx.Uow.PolicyComServerRepository.DeleteRange(x => x.PolicyId == policyId);
            ctx.Uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = 1;
            return actionResult;
        }
    }
}