using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using log4net;
using Toems_Common.Dto;

namespace Toems_DataModel
{
    public class ReportRepository
    {
        private readonly ToemsDbContext _context;

        public ReportRepository()
        {
            _context = new ToemsDbContext();
        }

        public List<DtoComputerUserLogins> GetUserLogins(string searchString)
        {
            return (from h in _context.Computers
                join g in _context.UserLogins on h.Id equals g.ComputerId
                where g.UserName.Contains(searchString)
                orderby g.LogoutDateTime descending
                select new
                {
                    computerName = h.Name,
                    userName = g.UserName,
                    loginTime = g.LoginDateTime,
                    logoutTime = g.LogoutDateTime,
                }).AsEnumerable().Select(x => new DtoComputerUserLogins()
            {
                ComputerName = x.computerName,
                UserName = x.userName,
                LoginTime = x.loginTime.ToLocalTime(),
                LogoutTime = x.logoutTime.ToLocalTime()
            }).ToList();
        }

        public List<DtoProcessWithCount> GetTopProcessCountsForGroup(DateTime dateCutoff, int limit, int groupId)
        {
            var groupMembers = (from gm in _context.GroupMemberships where gm.GroupId == groupId select gm.ComputerId);
            var res = (from cp in _context.ComputerProcesses
                where cp.StartTimeUtc > dateCutoff && groupMembers.Contains(cp.ComputerId)
                group cp by cp.ProcessId
                into grouped
                select new
                {
                    id = grouped.Key,
                    total = (long) grouped.Count()
                }).OrderByDescending(x => x.total).Take(limit).ToList();

            var list = new List<DtoProcessWithCount>();
            foreach (var grouping in res)
            {
                var process = new DtoProcessWithCount();
                var p = (from pr in _context.ProcessInventory where pr.Id == grouping.id select pr).FirstOrDefault();
                if (p == null) continue;
                process.Name = p.Name;
                process.Id = p.Id;
                process.Path = p.Path;
                process.TotalCount = grouping.total;
                list.Add(process);
            }

            return list;
        }

        public List<DtoProcessWithCount> GetTopProcessCountsForComputer(DateTime dateCutoff, int limit, int computerId)
        {
            var res = (from cp in _context.ComputerProcesses
                where cp.StartTimeUtc > dateCutoff && cp.ComputerId == computerId
                group cp by cp.ProcessId
                into grouped
                select new
                {
                    id = grouped.Key,
                    total = (long) grouped.Count()
                }).OrderByDescending(x => x.total).Take(limit).ToList();

            var list = new List<DtoProcessWithCount>();
            foreach (var grouping in res)
            {
                var process = new DtoProcessWithCount();
                var p = (from pr in _context.ProcessInventory where pr.Id == grouping.id select pr).FirstOrDefault();
                if (p == null) continue;
                process.Name = p.Name;
                process.Id = p.Id;
                process.Path = p.Path;
                process.TotalCount = grouping.total;
                list.Add(process);
            }

            return list;
        }

        public List<DtoProcessWithCount> GetTopProcessCountsForUser(DateTime dateCutoff, int limit, string userName)
        {
            if (string.IsNullOrEmpty(userName)) return new List<DtoProcessWithCount>();
            userName = userName.ToLower();
            var res = (from cp in _context.ComputerProcesses
                where cp.StartTimeUtc > dateCutoff && cp.Username.ToLower().Equals(userName)
                group cp by cp.ProcessId
                into grouped
                select new
                {
                    id = grouped.Key,
                    total = (long)grouped.Count()
                }).OrderByDescending(x => x.total).Take(limit).ToList();

            var list = new List<DtoProcessWithCount>();
            foreach (var grouping in res)
            {
                var process = new DtoProcessWithCount();
                var p = (from pr in _context.ProcessInventory where pr.Id == grouping.id select pr).FirstOrDefault();
                if (p == null) continue;
                process.Name = p.Name;
                process.Id = p.Id;
                process.Path = p.Path;
                process.TotalCount = grouping.total;
                list.Add(process);
            }

            return list;
        }

        public List<DtoProcessWithCount> GetTopProcessCounts(DateTime dateCutoff, int limit)
        {
            var res = (from cp in _context.ComputerProcesses
                where cp.StartTimeUtc > dateCutoff
                group cp by cp.ProcessId
                into grouped
                select new
                {
                    id = grouped.Key,
                    total = (long) grouped.Count()
                }).OrderByDescending(x => x.total).Take(limit).ToList();

            var list = new List<DtoProcessWithCount>();
            foreach (var grouping in res)
            {
                var process = new DtoProcessWithCount();
                var p = (from pr in _context.ProcessInventory where pr.Id == grouping.id select pr).FirstOrDefault();
                if (p == null) continue;
                process.Name = p.Name;
                process.Id = p.Id;
                process.Path = p.Path;
                process.TotalCount = grouping.total;
                list.Add(process);
            }

            return list;
        }

        public List<DtoProcessWithTime> GetTopProcessTimesForGroup(DateTime dateCutoff, int limit, int groupId)
        {
            var groupMembers = (from gm in _context.GroupMemberships where gm.GroupId == groupId select gm.ComputerId);
            var res = from cp in _context.ComputerProcesses
                where cp.StartTimeUtc > dateCutoff && groupMembers.Contains(cp.ComputerId)
                select new
                {
                    pid = cp.ProcessId,
                    totalTime = DbFunctions.DiffMinutes(cp.StartTimeUtc, cp.CloseTimeUtc)
                };

            var b = (from a in res
                group a by a.pid
                into g
                select new
                {
                    id = g.Key,
                    total = g.Sum(z => (long) z.totalTime),
                }).OrderByDescending(x => x.total).Take(limit).ToList();

            var list = new List<DtoProcessWithTime>();
            foreach (var grouping in b)
            {
                var process = new DtoProcessWithTime();
                var p = (from pr in _context.ProcessInventory where pr.Id == grouping.id select pr).FirstOrDefault();
                if (p == null) continue;
                process.Name = p.Name;
                process.Id = p.Id;
                process.Path = p.Path;
                process.TotalTime = grouping.total;
                list.Add(process);
            }

            return list;
        }

