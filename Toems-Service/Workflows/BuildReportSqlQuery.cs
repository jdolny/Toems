using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Service.Entity;

namespace Toems_Service.Workflows
{
    public class BuildReportSqlQuery
    {
        private StringBuilder sb;

        public BuildReportSqlQuery()
        {
            sb = new StringBuilder();
        }

        private string BuildGroupBy(string groupBy)
        {
            Dictionary<string, int> queryTypeCount = new Dictionary<string, int>();
            queryTypeCount.Add("Bios", 0);
            queryTypeCount.Add("System", 0);
            queryTypeCount.Add("Hard Drive", 0);
            queryTypeCount.Add("OS", 0);
            queryTypeCount.Add("Printer", 0);
            queryTypeCount.Add("Processor", 0);
            queryTypeCount.Add("Application", 0);
            queryTypeCount.Add("Windows Update", 0);
            queryTypeCount.Add("Firewall", 0);
            queryTypeCount.Add("AntiVirus", 0);
            queryTypeCount.Add("BitLocker", 0);
            queryTypeCount.Add("Logical Volumes", 0);
            queryTypeCount.Add("Network Adapters", 0);
            queryTypeCount.Add("Certificates", 0);
            queryTypeCount.Add("Category", 0);
            queryTypeCount.Add("Gpu", 0);
            queryTypeCount.Add("Group", 0);

            if (string.IsNullOrEmpty(groupBy)) return "";

            if (groupBy.StartsWith("("))
            {
                var cia = groupBy.Replace(".value", "");
                cia = cia.Trim('(').Trim(')');
                var ciaType = cia.Split('_').First();
                if (ciaType.Equals("ci"))
                {
                    int ciId;
                    if (int.TryParse(cia.Split('_')[1], out ciId))
                    {
                        return @" GROUP BY ci" + ciId + ".value";
                    }
                }
                else if (ciaType.Equals("ca"))
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

                if (groupBy.Split('.')[0] == "Computer")
                    return " GROUP BY a." + groupBy.Split('.')[1];
                else if (groupBy.Split('.')[1] == "Bios")
                    return $" GROUP BY b{groupBy.Split('.').First()}." + groupBy.Split('.')[2];
                else if (groupBy.Split('.')[1] == "System")
                    return $" GROUP BY c{groupBy.Split('.').First()}." + groupBy.Split('.')[2];
                else if (groupBy.Split('.')[1] == "Hard Drive")
                    return $" GROUP BY d{groupBy.Split('.').First()}." + groupBy.Split('.')[2];
                else if (groupBy.Split('.')[1] == "OS")
                    return $" GROUP BY e{groupBy.Split('.').First()}." + groupBy.Split('.')[2];
                else if (groupBy.Split('.')[1] == "Printer")
                    return $" GROUP BY f{groupBy.Split('.').First()}." + groupBy.Split('.')[2];
                else if (groupBy.Split('.')[1] == "Processor")
                    return $" GROUP BY g{groupBy.Split('.').First()}." + groupBy.Split('.')[2];
                else if (groupBy.Split('.')[1] == "Application")
                    return $" GROUP BY i{groupBy.Split('.').First()}." + groupBy.Split('.')[2];
                else if (groupBy.Split('.')[1] == "Windows Update")
                    return $" GROUP BY k{groupBy.Split('.').First()}." + groupBy.Split('.')[2];
                else if (groupBy.Split('.')[1] == "Firewall")
                    return $" GROUP BY l{groupBy.Split('.').First()}." + groupBy.Split('.')[2];
                else if (groupBy.Split('.')[1] == "AntiVirus")
                    return $" GROUP BY m{groupBy.Split('.').First()}." + groupBy.Split('.')[2];
                else if (groupBy.Split('.')[1] == "BitLocker")
                    return $" GROUP BY n{groupBy.Split('.').First()}." + groupBy.Split('.')[2];
                else if (groupBy.Split('.')[1] == "Logical Volumes")
                    return $" GROUP BY o{groupBy.Split('.').First()}." + groupBy.Split('.')[2];
                else if (groupBy.Split('.')[1] == "Network Adapters")
                    return $" GROUP BY p{groupBy.Split('.').First()}." + groupBy.Split('.')[2];
                else if (groupBy.Split('.')[1] == "Certificates")
                    return $" GROUP BY r{groupBy.Split('.').First()}." + groupBy.Split('.')[2];
                else if (groupBy.Split('.')[1] == "Category")
                    return $" GROUP BY t{groupBy.Split('.').First()}." + groupBy.Split('.')[2];
                else if (groupBy.Split('.')[1] == "Gpu")
                    return $" GROUP BY u{groupBy.Split('.').First()}." + groupBy.Split('.')[2];
                else if (groupBy.Split('.')[1] == "Group")
                    return $" GROUP BY w{groupBy.Split('.').First()}." + groupBy.Split('.')[2];
            }

            return "";
        }

