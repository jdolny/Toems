using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Service.Entity;

namespace Toems_Service.Workflows
{
    public class BuildSqlQuery
    {
        private StringBuilder sb;

        public BuildSqlQuery()
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
        
       private void ValidateQuery(EntitySmartGroupQuery query, bool isFirst, bool isNotCase)
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

        public DtoRawSqlQuery Run(List<EntitySmartGroupQuery> queries)
        {
            var sqlQuery = new DtoRawSqlQuery();
            if (queries.Count == 1 && queries[0].AndOr.Equals("Not", StringComparison.OrdinalIgnoreCase))
            {
                var query = queries[0];
                ValidateQuery(query, isFirst: false, isNotCase: true); // isFirst=false (irrelevant here)
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

                sb.Append($@"
                    SELECT a.computer_id, a.computer_name
                    FROM computers AS a
                    WHERE a.computer_id NOT IN ({subWhereClause})
                    AND a.provision_status = 8 AND a.last_inventory_time_local > '2019-01-01'
                    GROUP BY a.computer_name, a.computer_id");

                sqlQuery.Sql = sb.ToString();
                sqlQuery.Parameters = new List<string> { query.Value };
                return sqlQuery;
            }
            else
            {
                // Validate all queries first
                for (int i = 0; i < queries.Count; i++)
                {
                    ValidateQuery(queries[i], i == 0, false);
                }
                
                sb.Append(@"SELECT a.computer_id,a.computer_name from computers as a ");

                if (queries.Any(x => x.Table == "Bios"))
                    sb.Append("LEFT JOIN bios_inventory b on a.computer_id = b.computer_id ");
                if (queries.Any(x => x.Table == "System"))
                    sb.Append("LEFT JOIN computer_system_inventory c on a.computer_id = c.computer_id ");
                if (queries.Any(x => x.Table == "Hard Drive"))
                    sb.Append("LEFT JOIN hdd_inventory d on a.computer_id = d.computer_id ");
                if (queries.Any(x => x.Table == "OS"))
                    sb.Append("LEFT JOIN os_inventory e on a.computer_id = e.computer_id ");
                if (queries.Any(x => x.Table == "Printer"))
                    sb.Append("LEFT JOIN printer_inventory f on a.computer_id = f.computer_id ");
                if (queries.Any(x => x.Table == "Processor"))
                    sb.Append("LEFT JOIN processor_inventory g on a.computer_id = g.computer_id ");
                if (queries.Any(x => x.Table == "Application"))
                {
                    sb.Append("LEFT JOIN computer_software h on a.computer_id = h.computer_id ");
                    sb.Append("LEFT JOIN software_inventory i on h.software_id = i.software_inventory_id ");
                }
                if (queries.Any(x => x.Table == "Windows Update"))
                {
                    sb.Append("LEFT JOIN computer_updates j on a.computer_id = j.computer_id ");
                    sb.Append("LEFT JOIN wu_inventory k on j.wu_inventory_id = k.wu_inventory_id ");
                }
                if (queries.Any(x => x.Table == "Firewall"))
                    sb.Append("LEFT JOIN firewall_inventory l on a.computer_id = l.computer_id ");
                if (queries.Any(x => x.Table == "AntiVirus"))
                    sb.Append("LEFT JOIN antivirus_inventory m on a.computer_id = m.computer_id ");
                if (queries.Any(x => x.Table == "BitLocker"))
                    sb.Append("LEFT JOIN bitlocker_inventory n on a.computer_id = n.computer_id ");
                if (queries.Any(x => x.Table == "Logical Volumes"))
                    sb.Append("LEFT JOIN logical_volume_inventory o on a.computer_id = o.computer_id ");
                if (queries.Any(x => x.Table == "Network Adapters"))
                    sb.Append("LEFT JOIN nic_inventory p on a.computer_id = p.computer_id ");
                if (queries.Any(x => x.Table == "Certificates"))
                {
                    sb.Append("LEFT JOIN computer_certificates q on a.computer_id = q.computer_id ");
                    sb.Append("LEFT JOIN certificate_inventory r on q.certificate_id = r.certificate_inventory_id ");
                }
                if (queries.Any(x => x.Table == "Category"))
                {
                    sb.Append("LEFT JOIN computer_categories s on a.computer_id = s.computer_id ");
                    sb.Append("LEFT JOIN categories t on s.category_id = t.category_id ");
                }
                if (queries.Any(x => x.Table == "Gpu"))
                    sb.Append("LEFT JOIN computer_gpu_inventory u on a.computer_id = u.computer_id ");
                if (queries.Any(x => x.Table == "Group"))
                {
                    sb.Append($"LEFT JOIN group_memberships v on a.computer_id = v.computer_id ");
                    sb.Append($"LEFT JOIN groups w on v.group_id = w.group_id ");
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
                    var tableAs = string.Empty;
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
                    else if (query.Table.Equals("Windows Update", StringComparison.OrdinalIgnoreCase) && (query.Field == "is_installed" || query.Field == "install_date"))
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
                    string andOr = isFirst ? "" : query.AndOr; // Force first to be empty
                    sb.Append(andOr + " " + query.LeftParenthesis + " " + tableAs + "." + QuoteIdentifier(query.Field) + " ");
                    sb.Append(query.Operator + " @p" + counter + " " + query.RightParenthesis + " ");

                    parameters.Add(query.Value);
                    isFirst = false;
                }
                sb.Append(") AND (a.provision_status = 8 OR a.provision_status = 6 OR a.provision_status = 13)");
                sb.Append(" GROUP BY a.computer_name, a.computer_id");

                sqlQuery.Sql = sb.ToString();
                sqlQuery.Parameters = parameters;

                return sqlQuery;
            }

        }
    }
}
