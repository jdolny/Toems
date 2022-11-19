using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toems_Common.Enum;

namespace Toems_Common.Dto
{
    public class DtoSingleToecDeploy
    {
        public string ComputerName;
        public string Username;
        public string Password;
        public string Domain;
        public EnumToecDeployJob.JobType JobType;
    }
}
