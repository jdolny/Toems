using System;
using System.Data;
using System.Data.SqlClient;
using log4net;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Toems_Common.Dto;
using Toems_ServiceCore.Data;

namespace Toems_DataModel
{
    public class RawSqlRepository(ToemsDbContext _context)
    {

        private readonly ILog log = LogManager.GetLogger(typeof (RawSqlRepository));


        public int Execute(string sql)
        {
            return _context.Database.ExecuteSqlRaw(sql);
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
                    var connection = _context.Database.GetDbConnection();
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
                    var connection = _context.Database.GetDbConnection();
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

                            //mysql
                            foreach (var param in query.Parameters)
                            {
                                command.Parameters.Add(new MySqlParameter("p" + counter, param));
                                counter++;
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
