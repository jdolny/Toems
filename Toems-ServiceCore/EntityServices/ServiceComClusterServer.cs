using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceComClusterServer(EntityContext ectx)
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
                ectx.Uow.ComServerClusterServerRepository.Insert(server);
            }
            ectx.Uow.Save();

            actionResult.Success = true;
            return actionResult;
        }

        private bool DeleteClusterServers(int clusterId)
        {
            ectx.Uow.ComServerClusterServerRepository.DeleteRange(x => x.ComServerClusterId == clusterId);
            ectx.Uow.Save();
            return true;
        }


    }
}