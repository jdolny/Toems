using System;
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
        
         private static readonly HashSet<string> AllowedAndOr = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "", "AND", "OR" };
        private static readonly HashSet<string> AllowedOperators = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "=", "!=", "<", ">", "<=", ">=", "LIKE", "NOT LIKE" };
        private static readonly HashSet<string> AllowedParentheses = new HashSet<string> { "", "(", ")" };
        private static readonly HashSet<string> AllowedTables = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Computer", "Bios", "System", "Hard Drive", "OS", "Printer", "Processor", "Application",
            "Windows Update", "Firewall", "AntiVirus", "BitLocker", "Logical Volumes", "Network Adapters",
            "Certificates", "Category", "Gpu", "Group"
        };
        
        private static readonly Dictionary<string, HashSet<string>> AllowedFields = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase)
        {
            { "Computer", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "computer_name", "provisioned_time_local", "last_checkin_time_local", "last_ip", "is_ad_sync", "client_version", "last_inventory_time_local", "ad_disabled", "provision_status" } },
            { "Bios", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "serial_number", "version" , "sm_bios_version" } },
            { "System", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "manufacturer", "model", "domain", "workgroup", "memory" } },
            { "Hard Drive", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "model", "firmware", "serial_number", "size", "smart_status" } },
            { "OS", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "name", "version", "build", "arch", "sp_major", "sp_minor", "release_id", "uac_status", "local_time_zone", "location_enabled", "last_location_update_utc", "update_server", "update_server_target_group" } },
            { "Printer", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "name" , "driver_name", "is_local", "is_network", "share_name", "system_name" } },
            { "Processor", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "name", "clock_speed", "cores" } },
            { "Application", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "name", "version", "major", "minor", "build", "revision" } },
            { "Windows Update", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "title", "install_date" , "is_installed" } },
            { "Firewall", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "domain_enabled", "private_enabled", "public_enabled"} },
            { "AntiVirus", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "display_name", "provider", "rt_scanner" , "definition_status", "product_state"} },
            { "BitLocker", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "status" , "drive_letter"  } },
            { "Logical Volumes", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "drive", "free_space_gb", "free_space_percent", "size_gb" } },
            { "Network Adapters", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "nic_name", "nic_description", "nic_type", "nic_mac", "nic_status", "nic_speed", "nic_ips", "nic_gateways" } },
            { "Certificates", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "store","subject", "friendlyname", "thumbprint", "serial", "notbefore_utc" , "notafter_utc"} },
            { "Category", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "category_name" } },
            { "Gpu", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "computer_gpu_name", "computer_gpu_ram" } },
            { "Group", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "group_id", "group_name" } }
        };
        
        private string QuoteIdentifier(string identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier))
                throw new ArgumentException("Invalid field name");
            return "`" + identifier.Replace("`", "``") + "`";
        }
        
       private void ValidateQuery(DtoCustomComputerQuery query, bool isFirst, bool isNotCase)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            // Validate Table
            string normalizedTable = query.Table;
            if (query.Table.StartsWith("("))
            {
                var cia = query.Table.Trim('(').Trim(')');
                var parts = cia.Split('_');
                if (parts.Length != 2 || (parts[0] != "ci" && parts[0] != "ca") || !int.TryParse(parts[1], out _))
                    throw new ArgumentException("Invalid custom table format: " + query.Table);
                normalizedTable = parts[0]; // e.g., "ci" or "ca"
            }
            else if (!AllowedTables.Contains(query.Table))
            {
                throw new ArgumentException("Invalid table: " + query.Table);
            }

            // Validate Field
            if (normalizedTable == "ci" || normalizedTable == "ca")
            {
                // Custom tables always use the 'value' column
                if (!query.Field.Equals("value", StringComparison.OrdinalIgnoreCase))
                    throw new ArgumentException($"Invalid field '{query.Field}' for custom table '{query.Table}'. Only 'value' is allowed.");
            }
            else if (!AllowedFields[query.Table].Contains(query.Field))
            {
                throw new ArgumentException($"Invalid field '{query.Field}' for table '{query.Table}'");
            }

            // Validate AndOr
            if (isNotCase)
            {
                if (!query.AndOr.Equals("Not", StringComparison.OrdinalIgnoreCase))
                    throw new ArgumentException("Invalid AndOr for Not case: " + query.AndOr);
            }
            else
            {
                var normalizedAndOr = isFirst ? "" : query.AndOr;
                if (!AllowedAndOr.Contains(normalizedAndOr))
                    throw new ArgumentException("Invalid AndOr: " + query.AndOr);
            }

            // Validate Operator
            if (!AllowedOperators.Contains(query.Operator))
                throw new ArgumentException("Invalid operator: " + query.Operator);

            // Validate Parentheses
            if (!AllowedParentheses.Contains(query.LeftParenthesis) || (query.LeftParenthesis == ")" && !string.IsNullOrEmpty(query.LeftParenthesis)))
                throw new ArgumentException("Invalid left parenthesis: " + query.LeftParenthesis);
            if (!AllowedParentheses.Contains(query.RightParenthesis) || (query.RightParenthesis == "(" && !string.IsNullOrEmpty(query.RightParenthesis)))
                throw new ArgumentException("Invalid right parenthesis: " + query.RightParenthesis);
        }

        private string BuildGroupBy(string groupBy)
        {
            if (string.IsNullOrEmpty(groupBy))
                return "";

            if (groupBy.StartsWith("("))
            {
                var cia = groupBy.Replace(".value", "").Trim('(').Trim(')');
                var parts = cia.Split('_');
                if (parts.Length != 2 || (parts[0] != "ci" && parts[0] != "ca") || !int.TryParse(parts[1], out int id))
                    throw new ArgumentException("Invalid custom table format in GroupBy: " + groupBy);
                if (!groupBy.EndsWith(".value", StringComparison.OrdinalIgnoreCase))
                    throw new ArgumentException("Invalid field in GroupBy for custom table: only 'value' is allowed");
                return $" GROUP BY ci{id}.[value]";
            }
            else
            {
                var parts = groupBy.Split('.');
                if (parts.Length != 2)
                    throw new ArgumentException("Invalid GroupBy format: " + groupBy);
                var table = parts[0];
                var field = parts[1];

                if (!AllowedTables.Contains(table))
                    throw new ArgumentException("Invalid table in GroupBy: " + table);
                if (!AllowedFields[table].Contains(field))
                    throw new ArgumentException($"Invalid field '{field}' in GroupBy for table '{table}'");

                string tableAlias;
                switch (table.ToLower())
                {
                    case "computer": tableAlias = "a"; break;
                    case "bios": tableAlias = "b"; break;
                    case "system": tableAlias = "c"; break;
                    case "hard drive": tableAlias = "d"; break;
                    case "os": tableAlias = "e"; break;
                    case "printer": tableAlias = "f"; break;
                    case "processor": tableAlias = "g"; break;
                    case "application": tableAlias = "i"; break;
                    case "windows update": tableAlias = field == "is_installed" || field == "install_date" ? "j" : "k"; break;
                    case "firewall": tableAlias = "l"; break;
                    case "antivirus": tableAlias = "m"; break;
                    case "bitlocker": tableAlias = "n"; break;
                    case "logical volumes": tableAlias = "o"; break;
                    case "network adapters": tableAlias = "p"; break;
                    case "certificates": tableAlias = "r"; break;
                    case "category": tableAlias = "t"; break;
                    case "gpu": tableAlias = "u"; break;
                    case "group": tableAlias = "w"; break;
                    default: throw new ArgumentException("Invalid table in GroupBy: " + table);
                }
                return $" GROUP BY {tableAlias}.[{field}]";
            }
        }

        public DtoRawSqlQuery Run(List<DtoCustomComputerQuery> queries)
        {
            // Validate all queries upfront
            for (int i = 0; i < queries.Count; i++)
            {
                ValidateQuery(queries[i], i == 0, queries.Count == 1 && queries[0].AndOr.Equals("Not", StringComparison.OrdinalIgnoreCase));
            }
            var sqlQuery = new DtoRawSqlQuery();
            if (queries.Count == 1 && queries[0].AndOr.Equals("Not", StringComparison.OrdinalIgnoreCase))
            {
                var query = queries[0];
                string subWhereClause = string.Empty;
                string tableField = string.Empty;
                if (query.Table.Equals("Application", StringComparison.OrdinalIgnoreCase))
                {
                    tableField = "software_inventory.name";
                    subWhereClause = $@"
                        SELECT computers.computer_id
                        FROM computers
                        LEFT JOIN computer_software ON computers.computer_id = computer_software.computer_id 
                        LEFT JOIN software_inventory ON computer_software.software_id = software_inventory.software_inventory_id
                        WHERE {tableField} {query.Operator} @p1";
                }
                else if (query.Table.Equals("Windows Update", StringComparison.OrdinalIgnoreCase))
                {
                    tableField = "wu_inventory.title";
                    subWhereClause = $@"
                        SELECT computers.computer_id
                        FROM computers
                        LEFT JOIN computer_updates ON computers.computer_id = computer_updates.computer_id 
                        LEFT JOIN wu_inventory ON computer_updates.wu_inventory_id = wu_inventory.wu_inventory_id
                        WHERE {tableField} {query.Operator} @p1";
                }
                else if (query.Table.Equals("Certificates", StringComparison.OrdinalIgnoreCase))
                {
                    tableField = "certificate_inventory.subject";
                    subWhereClause = $@"
                        SELECT computers.computer_id
                        FROM computers
                        LEFT JOIN computer_certificates ON computers.computer_id = computer_certificates.computer_id 
                        LEFT JOIN certificate_inventory ON computer_certificates.certificate_id = certificate_inventory.certificate_inventory_id
                        WHERE {tableField} {query.Operator} @p1";
                }
                else
                {
                    throw new ArgumentException("Unsupported table for Not query: " + query.Table);
                }

                string selectColumns = query.Table.Equals("Certificates", StringComparison.OrdinalIgnoreCase)
                    ? "a.computer_id, a.computer_name"
                    : "a.computer_name";
                sb.Append($@"
                    SELECT {selectColumns}
                    FROM computers AS a
                    WHERE a.computer_id NOT IN ({subWhereClause})
                    AND a.last_inventory_time_local > '2019-01-01'");

                if (query.IncludeArchived && query.IncludePreProvisioned)
                    sb.Append(" AND (a.provision_status = 8 OR a.provision_status = 11 OR a.provision_status = 6)");
                else if (query.IncludeArchived)
                    sb.Append(" AND (a.provision_status = 8 OR a.provision_status = 11)");
                else if (query.IncludePreProvisioned)
                    sb.Append(" AND (a.provision_status = 8 OR a.provision_status = 6)");
                else
                    sb.Append(" AND a.provision_status = 8");

                sb.Append(BuildGroupBy(query.GroupBy));
                sqlQuery.Sql = sb.ToString();
                sqlQuery.Parameters = new List<string> { query.Value };
                return sqlQuery;
            }

            string select = "SELECT a.computer_id, a.computer_name,";
            foreach (var query in queries.Where(x => x.Table.Equals("Computer", StringComparison.OrdinalIgnoreCase)))
            {
                if (query.Field.Equals("computer_name", StringComparison.OrdinalIgnoreCase)) continue;
                select += $" a.{QuoteIdentifier(query.Field)},";
            }
            foreach (var query in queries.Where(x => x.Table.Equals("Bios", StringComparison.OrdinalIgnoreCase)))
                select += $" b.{QuoteIdentifier(query.Field)},";
            foreach (var query in queries.Where(x => x.Table.Equals("System", StringComparison.OrdinalIgnoreCase)))
                select += $" c.{QuoteIdentifier(query.Field)},";
            foreach (var query in queries.Where(x => x.Table.Equals("Hard Drive", StringComparison.OrdinalIgnoreCase)))
                select += $" d.{QuoteIdentifier(query.Field)},";
            foreach (var query in queries.Where(x => x.Table.Equals("OS", StringComparison.OrdinalIgnoreCase)))
                select += $" e.{QuoteIdentifier(query.Field)},";
            foreach (var query in queries.Where(x => x.Table.Equals("Printer", StringComparison.OrdinalIgnoreCase)))
                select += $" f.{QuoteIdentifier(query.Field)},";
            foreach (var query in queries.Where(x => x.Table.Equals("Processor", StringComparison.OrdinalIgnoreCase)))
                select += $" g.{QuoteIdentifier(query.Field)},";
            foreach (var query in queries.Where(x => x.Table.Equals("Application", StringComparison.OrdinalIgnoreCase)))
                select += $" i.{QuoteIdentifier(query.Field)},";
            foreach (var query in queries.Where(x => x.Table.Equals("Windows Update", StringComparison.OrdinalIgnoreCase) && x.Field.Equals("is_installed", StringComparison.OrdinalIgnoreCase)))
                select += $" j.{QuoteIdentifier(query.Field)},";
            foreach (var query in queries.Where(x => x.Table.Equals("Windows Update", StringComparison.OrdinalIgnoreCase) && x.Field.Equals("install_date", StringComparison.OrdinalIgnoreCase)))
                select += $" j.{QuoteIdentifier(query.Field)},";
            foreach (var query in queries.Where(x => x.Table.Equals("Windows Update", StringComparison.OrdinalIgnoreCase) && x.Field.Equals("title", StringComparison.OrdinalIgnoreCase)))
                select += $" k.{QuoteIdentifier(query.Field)},";
            foreach (var query in queries.Where(x => x.Table.Equals("Firewall", StringComparison.OrdinalIgnoreCase)))
                select += $" l.{QuoteIdentifier(query.Field)},";
            foreach (var query in queries.Where(x => x.Table.Equals("AntiVirus", StringComparison.OrdinalIgnoreCase)))
                select += $" m.{QuoteIdentifier(query.Field)},";
            foreach (var query in queries.Where(x => x.Table.Equals("BitLocker", StringComparison.OrdinalIgnoreCase)))
                select += $" n.{QuoteIdentifier(query.Field)},";
            foreach (var query in queries.Where(x => x.Table.Equals("Logical Volumes", StringComparison.OrdinalIgnoreCase)))
                select += $" o.{QuoteIdentifier(query.Field)},";
            foreach (var query in queries.Where(x => x.Table.Equals("Network Adapters", StringComparison.OrdinalIgnoreCase)))
                select += $" p.{QuoteIdentifier(query.Field)},";
            foreach (var query in queries.Where(x => x.Table.Equals("Certificates", StringComparison.OrdinalIgnoreCase)))
                select += $" r.{QuoteIdentifier(query.Field)},";
            foreach (var query in queries.Where(x => x.Table.Equals("Category", StringComparison.OrdinalIgnoreCase)))
                select += $" t.{QuoteIdentifier(query.Field)},";
            foreach (var query in queries.Where(x => x.Table.Equals("Gpu", StringComparison.OrdinalIgnoreCase)))
                select += $" u.{QuoteIdentifier(query.Field)},";
            foreach (var query in queries.Where(x => x.Table.Equals("Group", StringComparison.OrdinalIgnoreCase)))
                select += $" w.{QuoteIdentifier(query.Field)},";
            foreach (var query in queries.Where(x => x.Table.StartsWith("(")))
            {
                var cia = query.Table.Trim('(').Trim(')');
                var ciaType = cia.Split('_').First();
                if (ciaType.Equals("ci", StringComparison.OrdinalIgnoreCase))
                {
                    if (int.TryParse(cia.Split('_')[1], out int ciId))
                    {
                        var script = new ServiceScriptModule().GetModule(ciId);
                        select += $" ci{ciId}.{QuoteIdentifier(query.Field)} AS [{script.Name.Replace("]", "]]")}],";
                    }
                }
                else if (ciaType.Equals("ca", StringComparison.OrdinalIgnoreCase))
                {
                    if (int.TryParse(cia.Split('_')[1], out int caId))
                    {
                        var attribute = new ServiceCustomAttribute().GetCustomAttribute(caId);
                        select += $" ca{caId}.{QuoteIdentifier(query.Field)} AS [{attribute.Name.Replace("]", "]]")}],";
                    }
                }
            }

            var selectTrimmed = select.Trim(',') + " ";
            sb.Append(selectTrimmed);
            sb.Append(@"FROM computers AS a ");

            if (queries.Any(x => x.Table == "Bios"))
                sb.Append("LEFT JOIN bios_inventory b ON a.computer_id = b.computer_id ");
            if (queries.Any(x => x.Table == "System"))
                sb.Append("LEFT JOIN computer_system_inventory c ON a.computer_id = c.computer_id ");
            if (queries.Any(x => x.Table == "Hard Drive"))
                sb.Append("LEFT JOIN hdd_inventory d ON a.computer_id = d.computer_id ");
            if (queries.Any(x => x.Table == "OS"))
                sb.Append("LEFT JOIN os_inventory e ON a.computer_id = e.computer_id ");
            if (queries.Any(x => x.Table == "Printer"))
                sb.Append("LEFT JOIN printer_inventory f ON a.computer_id = f.computer_id ");
            if (queries.Any(x => x.Table == "Processor"))
                sb.Append("LEFT JOIN processor_inventory g ON a.computer_id = g.computer_id ");
            if (queries.Any(x => x.Table == "Application"))
            {
                sb.Append("LEFT JOIN computer_software h ON a.computer_id = h.computer_id ");
                sb.Append("LEFT JOIN software_inventory i ON h.software_id = i.software_inventory_id ");
            }
            if (queries.Any(x => x.Table == "Windows Update"))
            {
                sb.Append("LEFT JOIN computer_updates j ON a.computer_id = j.computer_id ");
                sb.Append("LEFT JOIN wu_inventory k ON j.wu_inventory_id = k.wu_inventory_id ");
            }
            if (queries.Any(x => x.Table == "Firewall"))
                sb.Append("LEFT JOIN firewall_inventory l ON a.computer_id = l.computer_id ");
            if (queries.Any(x => x.Table == "AntiVirus"))
                sb.Append("LEFT JOIN antivirus_inventory m ON a.computer_id = m.computer_id ");
            if (queries.Any(x => x.Table == "BitLocker"))
                sb.Append("LEFT JOIN bitlocker_inventory n ON a.computer_id = n.computer_id ");
            if (queries.Any(x => x.Table == "Logical Volumes"))
                sb.Append("LEFT JOIN logical_volume_inventory o ON a.computer_id = o.computer_id ");
            if (queries.Any(x => x.Table == "Network Adapters"))
                sb.Append("LEFT JOIN nic_inventory p ON a.computer_id = p.computer_id ");
            if (queries.Any(x => x.Table == "Certificates"))
            {
                sb.Append("LEFT JOIN computer_certificates q ON a.computer_id = q.computer_id ");
                sb.Append("LEFT JOIN certificate_inventory r ON q.certificate_id = r.certificate_inventory_id ");
            }
            if (queries.Any(x => x.Table == "Category"))
            {
                sb.Append("LEFT JOIN computer_categories s ON a.computer_id = s.computer_id ");
                sb.Append("LEFT JOIN categories t ON s.category_id = t.category_id ");
            }
            if (queries.Any(x => x.Table == "Gpu"))
                sb.Append("LEFT JOIN computer_gpu_inventory u ON a.computer_id = u.computer_id ");
            if (queries.Any(x => x.Table == "Group"))
            {
                sb.Append("LEFT JOIN group_memberships v ON a.computer_id = v.computer_id ");
                sb.Append("LEFT JOIN groups w ON v.group_id = w.group_id ");
            }

            var scriptModuleIds = new List<int>();
            var customAttributeIds = new List<int>();
            foreach (var query in queries)
            {
                if (query.Table.StartsWith("("))
                {
                    var cia = query.Table.Trim('(').Trim(')');
                    var ciaType = cia.Split('_').First();
                    if (ciaType.Equals("ci", StringComparison.OrdinalIgnoreCase))
                    {
                        if (int.TryParse(cia.Split('_')[1], out int ciId))
                            scriptModuleIds.Add(ciId);
                    }
                    else if (ciaType.Equals("ca", StringComparison.OrdinalIgnoreCase))
                    {
                        if (int.TryParse(cia.Split('_')[1], out int caId))
                            customAttributeIds.Add(caId);
                    }
                }
            }

            if (scriptModuleIds.Count > 0)
            {
                foreach (var moduleId in scriptModuleIds.Distinct())
                {
                    sb.Append($"LEFT JOIN custom_inventory ci{moduleId} ON a.computer_id = ci{moduleId}.computer_id AND ci{moduleId}.script_module_id = {moduleId} ");
                }
            }

            if (customAttributeIds.Count > 0)
            {
                foreach (var attributeId in customAttributeIds.Distinct())
                {
                    sb.Append($"LEFT JOIN custom_computer_attributes ca{attributeId} ON a.computer_id = ca{attributeId}.computer_id AND ca{attributeId}.custom_attribute_id = {attributeId} ");
                }
            }


            sb.Append("WHERE (");
            int counter = 0;
            var parameters = new List<string>();
            bool isFirst = true;
            foreach (var query in queries)
            {
                var tableAs = "";
                if (query.Table.Equals("Computer", StringComparison.OrdinalIgnoreCase))
                    tableAs = "a";
                else if (query.Table.Equals("Bios", StringComparison.OrdinalIgnoreCase))
                    tableAs = "b";
                else if (query.Table.Equals("System", StringComparison.OrdinalIgnoreCase))
                    tableAs = "c";
                else if (query.Table.Equals("Hard Drive", StringComparison.OrdinalIgnoreCase))
                    tableAs = "d";
                else if (query.Table.Equals("OS", StringComparison.OrdinalIgnoreCase))
                    tableAs = "e";
                else if (query.Table.Equals("Printer", StringComparison.OrdinalIgnoreCase))
                    tableAs = "f";
                else if (query.Table.Equals("Processor", StringComparison.OrdinalIgnoreCase))
                    tableAs = "g";
                else if (query.Table.Equals("Application", StringComparison.OrdinalIgnoreCase))
                    tableAs = "i";
                else if (query.Table.Equals("Windows Update", StringComparison.OrdinalIgnoreCase) && (query.Field.Equals("is_installed", StringComparison.OrdinalIgnoreCase) || query.Field.Equals("install_date", StringComparison.OrdinalIgnoreCase)))
                    tableAs = "j";
                else if (query.Table.Equals("Windows Update", StringComparison.OrdinalIgnoreCase))
                    tableAs = "k";
                else if (query.Table.Equals("Firewall", StringComparison.OrdinalIgnoreCase))
                    tableAs = "l";
                else if (query.Table.Equals("AntiVirus", StringComparison.OrdinalIgnoreCase))
                    tableAs = "m";
                else if (query.Table.Equals("BitLocker", StringComparison.OrdinalIgnoreCase))
                    tableAs = "n";
                else if (query.Table.Equals("Logical Volumes", StringComparison.OrdinalIgnoreCase))
                    tableAs = "o";
                else if (query.Table.Equals("Network Adapters", StringComparison.OrdinalIgnoreCase))
                    tableAs = "p";
                else if (query.Table.Equals("Certificates", StringComparison.OrdinalIgnoreCase))
                    tableAs = "r";
                else if (query.Table.Equals("Category", StringComparison.OrdinalIgnoreCase))
                    tableAs = "t";
                else if (query.Table.Equals("Gpu", StringComparison.OrdinalIgnoreCase))
                    tableAs = "u";
                else if (query.Table.Equals("Group", StringComparison.OrdinalIgnoreCase))
                    tableAs = "w";
                else if (query.Table.StartsWith("("))
                {
                    var cia = query.Table.Trim('(').Trim(')');
                    var ciaType = cia.Split('_').First().ToLower();
                    var id = cia.Split('_')[1];
                    tableAs = ciaType + id;
                }

                counter++;
                string andOr = isFirst ? "" : query.AndOr;
                sb.Append(andOr + " " + query.LeftParenthesis + " " + tableAs + "." + QuoteIdentifier(query.Field) + " ");
                sb.Append(query.Operator + " @p" + counter + " " + query.RightParenthesis + " ");

                parameters.Add(query.Value);
                isFirst = false;
            }

            if (queries.First().IncludeArchived && queries.First().IncludePreProvisioned)
                sb.Append(") AND (a.provision_status = 8 OR a.provision_status = 11 OR a.provision_status = 6 OR a.provision_status = 13)");
            else if (queries.First().IncludeArchived)
                sb.Append(") AND (a.provision_status = 8 OR a.provision_status = 11 OR a.provision_status = 13)");
            else if (queries.First().IncludePreProvisioned)
                sb.Append(") AND (a.provision_status = 8 OR a.provision_status = 6 OR a.provision_status = 13)");
            else
                sb.Append(") AND (a.provision_status = 8 OR a.provision_status = 13)");

            var gb = BuildGroupBy(queries.First().GroupBy);
            if (!string.IsNullOrEmpty(gb))
                sb.Append(gb);

            sqlQuery.Sql = sb.ToString();
            sqlQuery.Parameters = parameters;
            return sqlQuery;

        }
    }
}
