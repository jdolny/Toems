namespace Toems_Common.Enum
{
    public class EnumPolicy
    {
        public enum Frequency
        {
            Ongoing = 0,
            OncePerComputer = 1,
            OncePerUserPerComputer = 2,
            OncePerDay = 3,
            OncePerWeek = 4,
            OncePerMonth = 5,
            EveryXhours = 6,
            EveryXdays = 7
        }

        public enum Trigger
        {
            StartupOrCheckin = 0,
            Startup = 1,
            Checkin = 2,
            Login = 3
            
        }

        public enum CompletedAction
        {
            DoNothing = 0,
            Reboot = 1,
            RebootIfNoLogins = 2
        }

        public enum ExecutionType
        {
            Install = 0,
            Cache = 1
        }

        public enum ErrorAction
        {
            AbortCurrentPolicy = 0,
            AbortRemainingPolicies = 1,
            Continue = 2
        }

        public enum LogLevel
        {
            Full = 0,
            HiddenArguments = 1,
            None = 2
        }

        public enum FrequencyMissedAction
        {
            NextOpportunity = 0,
            ScheduleDayOnly = 1
        }

        public enum AutoArchiveType
        {
            None = 0,
            WhenComplete = 1,
            AfterXdays = 2
        }

        public enum StartupDelayType
        {
            None = 0,
            ForXseconds = 1,
            FileCondition = 2
        }

        public enum InventoryAction
        {
            Disabled = 0,
            Before = 1,
            After = 2,
            Both = 3
        }

        public enum RemoteAccess
        {
            NotConfigured = 0,
            Disabled = 1,
            Enabled = 2,
            ForceReinstall = 3
        }

        public enum WuType
        {
            Disabled = 0,
            MicrosoftSkipUpgrades = 1,
            WsusSkipUpgrades = 2,
            Microsoft = 3,
            Wsus = 4
        }

        public enum PolicyComCondition
        {
            Any = 0,
            Selective = 1
        }




    }
}