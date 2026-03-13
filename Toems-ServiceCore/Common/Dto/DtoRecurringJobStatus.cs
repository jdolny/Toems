namespace Toems_Common.Dto
{
    public class DtoRecurringJobStatus
    {
        public string Name { get; set; }
        public string LastRun { get; set; }
        public string Status { get; set; }
        public string NextRun { get; set; }
    }
}
