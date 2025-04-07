using System;
using System.Data;
using System.Data.SqlClient;
using log4net;
using MySql.Data.MySqlClient;
using Toems_Common.Dto;

namespace Toems_DataModel
{
    public class RawSqlRepository
    {
        private readonly ToemsDbContext _context;
        private readonly ILog log = LogManager.GetLogger(typeof (RawSqlRepository));

        public RawSqlRepository()
        {
            _context = new ToemsDbContext("toems_read_only");
        }

        public int Execute(string sql)
        {
            return _context.Database.ExecuteSqlCommand(sql);
        }

        public DataSet ExecuteCustomSqlReportReader(string sql)
        {
            try
            {
                log.Debug("Custom SQL Query: " + sql);
                DataSet resultDataSet = new DataSet("DataSet");
                DataTable resultTable = new DataTable("Result");
                resultDataSet.Tables.Add(resultTable);
                using (_context)
                {
                    var connection = _context.Database.Connection;
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = sql;
                    //command.CommandType = CommandType.Text;

                    resultTable.Load(command.ExecuteReader());
                }
                return resultDataSet;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return null;
        }
    
        public DataSet ExecuteReader(DtoRawSqlQuery query)
        {
            try
            {
                log.Debug("Custom SQL Query: " + query.Sql);
                DataSet resultDataSet = new DataSet("DataSet");
                DataTable resultTable = new DataTable("Result");
                resultDataSet.Tables.Add(resultTable);
                using (_context)
                {
                    var connection = _context.Database.Connection;
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = query.Sql;
                    //command.CommandType = CommandType.Text;
                    int counter = 1;

                    var providerName = System.Configuration.ConfigurationManager.
                        ConnectionStrings["toems"].ProviderName;

                    if (query.Parameters != null && query.Parameters.Count > 0)
                    {

                        if (providerName.Equals("MySql.Data.MySqlClient"))
                        {
                            //mysql
                            foreach (var param in query.Parameters)
                            {
                                command.Parameters.Add(new MySqlParameter("p" + counter, param));
                                counter++;
                            }

                        }
                        else
                        {
                            //sqlserver
                            foreach (var param in query.Parameters)
                            {
                                command.Parameters.Add(new SqlParameter("p" + counter, param));
                                counter++;
                            }
                        }
                    }
                    resultTable.Load(command.ExecuteReader());
                }
                return resultDataSet;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return null;
        }
    }
}