        public DtoRawSqlQuery Run(List<DtoCustomComputerQuery> queries)
        {
            var sqlQuery = new DtoRawSqlQuery();
            if (queries.Count == 1 && queries[0].AndOr.Equals("Not"))
            {
                var query = queries[0];

                if (query.Table.Equals("Application"))
                {
                    sb.Append(@"select a.computer_name
                                from computers as a
                                where a.computer_id not in 
                                (
                                    SELECT computers.computer_id
                                    FROM computers
                                    LEFT join computer_software on computers.computer_id = computer_software.computer_id 
                                    LEFT join software_inventory on (computer_software.software_id = software_inventory.software_inventory_id)
                                    where software_inventory.name " + query.Operator + "'" + query.Value + "'" +
                              ") and a.last_inventory_time_local > '2019-01-01'");
                    if (queries.First().IncludeArchived && queries.First().IncludePreProvisioned)
                    {
                        sb.Append(" AND (a.provision_status = 8 OR a.provision_status = 11 OR a.provision_status = 6)");
                    }
                    else if (queries.First().IncludeArchived)
                    {
                        sb.Append(" AND (a.provision_status = 8 OR a.provision_status = 11)");
                    }

                    else if (queries.First().IncludePreProvisioned)
                    {
                        sb.Append(" AND (a.provision_status = 8 OR a.provision_status = 6)");
                    }
                    else
                    {
                        sb.Append(" AND a.provision_status = 8");
                    }

                    sb.Append(BuildGroupBy(queries.First().GroupBy));
                  

                }

                else if (query.Table.Equals("Windows Update"))
                {
                    sb.Append(@"select a.computer_name
                                from computers as a
                                where a.computer_id not in 
                                (
                                    SELECT computers.computer_id
                                    FROM computers
                                    LEFT join computer_updates on computers.computer_id = computer_updates.computer_id 
                                    LEFT join wu_inventory on (computer_updates.wu_inventory_id = wu_inventory.wu_inventory_id)
                                    where wu_inventory.title " + query.Operator + " " + "'" + query.Value + "'" +
                              ") and a.last_inventory_time_local > '2019-01-01'");
                    if (queries.First().IncludeArchived && queries.First().IncludePreProvisioned)
                    {
                        sb.Append(" AND (a.provision_status = 8 OR a.provision_status = 11 OR a.provision_status = 6)");
                    }
                    else if (queries.First().IncludeArchived)
                    {
                        sb.Append(" AND (a.provision_status = 8 OR a.provision_status = 11)");
                    }

                    else if (queries.First().IncludePreProvisioned)
                    {
                        sb.Append(" AND (a.provision_status = 8 OR a.provision_status = 6)");
                    }
                    else
                    {
                        sb.Append(" AND a.provision_status = 8");
                    }

                    if (!string.IsNullOrEmpty(queries.First().GroupBy))
                    {
                        sb.Append($" GROUP BY {queries.First().GroupBy}");
                    }

                }
                else if (query.Table.Equals("Certificates"))
                {
                    sb.Append(@"select a.computer_id,a.computer_name
                                from computers as a
                                where a.computer_id not in 
                                (
                                    SELECT computers.computer_id
                                    FROM computers
                                    left join computer_certificates on computers.computer_id = computer_certificates.computer_id 
                                    left join certificate_inventory on (computer_certificates.certificate_id = certificate_inventory.certificate_inventory_id)
                                    where certificate_inventory.subject " + query.Operator + " " + "'" + query.Value + "'" +
                              ") and a.last_inventory_time_local > '2019-01-01'");

                    if (queries.First().IncludeArchived && queries.First().IncludePreProvisioned)
                    {
                        sb.Append(" AND (a.provision_status = 8 OR a.provision_status = 11 OR a.provision_status = 6)");
                    }
                    else if (queries.First().IncludeArchived)
                    {
                        sb.Append(" AND (a.provision_status = 8 OR a.provision_status = 11)");
                    }

                    else if (queries.First().IncludePreProvisioned)
                    {
                        sb.Append(" AND (a.provision_status = 8 OR a.provision_status = 6)");
                    }
                    else
                    {
                        sb.Append(" AND a.provision_status = 8");
                    }

                    if (!string.IsNullOrEmpty(queries.First().GroupBy))
                    {
                        sb.Append($" GROUP BY {queries.First().GroupBy}");
                    }
                }


                sqlQuery = new DtoRawSqlQuery();
                sqlQuery.Sql = sb.ToString();
                return sqlQuery;
            }
            Dictionary<string, int> selectQueryTypeCount = new Dictionary<string, int>();
            selectQueryTypeCount.Add("Bios", 0);
            selectQueryTypeCount.Add("System", 0);
            selectQueryTypeCount.Add("Hard Drive", 0);
            selectQueryTypeCount.Add("OS", 0);
            selectQueryTypeCount.Add("Printer", 0);
            selectQueryTypeCount.Add("Processor", 0);
            selectQueryTypeCount.Add("Application", 0);
            selectQueryTypeCount.Add("Windows Update", 0);
            selectQueryTypeCount.Add("Firewall", 0);
            selectQueryTypeCount.Add("AntiVirus", 0);
            selectQueryTypeCount.Add("BitLocker", 0);
            selectQueryTypeCount.Add("Logical Volumes", 0);
            selectQueryTypeCount.Add("Network Adapters", 0);
            selectQueryTypeCount.Add("Certificates", 0);
            selectQueryTypeCount.Add("Category", 0);
            selectQueryTypeCount.Add("Gpu", 0);
            selectQueryTypeCount.Add("Group", 0);

            string select = "SELECT a.computer_id, a.computer_name,";

            foreach (var query in queries.Where(x => x.Table.Equals("Computer")))
            {
                if (query.Field.Equals("computer_name")) continue;
                select += @" a." + query.Field + ",";
                
            }

            foreach (var query in queries.Where(x => x.Table.Equals("Bios")))
            {
                select += $" b{selectQueryTypeCount["Bios"]}." + query.Field + ",";
                selectQueryTypeCount["Bios"]++;
            }

            foreach (var query in queries.Where(x => x.Table.Equals("System")))
            {
                select += $" c{selectQueryTypeCount["System"]}." + query.Field + ",";
                selectQueryTypeCount["System"]++;
            }
            foreach (var query in queries.Where(x => x.Table.Equals("Hard Drive")))
            {
                select += $" d{selectQueryTypeCount["Hard Drive"]}." + query.Field + ",";
                selectQueryTypeCount["Hard Drive"]++;
            }
            foreach (var query in queries.Where(x => x.Table.Equals("OS")))
            {
                select += $" e{selectQueryTypeCount["OS"]}." + query.Field + ",";
                selectQueryTypeCount["OS"]++;
            }
            foreach (var query in queries.Where(x => x.Table.Equals("Printer")))
            {
                select += $" f{selectQueryTypeCount["Printer"]}." + query.Field + ",";
                selectQueryTypeCount["Printer"]++;
            }
            foreach (var query in queries.Where(x => x.Table.Equals("Processor")))
            {
                select += $" g{selectQueryTypeCount["Processor"]}." + query.Field + ",";
                selectQueryTypeCount["Processor"]++;
            }
            foreach (var query in queries.Where(x => x.Table.Equals("Application")))
            {
                select += $" i{selectQueryTypeCount["Application"]}." + query.Field + ",";
                selectQueryTypeCount["Application"]++;
            }
            foreach (var query in queries.Where(x => x.Table.Equals("Windows Update") && x.Field.Equals("is_installed")))
            {
                select += $" j{selectQueryTypeCount["Windows Update"]}." + query.Field + ",";
                selectQueryTypeCount["Windows Update"]++;
            }
            foreach (var query in queries.Where(x => x.Table.Equals("Windows Update") && x.Field.Equals("install_date")))
            {
                select += $" j{selectQueryTypeCount["Windows Update"]}." + query.Field + ",";
                selectQueryTypeCount["Windows Update"]++;
            }
            foreach (var query in queries.Where(x => x.Table.Equals("Windows Update") && x.Field.Equals("title")))
            {
                select += $" k{selectQueryTypeCount["Windows Update"]}." + query.Field + ",";
                selectQueryTypeCount["Windows Update"]++;
            }
            foreach (var query in queries.Where(x => x.Table.Equals("Firewall")))
            {
                select += $" l{selectQueryTypeCount["Firewall"]}." + query.Field + ",";
                selectQueryTypeCount["Firewall"]++;
            }
            foreach (var query in queries.Where(x => x.Table.Equals("AntiVirus")))
            {
                select += $" m{selectQueryTypeCount["AntiVirus"]}." + query.Field + ",";
                selectQueryTypeCount["AntiVirus"]++;
            }
            foreach (var query in queries.Where(x => x.Table.Equals("BitLocker")))
            {
                select += $" n{selectQueryTypeCount["BitLocker"]}." + query.Field + ",";
                selectQueryTypeCount["BitLocker"]++;
            }
            foreach (var query in queries.Where(x => x.Table.Equals("Logical Volumes")))
            {
                select += $" o{selectQueryTypeCount["Logical Volumes"]}." + query.Field + ",";
                selectQueryTypeCount["Logical Volumes"]++;
            }
            foreach (var query in queries.Where(x => x.Table.Equals("Network Adapters")))
            {
                select += $" p{selectQueryTypeCount["Network Adapters"]}." + query.Field + ",";
                selectQueryTypeCount["Network Adapters"]++;
            }
            foreach (var query in queries.Where(x => x.Table.Equals("Certificates")))
            {
                select += $" r{selectQueryTypeCount["Certificates"]}." + query.Field + ",";
                selectQueryTypeCount["Certificates"]++;
            }
            foreach (var query in queries.Where(x => x.Table.Equals("Category")))
            {
                select += $" t{selectQueryTypeCount["Category"]}." + query.Field + ",";
                selectQueryTypeCount["Category"]++;
            }
            foreach (var query in queries.Where(x => x.Table.Equals("Gpu")))
            {
                select += $" u{selectQueryTypeCount["Gpu"]}." + query.Field + ",";
                selectQueryTypeCount["Gpu"]++;
            }
            foreach (var query in queries.Where(x => x.Table.Equals("Group")))
            {
                select += $" w{selectQueryTypeCount["Group"]}." + query.Field + ",";
                selectQueryTypeCount["Group"]++;
            }

            foreach (var query in queries)
            {
                if (query.Table.StartsWith("("))
                {
                    var cia = query.Table.Trim('(').Trim(')');
                    var ciaType = cia.Split('_').First();
                    if (ciaType.Equals("ci"))
                    {
                        int ciId;
                        if (int.TryParse(cia.Split('_')[1], out ciId))
                        {
                            var script = new ServiceScriptModule().GetModule(ciId);
                            select += @" ci" + ciId + "." + query.Field + " as " + "\"" + script.Name + "\"" + ",";
                        }
                    }
                    else if (ciaType.Equals("ca"))
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
            sb.Append(@" FROM computers as a ");

            for (int i = 0; i < queries.Where(x => x.Table == "Bios").Count(); i++)
                sb.Append($"LEFT JOIN bios_inventory b{i} on a.computer_id = b{i}.computer_id ");
            for (int i = 0; i < queries.Where(x => x.Table == "System").Count(); i++)
                sb.Append($"LEFT JOIN computer_system_inventory c{i} on a.computer_id = c{i}.computer_id ");
            for (int i = 0; i < queries.Where(x => x.Table == "Hard Drive").Count(); i++)
                sb.Append($"LEFT JOIN hdd_inventory d{i} on a.computer_id = d{i}.computer_id ");
            for (int i = 0; i < queries.Where(x => x.Table == "OS").Count(); i++)
                sb.Append($"LEFT JOIN os_inventory e{i} on a.computer_id = e{i}.computer_id ");
            for (int i = 0; i < queries.Where(x => x.Table == "Printer").Count(); i++)
                sb.Append($"LEFT JOIN printer_inventory f{i} on a.computer_id = f{i}.computer_id ");
            for (int i = 0; i < queries.Where(x => x.Table == "Processor").Count(); i++)
                sb.Append($"LEFT JOIN processor_inventory g{i} on a.computer_id = g{i}.computer_id ");
            for (int i = 0; i < queries.Where(x => x.Table == "Application").Count(); i++)
            {
                sb.Append($"LEFT JOIN computer_software h{i} on a.computer_id = h{i}.computer_id ");
                sb.Append($"LEFT JOIN software_inventory i{i} on h{i}.software_id = i{i}.software_inventory_id ");
            }
            for (int i = 0; i < queries.Where(x => x.Table == "Windows Update").Count(); i++)
            {
                sb.Append($"LEFT JOIN computer_updates j{i} on a.computer_id = j{i}.computer_id ");
                sb.Append($"LEFT JOIN wu_inventory k{i} on j{i}.wu_inventory_id = k{i}.wu_inventory_id ");
            }
            for (int i = 0; i < queries.Where(x => x.Table == "Firewall").Count(); i++)
                sb.Append($"LEFT JOIN firewall_inventory l{i} on a.computer_id = l{i}.computer_id ");
            for (int i = 0; i < queries.Where(x => x.Table == "AntiVirus").Count(); i++)
                sb.Append($"LEFT JOIN antivirus_inventory m{i} on a.computer_id = m{i}.computer_id ");
            for (int i = 0; i < queries.Where(x => x.Table == "BitLocker").Count(); i++)
                sb.Append($"LEFT JOIN bitlocker_inventory n{i} on a.computer_id = n{i}.computer_id ");
            for (int i = 0; i < queries.Where(x => x.Table == "Logical Volumes").Count(); i++)
                sb.Append($"LEFT JOIN logical_volume_inventory o{i} on a.computer_id = o{i}.computer_id ");
            for (int i = 0; i < queries.Where(x => x.Table == "Network Adapters").Count(); i++)
                sb.Append($"LEFT JOIN nic_inventory p{i} on a.computer_id = p{i}.computer_id ");
            for (int i = 0; i < queries.Where(x => x.Table == "Certificates").Count(); i++)
            {
                sb.Append($"LEFT JOIN computer_certificates q{i} on a.computer_id = q{i}.computer_id ");
                sb.Append($"LEFT JOIN certificate_inventory r{i} on q{i}.certificate_id = r{i}.certificate_inventory_id ");
            }
            for (int i = 0; i < queries.Where(x => x.Table == "Category").Count(); i++)
            {
                sb.Append($"LEFT JOIN computer_categories s{i} on a.computer_id = s{i}.computer_id ");
                sb.Append($"LEFT JOIN categories t{i} on s{i}.category_id = t{i}.category_id ");
            }
            for (int i = 0; i < queries.Where(x => x.Table == "Gpu").Count(); i++)
                sb.Append($"LEFT JOIN computer_gpu_inventory u{i} on a.computer_id = u{i}.computer_id ");
            for (int i = 0; i < queries.Where(x => x.Table == "Group").Count(); i++)
            {
                sb.Append($"LEFT JOIN group_memberships v{i} on a.computer_id = v{i}.computer_id ");
                sb.Append($"LEFT JOIN groups w{i} on v{i}.group_id = w{i}.group_id ");
            }


            var scriptModuleIds = new List<int>();
            var customAttributeIds = new List<int>();
            foreach (var query in queries)
            {
                if (query.Table.StartsWith("("))
                {
                    var cia = query.Table.Trim('(').Trim(')');
                    var ciaType = cia.Split('_').First();
                    if (ciaType.Equals("ci"))
                    {
                        int ciId;
                        if (int.TryParse(cia.Split('_')[1], out ciId))
                            scriptModuleIds.Add(ciId);
                    }
                    else if (ciaType.Equals("ca"))
                    {
                        int caId;
                        if (int.TryParse(cia.Split('_')[1], out caId))
                            customAttributeIds.Add(caId);
                    }

                }
            }

            if (scriptModuleIds.Count > 0)
            {
                foreach (var moduleId in scriptModuleIds)
                {
                    sb.Append("LEFT JOIN custom_inventory ci" + moduleId + " on a.computer_id = ci" + moduleId + ".computer_id AND ci" + moduleId + ".script_module_id in ");
                    sb.Append("(" + moduleId + ") ");
                }
            }

            if (customAttributeIds.Count > 0)
            {
                foreach (var attributeId in customAttributeIds)
                {
                    sb.Append("LEFT JOIN custom_computer_attributes ca" + attributeId + " on a.computer_id = ca" + attributeId + ".computer_id AND ca" + attributeId + ".custom_attribute_id in ");
                    sb.Append("(" + attributeId + ") ");
                }
            }


            sb.Append("WHERE (");
            int counter = 0;
            var parameters = new List<string>();
            Dictionary<string, int> queryTypeCount = new Dictionary<string, int>();
            queryTypeCount.Add("Bios", 0);
            queryTypeCount.Add("System", 0);
            queryTypeCount.Add("Hard Drive", 0);
            queryTypeCount.Add("OS", 0);
            queryTypeCount.Add("Printer", 0);
            queryTypeCount.Add("Processor", 0);
            queryTypeCount.Add("Application", 0);
            queryTypeCount.Add("Windows Update", 0);
            queryTypeCount.Add("Firewall", 0);
            queryTypeCount.Add("AntiVirus", 0);
            queryTypeCount.Add("BitLocker", 0);
            queryTypeCount.Add("Logical Volumes", 0);
            queryTypeCount.Add("Network Adapters", 0);
            queryTypeCount.Add("Certificates", 0);
            queryTypeCount.Add("Category", 0);
            queryTypeCount.Add("Gpu", 0);
            queryTypeCount.Add("Group", 0);
            foreach (var query in queries)
            {
                var tableAs = "";
                if (query.Table == "Computer")
                    tableAs = "a";
                else if (query.Table == "Bios")
                {
                    tableAs = $"b{queryTypeCount["Bios"]}";
                    queryTypeCount["Bios"]++;
                }
                else if (query.Table == "System")
                {
                    tableAs = $"c{queryTypeCount["System"]}";
                    queryTypeCount["System"]++;
                }
                else if (query.Table == "Hard Drive")
                {
                    tableAs = $"d{queryTypeCount["Hard Drive"]}";
                    queryTypeCount["Hard Drive"]++;
                }
                else if (query.Table == "OS")
                {
                    tableAs = $"e{queryTypeCount["OS"]}";
                    queryTypeCount["OS"]++;
                }
                else if (query.Table == "Printer")
                {
                    tableAs = $"f{queryTypeCount["Printer"]}";
                    queryTypeCount["Printer"]++;
                }
                else if (query.Table == "Processor")
                {
                    tableAs = $"g{queryTypeCount["Processor"]}";
                    queryTypeCount["Processor"]++;
                }
                else if (query.Table == "Application")
                {
                    tableAs = $"i{queryTypeCount["Application"]}";
                    queryTypeCount["Application"]++;
                }
                else if (query.Table == "Windows Update" && query.Field == "is_installed")
                {
                    tableAs = $"j{queryTypeCount["Windows Update"]}";
                    queryTypeCount["Windows Update"]++;
                }
                else if (query.Table == "Windows Update" && query.Field == "install_date")
                {
                    tableAs = $"j{queryTypeCount["Windows Update"]}";
                    queryTypeCount["Windows Update"]++;
                }
                else if (query.Table == "Windows Update")
                {
                    tableAs = $"k{queryTypeCount["Windows Update"]}";
                    queryTypeCount["Windows Update"]++;
                }
                else if (query.Table == "Firewall")
                {
                    tableAs = $"l{queryTypeCount["Firewall"]}";
                    queryTypeCount["Firewall"]++;
                }
                else if (query.Table == "AntiVirus")
                {
                    tableAs = $"m{queryTypeCount["AntiVirus"]}";
                    queryTypeCount["AntiVirus"]++;
                }
                else if (query.Table == "BitLocker")
                {
                    tableAs = $"n{queryTypeCount["BitLocker"]}";
                    queryTypeCount["BitLocker"]++;
                }
                else if (query.Table == "Logical Volumes")
                {
                    tableAs = $"o{queryTypeCount["Logical Volumes"]}";
                    queryTypeCount["Logical Volumes"]++;
                }
                else if (query.Table == "Network Adapters")
                {
                    tableAs = $"p{queryTypeCount["Network Adapters"]}";
                    queryTypeCount["Network Adapters"]++;
                }
                else if (query.Table == "Certificates")
                {
                    tableAs = $"r{queryTypeCount["Certificates"]}";
                    queryTypeCount["Certificates"]++;
                }
                else if (query.Table == "Category")
                {
                    tableAs = $"t{queryTypeCount["Category"]}";
                    queryTypeCount["Category"]++;
                }
                else if (query.Table == "Gpu")
                {
                    tableAs = $"u{queryTypeCount["Gpu"]}";
                    queryTypeCount["Gpu"]++;
                }
                else if (query.Table == "Group")
                {
                    tableAs = $"w{queryTypeCount["Group"]}";
                    queryTypeCount["Group"]++;
                }
                else
                {
                    if (query.Table.StartsWith("("))
                    {
                        var cia = query.Table.Trim('(').Trim(')');
                        var ciaType = cia.Split('_').First();
                        if (ciaType.Equals("ci"))
                        {
                            int ciId;
                            if (int.TryParse(cia.Split('_')[1], out ciId))
                                tableAs = "ci" + ciId;
                        }
                        else if (ciaType.Equals("ca"))
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

            if (queries.First().IncludeArchived && queries.First().IncludePreProvisioned)
            {
                sb.Append(") AND (a.provision_status = 8 OR a.provision_status = 11 OR a.provision_status = 6 OR a.provision_status = 13)");
            }
            else if (queries.First().IncludeArchived)
            {
                sb.Append(") AND (a.provision_status = 8 OR a.provision_status = 11 OR a.provision_status = 13)");
            }

            else if (queries.First().IncludePreProvisioned)
            {
                sb.Append(") AND (a.provision_status = 8 OR a.provision_status = 6 OR a.provision_status = 13)");
            }
            else
            {
                sb.Append(") AND (a.provision_status = 8 OR a.provision_status = 13)");
            }

            var gb = BuildGroupBy(queries.First().GroupBy);
            if(!string.IsNullOrEmpty(gb))
            {
                sb.Append(gb);
            }

            sqlQuery.Sql = sb.ToString();
            sqlQuery.Parameters = parameters;

            return sqlQuery;

           
        }
    }
}
