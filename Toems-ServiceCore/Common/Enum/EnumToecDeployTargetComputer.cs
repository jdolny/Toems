namespace Toems_Common.Enum
{
    public class EnumToecDeployTargetComputer
    {
        public enum TargetStatus
        {
            AwaitingAction = 0,
            Queued = 1,
            Installing = 2,
            Reinstalling = 3,
            Uninstalling = 4,
            InstallComplete = 5,
            ReinstallComplete = 6,
            UninstallComplete = 7,
            Failed = 8
        }
    }
}