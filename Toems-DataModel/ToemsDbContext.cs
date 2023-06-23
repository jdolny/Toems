using System.Data.Entity;
using Toems_Common.Entity;

namespace Toems_DataModel
{
    public class ToemsDbContext : DbContext
    {
        public ToemsDbContext() : base("toems")
        {
        }

        public DbSet<EntityPolicyModules> PolicyModules { get; set; }
        public DbSet<EntityPolicy> Policies { get; set; }
        public DbSet<EntityPrinterModule> PrinterModules { get; set; }
        public DbSet<EntityAuditLog> AuditLogs { get; set; }
        public DbSet<EntityVersion> Versions { get; set; }
        public DbSet<EntitySetting> Settings { get; set; }
        public DbSet<EntityUserGroupRight> UserGroupRight { get; set; }
        public DbSet<EntityToemsUserGroup> UserGroups { get; set; }      
        public DbSet<EntityUserLockout> UserLockouts { get; set; }
        public DbSet<EntityUserRight> UserRight { get; set; }
        public DbSet<EntityToemsUser> Users { get; set; }
        public DbSet<EntityGroup> Groups { get; set; }
        public DbSet<EntityGroupPolicy> GroupPolicies { get; set; }
        public DbSet<EntityUploadedFile> UploadedFiles { get; set; }
        public DbSet<EntitySoftwareModule> SoftwareModules { get; set; }
        public DbSet<EntityCommandModule> CommandModules { get; set; }
        public DbSet<EntityFileCopyModule> FileCopyModules { get; set; }
        public DbSet<EntityScriptModule> ScriptModules { get; set; }
        public DbSet<EntityComputer> Computers { get; set; }
        public DbSet<EntityGroupMembership> GroupMemberships { get; set; }
        public DbSet<EntityActiveGroupPolicy> ActiveGroupPolicies { get; set; }
        public DbSet<EntityActiveClientPolicy> ActiveClientPolicies { get; set; }
        public DbSet<EntityCertificate> Certificates { get; set; }
        public DbSet<EntityBiosInventory> BiosInventory { get; set; }
        public DbSet<EntityComputerSystemInventory> ComputerSystemInventory { get; set; }
        public DbSet<EntityHardDriveInventory> HardDriveInventory { get; set; }
        public DbSet<EntityOsInventory> OsInventory { get; set; }
        public DbSet<EntityPrinterInventory> PrinterInventory { get; set; }
        public DbSet<EntityProcessorInventory> ProcessorInventory { get; set; }
        public DbSet<EntitySoftwareInventory> SoftwareInventory { get; set; }
        public DbSet<EntityWindowsUpdateInventory> WuInventory { get; set; }
        public DbSet<EntityComputerSoftware> ComputerSoftware { get; set; }
        public DbSet<EntityUserLogin> UserLogins { get; set; }
        public DbSet<EntityModule> Modules { get; set; }
        public DbSet<EntitySmartGroupQuery> SmartGroupQueries { get; set; }
        public DbSet<EntityPolicyHistory> PolicyHistories { get; set; }
        public DbSet<EntityCustomInventory> CustomInventories { get; set; }
        public DbSet<EntityImpersonationAccount> ImpersonationAccounts { get; set; }
        public DbSet<EntityClientComServer> ClientComServers { get; set; }
        public DbSet<EntityComServerCluster> ComServerClusters { get; set; }
        public DbSet<EntityComServerClusterServer> ComServerClusterServers { get; set; }
        public DbSet<EntityPinnedPolicy> PinnedPolicies { get; set; }
        public DbSet<EntityPolicyHashHistory> PolicyHashHistory { get; set; }
        public DbSet<EntityResetRequest> ResetRequests { get; set; }
        public DbSet<EntityApprovalRequest> ApprovalRequests { get; set; }
        public DbSet<EntityComputerUpdates> ComputerUpdates { get; set; }
        public DbSet<EntityNicInventory> NicInventory { get; set; }
        public DbSet<EntitySchedule> Schedules { get; set; }
        public DbSet<EntityWolRelay> WolRelays { get; set; }
        public DbSet<EntityProcessInventory> ProcessInventory { get; set; }
        public DbSet<EntityComputerProcess> ComputerProcesses { get; set; }
        public DbSet<EntityAntivirusInventory> Antivirus { get; set; }
        public DbSet<EntityBitlockerInventory> Bitlocker { get; set; }
        public DbSet<EntityFirewallInventory> Firewall { get; set; }
        public DbSet<EntityLogicalVolumeInventory> LogicalVolume { get; set; }
        public DbSet<EntityCurrentDownload> CurrentDownloads { get; set; }
        public DbSet<EntityWuModule> WindowsUpdateModules { get; set; }
        public DbSet<EntityPolicyComServer> PolicyComServers { get; set; }
        public DbSet<EntityCategory> Categories { get; set; }
        public DbSet<EntityPolicyCategory> PolicyCategories { get; set; }
        public DbSet<EntityComputerCategory> ComputerCategories { get; set; }
        public DbSet<EntityGroupCategory> GroupCategories { get; set; }
        public DbSet<EntityModuleCategory> ModuleCategories { get; set; }
        public DbSet<EntityExternalDownload> ExternalDownloads { get; set; }
        public DbSet<EntityCustomAttribute> CustomAttributes { get; set; }
        public DbSet<EntityCustomComputerAttribute> CustomComputerAttributes { get; set; }
        public DbSet<EntityCustomAssetType> CustomAssetTypes { get; set; }
        public DbSet<EntityAsset> Assets { get; set; }
        public DbSet<EntityAssetAttribute> AssetAttributes { get; set; }
        public DbSet<EntityComment> Comments { get; set; }
        public DbSet<EntityAttachment> Attachments { get; set; }
        public DbSet<EntityAssetComment> AssetComments { get; set; }
        public DbSet<EntityAssetAttachment> AssetAttachments { get; set; }
        public DbSet<EntityComputerComment> ComputerComments { get; set; }
        public DbSet<EntityComputerAttachment> ComputerAttachments { get; set; }
        public DbSet<EntitySoftwareAssetSoftware> SoftwareAssetSoftwares { get; set; }
        public DbSet<EntityAssetGroup> AssetGroups { get; set; }
        public DbSet<EntityAssetGroupMember> AssetGroupMembers { get; set; }
        public DbSet<EntityAssetCategory> AssetCategories { get; set; }
        public DbSet<EntityPinnedGroup> PinnedGroups { get; set; }
        public DbSet<EntityComputerCertificate> ComputerCertificates { get; set; }
        public DbSet<EntityCertificateInventory> CertificateInventory { get; set; }
        public DbSet<EntityMessageModule> MessageModules { get; set; }
        public DbSet<EntityActiveSocket> ActiveSockets { get; set; }
        public DbSet<EntityCustomBootMenu> CustomBootMenus { get; set; }
        public DbSet<EntityImageProfileTemplate> ImageProfileTemplates { get; set; }
        public DbSet<EntitySysprepModule> SysprepModules { get; set; }
        public DbSet<EntityImage> Images { get; set; }
        public DbSet<EntityImageProfile> ImageProfiles { get; set; }
        public DbSet<EntityImageCategory> ImageCategories { get; set; }
        public DbSet<EntityImageProfileScript> ImageProfileScripts { get; set; }
        public DbSet<EntityImageProfileSysprepTag> ImageProfileSyspreps { get; set; }
        public DbSet<EntityImageProfileFileCopy> ImageProfileFileCopy { get; set; }
        public DbSet<EntityActiveImagingTask> ActiveImagingTasks { get; set; }
        public DbSet<EntityActiveMulticastSession> ActiveMulticastSessions { get; set; }
        public DbSet<EntityMulticastPort> MulticastPorts { get; set; }
        public DbSet<EntityComputerLog> ComputerLogs { get; set; }
        public DbSet<EntityClientImagingId> ClientImagingIds { get; set; }
        public DbSet<EntitySysprepAnswerfile> SysprepAnswerFiles { get; set; }
        public DbSet<EntitySetupCompleteFile> SetupCompleteFiles { get; set; }
        public DbSet<EntityComputerGpuInventory> ComputerGpus { get; set; }
        public DbSet<EntityToecDeployJob> ToecDeployJob { get; set; }
        public DbSet<EntityToecTargetList> ToecTargetList { get; set; }
        public DbSet<EntityToecTargetListComputer> ToecTargetListComputer { get; set; }
        public DbSet<EntityToecTargetListOu> ToecTargetListOu { get; set; }
        public DbSet<EntityToecDeployThread> ToecDeployThread { get; set; }
        public DbSet<EntityWinPeModule> WinPeModules { get; set; }
        public DbSet<EntityUserGroupMembership> UserGroupMemberships { get; set; }
        public DbSet<EntityUserGroupImages> UserGroupImages { get; set; }
        public DbSet<EntityUserGroupComputerGroups> UserGroupComputerGroups { get; set; }
        public DbSet<EntityToemsUsersGroups> ToemsUsersGroups { get; set; }
        public DbSet<EntityToemsUsersImages> ToemsUsersImages { get; set; }
        public DbSet<EntityImageReplicationServer> ImageReplicationServers { get; set; }
        public DbSet<EntityDefaultImageReplicationServer> DefaultImageReplicationServers { get; set; }
        public DbSet<EntityWieBuild> WieBuilds { get; set; }
        public DbSet<EntityWingetManifestDownload> WingetManifestDownloads { get; set; }
        public DbSet<EntityWingetInstallerManifest> WingetInstallerManifests { get; set; }
        public DbSet<EntityWingetVersionManifest> WingetVersionManifests { get; set; }
        public DbSet<EntityWingetLocaleManifest> WingetLocaleManifests { get; set; }
        public DbSet<EntityWingetModule> WingetModules { get; set; }



    }
}