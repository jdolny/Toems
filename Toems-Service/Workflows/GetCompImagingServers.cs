using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toems_Common.Dto.client;
using Toems_Common.Entity;
using Toems_DataModel;
using Toems_Service.Entity;

namespace Toems_Service.Workflows
{
    public class GetCompImagingServers
    {
        public List<EntityClientComServer> Run(int computerId, bool includePassive=false)
        {
            var uow = new UnitOfWork();
            var defaultCluster = uow.ComServerClusterRepository.GetFirstOrDefault(x => x.IsDefault);

            var computerGroupMemberships = new ServiceComputer().GetAllGroupMemberships(computerId);
            var computerGroups = uow.ComputerRepository.GetAllComputerGroups(computerId).OrderBy(x => x.ImagingPriority).ThenBy(x => x.Name).ToList();
            List<int> imagingServerIds = new List<int>();

            if (computerGroups.Count() == 0)
            {
                //use default
                List<DtoClientComServers> clusterImagingServers;
                if (includePassive)
                    clusterImagingServers = uow.ComServerClusterServerRepository.GetImagingClusterServers(defaultCluster.Id);
                else
                    clusterImagingServers = uow.ComServerClusterServerRepository.GetImagingClusterServers(defaultCluster.Id).Where(x => x.Role.Equals("Active")).ToList();
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
                        clusterImagingServers = uow.ComServerClusterServerRepository.GetImagingClusterServers(group.ClusterId);
                    else
                        clusterImagingServers = uow.ComServerClusterServerRepository.GetImagingClusterServers(group.ClusterId).Where(x => x.Role.Equals("Active")).ToList();
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
                        clusterImagingServers = uow.ComServerClusterServerRepository.GetImagingClusterServers(defaultCluster.Id);
                    else
                        clusterImagingServers = uow.ComServerClusterServerRepository.GetImagingClusterServers(defaultCluster.Id).Where(x => x.Role.Equals("Active")).ToList();
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
                var comServer = uow.ClientComServerRepository.GetById(comServerId);
                listComServers.Add(comServer);
            }

            return listComServers;
        }
    }
}
