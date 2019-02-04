using System.Collections.Generic;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceComClusterServer
    {
        private readonly UnitOfWork _uow;

        public ServiceComClusterServer()
        {
            _uow = new UnitOfWork();
        }

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
                _uow.ComServerClusterServerRepository.Insert(server);
            _uow.Save();

            actionResult.Success = true;
            return actionResult;
        }

        private bool DeleteClusterServers(int clusterId)
        {
            _uow.ComServerClusterServerRepository.DeleteRange(x => x.ComServerClusterId == clusterId);
            _uow.Save();
            return true;
        }


    }
}