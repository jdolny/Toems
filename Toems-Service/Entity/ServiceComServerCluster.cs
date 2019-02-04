using System.Collections.Generic;
using System.Linq;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceComServerCluster
    {
        private readonly UnitOfWork _uow;

        public ServiceComServerCluster()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult Add(EntityComServerCluster cluster)
        {
            var actionResult = new DtoActionResult();
            var validationResult = Validate(cluster, true);
            if (validationResult.Success)
            {
                if (cluster.IsDefault)
                {
                    var clusters = _uow.ComServerClusterRepository.Get(x => x.IsDefault);
                    foreach (var clust in clusters)
                    {
                        clust.IsDefault = false;
                        _uow.ComServerClusterRepository.Update(clust, clust.Id);
                    }
                }

                _uow.ComServerClusterRepository.Insert(cluster);
                _uow.Save();
                actionResult.Success = true;
                actionResult.Id = cluster.Id;
            }
            else
            {
                return new DtoActionResult() { ErrorMessage = validationResult.ErrorMessage };
            }

            return actionResult;
        }

        public DtoActionResult Delete(int clusterId)
        {
            var u = GetCluster(clusterId);
            if (u == null) return new DtoActionResult { ErrorMessage = "Cluster Not Found", Id = 0 };

            var groupsWithCluster = _uow.GroupRepository.Get(x => x.ClusterId == clusterId);

            foreach (var group in groupsWithCluster)
            {
                 group.ClusterId = -1;
                _uow.GroupRepository.Update(group, group.Id);
            }

            _uow.ComServerClusterRepository.Delete(clusterId);
            _uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.Id;
            return actionResult;
        }

        public EntityComServerCluster GetCluster(int clusterId)
        {
            return _uow.ComServerClusterRepository.GetById(clusterId);
        }

        public List<EntityComServerCluster> GetAll()
        {
            return _uow.ComServerClusterRepository.Get();
        }

        public List<EntityComServerCluster> Search(DtoSearchFilter filter)
        {
            return _uow.ComServerClusterRepository.Get(x => x.Name.Contains(filter.SearchText));
        }

        public List<EntityComServerClusterServer> GetClusterServers(int clusterId)
        {
            return _uow.ComServerClusterServerRepository.Get(x => x.ComServerClusterId == clusterId);
        }

        public string TotalCount()
        {
            return _uow.ComServerClusterRepository.Count();
        }

        public DtoActionResult Update(EntityComServerCluster cluster)
        {
            var u = GetCluster(cluster.Id);
            if (u == null) return new DtoActionResult {ErrorMessage = "Cluster Not Found", Id = 0};
            var actionResult = new DtoActionResult();

            var validationResult = Validate(cluster, false);
            if (validationResult.Success)
            {
                if (cluster.IsDefault)
                {
                    var clusters = _uow.ComServerClusterRepository.Get(x => x.IsDefault);
                    foreach (var clust in clusters)
                    {
                        clust.IsDefault = false;
                        _uow.ComServerClusterRepository.Update(clust, clust.Id);
                    }
                }

                _uow.ComServerClusterRepository.Update(cluster, u.Id);
                _uow.Save();
                actionResult.Success = true;
                actionResult.Id = cluster.Id;
            }
            else
            {
                return new DtoActionResult() { ErrorMessage = validationResult.ErrorMessage };
            }
            return actionResult;
        }

        public DtoValidationResult Validate(EntityComServerCluster comServerCluster, bool isNew)
        {
            var validationResult = new DtoValidationResult { Success = true };

            if (string.IsNullOrEmpty(comServerCluster.Name) || !comServerCluster.Name.All(c => char.IsLetterOrDigit(c) || c == '_' || c == '-' || c == ' '))
            {
                validationResult.Success = false;
                validationResult.ErrorMessage = "Com Server Cluster Name Is Not Valid";
                return validationResult;
            }

            if (isNew)
            {
                if (_uow.ComServerClusterRepository.Exists(h => h.Name == comServerCluster.Name))
                {
                    validationResult.Success = false;
                    validationResult.ErrorMessage = "A Com Server Cluster With This Name Already Exists";
                    return validationResult;
                }
            }
            else
            {
                var original = _uow.ComServerClusterRepository.GetById(comServerCluster.Id);
                if (original.Name != comServerCluster.Name)
                {
                    if (_uow.ComServerClusterRepository.Exists(h => h.Name == comServerCluster.Name))
                    {
                        validationResult.Success = false;
                        validationResult.ErrorMessage = "A Com Server Cluster With This Name Already Exists";
                        return validationResult;
                    }
                }
            }

            return validationResult;


        }


    }
}