using Toems_DataModel;

namespace Toems_ServiceCore.Infrastructure
{
    public class ServiceRawSql
    {
        private readonly RawSqlRepository _rawSqlRepository;

        public ServiceRawSql()
        {
            _rawSqlRepository = new RawSqlRepository();
        }

        public int ExecuteQuery(string query)
        {
            return _rawSqlRepository.Execute(query);
        }
    }
}
