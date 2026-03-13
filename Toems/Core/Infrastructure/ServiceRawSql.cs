using Toems_DataModel;

namespace Toems_ServiceCore.Infrastructure
{
    public class ServiceRawSql(ServiceContext ctx)
    {

        public int ExecuteQuery(string query)
        {
            return ctx.Uow.RawSqlRepository.Execute(query);
        }
    }
}
