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

       

      

       

      

       
    }
}
