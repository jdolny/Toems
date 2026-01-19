using System.Data;
using System.Text;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Enum;
using Toems_DataModel;
using Toems_Service;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceReport(EntityContext ectx, ServiceUser userService)
    {
        public List<DtoComputerUserLogins> GetUserLogins(string searchString)
        {
            return new ReportRepository().GetUserLogins(searchString);
        }

        public DataSet GetInventory(List<DtoCustomComputerQuery> queries)
        {
            var sql = new Toems_Service.Workflows.BuildReportSqlQuery().Run(queries);
            if(sql == null) return null;
            return new RawSqlRepository().ExecuteReader(sql);
        }

        public DataSet GetSqlQueryReport(DtoApiStringResponse sql)
        {
            return new RawSqlRepository().ExecuteCustomSqlReportReader(sql.Value);
        }

        public DataSet GetAssetInventory(List<DtoCustomComputerQuery> queries)
        {
            var sql = new Toems_Service.Workflows.BuildAssetSqlQuery().Run(queries);
            if (sql == null) return null;
            return new RawSqlRepository().ExecuteReader(sql);
        }

        public List<DtoProcessWithTime> GetTopProcessTimes(DateTime dateCutoff, int limit)
        {
            return new ReportRepository().GetTopProcessTimes(dateCutoff,limit);
        }

        public List<DtoProcessWithCount> GetTopProcessCounts(DateTime dateCutoff, int limit)
        {
            return new ReportRepository().GetTopProcessCounts(dateCutoff, limit);
        }

        public DtoApiStringResponse GetCheckinCounts()
        {
            var uow = new UnitOfWork();
            var currentTime = DateTime.Now;
            var hourMinus1 = currentTime - TimeSpan.FromHours(1);
            var hourMinus2 = currentTime - TimeSpan.FromHours(2);
            var hourMinus3 = currentTime - TimeSpan.FromHours(3);
            var hourMinus4 = currentTime - TimeSpan.FromHours(4);
            var hourMinus5 = currentTime - TimeSpan.FromHours(5);
            var hourMinus6 = currentTime - TimeSpan.FromHours(6);
            var hourMinus7 = currentTime - TimeSpan.FromHours(7);
            var hourMinus8 = currentTime - TimeSpan.FromHours(8);
            var hourMinus9 = currentTime - TimeSpan.FromHours(9);
            var hourMinus10 = currentTime - TimeSpan.FromHours(10);
            var hourMinus11 = currentTime - TimeSpan.FromHours(11);
            var hourMinus12 = currentTime - TimeSpan.FromHours(12);

            var sb = new StringBuilder();

            sb.Append(uow.ComputerRepository.Count(x => x.LastCheckinTime >= hourMinus1));
            sb.Append(",");
            sb.Append(uow.ComputerRepository.Count(x => (x.LastCheckinTime >= hourMinus2) && x.LastCheckinTime <= hourMinus1));
            sb.Append(",");
            sb.Append(uow.ComputerRepository.Count(x => (x.LastCheckinTime >= hourMinus3) && x.LastCheckinTime <= hourMinus2));
            sb.Append(",");
            sb.Append(uow.ComputerRepository.Count(x => (x.LastCheckinTime >= hourMinus4) && x.LastCheckinTime <= hourMinus3));
            sb.Append(",");
            sb.Append(uow.ComputerRepository.Count(x => (x.LastCheckinTime >= hourMinus5) && x.LastCheckinTime <= hourMinus4));
            sb.Append(",");
            sb.Append(uow.ComputerRepository.Count(x => (x.LastCheckinTime >= hourMinus6) && x.LastCheckinTime <= hourMinus5));
            sb.Append(",");
            sb.Append(uow.ComputerRepository.Count(x => (x.LastCheckinTime >= hourMinus7) && x.LastCheckinTime <= hourMinus6));
            sb.Append(",");
            sb.Append(uow.ComputerRepository.Count(x => (x.LastCheckinTime >= hourMinus8) && x.LastCheckinTime <= hourMinus7));
            sb.Append(",");
            sb.Append(uow.ComputerRepository.Count(x => (x.LastCheckinTime >= hourMinus9) && x.LastCheckinTime <= hourMinus8));
            sb.Append(",");
            sb.Append(uow.ComputerRepository.Count(x => (x.LastCheckinTime >= hourMinus10) && x.LastCheckinTime <= hourMinus9));
            sb.Append(",");
            sb.Append(uow.ComputerRepository.Count(x => (x.LastCheckinTime >= hourMinus11) && x.LastCheckinTime <= hourMinus10));
            sb.Append(",");
            sb.Append(uow.ComputerRepository.Count(x => (x.LastCheckinTime >= hourMinus12) && x.LastCheckinTime <= hourMinus11));

            var response = new DtoApiStringResponse();
            response.Value = sb.ToString();
            return response;
        }

        public bool SendSmartReport()
        {
            if (ectx.Settings.GetSettingValue(SettingStrings.SmtpEnabled) != "1")
                return true;

            var uow = new UnitOfWork();
            var computers = uow.ComputerRepository.Get(x => x.ProvisionStatus == EnumProvisionStatus.Status.Provisioned);
            if (computers.Count == 0) return true;

            var sb = new StringBuilder();
            var errorsFound = false;
            sb.Append("The Following Hard Drives Have A Failed S.M.A.R.T. Status:\r\n\r\n");
            foreach (var computer in computers)
            {
                var localComputer = computer;
                var hdds = uow.HardDriveInventoryRepository.Get(x => x.ComputerId == localComputer.Id);
                foreach (var hdd in hdds)
                {
                    if (!hdd.Status.ToLower().Equals("ok"))
                    {
                        errorsFound = true;
                        sb.Append(computer.Name + "\t" + hdd.Model + "\t" + hdd.SerialNumber + "\t" + hdd.Status + Environment.NewLine);
                    }
                }
            }

            if (!errorsFound) return true;

            var emailList = new List<string>();
            var users = uow.UserRepository.Get();
            foreach (var user in users)
            {
                if (user.Membership.Equals("Administrator"))
                {
                    if (!string.IsNullOrEmpty(user.Email))
                        emailList.Add(user.Email);
                }
                else
                {
                    var rights = userService.GetUserRights(user.Id).Select(right => right.Right).ToList();
                    if (rights.Any(right => right == AuthorizationStrings.EmailSmart))
                    {
                        if (!string.IsNullOrEmpty(user.Email))
                            emailList.Add(user.Email);
                    }
                }
            }

            foreach (var email in emailList)
            {
                var mail = new MailServices
                {
                    Subject = "S.M.A.R.T Failure Report",
                    Body = sb.ToString(),
                    MailTo = email
                };

                mail.Send();
            }

            return true;
        }

        public bool SendLowDiskSpaceReport()
        {
            if (ectx.Settings.GetSettingValue(SettingStrings.SmtpEnabled) != "1")
                return true;

            var uow = new UnitOfWork();

            var sb = new StringBuilder();
            var errorsFound = false;
            var comServers = new Toems_Service.Workflows.ComServerFreeSpace().RunAllServers();

            sb.Append("The Following Com Servers Have Low Disk Space:\r\n\r\n");
            foreach (var comServer in comServers)
            {
                if(comServer.freePercent < 20)
                {
                    errorsFound = true;
                    sb.Append(comServer.name + "\t" + comServer.freePercent + "% Free");
                }
            }

            if (!errorsFound) return true;

            var emailList = new List<string>();
            var users = uow.UserRepository.Get();
            foreach (var user in users)
            {
                if (user.Membership.Equals("Administrator"))
                {
                    if (!string.IsNullOrEmpty(user.Email))
                        emailList.Add(user.Email);
                }
                else
                {
                    var rights = userService.GetUserRights(user.Id).Select(right => right.Right).ToList();
                    if (rights.Any(right => right == AuthorizationStrings.EmailLowDiskSpace))
                    {
                        if (!string.IsNullOrEmpty(user.Email))
                            emailList.Add(user.Email);
                    }
                }
            }

            foreach (var email in emailList)
            {
                var mail = new MailServices
                {
                    Subject = "Com Server Low Disk Space",
                    Body = sb.ToString(),
                    MailTo = email
                };

                mail.Send();
            }

            return true;
        }
    }
}