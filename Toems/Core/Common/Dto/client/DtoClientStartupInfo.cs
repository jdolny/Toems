using Toems_Common.Enum;

namespace Toems_Common.Dto.client
{
    public class DtoClientStartupInfo
    {
        public EnumStartupDelay.DelayType DelayType { get; set; }
        public string SubDelay { get; set; }
        public string ThresholdWindow { get; set; }
        public bool IsError { get; set; }
        public string ErrorMessage { get; set; }
        public string ExpectedClientVersion { get; set; }
    }
}
