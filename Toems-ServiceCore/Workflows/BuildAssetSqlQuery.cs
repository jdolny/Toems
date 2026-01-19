using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Service.Entity;
using Toems_ServiceCore.EntityServices;

namespace Toems_Service.Workflows
{
    public class BuildAssetSqlQuery
    {
        private StringBuilder sb;

        public BuildAssetSqlQuery()
        {
            sb = new StringBuilder();
        }

        private string BuildGroupBy(string groupBy)
        {
            if (string.IsNullOrEmpty(groupBy)) return "";

                if (groupBy.StartsWith("("))
                {
                    var cia = groupBy.Replace(".value", "");
                    cia = cia.Trim('(').Trim(')');
                    var ciaType = cia.Split('_').First();
                    if (ciaType.Equals("ca"))
                    {
                        int caId;
                        if (int.TryParse(cia.Split('_')[1], out caId))
                        {
                            return " GROUP BY ca" + caId + ".value";
                        }
                    }
                }
                else
                {
                    var table = groupBy.Split('.').First();
                    var field = groupBy.Split('.')[1];

                    if (table == "Asset ")
                        return " GROUP BY a." + field;
                    else if (table == "Asset Type")
                    return " GROUP BY b." + field;
             
            }

            return "";
        }

        public DtoRawSqlQuery Run(List<DtoCustomComputerQuery> queries)
        {
            if (queries == null || queries.Count == 0) return null;
            var sqlQuery = new DtoRawSqlQuery();
          

            string select = "SELECT a.asset_id, a.asset_display_name,";

            foreach (var query in queries.Where(x => x.Table.Equals("Asset Type")))
            {
                select += @" b." + query.Field + ",";
            }
            foreach (var query in queries.Where(x => x.Table.Equals("Category")))
            {
                select += @" d." + query.Field + ",";
            }

            foreach (var query in queries)
            {
                if (query.Table.StartsWith("("))
                {
                    var cia = query.Table.Trim('(').Trim(')');
                    var ciaType = cia.Split('_').First();
                    if (ciaType.Equals("ca"))
                    {
                        int caId;
                        if (int.TryParse(cia.Split('_')[1], out caId))
                        {
                            var attribute = new ServiceCustomAttribute().GetCustomAttribute(caId);
                            select += @" ca" + caId + "." + query.Field + " as " + "\"" + attribute.Name + "\"" + ",";
                        }
                    }

                }
             
            }

            var selectTrimmed = select.Trim(',') + " ";

            sb.Append(selectTrimmed);
            sb.Append(@" FROM assets as a ");

            if (queries.Any(x => x.Table == "Asset Type"))
            {
                sb.Append("LEFT JOIN custom_asset_types b on a.asset_type_id = b.custom_asset_type_id ");
            }
            if (queries.Any(x => x.Table == "Category"))
            {
                sb.Append("LEFT JOIN custom_asset_categories c on a.asset_id = c.custom_asset_id ");
                sb.Append("LEFT JOIN categories d on c.category_id = d.category_id ");
            }

            var customAttributeIds = new List<int>();
            foreach (var query in queries)
            {
                if (query.Table.StartsWith("("))
                {
                    var cia = query.Table.Trim('(').Trim(')');
                    var ciaType = cia.Split('_').First();
                  
                    if (ciaType.Equals("ca"))
                    {
                        int caId;
                        if (int.TryParse(cia.Split('_')[1], out caId))
                            customAttributeIds.Add(caId);
                    }

                }
            }


            if (customAttributeIds.Count > 0)
            {
                foreach (var attributeId in customAttributeIds)
                {
                    sb.Append("LEFT JOIN asset_attributes ca" + attributeId + " on a.asset_id = ca" + attributeId + ".asset_id AND ca" + attributeId + ".custom_attribute_id in ");
                    sb.Append("(" + attributeId + ") ");
                }
            }


            sb.Append("WHERE (");
            int counter = 0;
            var parameters = new List<string>();
            foreach (var query in queries)
            {
                var tableAs = "";
                if (query.Table == "Asset")
                    tableAs = "a";
                else if (query.Table == "Asset Type")
                    tableAs = "b";
                else if (query.Table == "Category")
                    tableAs = "d";

                else
                {
                    if (query.Table.StartsWith("("))
                    {
                        var cia = query.Table.Trim('(').Trim(')');
                        var ciaType = cia.Split('_').First();
                      
                        if (ciaType.Equals("ca"))
                        {
                            int caId;
                            if (int.TryParse(cia.Split('_')[1], out caId))
                                tableAs = "ca" + caId;
                        }
                    }
                }
                counter++;

                sb.Append(query.AndOr + " " + query.LeftParenthesis + " " + tableAs + "." + query.Field + " ");                
                sb.Append(query.Operator + " @p" + counter + " " + query.RightParenthesis);

                parameters.Add(query.Value);
            }


            if (!queries.First().IncludeArchived)
            {
                sb.Append(") AND (a.is_archived = 0)");
            }
            else
            {
                sb.Append(")");
            }

        

            sb.Append(BuildGroupBy(queries.First().GroupBy));


            sqlQuery.Sql = sb.ToString();
            sqlQuery.Parameters = parameters;

            return sqlQuery;

           
        }
    }
}
