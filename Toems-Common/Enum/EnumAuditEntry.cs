namespace Toems_Common.Enum
{
    public class EnumAuditEntry
    {
        public enum AuditType
        {
            Create = 1,
            Update = 2,
            Archive = 3,
            Delete = 4,
            ActivatePolicy = 5,
            DeactivatePolicy = 6,
            ApproveProvision = 7,
            ApproveReset = 8,
            AddPreProvision = 9,
            SuccessfulLogin = 10,
            FailedLogin = 11,

            Message = 13,
            Reboot = 14,
            Shutdown = 15,
            Wakeup = 16,
            Restore = 17,
            OnDemandMulticast = 18,
            Multicast = 19,
            Deploy = 20,
            Upload = 21,
            OndUpload = 22,
            OndDeploy = 23
        }
    }
}