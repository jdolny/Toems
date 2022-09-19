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

        public DtoRawSqlQuery Run(List<EntitySmartGroupQuery> queries)
        {
            var sqlQuery = new DtoRawSqlQuery();
            if (queries.Count == 1 && queries[0].AndOr.Equals("Not"))
            {
                var query = queries[0];
               
                if (query.Table.Equals("Application"))
                {
                    sb.Append(@"select a.computer_id,a.computer_name
                                from computers as a
                                where a.computer_id not in 
                                (
                                    SELECT computers.computer_id
                                    FROM computers
                                    left join computer_software on computers.computer_id = computer_software.computer_id 
                                    left join software_inventory on (computer_software.software_id = software_inventory.software_inventory_id)
                                    where software_inventory.name " + query.Operator + "'" + query.Value + "'" + 
                                ")" +
                                "and a.provision_status = 8 and a.last_inventory_time_local > '2019-01-01'");
                    sb.Append(" GROUP BY a.computer_name, a.computer_id");
                }

                else if (query.Table.Equals("Windows Update"))
                {
                    sb.Append(@"select a.computer_id,a.computer_name
                                from computers as a
                                where a.computer_id not in 
                                (
                                    SELECT computers.computer_id
                                    FROM computers
                                    left join computer_updates on computers.computer_id = computer_updates.computer_id 
                                    left join wu_inventory on (computer_updates.wu_inventory_id = wu_inventory.wu_inventory_id)
                                    where wu_inventory.title " + query.Operator + " " + "'" + query.Value + "'" +
                               ")" +
                               " and a.provision_status = 8 and a.last_inventory_time_local > '2019-01-01'");
                    sb.Append(" GROUP BY a.computer_name, a.computer_id");
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
                               ")" +
                               " and a.provision_status = 8 and a.last_inventory_time_local > '2019-01-01'");
                    sb.Append(" GROUP BY a.computer_name, a.computer_id");
                }


                sqlQuery = new DtoRawSqlQuery();
                sqlQuery.Sql = sb.ToString();
                return sqlQuery;
                    
                
            }
            else
            {
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
                foreach (var query in queries)
                {
                    var tableAs = "";
                    if (query.Table == "Computer")
                        tableAs = "a";
                    else if (query.Table == "Bios")
                        tableAs = "b";
                    else if (query.Table == "System")
                        tableAs = "c";
                    else if (query.Table == "Hard Drive")
                        tableAs = "d";
                    else if (query.Table == "OS")
                        tableAs = "e";
                    else if (query.Table == "Printer")
                        tableAs = "f";
                    else if (query.Table == "Processor")
                        tableAs = "g";
                    else if (query.Table == "Application")
                        tableAs = "i";
                    else if (query.Table == "Windows Update" && query.Field == "is_installed")
                        tableAs = "j";
                    else if (query.Table == "Windows Update" && query.Field == "install_date")
                        tableAs = "j";
                    else if (query.Table == "Windows Update")
                        tableAs = "k";
                    else if (query.Table == "Firewall")
                        tableAs = "l";
                    else if (query.Table == "AntiVirus")
                        tableAs = "m";
                    else if (query.Table == "BitLocker")
                        tableAs = "n";
                    else if (query.Table == "Logical Volumes")
                        tableAs = "o";
                    else if (query.Table == "Network Adapters")
                        tableAs = "p";
                    else if (query.Table == "Certificates")
                        tableAs = "r";
                    else if (query.Table == "Category")
                        tableAs = "t";
                    else if (query.Table == "Gpu")
                        tableAs = "u";
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
                sb.Append(") AND (a.provision_status = 8 OR a.provision_status = 6 OR a.provision_status = 13)");
                sb.Append(" GROUP BY a.computer_name, a.computer_id");
                sqlQuery = new DtoRawSqlQuery();
                sqlQuery.Sql = sb.ToString();
                sqlQuery.Parameters = parameters;

                return sqlQuery;
            }

        }
    }
}
