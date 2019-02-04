using System;
using Toems_Common.Enum;

namespace Toems_Common.Dto
{
    public class DtoComputerPolicyHistory
    {
        public int PolicyId { get; set; }
        public string PolicyName { get; set; }
        public string PolicyHash { get; set; }
        public EnumPolicyHistory.RunResult Result { get; set; }
        public DateTime RunTime { get; set; }
    }
}
