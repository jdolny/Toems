using Toems_Common.Dto.client;
using Toems_DataModel;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.Workflows
{
    public class GetCompEmServers(ServiceContext ctx)
    {
        public List<DtoClientComServers> Run(string computerGuid)
        {
            var computer = ctx.Uow.ComputerRepository.GetFirstOrDefault(x => x.Guid.Equals(computerGuid));
            if (computer == null) return null;
            var defaultCluster = ctx.Uow.ComServerClusterRepository.GetFirstOrDefault(x => x.IsDefault);
            if (defaultCluster == null) return null;

            var computerGroups = ctx.Uow.ComputerRepository.GetAllComputerGroups(computer.Id).OrderBy(x => x.EmPriority).ThenBy(x => x.Name).ToList();
            if (computerGroups.Count == 0)
            {
                var emServers = ctx.Uow.ComServerClusterServerRepository.GetEMClusterServers(defaultCluster.Id);
                if (emServers != null)
                {
                    if (emServers.Count > 0)
                        return emServers;
                    else
                        return null; //default cluster has no em servers
                }
            }
            else
            {
                foreach(var group in computerGroups)
                {
                    //check if assigned cluster has any em servers, if not, go to next group
                    if(group.ClusterId == -1)
                        group.ClusterId = defaultCluster.Id;

                    var emServers = ctx.Uow.ComServerClusterServerRepository.GetEMClusterServers(group.ClusterId);
                    if(emServers != null)
                    {
                        if (emServers.Count > 0)
                            return emServers;
                        else
                            return null; //default cluster has no em servers
                    }
                }
            }

            //no em servers found assigned to any group for this computer, return default cluster
            var defaultemServers = ctx.Uow.ComServerClusterServerRepository.GetEMClusterServers(defaultCluster.Id);
            if (defaultemServers != null)
            {
                if (defaultemServers.Count > 0)
                    return defaultemServers;
                else
                    return null; //default cluster has no em servers
            }

            return null;

        }
    }
}
