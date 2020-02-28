using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toems_Common.Entity;
using Toems_DataModel;
using Toems_Service.Entity;

namespace Toems_Service.Workflows
{
    public class GetMulticastServer
    {
        private readonly EntityGroup _group;
        private readonly Random _random;
        private UnitOfWork _uow;
        private EntityComServerCluster _cluster;
        public GetMulticastServer(EntityGroup group)
        {
            _group = group;
            _random = new Random();
            _uow = new UnitOfWork();
        }

        public int? Run()
        {
            //Find the best multicast server to use

            var serverId = -1;


            if (_group.ClusterId == -1) //-1 is default cluster
            {
                _cluster = _uow.ComServerClusterRepository.GetFirstOrDefault(x => x.IsDefault);
                if (_cluster == null) return null;

            }
            else
            {

                _cluster = _uow.ComServerClusterRepository.GetById(_group.ClusterId);
                if (_cluster == null) return null;

            }



            var availableMulticastServers = _uow.ComServerClusterServerRepository.Get(x => x.ComServerClusterId == _cluster.Id && x.IsMulticastServer);

            if (!availableMulticastServers.Any())
                return null;

            var taskInUseDict = new Dictionary<int, int>();
            foreach (var mServer in availableMulticastServers)
            {
                var counter =
                    new ServiceActiveMulticastSession().GetAll()
                        .Count(x => x.ComServerId == mServer.ComServerId);

                taskInUseDict.Add(mServer.ComServerId, counter);
            }

            if (taskInUseDict.Count == 1)
                serverId = taskInUseDict.Keys.First();

            else if (taskInUseDict.Count > 1)
            {
                var orderedInUse = taskInUseDict.OrderBy(x => x.Value);

                if (taskInUseDict.Values.Distinct().Count() == 1)
                {
                    //all multicast server have equal tasks - randomly choose one.

                    var index = _random.Next(0, taskInUseDict.Count);
                    serverId = taskInUseDict[index];
                }
                else
                {
                    //Just grab the first one with the smallest queue, could be a duplicate but will eventually even out on it's own               
                    serverId = orderedInUse.First().Key;
                }
            }
            return serverId;
        }


    }
}
