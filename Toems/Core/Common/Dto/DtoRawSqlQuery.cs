using System.Collections.Generic;

namespace Toems_Common.Dto
{
    public class DtoRawSqlQuery
    {
        public string Sql { get; set; }
        public List<string> Parameters { get; set; } 
    }
}
