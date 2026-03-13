using System;

namespace Toems_Common.Dto
{
    public class DtoComputerUserLogins
    {
        public string ComputerName { get; set; }
        public string UserName { get; set; }
        public DateTime LoginTime { get; set; }
        public DateTime LogoutTime { get; set; }
    }
}
