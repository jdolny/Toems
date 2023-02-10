using System;
using Toems_Common.Entity;
namespace Toems_DataModel
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<EntityAuditLog> AuditLogRepository { get; }
        IGenericRepository<EntityPrinterModule> PrinterModuleRepository { get; }
        IGenericRepository<EntityVersion> VersionRepository { get; }
        IGenericRepository<EntitySetting> SettingRepository { get; }
        IGenericRepository<EntityToemsUserGroup> UserGroupRepository { get; }
        IGenericRepository<EntityUserGroupRight> UserGroupRightRepository { get; }
        IGenericRepository<EntityUserLockout> UserLockoutRepository { get; }
        ToemsUserRepository UserRepository { get; }
        IGenericRepository<EntityUserRight> UserRightRepository { get; }
        PolicyRepository PolicyRepository { get; }
        IGenericRepository<EntityPolicyModules> PolicyModulesRepository { get; }
        IGenericRepository<EntityUploadedFile> UploadedFileRepository { get; }
        IGenericRepository<EntitySoftwareModule> SoftwareModuleRepository { get; } 
        GroupPolicyRepository GroupPolicyRepository { get; }
        GroupRepository GroupRepository { get; }
        IGenericRepository<EntityCommandModule> CommandModuleRepository { get; }
        IGenericRepository<EntityFileCopyModule> FileCopyModuleRepository { get;  }
        IGenericRepository<EntityScriptModule> ScriptModuleRepository { get;  }
        ComputerRepository ComputerRepository { get; }
        IGenericRepository<EntityGroupMembership> GroupMembershipRepository { get; }
        IGenericRepository<EntityActiveGroupPolicy> ActiveGroupPoliciesRepository { get; }
        IGenericRepository<EntityActiveClientPolicy> ActiveClientPolicies { get; }
        IGenericRepository<EntityCertificate> CertificateRepository { get; }

        IGenericRepository<EntityBiosInventory> BiosInventoryRepository { get; }
        IGenericRepository<EntityComputerSystemInventory> ComputerSystemInventoryRepository { get; }
        IGenericRepository<EntityHardDriveInventory> HardDriveInventoryRepository { get;  }
        IGenericRepository<EntityOsInventory> OsInventoryRepository { get;  }
        IGenericRepository<EntityPrinterInventory> PrinterInventoryRepository { get;  }
        IGenericRepository<EntityProcessorInventory> ProcessorInventoryRepository { get;  }
        IGenericRepository<EntitySoftwareInventory> SoftwareInventoryRepository { get;  }
        IGenericRepository<EntityWindowsUpdateInventory> WuInventoryRepository { get;  }
        IGenericRepository<EntityComputerSoftware> ComputerSoftwareRepository { get;  }
        IGenericRepository<EntityUserLogin> UserLoginRepository { get; }
        ModuleRepository ModuleRepository { get; }
        IGenericRepository<EntitySmartGroupQuery> SmartGroupQueryRepository { get; }
        PolicyHistoryRepository PolicyHistoryRepository { get; }
        IGenericRepository<EntityCustomInventory> CustomInventoryRepository { get; }
        IGenericRepository<EntityImpersonationAccount> ImpersonationAccountRepository { get; }
        IGenericRepository<EntityClientComServer> ClientComServerRepository { get; }
        IGenericRepository<EntityComServerCluster> ComServerClusterRepository { get; }
        ComServerClusterServersRepository ComServerClusterServerRepository { get; }
        IGenericRepository<EntityPinnedPolicy> PinnedPolicyRepository { get; }
        IGenericRepository<EntityPolicyHashHistory> PolicyHashHistoryRepository { get; }
        IGenericRepository<EntityResetRequest> ResetRequestRepository { get; }
        IGenericRepository<EntityApprovalRequest> ApprovalRequestRepository { get; }
        IGenericRepository<EntityComputerUpdates> ComputerUpdatesRepository { get; }
        IGenericRepository<EntityNicInventory> NicInventoryRepository { get; }
        IGenericRepository<EntitySchedule> ScheduleRepository { get; }
        IGenericRepository<EntityWolRelay> WolRelayRepository { get; }
        IGenericRepository<EntityProcessInventory> ProcessInventoryRepository { get; }
        IGenericRepository<EntityComputerProcess> ComputerProcessRepository { get; }
        IGenericRepository<EntityAntivirusInventory> AntivirusRepository { get; }
        IGenericRepository<EntityFirewallInventory> FirewallRepository { get; }
        IGenericRepository<EntityBitlockerInventory> BitlockerRepository { get; }
        IGenericRepository<EntityLogicalVolumeInventory> LogicalVolumeRepository { get; } 
        IGenericRepository<EntityCurrentDownload> CurrentDownloadsRepository { get; }
        IGenericRepository<EntityWuModule> WindowsUpdateModuleRepository { get; }
        IGenericRepository<EntityPolicyComServer> PolicyComServerRepository { get; }
        IGenericRepository<EntityCategory> CategoryRepository { get; }
        IGenericRepository<EntityComputerCategory> ComputerCategoryRepository { get; }
        IGenericRepository<EntityGroupCategory> GroupCategoryRepository { get; }
        IGenericRepository<EntityPolicyCategory> PolicyCategoryRepository { get; }
        IGenericRepository<EntityModuleCategory> ModuleCategoryRepository { get; }
        IGenericRepository<EntityExternalDownload> ExternalDownloadRepository { get; }
        CustomAttributeRepository CustomAttributeRepository { get; }
        IGenericRepository<EntityCustomComputerAttribute> CustomComputerAttributeRepository { get; }
        IGenericRepository<EntityCustomAssetType> CustomAssetTypeRepository { get; }
        AssetRepository AssetRepository { get;  }
        IGenericRepository<EntityAssetAttribute> AssetAttributeRepository { get; }
        IGenericRepository<EntityComment> CommentRepository { get; }
        IGenericRepository<EntityAttachment> AttachmentRepository { get; }
        IGenericRepository<EntityAssetComment> AssetCommentRepository { get; }
        AssetAttachmentRepository AssetAttachmentRepository { get; }
        IGenericRepository<EntityComputerComment> ComputerCommentRepository { get; }
        IGenericRepository<EntityComputerAttachment> ComputerAttachmentRepository { get; }
        IGenericRepository<EntitySoftwareAssetSoftware> SoftwareAssetSoftwareRepository { get; }
        AssetGroupRepository AssetGroupRepository { get; }
        IGenericRepository<EntityAssetGroupMember> AssetGroupMemberRepository { get; }
        IGenericRepository<EntityAssetCategory> AssetCategoryRepository { get; }
        IGenericRepository<EntityPinnedGroup> PinnedGroupRepository { get; }
        IGenericRepository<EntityCertificateInventory> CertificateInventoryRepository { get; }
        IGenericRepository<EntityComputerCertificate> ComputerCertificateRepository { get; }
        IGenericRepository<EntityMessageModule> MessageModuleRepository { get; }
        IGenericRepository<EntityActiveSocket> ActiveSocketRepository { get; }
        IGenericRepository<EntityCustomBootMenu> CustomBootMenuRepository { get; }
        IGenericRepository<EntityImageProfileTemplate> ImageProfileTemplateRepository { get; }
        IGenericRepository<EntitySysprepModule> SysprepModuleRepository { get; }
        IGenericRepository<EntityImage> ImageRepository { get; }
        ImageProfileRepository ImageProfileRepository { get; }
        IGenericRepository<EntityImageCategory> ImageCategoryRepository { get; }
        IGenericRepository<EntityImageProfileScript> ImageProfileScriptRepository { get; }
        IGenericRepository<EntityImageProfileSysprepTag> ImageProfileSysprepRepository { get; }
        IGenericRepository<EntityImageProfileFileCopy> ImageProfileFileCopyRepository { get; }
        ActiveImagingTaskRepository ActiveImagingTaskRepository { get; }
        IGenericRepository<EntityActiveMulticastSession> ActiveMulticastSessionRepository { get; }
        IGenericRepository<EntityMulticastPort> MulticastPortRepository { get; }
        IGenericRepository<EntityComputerLog> ComputerLogRepository { get; }
        IGenericRepository<EntityClientImagingId> ClientImagingIdRepository { get; }

        IGenericRepository<EntitySysprepAnswerfile> SysprepAnswerFileRepository { get; }
        IGenericRepository<EntitySetupCompleteFile> SetupCompleteFileRepository { get; }

        IGenericRepository<EntityComputerGpuInventory> ComputerGpuRepository { get; }
        IGenericRepository<EntityToecDeployJob> ToecDeployJobRepository { get; }
        IGenericRepository<EntityToecTargetList> ToecTargetListRepository { get; }
        IGenericRepository<EntityToecTargetListComputer> ToecTargetListComputerRepository { get; }
        IGenericRepository<EntityToecTargetListOu> ToecTargetListOuRepository { get; }

        IGenericRepository<EntityToecDeployThread> ToecDeployThreadRepository { get; }
        IGenericRepository<EntityWinPeModule> WinPeModuleRepository { get; }

        IGenericRepository<EntityUserGroupMembership> UserGroupMembershipRepository { get; }
        IGenericRepository<EntityUserGroupImages> UserGroupImagesRepository { get; }
        IGenericRepository<EntityUserGroupComputerGroups> UserGroupComputerGroupsRepository { get; }
        IGenericRepository<EntityToemsUsersImages> ToemsUsersImagesRepository { get; }
        IGenericRepository<EntityToemsUsersGroups> ToemsUsersGroupsRepository { get; }
        void Save();
    }
}