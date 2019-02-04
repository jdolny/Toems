using System.Collections.Generic;
using System.Linq;
using Toems_Common.Dto;
using Toems_Common.Dto.client;
using Toems_DataModel;

namespace Toems_Service.Workflows
{
    public class GetClientComServers
    {
        public List<DtoClientComServers> Run(string computerGuid)
        {
            var uow = new UnitOfWork();
            var resultingClusterId = -1;
            var computer = uow.ComputerRepository.GetFirstOrDefault(x => x.Guid.Equals(computerGuid));
            if (computer == null) return null;
            var defaultCluster = uow.ComServerClusterRepository.GetFirstOrDefault(x => x.IsDefault);
            if (defaultCluster == null) return null;

            var computerGroups = uow.ComputerRepository.GetAllComputerGroups(computer.Id);
            var clusterGroupIds = computerGroups.Select(@group => @group.ClusterId).Distinct().ToList();
            //This isn't great, if the computer is a part of multiple groups with different cluster id's, the first one that is not default is chosen
            foreach (var id in clusterGroupIds.Where(id => id != -1 && id != defaultCluster.Id))
            {
                resultingClusterId = id;
                break;
            }

            if (resultingClusterId == -1)
                resultingClusterId = defaultCluster.Id;

            return uow.ComServerClusterServerRepository.GetClusterServers(resultingClusterId);

        }
    }
}
