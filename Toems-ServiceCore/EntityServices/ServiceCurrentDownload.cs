using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceCurrentDownload(EntityContext ectx)
    {
        public DtoActionResult Add(EntityCurrentDownload currentDownload)
        {
            var actionResult = new DtoActionResult();
            ectx.Uow.CurrentDownloadsRepository.Insert(currentDownload);
            ectx.Uow.Save();
            actionResult.Success = true;
            actionResult.Id = currentDownload.Id;
            return actionResult;
        }

        public List<DtoComServerConnection> GetUsageCounts()
        {
            var listConnections = new List<DtoComServerConnection>();
            var comServers = ectx.Uow.ClientComServerRepository.Get();
            foreach (var comServer in comServers)
            {
                var comConnection = new DtoComServerConnection();
                comConnection.ComUrl = comServer.Url;
                comConnection.Connections = TotalCount(comServer.Url);
                listConnections.Add(comConnection);
            }

            return listConnections;
        }

        public bool DeleteByClientId(int clientId,string comServer)
        {
            ectx.Uow.CurrentDownloadsRepository.DeleteRange(x => x.ComputerId == clientId);
            ectx.Uow.Save();
            return true;
        }

        public int TotalCount(string comServer)
        {
            return Convert.ToInt32(ectx.Uow.CurrentDownloadsRepository.Count(x => x.ComServer.Equals(comServer)));
        }

        public EntityCurrentDownload GetByClientId(int clientId,string comServer)
        {
            return ectx.Uow.CurrentDownloadsRepository.GetFirstOrDefault(x => x.ComputerId == clientId);
        }

        public DtoActionResult Update(EntityCurrentDownload currentDownload)
        {
            var actionResult = new DtoActionResult();
            ectx.Uow.CurrentDownloadsRepository.Update(currentDownload, currentDownload.Id);
            ectx.Uow.Save();
            actionResult.Success = true;
            actionResult.Id = currentDownload.Id;
            return actionResult;
        }

        public int ExpireOldConnections()
        {
            var limit = DateTime.Now - TimeSpan.FromMinutes(60);
            ectx.Uow.CurrentDownloadsRepository.DeleteRange(x => x.LastRequestTimeLocal < limit);
            ectx.Uow.Save();
            return Convert.ToInt32(ectx.Uow.CurrentDownloadsRepository.Count());
        }
    }
}
