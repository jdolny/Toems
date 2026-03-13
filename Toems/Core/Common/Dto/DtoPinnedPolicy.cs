namespace Toems_Common.Dto
{
    public class DtoPinnedPolicy
    {
        public int PolicyId { get; set; }
        public string PolicyName { get; set; }
        public int SuccessCount { get; set; }
        public int SkippedCount { get; set; }
        public int FailedCount { get; set; }
        public string Description { get; set; }
    }
}
