namespace Toems_Common.Enum
{
    public class EnumToecDeployJob
    {
        public enum JobType
        {
            Install = 0,
            Reinstall = 1,
            Uninstall = 2
           
        }

        public enum RunMode
        {
            Single = 0,
            Continuous = 1
        }
    }
}