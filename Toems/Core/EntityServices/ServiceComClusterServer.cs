using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceComClusterServer(ServiceContext ctx)
    {
        public DtoActionResult AddList(List<EntityComServerClusterServer> listOfServers)
        {
            var actionResult = new DtoActionResult();
            if (listOfServers.Count == 0)
                return new DtoActionResult() {ErrorMessage = "No Cluster Servers Were Defined"};
            var clusterServer = listOfServers[0];
            if (clusterServer != null)
            {
                DeleteClusterServers(clusterServer.ComServerClusterId);
            }
            foreach (var server in listOfServers)
            {
                if (server.IsMulticastServer)
                    server.IsImagingServer = true; //a multicast server must be an imaging server
                ctx.Uow.ComServerClusterServerRepository.Insert(server);
            }
            ctx.Uow.Save();

            actionResult.Success = true;
            return actionResult;
        }

        private bool DeleteClusterServers(int clusterId)
        {
            ctx.Uow.ComServerClusterServerRepository.DeleteRange(x => x.ComServerClusterId == clusterId);
            ctx.Uow.Save();
            return true;
        }


    }
}