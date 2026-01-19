using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toems_Common.Entity;
using Toems_DataModel;
using Toems_Service.Entity;
using Toems_ServiceCore.EntityServices;

namespace Toems_Service.Workflows
{
    public class GetCompTftpServers
    {

        public List<EntityClientComServer> Run(int computerId)
        {
            var uow = new UnitOfWork();
            var defaultCluster = uow.ComServerClusterRepository.GetFirstOrDefault(x => x.IsDefault);
            var computerGroupMemberships = new ServiceComputer().GetAllGroupMemberships(computerId);
            var computerGroups = uow.ComputerRepository.GetAllComputerGroups(computerId).OrderBy(x => x.ImagingPriority).ThenBy(x => x.Name).ToList();
            List<int> tftpServerIds = new List<int>();

            if (computerGroups.Count() == 0)
            {
                //use default
                var clusterTftpServers = uow.ComServerClusterServerRepository.GetTftpClusterServers(defaultCluster.Id);
                if (clusterTftpServers != null)
                {
                    if (clusterTftpServers.Count > 0)
                        tftpServerIds = clusterTftpServers.Select(x => x.ComServerId).ToList();
                    else
                        return null; //default cluster has no tftp servers
                }
            }
            else
            {
                foreach (var group in computerGroups)
                {
                    //check if assigned cluster has any tftp servers, if not, go to next group
                    if (group.ClusterId == -1)
                        group.ClusterId = defaultCluster.Id;
                    var clusterTftpServers = uow.ComServerClusterServerRepository.GetTftpClusterServers(group.ClusterId);
                    if (clusterTftpServers != null)
                    {
                        if (clusterTftpServers.Count > 0)
                        {
                            tftpServerIds = clusterTftpServers.Select(x => x.ComServerId).ToList();
                            break;
                        }
                    }
                }

                if (tftpServerIds.Count == 0)
                {
                    //no groups with a tftp server found for this computer, use default cluster
                    //use default
                    var clusterTftpServers = uow.ComServerClusterServerRepository.GetTftpClusterServers(defaultCluster.Id);
                    if (clusterTftpServers != null)
                    {
                        if (clusterTftpServers.Count > 0)
                            tftpServerIds = clusterTftpServers.Select(x => x.ComServerId).ToList();
                        else
                            return null; //default cluster has no tftp servers
                    }
                }
            }

        
            var listComServers = new List<EntityClientComServer>();
            foreach (var comServerId in tftpServerIds)
            {
                var comServer = uow.ClientComServerRepository.GetById(comServerId);
                listComServers.Add(comServer);
            }

            return listComServers;
        }
    }
}
