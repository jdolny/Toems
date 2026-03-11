using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceComServerCluster(ServiceContext ctx)
    {
        public DtoActionResult Add(EntityComServerCluster cluster)
        {
            var actionResult = new DtoActionResult();
            var validationResult = Validate(cluster, true);
            if (validationResult.Success)
            {
                if (cluster.IsDefault)
                {
                    var clusters = ctx.Uow.ComServerClusterRepository.Get(x => x.IsDefault);
                    foreach (var clust in clusters)
                    {
                        clust.IsDefault = false;
                        ctx.Uow.ComServerClusterRepository.Update(clust, clust.Id);
                    }
                }

                ctx.Uow.ComServerClusterRepository.Insert(cluster);
                ctx.Uow.Save();
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

            var groupsWithCluster = ctx.Uow.GroupRepository.Get(x => x.ClusterId == clusterId);

            foreach (var group in groupsWithCluster)
            {
                 group.ClusterId = -1;
                ctx.Uow.GroupRepository.Update(group, group.Id);
            }

            ctx.Uow.ComServerClusterRepository.Delete(clusterId);
            ctx.Uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.Id;
            return actionResult;
        }

        public EntityComServerCluster GetCluster(int clusterId)
        {
            return ctx.Uow.ComServerClusterRepository.GetById(clusterId);
        }

        public List<EntityComServerCluster> GetAll()
        {
            return ctx.Uow.ComServerClusterRepository.Get();
        }

        public List<EntityComServerCluster> Search(DtoSearchFilter filter)
        {
            return ctx.Uow.ComServerClusterRepository.Get(x => x.Name.Contains(filter.SearchText));
        }

        public List<EntityComServerClusterServer> GetClusterServers(int clusterId)
        {
            return ctx.Uow.ComServerClusterServerRepository.Get(x => x.ComServerClusterId == clusterId);
        }

        public string TotalCount()
        {
            return ctx.Uow.ComServerClusterRepository.Count();
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
                    var clusters = ctx.Uow.ComServerClusterRepository.Get(x => x.IsDefault);
                    foreach (var clust in clusters)
                    {
                        clust.IsDefault = false;
                        ctx.Uow.ComServerClusterRepository.Update(clust, clust.Id);
                    }
                }

                ctx.Uow.ComServerClusterRepository.Update(cluster, u.Id);
                ctx.Uow.Save();
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
                if (ctx.Uow.ComServerClusterRepository.Exists(h => h.Name == comServerCluster.Name))
                {
                    validationResult.Success = false;
                    validationResult.ErrorMessage = "A Com Server Cluster With This Name Already Exists";
                    return validationResult;
                }
            }
            else
            {
                var original = ctx.Uow.ComServerClusterRepository.GetById(comServerCluster.Id);
                if (original.Name != comServerCluster.Name)
                {
                    if (ctx.Uow.ComServerClusterRepository.Exists(h => h.Name == comServerCluster.Name))
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