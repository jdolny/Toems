using System;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using log4net;
using Toems_Common.Entity;

namespace Toems_DataModel
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ToemsDbContext _context = new ToemsDbContext();

        private static readonly ILog Logger =
         LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private IGenericRepository<EntityPrinterModule> _printerModuleRepository; 
        private IGenericRepository<EntityAuditLog> _auditLogRepository;
        private IGenericRepository<EntityVersion> _versionRepository;
      
     
        private IGenericRepository<EntitySetting> _settingRepository;
    
        private IGenericRepository<EntityToemsUserGroup> _userGroupRepository;
        private IGenericRepository<EntityUserGroupRight> _userGroupRightRepository;
       
        private IGenericRepository<EntityUserLockout> _userLockoutRepository;
        private ToemsUserRepository _userRepository;
        private IGenericRepository<EntityUserRight> _userRightRepository;
        private PolicyRepository _policyRepository;
        private IGenericRepository<EntityPolicyModules> _policyModulesRepository;
        private IGenericRepository<EntityUploadedFile> _uploadedFileRepository;
        private IGenericRepository<EntitySoftwareModule> _softwareModuleRepository;
        private GroupPolicyRepository _groupPolicyRepository;
        private IGenericRepository<EntityCommandModule> _commandModuleRepository;
        private IGenericRepository<EntityFileCopyModule> _fileCopyModuleRepository;
        private IGenericRepository<EntityScriptModule> _scriptModuleRepository;
        private ComputerRepository _computerRepository;
        private IGenericRepository<EntityGroupMembership> _groupMembershipRepository;
        private IGenericRepository<EntityActiveGroupPolicy> _activeGroupPoliciesRepository;
        private IGenericRepository<EntityActiveClientPolicy> _activeClientPolicyRepository;
        private IGenericRepository<EntityCertificate> _certificateRepository;

        private IGenericRepository<EntityBiosInventory> _biosInventoryRepository;
        private IGenericRepository<EntityComputerSystemInventory> _computerSystemInventoryRepository;
        private IGenericRepository<EntityHardDriveInventory> _hardDriveInventoryRepository;
        private IGenericRepository<EntityOsInventory> _osInventoryRepository;
        private IGenericRepository<EntityPrinterInventory> _printerInventoryRepository;
        private IGenericRepository<EntityProcessorInventory> _processorInventoryRepository;
        private IGenericRepository<EntitySoftwareInventory> _softwareInventoryRepository;
        private IGenericRepository<EntityWindowsUpdateInventory> _wuInventoryRepository;
        private IGenericRepository<EntityComputerSoftware> _computerSoftwareRepository;
        private IGenericRepository<EntityUserLogin> _userLoginRepository;
        private ModuleRepository _moduleRepository;
        private IGenericRepository<EntitySmartGroupQuery> _smartGroupQueryRepository;
        private PolicyHistoryRepository _policyHistoryRepository;
        private IGenericRepository<EntityCustomInventory> _customInventoryRepository; 
        private GroupRepository _groupRepository;
        private IGenericRepository<EntityImpersonationAccount> _impersonationAccountRepository;
        private IGenericRepository<EntityClientComServer> _clientComServerRepository;
        private IGenericRepository<EntityComServerCluster> _comServerClusterRepository;
        private ComServerClusterServersRepository _comServerClusterServerRepository;
        private IGenericRepository<EntityPinnedPolicy> _pinnedPolicyRepository;
        private IGenericRepository<EntityPolicyHashHistory> _policyHashHistory;
        private IGenericRepository<EntityResetRequest> _resetRequestRepository;
        private IGenericRepository<EntityApprovalRequest> _approvalRequestRepository;
        private IGenericRepository<EntityComputerUpdates> _computerUpdatesRepository;
        private IGenericRepository<EntityNicInventory> _nicInventoryRepository;
        private IGenericRepository<EntitySchedule> _scheduleRepository;
        private IGenericRepository<EntityWolRelay> _wolRelayRepository;
        private IGenericRepository<EntityProcessInventory> _processInventoryRepository;
        private IGenericRepository<EntityComputerProcess> _computerProcessRepository;
        private IGenericRepository<EntityAntivirusInventory> _antivirusRepository;
        private IGenericRepository<EntityFirewallInventory> _firewallRepository;
        private IGenericRepository<EntityBitlockerInventory> _bitlockerInventory;
        private IGenericRepository<EntityLogicalVolumeInventory> _logicalVolumeRepository;
        private IGenericRepository<EntityCurrentDownload> _currentDownloadsRepository;
        private IGenericRepository<EntityWuModule> _windowsUpdateModuleRepository;
        private IGenericRepository<EntityPolicyComServer> _policyComServerRepository;
        private IGenericRepository<EntityCategory> _categoryRepository;
        private IGenericRepository<EntityComputerCategory> _computerCategoryRepository;
        private IGenericRepository<EntityGroupCategory> _groupCategoryRepository;
        private IGenericRepository<EntityPolicyCategory> _policyCategoryRepository;
        private IGenericRepository<EntityModuleCategory> _moduleCategoryRepository;
        private IGenericRepository<EntityExternalDownload> _externalDownloadRepository;
        CustomAttributeRepository _customAttributeRepository;
        private IGenericRepository<EntityCustomComputerAttribute> _customComputerAttributeRepository;
        private IGenericRepository<EntityCustomAssetType> _customAssetTypeRepository;
        private AssetRepository _assetRepository;
        private IGenericRepository<EntityAssetAttribute> _assetAttributeRepository;
        private IGenericRepository<EntityComment> _commentRepository;
        private IGenericRepository<EntityAttachment> _attachmentRepository;
        private IGenericRepository<EntityAssetComment> _assetCommentRepository;
        private AssetAttachmentRepository _assetAttachmentRepository;
        private IGenericRepository<EntityComputerComment> _computerCommentRepository;
        private IGenericRepository<EntityComputerAttachment> _computerAttachmentRepository;
        private IGenericRepository<EntitySoftwareAssetSoftware> _softwareAssetSoftwareRepository;
        private AssetGroupRepository _assetGroupRepository;
        private IGenericRepository<EntityAssetGroupMember> _assetGroupMemberRepository;
        private IGenericRepository<EntityAssetCategory> _assetCategoryRepository;
        private IGenericRepository<EntityPinnedGroup> _pinnedGroupRepository;
        private IGenericRepository<EntityCertificateInventory> _certificateInventoryRepository;
        private IGenericRepository<EntityComputerCertificate> _computerCertificateRepository;
        private IGenericRepository<EntityMessageModule> _messageModuleRepository;
        private IGenericRepository<EntityActiveSocket> _activeSocketRepository;
        private IGenericRepository<EntityCustomBootMenu> _customBootMenuRepository;
        private IGenericRepository<EntitySysprepModule> _sysprepModuleRepository;

        private IGenericRepository<EntityImageProfileTemplate> _imageProfileTemplateRepository;
        private IGenericRepository<EntityImage> _imageRepository;
        private IGenericRepository<EntityImageCategory> _imageCategoryRepository;
        private ImageProfileRepository _imageProfileRepository;
        private IGenericRepository<EntityImageProfileScript> _imageProfileScriptRepository;
        private IGenericRepository<EntityImageProfileSysprepTag> _imageProfileSysprepRepository;
        private IGenericRepository<EntityImageProfileFileCopy> _imageProfileFileCopyRepository;
        private IGenericRepository<EntityActiveMulticastSession> _activeMulticastSessionRepository;
        private ActiveImagingTaskRepository _activeImagingTaskRepository;
        private IGenericRepository<EntityMulticastPort> _multicastPortRepository;
        private IGenericRepository<EntityComputerLog> _computerLogRepository;
        private IGenericRepository<EntityClientImagingId> _clientImagingIdRepository;
        private IGenericRepository<EntitySysprepAnswerfile> _sysprepAnswerFilesRepository;
        private IGenericRepository<EntitySetupCompleteFile> _setupCompleteFilesRepository;
        private IGenericRepository<EntityComputerGpuInventory> _computerGpuRepository;
        private IGenericRepository<EntityToecDeployJob> _toecDeployJobRepository;
        private IGenericRepository<EntityToecTargetList> _toecTargetListRepository;
        private IGenericRepository<EntityToecTargetListComputer> _toecTargetListComputerRepository;
        private IGenericRepository<EntityToecTargetListOu> _toecTargetListOuRepository;
        private IGenericRepository<EntityToecDeployThread> _toecDeployThreadRepository;
        private IGenericRepository<EntityWinPeModule> _winPeModuleRepository;
        private IGenericRepository<EntityUserGroupMembership> _userGroupMembershipRepository;
        private IGenericRepository<EntityUserGroupImages> _userGroupImagesRepository;
        private IGenericRepository<EntityUserGroupComputerGroups> _userGroupComputerGroupsRepository;
        private IGenericRepository<EntityToemsUsersImages> _toemsUsersImagesRepository;
        private IGenericRepository<EntityToemsUsersGroups> _toemsUsersGroupsRepository;
        private IGenericRepository<EntityImageReplicationServer> _imageReplicationServerRepository;
        private IGenericRepository<EntityDefaultImageReplicationServer> _defaultImageReplicationServerRepository;
        private IGenericRepository<EntityWieBuild> _wieBuildRepository;
        private IGenericRepository<EntityWingetManifestDownload> _wingetManifestDownloadRepository;
        private IGenericRepository<EntityWingetInstallerManifest> _wingetInstallerManifestRepository;
        private IGenericRepository<EntityWingetVersionManifest> _wingetVersionManifestRepository;
        private WingetLocaleRepository _wingetLocaleManifestRepository;
        private IGenericRepository<EntityWingetModule> _wingetModuleRepository;
        private IGenericRepository<EntityToemsUserOptions> _toemsUserOptionsRepository;
        private IGenericRepository<EntityBrowserToken> _browserTokenRepository;


        private bool disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        public IGenericRepository<EntityBrowserToken> BrowserTokenRepository => _browserTokenRepository ?? (_browserTokenRepository = new GenericRepository<EntityBrowserToken>(_context));
        public IGenericRepository<EntityToemsUserOptions> ToemsUserOptionsRepository => _toemsUserOptionsRepository ?? (_toemsUserOptionsRepository = new GenericRepository<EntityToemsUserOptions>(_context));

        public IGenericRepository<EntityWingetModule> WingetModuleRepository => _wingetModuleRepository ?? (_wingetModuleRepository = new GenericRepository<EntityWingetModule>(_context));

        public IGenericRepository<EntityWingetInstallerManifest> WingetInstallerManifestRepository
        {
            get
            {
                return _wingetInstallerManifestRepository ?? (_wingetInstallerManifestRepository = new GenericRepository<EntityWingetInstallerManifest>(_context));
            }

        }
        public IGenericRepository<EntityWingetVersionManifest> WingetVersionManifestRepository
        {
            get
            {
                return _wingetVersionManifestRepository ?? (_wingetVersionManifestRepository = new GenericRepository<EntityWingetVersionManifest>(_context));
            }

        }
        public WingetLocaleRepository WingetLocaleManifestRepository
        {
            get
            {
                return _wingetLocaleManifestRepository ?? (_wingetLocaleManifestRepository = new WingetLocaleRepository(_context));
            }

        }

        public IGenericRepository<EntityWingetManifestDownload> WingetManifestDownloadRepository
        {
            get
            {
                return _wingetManifestDownloadRepository ?? (_wingetManifestDownloadRepository = new GenericRepository<EntityWingetManifestDownload>(_context));
            }

        }
        public IGenericRepository<EntityWieBuild> WieBuildRepository
        {
            get
            {
                return _wieBuildRepository ?? (_wieBuildRepository = new GenericRepository<EntityWieBuild>(_context));
            }
        }

        public IGenericRepository<EntityDefaultImageReplicationServer> DefaultImageReplicationServerRepository
        {
            get
            {
                return _defaultImageReplicationServerRepository ?? (_defaultImageReplicationServerRepository = new GenericRepository<EntityDefaultImageReplicationServer>(_context));
            }
        }

        public IGenericRepository<EntityImageReplicationServer> ImageReplicationServerRepository
        {
            get
            {
                return _imageReplicationServerRepository ?? (_imageReplicationServerRepository = new GenericRepository<EntityImageReplicationServer>(_context));
            }
        }
        public IGenericRepository<EntityToemsUsersGroups> ToemsUsersGroupsRepository
        {
            get
            {
                return _toemsUsersGroupsRepository ?? (_toemsUsersGroupsRepository = new GenericRepository<EntityToemsUsersGroups>(_context));
            }
        }
        public IGenericRepository<EntityToemsUsersImages> ToemsUsersImagesRepository
        {
            get
            {
                return _toemsUsersImagesRepository ?? (_toemsUsersImagesRepository = new GenericRepository<EntityToemsUsersImages>(_context));
            }
        }
        public IGenericRepository<EntityUserGroupComputerGroups> UserGroupComputerGroupsRepository
        {
            get
            {
                return _userGroupComputerGroupsRepository ?? (_userGroupComputerGroupsRepository = new GenericRepository<EntityUserGroupComputerGroups>(_context));
            }
        }
        public IGenericRepository<EntityUserGroupImages> UserGroupImagesRepository
        {
            get
            {
                return _userGroupImagesRepository ?? (_userGroupImagesRepository = new GenericRepository<EntityUserGroupImages>(_context));
            }
        }

        public IGenericRepository<EntityUserGroupMembership> UserGroupMembershipRepository
        {
            get
            {
                return _userGroupMembershipRepository ?? (_userGroupMembershipRepository = new GenericRepository<EntityUserGroupMembership>(_context));
            }
        }

        public IGenericRepository<EntityWinPeModule> WinPeModuleRepository
        {
            get
            {
                return _winPeModuleRepository ?? (_winPeModuleRepository = new GenericRepository<EntityWinPeModule>(_context));
            }
        }

        public IGenericRepository<EntityToecDeployThread> ToecDeployThreadRepository
        {
            get
            {
                return _toecDeployThreadRepository ?? (_toecDeployThreadRepository = new GenericRepository<EntityToecDeployThread>(_context));
            }
        }

        public IGenericRepository<EntityToecDeployJob> ToecDeployJobRepository
        {
            get
            {
                return _toecDeployJobRepository ?? (_toecDeployJobRepository = new GenericRepository<EntityToecDeployJob>(_context));
            }
        }

        public IGenericRepository<EntityToecTargetList> ToecTargetListRepository
        {
            get
            {
                return _toecTargetListRepository ?? (_toecTargetListRepository = new GenericRepository<EntityToecTargetList>(_context));
            }
        }

        public IGenericRepository<EntityToecTargetListComputer> ToecTargetListComputerRepository
        {
            get
            {
                return _toecTargetListComputerRepository ?? (_toecTargetListComputerRepository = new GenericRepository<EntityToecTargetListComputer>(_context));
            }
        }

        public IGenericRepository<EntityToecTargetListOu> ToecTargetListOuRepository
        {
            get
            {
                return _toecTargetListOuRepository ?? (_toecTargetListOuRepository = new GenericRepository<EntityToecTargetListOu>(_context));
            }
        }

        public IGenericRepository<EntityComputerGpuInventory> ComputerGpuRepository
        {
            get
            {
                return _computerGpuRepository ?? (_computerGpuRepository = new GenericRepository<EntityComputerGpuInventory>(_context));
            }
        }

        public IGenericRepository<EntitySysprepAnswerfile> SysprepAnswerFileRepository
        {
            get
            {
                return _sysprepAnswerFilesRepository ?? (_sysprepAnswerFilesRepository = new GenericRepository<EntitySysprepAnswerfile>(_context));
            }
        }

        public IGenericRepository<EntitySetupCompleteFile> SetupCompleteFileRepository
        {
            get
            {
                return _setupCompleteFilesRepository ?? (_setupCompleteFilesRepository = new GenericRepository<EntitySetupCompleteFile>(_context));
            }
        }

        public IGenericRepository<EntityClientImagingId> ClientImagingIdRepository
        {
            get
            {
                return _clientImagingIdRepository ?? (_clientImagingIdRepository = new GenericRepository<EntityClientImagingId>(_context));
            }
        }
        public IGenericRepository<EntityComputerLog> ComputerLogRepository
        {
            get
            {
                return _computerLogRepository ?? (_computerLogRepository = new GenericRepository<EntityComputerLog>(_context));
            }
        }

        public IGenericRepository<EntityMulticastPort> MulticastPortRepository
        {
            get
            {
                return _multicastPortRepository ?? (_multicastPortRepository = new GenericRepository<EntityMulticastPort>(_context));
            }
        }

        public IGenericRepository<EntityActiveMulticastSession> ActiveMulticastSessionRepository
        {
            get
            {
                return _activeMulticastSessionRepository ?? (_activeMulticastSessionRepository = new GenericRepository<EntityActiveMulticastSession>(_context));
            }
        }

        public ActiveImagingTaskRepository ActiveImagingTaskRepository
        {
            get
            {
                return _activeImagingTaskRepository ?? (_activeImagingTaskRepository = new ActiveImagingTaskRepository(_context));
            }
        }

        public IGenericRepository<EntityImageProfileFileCopy> ImageProfileFileCopyRepository
        {
            get
            {
                return _imageProfileFileCopyRepository ?? (_imageProfileFileCopyRepository = new GenericRepository<EntityImageProfileFileCopy>(_context));
            }
        }

        public IGenericRepository<EntityImageProfileSysprepTag> ImageProfileSysprepRepository
        {
            get
            {
                return _imageProfileSysprepRepository ?? (_imageProfileSysprepRepository = new GenericRepository<EntityImageProfileSysprepTag>(_context));
            }
        }

        public IGenericRepository<EntityImageProfileScript> ImageProfileScriptRepository
        {
            get
            {
                return _imageProfileScriptRepository ?? (_imageProfileScriptRepository = new GenericRepository<EntityImageProfileScript>(_context));
            }
        }

        public IGenericRepository<EntityImageCategory> ImageCategoryRepository
        {
            get
            {
                return _imageCategoryRepository ?? (_imageCategoryRepository = new GenericRepository<EntityImageCategory>(_context));
            }
        }

        public ImageProfileRepository ImageProfileRepository
        {
            get
            {
                return _imageProfileRepository ?? (_imageProfileRepository = new ImageProfileRepository(_context));
            }
        }

        public IGenericRepository<EntityImage> ImageRepository
        {
            get
            {
                return _imageRepository ?? (_imageRepository = new GenericRepository<EntityImage>(_context));
            }
        }

        public IGenericRepository<EntitySysprepModule> SysprepModuleRepository
        {
            get
            {
                return _sysprepModuleRepository ?? (_sysprepModuleRepository = new GenericRepository<EntitySysprepModule>(_context));
            }
        }

        public IGenericRepository<EntityImageProfileTemplate> ImageProfileTemplateRepository
        {
            get
            {
                return _imageProfileTemplateRepository ?? (_imageProfileTemplateRepository = new GenericRepository<EntityImageProfileTemplate>(_context));
            }
        }

        public IGenericRepository<EntityCustomBootMenu> CustomBootMenuRepository
        {
            get
            {
                return _customBootMenuRepository ?? (_customBootMenuRepository = new GenericRepository<EntityCustomBootMenu>(_context));
            }
        }

        public IGenericRepository<EntityActiveSocket> ActiveSocketRepository
        {
            get
            {
                return _activeSocketRepository ?? (_activeSocketRepository = new GenericRepository<EntityActiveSocket>(_context));
            }
        }

        public IGenericRepository<EntityMessageModule> MessageModuleRepository
        {
            get
            {
                return _messageModuleRepository ?? (_messageModuleRepository = new GenericRepository<EntityMessageModule>(_context));
            }
        }

        public IGenericRepository<EntityCertificateInventory> CertificateInventoryRepository
        {
            get
            {
                return _certificateInventoryRepository ?? (_certificateInventoryRepository = new GenericRepository<EntityCertificateInventory>(_context));
            }
        }

        public IGenericRepository<EntityComputerCertificate> ComputerCertificateRepository
        {
            get
            {
                return _computerCertificateRepository ?? (_computerCertificateRepository = new GenericRepository<EntityComputerCertificate>(_context));
            }
        }

        public IGenericRepository<EntityPinnedGroup> PinnedGroupRepository
        {
            get
            {
                return _pinnedGroupRepository ?? (_pinnedGroupRepository = new GenericRepository<EntityPinnedGroup>(_context));
            }
        }

        public IGenericRepository<EntityAssetCategory> AssetCategoryRepository
        {
            get
            {
                return _assetCategoryRepository ?? (_assetCategoryRepository = new GenericRepository<EntityAssetCategory>(_context));
            }
        }

        public IGenericRepository<EntityAssetGroupMember> AssetGroupMemberRepository
        {
            get
            {
                return _assetGroupMemberRepository ?? (_assetGroupMemberRepository = new GenericRepository<EntityAssetGroupMember>(_context));
            }
        }

        public AssetGroupRepository AssetGroupRepository
        {
            get
            {
                return _assetGroupRepository ?? (_assetGroupRepository = new AssetGroupRepository(_context));
            }
        }

        public IGenericRepository<EntitySoftwareAssetSoftware> SoftwareAssetSoftwareRepository
        {
            get
            {
                return _softwareAssetSoftwareRepository ?? (_softwareAssetSoftwareRepository = new GenericRepository<EntitySoftwareAssetSoftware>(_context));
            }
        }

        public IGenericRepository<EntityAssetComment> AssetCommentRepository
        {
            get
            {
                return _assetCommentRepository ?? (_assetCommentRepository = new GenericRepository<EntityAssetComment>(_context));
            }
        }

        public AssetAttachmentRepository AssetAttachmentRepository
        {
            get
            {
                return _assetAttachmentRepository ?? (_assetAttachmentRepository = new AssetAttachmentRepository(_context));
            }
        }

        public IGenericRepository<EntityComputerComment> ComputerCommentRepository
        {
            get
            {
                return _computerCommentRepository ?? (_computerCommentRepository = new GenericRepository<EntityComputerComment>(_context));
            }
        }

        public IGenericRepository<EntityComputerAttachment> ComputerAttachmentRepository
        {
            get
            {
                return _computerAttachmentRepository ?? (_computerAttachmentRepository = new GenericRepository<EntityComputerAttachment>(_context));
            }
        }

        public IGenericRepository<EntityAttachment> AttachmentRepository
        {
            get
            {
                return _attachmentRepository ?? (_attachmentRepository = new GenericRepository<EntityAttachment>(_context));
            }
        }

        public IGenericRepository<EntityComment> CommentRepository
        {
            get
            {
                return _commentRepository ?? (_commentRepository = new GenericRepository<EntityComment>(_context));
            }
        }

        public IGenericRepository<EntityAssetAttribute> AssetAttributeRepository
        {
            get
            {
                return _assetAttributeRepository ?? (_assetAttributeRepository = new GenericRepository<EntityAssetAttribute>(_context));
            }
        }

        public AssetRepository AssetRepository
        {
            get
            {
                return _assetRepository ?? (_assetRepository = new AssetRepository(_context));
            }
        }

        public IGenericRepository<EntityCustomAssetType> CustomAssetTypeRepository
        {
            get
            {
                return _customAssetTypeRepository ?? (_customAssetTypeRepository = new GenericRepository<EntityCustomAssetType>(_context));
            }
        }

        public IGenericRepository<EntityCustomComputerAttribute> CustomComputerAttributeRepository
        {
            get
            {
                return _customComputerAttributeRepository ?? (_customComputerAttributeRepository = new GenericRepository<EntityCustomComputerAttribute>(_context));
            }
        }

        public CustomAttributeRepository CustomAttributeRepository
        {
            get
            {
                return _customAttributeRepository ?? (_customAttributeRepository = new CustomAttributeRepository(_context));
            }
        }

        public IGenericRepository<EntityExternalDownload> ExternalDownloadRepository
        {
            get
            {
                return _externalDownloadRepository ?? (_externalDownloadRepository = new GenericRepository<EntityExternalDownload>(_context));
            }
        }

        public IGenericRepository<EntityModuleCategory> ModuleCategoryRepository
        {
            get
            {
                return _moduleCategoryRepository ?? (_moduleCategoryRepository = new GenericRepository<EntityModuleCategory>(_context));
            }
        }
        public IGenericRepository<EntityGroupCategory> GroupCategoryRepository
        {
            get
            {
                return _groupCategoryRepository ?? (_groupCategoryRepository = new GenericRepository<EntityGroupCategory>(_context));
            }
        }

        public IGenericRepository<EntityComputerCategory> ComputerCategoryRepository
        {
            get
            {
                return _computerCategoryRepository ?? (_computerCategoryRepository = new GenericRepository<EntityComputerCategory>(_context));
            }
        }
        public IGenericRepository<EntityPolicyCategory> PolicyCategoryRepository
        {
            get
            {
                return _policyCategoryRepository ?? (_policyCategoryRepository = new GenericRepository<EntityPolicyCategory>(_context));
            }
        }

        public IGenericRepository<EntityCategory> CategoryRepository
        {
            get
            {
                return _categoryRepository ?? (_categoryRepository = new GenericRepository<EntityCategory>(_context));
            }
        }
        public IGenericRepository<EntityPolicyComServer> PolicyComServerRepository
        {
            get
            {
                return _policyComServerRepository ?? (_policyComServerRepository = new GenericRepository<EntityPolicyComServer>(_context));
            }
        }

        public IGenericRepository<EntityWuModule> WindowsUpdateModuleRepository
        {
            get
            {
                return _windowsUpdateModuleRepository ?? (_windowsUpdateModuleRepository = new GenericRepository<EntityWuModule>(_context));
            }
        }

        public IGenericRepository<EntityCurrentDownload> CurrentDownloadsRepository
        {
            get
            {
                return _currentDownloadsRepository ?? (_currentDownloadsRepository = new GenericRepository<EntityCurrentDownload>(_context));
            }
        }
        public IGenericRepository<EntityLogicalVolumeInventory> LogicalVolumeRepository
        {
            get
            {
                return _logicalVolumeRepository ?? (_logicalVolumeRepository = new GenericRepository<EntityLogicalVolumeInventory>(_context));
            }
        }

        public IGenericRepository<EntityBitlockerInventory> BitlockerRepository
        {
            get
            {
                return _bitlockerInventory ?? (_bitlockerInventory = new GenericRepository<EntityBitlockerInventory>(_context));
            }
        }

        public IGenericRepository<EntityFirewallInventory> FirewallRepository
        {
            get
            {
                return _firewallRepository ?? (_firewallRepository = new GenericRepository<EntityFirewallInventory>(_context));
            }
        }

        public IGenericRepository<EntityAntivirusInventory> AntivirusRepository
        {
            get
            {
                return _antivirusRepository ?? (_antivirusRepository = new GenericRepository<EntityAntivirusInventory>(_context));
            }

        }
        public IGenericRepository<EntityProcessInventory> ProcessInventoryRepository
        {
            get
            {
                return _processInventoryRepository ?? (_processInventoryRepository = new GenericRepository<EntityProcessInventory>(_context));
            }
        }

        public IGenericRepository<EntityComputerProcess> ComputerProcessRepository
        {
            get
            {
                return _computerProcessRepository ?? (_computerProcessRepository = new GenericRepository<EntityComputerProcess>(_context));
            }


        }

        public IGenericRepository<EntityWolRelay> WolRelayRepository
        {
            get
            {
                return _wolRelayRepository ?? (_wolRelayRepository = new GenericRepository<EntityWolRelay>(_context));
            }


        }
        public IGenericRepository<EntitySchedule> ScheduleRepository
        {
            get
            {
                return _scheduleRepository ?? (_scheduleRepository = new GenericRepository<EntitySchedule>(_context));
            }

        }

        public IGenericRepository<EntityNicInventory> NicInventoryRepository
        {
            get
            {
                return _nicInventoryRepository ?? (_nicInventoryRepository = new GenericRepository<EntityNicInventory>(_context));
            }


        }
        public IGenericRepository<EntityComputerUpdates> ComputerUpdatesRepository
        {
            get
            {
                return _computerUpdatesRepository ?? (_computerUpdatesRepository = new GenericRepository<EntityComputerUpdates>(_context));
            }

        }
        public IGenericRepository<EntityApprovalRequest> ApprovalRequestRepository
        {
            get
            {
                return _approvalRequestRepository ?? (_approvalRequestRepository = new GenericRepository<EntityApprovalRequest>(_context));
            }
        }

        public IGenericRepository<EntityResetRequest> ResetRequestRepository
        {
            get
            {
                return _resetRequestRepository ?? (_resetRequestRepository = new GenericRepository<EntityResetRequest>(_context));
            }
        }
        public IGenericRepository<EntityPinnedPolicy> PinnedPolicyRepository
        {
            get
            {
                return _pinnedPolicyRepository ?? (_pinnedPolicyRepository = new GenericRepository<EntityPinnedPolicy>(_context));
            }


        }
        public IGenericRepository<EntityPolicyHashHistory> PolicyHashHistoryRepository
        {
            get
            {
                return _policyHashHistory ?? (_policyHashHistory = new GenericRepository<EntityPolicyHashHistory>(_context));
            }

        }

        public ComServerClusterServersRepository ComServerClusterServerRepository
        {
            get
            {
                return _comServerClusterServerRepository ?? (_comServerClusterServerRepository = new ComServerClusterServersRepository(_context));
            }

        }
        public IGenericRepository<EntityComServerCluster> ComServerClusterRepository
        {
            get
            {
                return _comServerClusterRepository ?? (_comServerClusterRepository = new GenericRepository<EntityComServerCluster>(_context));
            }
        }

        public IGenericRepository<EntityImpersonationAccount> ImpersonationAccountRepository
        {
            get
            {
                return _impersonationAccountRepository ?? (_impersonationAccountRepository = new GenericRepository<EntityImpersonationAccount>(_context));
            }
        }

        public IGenericRepository<EntityClientComServer> ClientComServerRepository
        {
            get
            {
                return _clientComServerRepository ?? (_clientComServerRepository = new GenericRepository<EntityClientComServer>(_context));
            }
        }

        public IGenericRepository<EntityCustomInventory> CustomInventoryRepository
        {
            get
            {
                return _customInventoryRepository ?? (_customInventoryRepository = new GenericRepository<EntityCustomInventory>(_context));
            }
        }

        public PolicyHistoryRepository PolicyHistoryRepository
        {
            get
            {
                return _policyHistoryRepository ?? (_policyHistoryRepository = new PolicyHistoryRepository(_context));
            }
        }

        public IGenericRepository<EntitySmartGroupQuery> SmartGroupQueryRepository
        {
            get
            {
                return _smartGroupQueryRepository ?? (_smartGroupQueryRepository = new GenericRepository<EntitySmartGroupQuery>(_context));
            }
        }
        public ModuleRepository ModuleRepository
        {
            get
            {
                return _moduleRepository ?? (_moduleRepository = new ModuleRepository(_context));
            }
        }

        public IGenericRepository<EntityUserLogin> UserLoginRepository
        {
            get
            {
                return _userLoginRepository ?? (_userLoginRepository = new GenericRepository<EntityUserLogin>(_context));
            }
        }

        public IGenericRepository<EntityComputerSoftware> ComputerSoftwareRepository
        {
            get
            {
                return _computerSoftwareRepository ?? (_computerSoftwareRepository = new GenericRepository<EntityComputerSoftware>(_context));
            }
        }

        public IGenericRepository<EntityWindowsUpdateInventory> WuInventoryRepository
        {
            get
            {
                return _wuInventoryRepository ?? (_wuInventoryRepository = new GenericRepository<EntityWindowsUpdateInventory>(_context));
            }
        }

        public IGenericRepository<EntitySoftwareInventory> SoftwareInventoryRepository
        {
            get
            {
                return _softwareInventoryRepository ?? (_softwareInventoryRepository = new GenericRepository<EntitySoftwareInventory>(_context));
            }

        }
        public IGenericRepository<EntityProcessorInventory> ProcessorInventoryRepository
        {
            get
            {
                return _processorInventoryRepository ?? (_processorInventoryRepository = new GenericRepository<EntityProcessorInventory>(_context));
            }
        }

        public IGenericRepository<EntityPrinterInventory> PrinterInventoryRepository
        {
            get
            {
                return _printerInventoryRepository ?? (_printerInventoryRepository = new GenericRepository<EntityPrinterInventory>(_context));
            }
        }

        public IGenericRepository<EntityOsInventory> OsInventoryRepository
        {
            get
            {
                return _osInventoryRepository ?? (_osInventoryRepository = new GenericRepository<EntityOsInventory>(_context));
            }
        }

        public IGenericRepository<EntityHardDriveInventory> HardDriveInventoryRepository
        {
            get
            {
                return _hardDriveInventoryRepository ?? (_hardDriveInventoryRepository = new GenericRepository<EntityHardDriveInventory>(_context));
            }
        }

        public IGenericRepository<EntityComputerSystemInventory> ComputerSystemInventoryRepository
        {
            get
            {
                return _computerSystemInventoryRepository ?? (_computerSystemInventoryRepository = new GenericRepository<EntityComputerSystemInventory>(_context));
            }
        }

        public IGenericRepository<EntityBiosInventory> BiosInventoryRepository
        {
            get
            {
                return _biosInventoryRepository ?? (_biosInventoryRepository = new GenericRepository<EntityBiosInventory>(_context));
            }
        }

        public IGenericRepository<EntityCertificate> CertificateRepository
        {
            get
            {
                return _certificateRepository ?? (_certificateRepository = new GenericRepository<EntityCertificate>(_context));
            }
        }

        public IGenericRepository<EntityActiveClientPolicy> ActiveClientPolicies
        {
            get
            {
                return _activeClientPolicyRepository ?? (_activeClientPolicyRepository = new GenericRepository<EntityActiveClientPolicy>(_context));
            }
        }

        public IGenericRepository<EntityActiveGroupPolicy> ActiveGroupPoliciesRepository
        {
            get
            {
                return _activeGroupPoliciesRepository ?? (_activeGroupPoliciesRepository = new GenericRepository<EntityActiveGroupPolicy>(_context));
            }
        }

        public IGenericRepository<EntityGroupMembership> GroupMembershipRepository
        {
            get
            {
                return _groupMembershipRepository ?? (_groupMembershipRepository = new GenericRepository<EntityGroupMembership>(_context));
            }
        }

        public ComputerRepository ComputerRepository
        {
            get
            {
                return _computerRepository ?? (_computerRepository = new ComputerRepository(_context));
            }
        }

        public IGenericRepository<EntityScriptModule> ScriptModuleRepository
        {
            get
            {
                return _scriptModuleRepository ?? (_scriptModuleRepository = new GenericRepository<EntityScriptModule>(_context));
            }
        }

        public IGenericRepository<EntityFileCopyModule> FileCopyModuleRepository
        {
            get
            {
                return _fileCopyModuleRepository ?? (_fileCopyModuleRepository = new GenericRepository<EntityFileCopyModule>(_context));
            }
        }

        public IGenericRepository<EntityCommandModule> CommandModuleRepository
        {
            get
            {
                return _commandModuleRepository ?? (_commandModuleRepository = new GenericRepository<EntityCommandModule>(_context));
            }
        }

        public IGenericRepository<EntitySoftwareModule> SoftwareModuleRepository
        {
            get
            {
                return _softwareModuleRepository ?? (_softwareModuleRepository = new GenericRepository<EntitySoftwareModule>(_context));
            }
        }

        public IGenericRepository<EntityUploadedFile> UploadedFileRepository
        {
            get
            {
                return _uploadedFileRepository ?? (_uploadedFileRepository = new GenericRepository<EntityUploadedFile>(_context));
            }
        }

        public GroupRepository GroupRepository
        {
            get
            {
                return _groupRepository ?? (_groupRepository = new GroupRepository(_context));
            }

        }

        public GroupPolicyRepository GroupPolicyRepository
        {
            get { return _groupPolicyRepository ?? (_groupPolicyRepository = new GroupPolicyRepository(_context)); }
        }

        public IGenericRepository<EntityPolicyModules> PolicyModulesRepository
        {
            get
            {
                return _policyModulesRepository ?? (_policyModulesRepository = new GenericRepository<EntityPolicyModules>(_context));
            }

        }
        public PolicyRepository PolicyRepository
        {
            get
            {
                return _policyRepository ?? (_policyRepository = new PolicyRepository(_context));
            }
        }

        public IGenericRepository<EntityPrinterModule> PrinterModuleRepository
        {
            get
            {
                return _printerModuleRepository ?? (_printerModuleRepository = new GenericRepository<EntityPrinterModule>(_context));
            }
        }


        public IGenericRepository<EntityAuditLog> AuditLogRepository
        {
            get
            {
                return _auditLogRepository ?? (_auditLogRepository = new GenericRepository<EntityAuditLog>(_context));
            }
        }

       
        public IGenericRepository<EntityVersion> VersionRepository
        {
            get
            {
                return _versionRepository ?? (_versionRepository = new GenericRepository<EntityVersion>(_context));
            }
        }


        public void Save()
        {
            try
            {
                _context.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var eve in ex.EntityValidationErrors)
                {
                    Logger.Error(
                        string.Format(
                            "{0}: Entity of type \"{1}\" in state \"{2}\" has the following validation errors:",
                            DateTime.Now, eve.Entry.Entity.GetType().Name, eve.Entry.State));
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Logger.Error(string.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage));
                    }
                }
                throw;
            }
            catch (DbUpdateException ex)
            {
                Logger.Error(ex.Message);
                Logger.Error(ex.InnerException);
                throw;
            }
        }

    


        public IGenericRepository<EntitySetting> SettingRepository
        {
            get { return _settingRepository ?? (_settingRepository = new GenericRepository<EntitySetting>(_context)); }
        }

   

     
      

        public IGenericRepository<EntityToemsUserGroup> UserGroupRepository
        {
            get
            {
                return _userGroupRepository ??
                       (_userGroupRepository = new GenericRepository<EntityToemsUserGroup>(_context));
            }
        }

        public IGenericRepository<EntityUserGroupRight> UserGroupRightRepository
        {
            get
            {
                return _userGroupRightRepository ??
                       (_userGroupRightRepository = new GenericRepository<EntityUserGroupRight>(_context));
            }
        }

      

        public IGenericRepository<EntityUserLockout> UserLockoutRepository
        {
            get
            {
                return _userLockoutRepository ??
                       (_userLockoutRepository = new GenericRepository<EntityUserLockout>(_context));
            }
        }

        public ToemsUserRepository UserRepository
        {
            get { return _userRepository ?? (_userRepository = new ToemsUserRepository(_context)); }
        }

        public IGenericRepository<EntityUserRight> UserRightRepository
        {
            get
            {
                return _userRightRepository ?? (_userRightRepository = new GenericRepository<EntityUserRight>(_context));
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }
    }
}