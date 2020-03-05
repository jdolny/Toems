using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_DataModel;
using Toems_Service.Entity;

namespace Toems_Service.Workflows
{

    public class GetBestCompImageServer
    {
        private readonly EntityComputer _computer;
        private readonly string _task;
        private readonly Random _random;
        private readonly UnitOfWork _uow;
        //private EntityComServerCluster _cluster;

        public GetBestCompImageServer(EntityComputer computer, string task)
        {
            _computer = computer;
            _task = task;
            _random = new Random();
            _uow = new UnitOfWork();
        }

        public int Run()
        {
            var comServerId = -1;
            //Find best com server to use

            var listOfImagingServers = new List<EntityClientComServer>();
            var defaultCluster = _uow.ComServerClusterRepository.GetFirstOrDefault(x => x.IsDefault);

            //if there is only 1 imaging server, no reason to continue, use that one
            var imagingServers = _uow.ClientComServerRepository.Get(x => x.IsImagingServer).ToList();
            if (imagingServers.Count == 1)
                return imagingServers.First().Id;


            //if computer is null, probably unregistered on demand, find default group
            if (_computer == null)
            {
                //use default
                var clusterImagingServers = _uow.ComServerClusterServerRepository.GetImagingClusterServers(defaultCluster.Id);
                if (clusterImagingServers != null)
                {
                    if (clusterImagingServers.Count > 0)
                    {
                        foreach(var serverId in clusterImagingServers.Select(x => x.ComServerId).ToList())
                        {
                            var comServer = _uow.ClientComServerRepository.GetById(serverId);
                            listOfImagingServers.Add(comServer);
                        }
                    }
                    else
                        return -1; //default cluster has no imaging servers
                }
            }
            else
            {
                listOfImagingServers = new GetCompImagingServers().Run(_computer.Id);
            }


            var queueSizesDict = new Dictionary<int, int>();
            var toRemove = new List<EntityClientComServer>();


            foreach (var comServer in listOfImagingServers)
            {
                if (comServer.ImagingMaxClients == 0)
                    toRemove.Add(comServer);
                else
                    queueSizesDict.Add(comServer.Id, comServer.ImagingMaxClients);
            }

            listOfImagingServers = listOfImagingServers.Except(toRemove).ToList();

            var taskInUseDict = new Dictionary<int, int>();
            foreach (var comServer in listOfImagingServers)
            {
                var counter = 0;
                foreach (var activeTask in new ServiceActiveImagingTask().GetAll().Where(x => x.Status != EnumTaskStatus.ImagingStatus.TaskCreated && x.Status != EnumTaskStatus.ImagingStatus.WaitingForLogin))
                {
                    if (activeTask.ComServerId == comServer.Id)
                    {
                        counter++;
                    }
                }

                taskInUseDict.Add(comServer.Id, counter);
            }

            var freeComServers = new List<EntityClientComServer>();
            foreach (var comServer in listOfImagingServers)
            {
                if (taskInUseDict[comServer.Id] < queueSizesDict[comServer.Id])
                    freeComServers.Add(comServer);
            }

            if (freeComServers.Count == 1)
                comServerId = freeComServers.First().Id;

            else if (freeComServers.Count > 1)
            {
                var freeDictionary = new Dictionary<int, int>();
                var slotsInUseList = new List<int>();
                foreach (var dp in freeComServers)
                {
                    freeDictionary.Add(dp.Id, taskInUseDict[dp.Id]);
                    slotsInUseList.Add(taskInUseDict[dp.Id]);
                }

                if (slotsInUseList.All(x => x == slotsInUseList[0]))
                {
                    //all image servers have equal free slots - randomly choose one.                 
                    var index = _random.Next(0, freeComServers.Count);
                    comServerId = freeComServers[index].Id;
                }
                else
                {
                    //Just grab the first one with the smallest queue, could be a duplicate but will eventually even out on it's own
                    var orderedInUse = freeDictionary.OrderBy(x => x.Value);
                    comServerId = orderedInUse.First().Key;
                }
            }
            else
            {
                //Free image servers count is 0, pick the one with the lowest number of tasks to be added to the queue
                var orderedInUse = taskInUseDict.OrderBy(x => x.Value);
                comServerId = orderedInUse.First().Key;
            }

            return comServerId;
        }
    }
}
