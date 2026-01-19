using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toems_DataModel;
namespace Toems_Service
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
