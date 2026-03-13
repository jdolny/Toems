using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using Microsoft.EntityFrameworkCore;
using Toems_Common.Dto;
using Toems_ServiceCore.Data;

namespace Toems_DataModel
{
    public class ReportRepository(ToemsDbContext _context)
    {
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

       
    }
}