        public List<DtoProcessWithTime> GetTopProcessTimesForComputer(DateTime dateCutoff, int limit, int computerId)
        {
            var res = from cp in _context.ComputerProcesses
                where cp.StartTimeUtc > dateCutoff && cp.ComputerId == computerId
                select new
                {
                    pid = cp.ProcessId,
                    totalTime = DbFunctions.DiffMinutes(cp.StartTimeUtc,cp.CloseTimeUtc)
                };

            var b = (from a in res
                group a by a.pid
                into g
                select new
                {
                    id = g.Key,
                    total = g.Sum(z => (long) z.totalTime),
                }).OrderByDescending(x => x.total).Take(limit).ToList();

            var list = new List<DtoProcessWithTime>();
            foreach (var grouping in b)
            {
                var process = new DtoProcessWithTime();
                var p = (from pr in _context.ProcessInventory where pr.Id == grouping.id select pr).FirstOrDefault();
                if (p == null) continue;
                process.Name = p.Name;
                process.Id = p.Id;
                process.Path = p.Path;
                process.TotalTime = grouping.total;
                list.Add(process);
            }

            return list;
        }

        public List<DtoProcessWithTime> GetTopProcessTimesForUser(DateTime dateCutoff, int limit, string userName)
        {
            userName = userName.ToLower();
            var res = from cp in _context.ComputerProcesses
                where cp.StartTimeUtc > dateCutoff && cp.Username.ToLower().Equals(userName)
                select new
                {
                    pid = cp.ProcessId,
                    totalTime = DbFunctions.DiffMinutes(cp.StartTimeUtc, cp.CloseTimeUtc)
                };

            var b = (from a in res
                group a by a.pid
                into g
                select new
                {
                    id = g.Key,
                    total = g.Sum(z => (long)z.totalTime),
                }).OrderByDescending(x => x.total).Take(limit).ToList();

            var list = new List<DtoProcessWithTime>();
            foreach (var grouping in b)
            {
                var process = new DtoProcessWithTime();
                var p = (from pr in _context.ProcessInventory where pr.Id == grouping.id select pr).FirstOrDefault();
                if (p == null) continue;
                process.Name = p.Name;
                process.Id = p.Id;
                process.Path = p.Path;
                process.TotalTime = grouping.total;
                list.Add(process);
            }

            return list;
        }


        public List<DtoProcessWithTime> GetTopProcessTimes(DateTime dateCutoff, int limit)
        {
            var res = from cp in _context.ComputerProcesses
                where cp.StartTimeUtc > dateCutoff
                select new
                {
                    pid = cp.ProcessId,
                    totalTime = DbFunctions.DiffMinutes(cp.StartTimeUtc, cp.CloseTimeUtc)
                };

            var b = (from a in res
                group a by a.pid
                into g
                select new
                {
                    id = g.Key,
                    total = g.Sum(z => (long) z.totalTime),
                }).OrderByDescending(x => x.total).Take(limit).ToList();

            var list = new List<DtoProcessWithTime>();
            foreach (var grouping in b)
            {
                var process = new DtoProcessWithTime();
                var p = (from pr in _context.ProcessInventory where pr.Id == grouping.id select pr).FirstOrDefault();
                if (p == null) continue;
                process.Name = p.Name;
                process.Id = p.Id;
                process.Path = p.Path;
                process.TotalTime = grouping.total;
                list.Add(process);
            }

            return list;

        }

        public List<DtoProcessWithUser> GetAllProcessForComputer(DateTime dateCutoff, int limit, int computerId)
        {
            return (from cp in _context.ComputerProcesses
                join pi in _context.ProcessInventory on cp.ProcessId equals pi.Id
                where cp.StartTimeUtc > dateCutoff && cp.ComputerId == computerId
                orderby cp.Id descending
                select new
                {
                    pName = pi.Name,
                    pPath = pi.Path,
                    uName = cp.Username,
                    uStart = cp.StartTimeUtc,
                    uEnd = cp.CloseTimeUtc
                }).Take(limit).AsEnumerable().Select(x => new DtoProcessWithUser()
            {
                ProcessName = x.pName,
                Path = x.pPath,
                UserName = x.uName,
                StartTime = x.uStart,
                EndTime = x.uEnd
            }).ToList();
        }

        public List<DtoProcessWithUser> GetAllProcessForUser(DateTime dateCutoff, int limit, string userName)
        {
            userName = userName.ToLower();
            return (from cp in _context.ComputerProcesses
                join pi in _context.ProcessInventory on cp.ProcessId equals pi.Id
                join comp in _context.Computers on cp.ComputerId equals comp.Id
                where cp.StartTimeUtc > dateCutoff && cp.Username.ToLower().Equals(userName)
                orderby cp.Id descending
                select new
                {
                    pName = pi.Name,
                    pPath = pi.Path,
                    uName = cp.Username,
                    uStart = cp.StartTimeUtc,
                    uEnd = cp.CloseTimeUtc,
                    compName = comp.Name,
                }).Take(limit).AsEnumerable().Select(x => new DtoProcessWithUser()
            {
                ProcessName = x.pName,
                Path = x.pPath,
                UserName = x.uName,
                StartTime = x.uStart,
                EndTime = x.uEnd,
                ComputerName = x.compName

            }).ToList();
        }
    }
}
