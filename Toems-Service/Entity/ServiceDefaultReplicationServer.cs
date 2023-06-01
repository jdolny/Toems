using System;
using System.Collections.Generic;
using System.Linq;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceDefaultReplicationServer
    {
        private readonly UnitOfWork _uow;

        public ServiceDefaultReplicationServer()
        {
            _uow = new UnitOfWork();
        }

        public List<EntityDefaultImageReplicationServer> GetDefaultImageReplicationComServers()
        {
            return _uow.DefaultImageReplicationServerRepository.Get();
        }

        public DtoActionResult AddOrUpdateDefaultImageReplicationServers(List<EntityDefaultImageReplicationServer> defaultImageComServers)
        {
            var first = defaultImageComServers.FirstOrDefault();
            if (first == null) return new DtoActionResult { ErrorMessage = "No Com Servers Were In The List", Id = 0 };
            var actionResult = new DtoActionResult();
            var pToRemove = _uow.DefaultImageReplicationServerRepository.Get();
            foreach (var imageComServer in defaultImageComServers)
            {
                var existing = _uow.DefaultImageReplicationServerRepository.GetFirstOrDefault(x => x.ComServerId == imageComServer.ComServerId);
                if (existing == null)
                {
                    _uow.DefaultImageReplicationServerRepository.Insert(imageComServer);
                }
                else
                {
                    pToRemove.Remove(existing);
                }

                actionResult.Id = 1;
            }

            //anything left in pToRemove does not exist anymore
            foreach (var p in pToRemove)
            {
                _uow.DefaultImageReplicationServerRepository.Delete(p.Id);
            }

            _uow.Save();
            actionResult.Success = true;
            return actionResult;
        }





    }
}