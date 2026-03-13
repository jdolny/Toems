using Toems_Common.Dto.client;
using Toems_Common.Entity;
using Toems_DataModel;
using Toems_ServiceCore.EntityServices;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.Workflows
{
    public class GetCompImagingServers(ServiceContext ctx)
    {
        public List<EntityClientComServer> Run(int computerId, bool includePassive=false)
        {
            var defaultCluster = ctx.Uow.ComServerClusterRepository.GetFirstOrDefault(x => x.IsDefault);

            var computerGroupMemberships = ctx.Computer.GetAllGroupMemberships(computerId);
            var computerGroups = ctx.Uow.ComputerRepository.GetAllComputerGroups(computerId).OrderBy(x => x.ImagingPriority).ThenBy(x => x.Name).ToList();
            List<int> imagingServerIds = new List<int>();

            if (computerGroups.Count() == 0)
            {
                //use default
                List<DtoClientComServers> clusterImagingServers;
                if (includePassive)
                    clusterImagingServers = ctx.Uow.ComServerClusterServerRepository.GetImagingClusterServers(defaultCluster.Id);
                else
                    clusterImagingServers = ctx.Uow.ComServerClusterServerRepository.GetImagingClusterServers(defaultCluster.Id).Where(x => x.Role.Equals("Active")).ToList();
                if (clusterImagingServers != null)
                {
                    if (clusterImagingServers.Count > 0)
                        imagingServerIds = clusterImagingServers.Select(x => x.ComServerId).ToList();
                    else
                        return null; //default cluster has no imaging servers
                }
            }
            else
            {
                foreach (var group in computerGroups)
                {
                    //check if assigned cluster has any imaging servers, if not, go to next group
                    if (group.ClusterId == -1)
                        group.ClusterId = defaultCluster.Id;
                    List<DtoClientComServers> clusterImagingServers;
                    if (includePassive)
                        clusterImagingServers = ctx.Uow.ComServerClusterServerRepository.GetImagingClusterServers(group.ClusterId);
                    else
                        clusterImagingServers = ctx.Uow.ComServerClusterServerRepository.GetImagingClusterServers(group.ClusterId).Where(x => x.Role.Equals("Active")).ToList();
                    if (clusterImagingServers != null)
                    {
                        if (clusterImagingServers.Count > 0)
                        {
                            imagingServerIds = clusterImagingServers.Select(x => x.ComServerId).ToList();
                            break; //use first found imaging cluster
                        }
                    }
                }

                if (imagingServerIds.Count == 0)
                {
                    //no groups with an imaging server found for this computer, use default cluster
                    //use default
                    List<DtoClientComServers> clusterImagingServers;
                    if (includePassive)
                        clusterImagingServers = ctx.Uow.ComServerClusterServerRepository.GetImagingClusterServers(defaultCluster.Id);
                    else
                        clusterImagingServers = ctx.Uow.ComServerClusterServerRepository.GetImagingClusterServers(defaultCluster.Id).Where(x => x.Role.Equals("Active")).ToList();
                    if (clusterImagingServers != null)
                    {
                        if (clusterImagingServers.Count > 0)
                            imagingServerIds = clusterImagingServers.Select(x => x.ComServerId).ToList();
                        else
                            return null; //default cluster has no tftp servers
                    }
                }
            }


            var listComServers = new List<EntityClientComServer>();
            foreach (var comServerId in imagingServerIds)
            {
                var comServer = ctx.Uow.ClientComServerRepository.GetById(comServerId);
                listComServers.Add(comServer);
            }

            return listComServers;
        }
    }
}
