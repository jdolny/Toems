using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceCurrentDownload
    {
        private readonly UnitOfWork _uow;

        public ServiceCurrentDownload()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult Add(EntityCurrentDownload currentDownload)
        {
            var actionResult = new DtoActionResult();
            _uow.CurrentDownloadsRepository.Insert(currentDownload);
            _uow.Save();
            actionResult.Success = true;
            actionResult.Id = currentDownload.Id;
            return actionResult;
        }

        public List<DtoComServerConnection> GetUsageCounts()
        {
            var listConnections = new List<DtoComServerConnection>();
            var comServers = _uow.ClientComServerRepository.Get();
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
            _uow.CurrentDownloadsRepository.DeleteRange(x => x.ComputerId == clientId);
            _uow.Save();
            return true;
        }

        public int TotalCount(string comServer)
        {
            return Convert.ToInt32(_uow.CurrentDownloadsRepository.Count(x => x.ComServer.Equals(comServer)));
        }

        public EntityCurrentDownload GetByClientId(int clientId,string comServer)
        {
            return _uow.CurrentDownloadsRepository.GetFirstOrDefault(x => x.ComputerId == clientId);
        }

        public DtoActionResult Update(EntityCurrentDownload currentDownload)
        {
            var actionResult = new DtoActionResult();
            _uow.CurrentDownloadsRepository.Update(currentDownload, currentDownload.Id);
            _uow.Save();
            actionResult.Success = true;
            actionResult.Id = currentDownload.Id;
            return actionResult;
        }

        public int ExpireOldConnections()
        {
            var limit = DateTime.Now - TimeSpan.FromMinutes(60);
            _uow.CurrentDownloadsRepository.DeleteRange(x => x.LastRequestTimeLocal < limit);
            _uow.Save();
            return Convert.ToInt32(_uow.CurrentDownloadsRepository.Count());
        }
    }
}
