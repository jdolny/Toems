using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceCurrentDownload(ServiceContext ctx)
    {
        public DtoActionResult Add(EntityCurrentDownload currentDownload)
        {
            var actionResult = new DtoActionResult();
            ctx.Uow.CurrentDownloadsRepository.Insert(currentDownload);
            ctx.Uow.Save();
            actionResult.Success = true;
            actionResult.Id = currentDownload.Id;
            return actionResult;
        }

        public List<DtoComServerConnection> GetUsageCounts()
        {
            var listConnections = new List<DtoComServerConnection>();
            var comServers = ctx.Uow.ClientComServerRepository.Get();
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
            ctx.Uow.CurrentDownloadsRepository.DeleteRange(x => x.ComputerId == clientId);
            ctx.Uow.Save();
            return true;
        }

        public int TotalCount(string comServer)
        {
            return Convert.ToInt32(ctx.Uow.CurrentDownloadsRepository.Count(x => x.ComServer.Equals(comServer)));
        }

        public EntityCurrentDownload GetByClientId(int clientId,string comServer)
        {
            return ctx.Uow.CurrentDownloadsRepository.GetFirstOrDefault(x => x.ComputerId == clientId);
        }

        public DtoActionResult Update(EntityCurrentDownload currentDownload)
        {
            var actionResult = new DtoActionResult();
            ctx.Uow.CurrentDownloadsRepository.Update(currentDownload, currentDownload.Id);
            ctx.Uow.Save();
            actionResult.Success = true;
            actionResult.Id = currentDownload.Id;
            return actionResult;
        }

        public int ExpireOldConnections()
        {
            var limit = DateTime.Now - TimeSpan.FromMinutes(60);
            ctx.Uow.CurrentDownloadsRepository.DeleteRange(x => x.LastRequestTimeLocal < limit);
            ctx.Uow.Save();
            return Convert.ToInt32(ctx.Uow.CurrentDownloadsRepository.Count());
        }
    }
}
