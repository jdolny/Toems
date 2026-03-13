using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceDefaultReplicationServer(ServiceContext ctx)
    {
        public List<EntityDefaultImageReplicationServer> GetDefaultImageReplicationComServers()
        {
            return ctx.Uow.DefaultImageReplicationServerRepository.Get();
        }

        public DtoActionResult AddOrUpdateDefaultImageReplicationServers(List<EntityDefaultImageReplicationServer> defaultImageComServers)
        {
            var first = defaultImageComServers.FirstOrDefault();
            if (first == null) return new DtoActionResult { ErrorMessage = "No Com Servers Were In The List", Id = 0 };
            var actionResult = new DtoActionResult();
            var pToRemove = ctx.Uow.DefaultImageReplicationServerRepository.Get();
            foreach (var imageComServer in defaultImageComServers)
            {
                var existing = ctx.Uow.DefaultImageReplicationServerRepository.GetFirstOrDefault(x => x.ComServerId == imageComServer.ComServerId);
                if (existing == null)
                {
                    ctx.Uow.DefaultImageReplicationServerRepository.Insert(imageComServer);
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
                ctx.Uow.DefaultImageReplicationServerRepository.Delete(p.Id);
            }

            ctx.Uow.Save();
            actionResult.Success = true;
            return actionResult;
        }





    }
}