using log4net;
using Toems_DataModel;
using Toems_ServiceCore.Data;
using Toems_ServiceCore.EntityServices;
using Toems_ServiceCore.NoInjectTemp;
using Toems_ServiceCore.Workflows;

namespace Toems_ServiceCore.Infrastructure;

public class ServiceContext(
    IConfiguration config,
    ILog log,
    UnitOfWork uow,
    IWebHostEnvironment environment,
    IServiceProvider serviceProvider,
    IToemsDbFactory dbFactory
    )
{
    public ILog Log { get; } = log;
    public IConfiguration Config { get; } = config;
    public IWebHostEnvironment Environment { get; } = environment;
    public IToemsDbFactory DbFactory { get; } = dbFactory;
    public UnitOfWork Uow { get; } = uow;
    
    
    
    private ServiceComputer _computer;
    public ServiceComputer Computer => _computer ??= serviceProvider.GetRequiredService<ServiceComputer>();
    
    private ServiceActiveClientPolicy _activeClientPolicy;
    public ServiceActiveClientPolicy ActiveClientPolicy => _activeClientPolicy ??= serviceProvider.GetRequiredService<ServiceActiveClientPolicy>();
    
    private ServiceActiveGroupPolicy _activeGroupPolicy;
    public ServiceActiveGroupPolicy ActiveGroupPolicy => _activeGroupPolicy ??=  serviceProvider.GetRequiredService<ServiceActiveGroupPolicy>();
    
    private ServiceActiveImagingTask _activeImagingTask;
    public ServiceActiveImagingTask ActiveImagingTask => _activeImagingTask ??= serviceProvider.GetRequiredService<ServiceActiveImagingTask>();
    
    private GroupService _group;
    public GroupService Group => _group ??= serviceProvider.GetRequiredService<GroupService>();
    
    private ServiceActiveMulticastSession _activeMulticastSession;
    public ServiceActiveMulticastSession ActiveMulticastSession => _activeMulticastSession ??= serviceProvider.GetRequiredService<ServiceActiveMulticastSession>();
    
    private ServiceActiveSocket _activeSocket;
    public ServiceActiveSocket ActiveSocket => _activeSocket ??= serviceProvider.GetRequiredService<ServiceActiveSocket>();
    
    private ServiceAntivirusInventory _antivirusInventory;
    public ServiceAntivirusInventory AntivirusInventory => _antivirusInventory ??= serviceProvider.GetRequiredService<ServiceAntivirusInventory>();
    
    private ServiceAppMonitor _appMonitor;
    public ServiceAppMonitor AppMonitor => _appMonitor ??= serviceProvider.GetRequiredService<ServiceAppMonitor>();
    
    private ServiceApprovalRequest _approvalRequest;
    public ServiceApprovalRequest ApprovalRequest => _approvalRequest ??= serviceProvider.GetRequiredService<ServiceApprovalRequest>();
    
    private ServiceAttachment _attachment;
    public ServiceAttachment Attachment => _attachment ??= serviceProvider.GetRequiredService<ServiceAttachment>();
    
    private ServiceAuditLog _auditLog;
    public ServiceAuditLog AuditLog => _auditLog ??= serviceProvider.GetRequiredService<ServiceAuditLog>();
    
    private ServiceBiosInventory _biosInventory;
    public ServiceBiosInventory BiosInventory => _biosInventory ??= serviceProvider.GetRequiredService<ServiceBiosInventory>();
    
    private ServiceBitlockerInventory _bitlockerInventory;
    public ServiceBitlockerInventory BitlockerInventory => _bitlockerInventory ??= serviceProvider.GetRequiredService<ServiceBitlockerInventory>();
    
    private ServiceBrowserToken _browserToken;
    public ServiceBrowserToken BrowserToken => _browserToken ??= serviceProvider.GetRequiredService<ServiceBrowserToken>();
    
    private ServiceCategory _category;
    public ServiceCategory Category => _category ??= serviceProvider.GetRequiredService<ServiceCategory>();
    
    private ServiceCertificate _certificate;
    public ServiceCertificate Certificate => _certificate ??= serviceProvider.GetRequiredService<ServiceCertificate>();
    
    private ServiceCertificateInventory _certificateInventory;
    public ServiceCertificateInventory CertificateInventory => _certificateInventory ??= serviceProvider.GetRequiredService<ServiceCertificateInventory>();
    
    private ServiceClientComServer _clientComServer;
    public ServiceClientComServer ClientComServer => _clientComServer ??= serviceProvider.GetRequiredService<ServiceClientComServer>();
    
    private ServiceComClusterServer _comClusterServer;
    public ServiceComClusterServer ComClusterServer => _comClusterServer ??= serviceProvider.GetRequiredService<ServiceComClusterServer>();
    
    private ServiceCommandModule _commandModule;
    public ServiceCommandModule CommandModule => _commandModule ??= serviceProvider.GetRequiredService<ServiceCommandModule>();
    
    private ServiceComputerAttachment _computerAttachment;
    public ServiceComputerAttachment ComputerAttachment => _computerAttachment ??= serviceProvider.GetRequiredService<ServiceComputerAttachment>();
    
    private ServiceComputerCertificate _computerCertificate;
    public ServiceComputerCertificate ComputerCertificate => _computerCertificate ??= serviceProvider.GetRequiredService<ServiceComputerCertificate>();
    
    private ServiceComputerCustomAttributes _computerCustomAttributes;
    public ServiceComputerCustomAttributes ComputerCustomAttributes => _computerCustomAttributes ??= serviceProvider.GetRequiredService<ServiceComputerCustomAttributes>();
    
    private ServiceComputerGpuInventory _computerGpuInventory;
    public ServiceComputerGpuInventory ComputerGpuInventory => _computerGpuInventory ??= serviceProvider.GetRequiredService<ServiceComputerGpuInventory>();
    
    private ServiceComputerLog _computerLog;
    public ServiceComputerLog ComputerLog => _computerLog ??= serviceProvider.GetRequiredService<ServiceComputerLog>();
    
    private ServiceComputerSoftware _computerSoftware;
    public ServiceComputerSoftware ComputerSoftware => _computerSoftware ??= serviceProvider.GetRequiredService<ServiceComputerSoftware>();
    
    private ServiceComputerSystemInventory _computerSystemInventory;
    public ServiceComputerSystemInventory ComputerSystemInventory => _computerSystemInventory ??= serviceProvider.GetRequiredService<ServiceComputerSystemInventory>();
    
    private ServiceComputerUpdates _computerUpdates;
    public ServiceComputerUpdates ComputerUpdates => _computerUpdates ??= serviceProvider.GetRequiredService<ServiceComputerUpdates>();
    
    private ServiceComServerCluster _comServerCluster;
    public ServiceComServerCluster ComServerCluster => _comServerCluster ??= serviceProvider.GetRequiredService<ServiceComServerCluster>();
    
    private ServiceCurrentDownload _currentDownload;
    public ServiceCurrentDownload CurrentDownload => _currentDownload ??= serviceProvider.GetRequiredService<ServiceCurrentDownload>();
    
    private ServiceCustomAssetType _customAssetType;
    public ServiceCustomAssetType CustomAssetType => _customAssetType ??= serviceProvider.GetRequiredService<ServiceCustomAssetType>();
    
    private ServiceCustomAttribute _customAttribute;
    public ServiceCustomAttribute CustomAttribute => _customAttribute ??= serviceProvider.GetRequiredService<ServiceCustomAttribute>();
    
    private ServiceCustomBootMenu _customBootMenu;
    public ServiceCustomBootMenu CustomBootMenu => _customBootMenu ??= serviceProvider.GetRequiredService<ServiceCustomBootMenu>();
    
    private ServiceDefaultReplicationServer _defaultReplicationServer;
    public ServiceDefaultReplicationServer DefaultReplicationServer => _defaultReplicationServer ??= serviceProvider.GetRequiredService<ServiceDefaultReplicationServer>();
    
    private ServiceExternalDownload _externalDownload;
    public ServiceExternalDownload ExternalDownload => _externalDownload ??= serviceProvider.GetRequiredService<ServiceExternalDownload>();
    
    private ServiceFileCopyModule _fileCopyModule;
    public ServiceFileCopyModule FileCopyModule => _fileCopyModule ??= serviceProvider.GetRequiredService<ServiceFileCopyModule>();
    
    private ServiceFirewallInventory _firewallInventory;
    public ServiceFirewallInventory FirewallInventory => _firewallInventory ??= serviceProvider.GetRequiredService<ServiceFirewallInventory>();
    
    private ServiceGroupCategory _groupCategory;
    public ServiceGroupCategory GroupCategory => _groupCategory ??= serviceProvider.GetRequiredService<ServiceGroupCategory>();
    
    private ServiceGroupMembership _groupMembership;
    public ServiceGroupMembership GroupMembership => _groupMembership ??= serviceProvider.GetRequiredService<ServiceGroupMembership>();
    
    private ServiceGroupPolicy _groupPolicy;
    public ServiceGroupPolicy GroupPolicy => _groupPolicy ??= serviceProvider.GetRequiredService<ServiceGroupPolicy>();
    
    private ServiceHardDriveInventory _hardDriveInventory;
    public ServiceHardDriveInventory HardDriveInventory => _hardDriveInventory ??= serviceProvider.GetRequiredService<ServiceHardDriveInventory>();
    
    private ServiceImage _image;
    public ServiceImage Image => _image ??= serviceProvider.GetRequiredService<ServiceImage>();
    
    private ServiceImageCategory _imageCategory;
    public ServiceImageCategory ImageCategory => _imageCategory ??= serviceProvider.GetRequiredService<ServiceImageCategory>();
    
    private ServiceImageProfile _imageProfile;
    public ServiceImageProfile ImageProfile => _imageProfile ??= serviceProvider.GetRequiredService<ServiceImageProfile>();
    
    private ServiceImageProfileFileCopy _imageProfileFileCopy;
    public ServiceImageProfileFileCopy ImageProfileFileCopy => _imageProfileFileCopy ??= serviceProvider.GetRequiredService<ServiceImageProfileFileCopy>();
    
    private ServiceImageProfileScript _imageProfileScript;
    public ServiceImageProfileScript ImageProfileScript => _imageProfileScript ??= serviceProvider.GetRequiredService<ServiceImageProfileScript>();
    
    private ServiceImageProfileSysprep _imageProfileSysprep;
    public ServiceImageProfileSysprep ImageProfileSysprep => _imageProfileSysprep ??= serviceProvider.GetRequiredService<ServiceImageProfileSysprep>();
    
    private ServiceImageProfileTemplate _imageProfileTemplate;
    public ServiceImageProfileTemplate ImageProfileTemplate => _imageProfileTemplate ??= serviceProvider.GetRequiredService<ServiceImageProfileTemplate>();
    
    private ServiceImageSchemaFE _imageSchemaFE;
    public ServiceImageSchemaFE ImageSchemaFE => _imageSchemaFE ??= serviceProvider.GetRequiredService<ServiceImageSchemaFE>();
    
    private ServiceImagingClientId _imagingClientId;
    public ServiceImagingClientId ImagingClientId => _imagingClientId ??= serviceProvider.GetRequiredService<ServiceImagingClientId>();
    
    private ServiceImpersonationAccount _impersonationAccount;
    public ServiceImpersonationAccount ImpersonationAccount => _impersonationAccount ??= serviceProvider.GetRequiredService<ServiceImpersonationAccount>();
    
    private ServiceLogicalVolumeInventory _logicalVolumeInventory;
    public ServiceLogicalVolumeInventory LogicalVolumeInventory => _logicalVolumeInventory ??= serviceProvider.GetRequiredService<ServiceLogicalVolumeInventory>();
    
    private ServiceManifestDownload _manifestDownload;
    public ServiceManifestDownload ManifestDownload => _manifestDownload ??= serviceProvider.GetRequiredService<ServiceManifestDownload>();
    
    private ServiceMessageModule _messageModule;
    public ServiceMessageModule MessageModule => _messageModule ??= serviceProvider.GetRequiredService<ServiceMessageModule>();
    
    private ServiceModule _module;
    public ServiceModule Module => _module ??= serviceProvider.GetRequiredService<ServiceModule>();
    
    private ServiceModuleCategory _moduleCategory;
    public ServiceModuleCategory ModuleCategory => _moduleCategory ??= serviceProvider.GetRequiredService<ServiceModuleCategory>();
    
    private ServiceNicInventory _nicInventory;
    public ServiceNicInventory NicInventory => _nicInventory ??= serviceProvider.GetRequiredService<ServiceNicInventory>();
    
    private ServiceOsInventory _osInventory;
    public ServiceOsInventory OsInventory => _osInventory ??= serviceProvider.GetRequiredService<ServiceOsInventory>();
    
    private ServicePinnedGroup _pinnedGroup;
    public ServicePinnedGroup PinnedGroup => _pinnedGroup ??= serviceProvider.GetRequiredService<ServicePinnedGroup>();
    
    private ServicePinnedPolicy _pinnedPolicy;
    public ServicePinnedPolicy PinnedPolicy => _pinnedPolicy ??= serviceProvider.GetRequiredService<ServicePinnedPolicy>();
    
    private ServicePolicy _policy;
    public ServicePolicy Policy => _policy ??= serviceProvider.GetRequiredService<ServicePolicy>();
    
    private ServicePolicyCategory _policyCategory;
    public ServicePolicyCategory PolicyCategory => _policyCategory ??= serviceProvider.GetRequiredService<ServicePolicyCategory>();
    
    private ServicePolicyComServer _policyComServer;
    public ServicePolicyComServer PolicyComServer => _policyComServer ??= serviceProvider.GetRequiredService<ServicePolicyComServer>();
    
    private ServicePolicyHistory _policyHistory;
    public ServicePolicyHistory PolicyHistory => _policyHistory ??= serviceProvider.GetRequiredService<ServicePolicyHistory>();
    
    private ServicePolicyModules _policyModules;
    public ServicePolicyModules PolicyModules => _policyModules ??= serviceProvider.GetRequiredService<ServicePolicyModules>();
    
    private ServicePort _port;
    public ServicePort Port => _port ??= serviceProvider.GetRequiredService<ServicePort>();
    
    private ServicePrinterInventory _printerInventory;
    public ServicePrinterInventory PrinterInventory => _printerInventory ??= serviceProvider.GetRequiredService<ServicePrinterInventory>();
    
    private ServicePrinterModule _printerModule;
    public ServicePrinterModule PrinterModule => _printerModule ??= serviceProvider.GetRequiredService<ServicePrinterModule>();
    
    private ServiceProcessorInventory _processorInventory;
    public ServiceProcessorInventory ProcessorInventory => _processorInventory ??= serviceProvider.GetRequiredService<ServiceProcessorInventory>();
    
    private ServiceRemoteAccess _remoteAccess;
    public ServiceRemoteAccess RemoteAccess => _remoteAccess ??= serviceProvider.GetRequiredService<ServiceRemoteAccess>();
    
    private ServiceReport _report;
    public ServiceReport Report => _report ??= serviceProvider.GetRequiredService<ServiceReport>();
    
    private ServiceResetRequest _resetRequest;
    public ServiceResetRequest ResetRequest => _resetRequest ??= serviceProvider.GetRequiredService<ServiceResetRequest>();
    
    private ServiceSchedule _schedule;
    public ServiceSchedule Schedule => _schedule ??= serviceProvider.GetRequiredService<ServiceSchedule>();
    
    private ServiceScriptModule _scriptModule;
    public ServiceScriptModule ScriptModule => _scriptModule ??= serviceProvider.GetRequiredService<ServiceScriptModule>();
    
    private ServiceSetting _setting;
    public ServiceSetting Setting => _setting ??= serviceProvider.GetRequiredService<ServiceSetting>();
    
    private ServiceSetupCompleteFile _setupCompleteFile;
    public ServiceSetupCompleteFile SetupCompleteFile => _setupCompleteFile ??= serviceProvider.GetRequiredService<ServiceSetupCompleteFile>();
    
    private ServiceSmartGroupQuery _smartGroupQuery;
    public ServiceSmartGroupQuery SmartGroupQuery => _smartGroupQuery ??= serviceProvider.GetRequiredService<ServiceSmartGroupQuery>();
    
    private ServiceSoftwareInventory _softwareInventory;
    public ServiceSoftwareInventory SoftwareInventory => _softwareInventory ??= serviceProvider.GetRequiredService<ServiceSoftwareInventory>();
    
    private ServiceSoftwareModule _softwareModule;
    public ServiceSoftwareModule SoftwareModule => _softwareModule ??= serviceProvider.GetRequiredService<ServiceSoftwareModule>();
    
    private ServiceSysprepAnswerFile _sysprepAnswerFile;
    public ServiceSysprepAnswerFile SysprepAnswerFile => _sysprepAnswerFile ??= serviceProvider.GetRequiredService<ServiceSysprepAnswerFile>();
    
    private ServiceSysprepModule _sysprepModule;
    public ServiceSysprepModule SysprepModule => _sysprepModule ??= serviceProvider.GetRequiredService<ServiceSysprepModule>();
    
    private ServiceToecDeployJob _toecDeployJob;
    public ServiceToecDeployJob ToecDeployJob => _toecDeployJob ??= serviceProvider.GetRequiredService<ServiceToecDeployJob>();
    
    private ServiceToecDeployTargetList _toecDeployTargetList;
    public ServiceToecDeployTargetList ToecDeployTargetList => _toecDeployTargetList ??= serviceProvider.GetRequiredService<ServiceToecDeployTargetList>();
    
    private ServiceUploadedFile _uploadedFile;
    public ServiceUploadedFile UploadedFile => _uploadedFile ??= serviceProvider.GetRequiredService<ServiceUploadedFile>();
    
    private ServiceUser _user;
    public ServiceUser User => _user ??= serviceProvider.GetRequiredService<ServiceUser>();
    
    private ServiceUserGroup _userGroup;
    public ServiceUserGroup UserGroup => _userGroup ??= serviceProvider.GetRequiredService<ServiceUserGroup>();
    
    private ServiceUserGroupComputerGroupMembership _userGroupComputerGroupMembership;
    public ServiceUserGroupComputerGroupMembership UserGroupComputerGroupMembership => _userGroupComputerGroupMembership ??= serviceProvider.GetRequiredService<ServiceUserGroupComputerGroupMembership>();
    
    private ServiceUserGroupImagesMembership _userGroupImagesMembership;
    public ServiceUserGroupImagesMembership UserGroupImagesMembership => _userGroupImagesMembership ??= serviceProvider.GetRequiredService<ServiceUserGroupImagesMembership>();
    
    private ServiceUserGroupMembership _userGroupMembership;
    public ServiceUserGroupMembership UserGroupMembership => _userGroupMembership ??= serviceProvider.GetRequiredService<ServiceUserGroupMembership>();
    
    private ServiceUserGroupRight _userGroupRight;
    public ServiceUserGroupRight UserGroupRight => _userGroupRight ??= serviceProvider.GetRequiredService<ServiceUserGroupRight>();
    
    private ServiceUserLockout _userLockout;
    public ServiceUserLockout UserLockout => _userLockout ??= serviceProvider.GetRequiredService<ServiceUserLockout>();
    
    private ServiceUserLogins _userLogins;
    public ServiceUserLogins UserLogins => _userLogins ??= serviceProvider.GetRequiredService<ServiceUserLogins>();
    
    private ServiceUserRight _userRight;
    public ServiceUserRight UserRight => _userRight ??= serviceProvider.GetRequiredService<ServiceUserRight>();
    
    private ServiceVersion _version;
    public ServiceVersion Version => _version ??= serviceProvider.GetRequiredService<ServiceVersion>();
    
    private ServiceWieBuild _wieBuild;
    public ServiceWieBuild WieBuild => _wieBuild ??= serviceProvider.GetRequiredService<ServiceWieBuild>();
    
    private ServiceWingetModule _wingetModule;
    public ServiceWingetModule WingetModule => _wingetModule ??= serviceProvider.GetRequiredService<ServiceWingetModule>();
    
    private ServiceWinPeModule _winPeModule;
    public ServiceWinPeModule WinPeModule => _winPeModule ??= serviceProvider.GetRequiredService<ServiceWinPeModule>();
    
    private ServiceWolRelay _wolRelay;
    public ServiceWolRelay WolRelay => _wolRelay ??= serviceProvider.GetRequiredService<ServiceWolRelay>();
    
    private ServiceWuInventory _wuInventory;
    public ServiceWuInventory WuInventory => _wuInventory ??= serviceProvider.GetRequiredService<ServiceWuInventory>();
    
    private ServiceWuModule _wuModule;
    public ServiceWuModule WuModule => _wuModule ??= serviceProvider.GetRequiredService<ServiceWuModule>();
    
    // Infrastructure services
    private AuthenticationService _authentication;
    public AuthenticationService Authentication => _authentication ??= serviceProvider.GetRequiredService<AuthenticationService>();
    
    private AuthorizationServices _authorization;
    public AuthorizationServices Authorization => _authorization ??= serviceProvider.GetRequiredService<AuthorizationServices>();
    
    private BcdServices _bcd;
    public BcdServices Bcd => _bcd ??= serviceProvider.GetRequiredService<BcdServices>();
    
    private ClientImagingServices _clientImaging;
    public ClientImagingServices ClientImaging => _clientImaging ??= serviceProvider.GetRequiredService<ClientImagingServices>();
    
    private EncryptionServices _encryption;
    public EncryptionServices Encryption => _encryption ??= serviceProvider.GetRequiredService<EncryptionServices>();
    
    private FilesystemServices _filessystem;
    public FilesystemServices Filessystem => _filessystem ??= serviceProvider.GetRequiredService<FilesystemServices>();
    
    private FileUploadServices _fileUpload;
    public FileUploadServices FileUpload => _fileUpload ??= serviceProvider.GetRequiredService<FileUploadServices>();
    
    private IpServices _ip;
    public IpServices Ip => _ip ??= serviceProvider.GetRequiredService<IpServices>();
    
    private LdapServices _ldap;
    public LdapServices Ldap => _ldap ??= serviceProvider.GetRequiredService<LdapServices>();
    
    private MailServices _mail;
    public MailServices Mail => _mail ??= serviceProvider.GetRequiredService<MailServices>();
    
    private PasswordGenerator _passwordGenerator;
    public PasswordGenerator PasswordGenerator => _passwordGenerator ??= serviceProvider.GetRequiredService<PasswordGenerator>();
    
    private ServiceClientPartition _clientPartition;
    public ServiceClientPartition ClientPartition => _clientPartition ??= serviceProvider.GetRequiredService<ServiceClientPartition>();
    
    private ServiceClientPartitionSchema _clientPartitionSchema;
    public ServiceClientPartitionSchema ClientPartitionSchema => _clientPartitionSchema ??= serviceProvider.GetRequiredService<ServiceClientPartitionSchema>();
    
    private ServiceFileDownloader _fileDownloader;
    public ServiceFileDownloader FileDownloader => _fileDownloader ??= serviceProvider.GetRequiredService<ServiceFileDownloader>();
    
    private ServiceFileHash _fileHash;
    public ServiceFileHash FileHash => _fileHash ??= serviceProvider.GetRequiredService<ServiceFileHash>();
    
    private ServiceGenerateCertificate _generateCertificate;
    public ServiceGenerateCertificate GenerateCertificate => _generateCertificate ??= serviceProvider.GetRequiredService<ServiceGenerateCertificate>();
    
    private ServiceImpersonation _impersonation;
    public ServiceImpersonation Impersonation => _impersonation ??= serviceProvider.GetRequiredService<ServiceImpersonation>();
    
    private ServiceMsiUpdater _msiUpdater;
    public ServiceMsiUpdater MsiUpdater => _msiUpdater ??= serviceProvider.GetRequiredService<ServiceMsiUpdater>();
    
    private ServiceOnlineKernel _onlineKernel;
    public ServiceOnlineKernel OnlineKernel => _onlineKernel ??= serviceProvider.GetRequiredService<ServiceOnlineKernel>();
    
    private ServiceProvision _provision;
    public ServiceProvision Provision => _provision ??= serviceProvider.GetRequiredService<ServiceProvision>();
    
    private ServiceProxyDhcp _proxyDhcp;
    public ServiceProxyDhcp ProxyDhcp => _proxyDhcp ??= serviceProvider.GetRequiredService<ServiceProxyDhcp>();
    
    private ServiceRawSql _rawSql;
    public ServiceRawSql RawSql => _rawSql ??= serviceProvider.GetRequiredService<ServiceRawSql>();
    
    private ServiceScManager _scManager;
    public ServiceScManager ScManager => _scManager ??= serviceProvider.GetRequiredService<ServiceScManager>();
    
    private ServiceSymmetricEncryption _symmetricEncryption;
    public ServiceSymmetricEncryption SymmetricEncryption => _symmetricEncryption ??= serviceProvider.GetRequiredService<ServiceSymmetricEncryption>();
    
    private StringManipulationServices _stringManipulation;
    public StringManipulationServices StringManipulation => _stringManipulation ??= serviceProvider.GetRequiredService<StringManipulationServices>();
    
    private ThrottledStream _throttledStream;
    public ThrottledStream ThrottledStream => _throttledStream ??= serviceProvider.GetRequiredService<ThrottledStream>();
    
    private ToemsQrProvider _toemsQrProvider;
    public ToemsQrProvider ToemsQrProvider => _toemsQrProvider ??= serviceProvider.GetRequiredService<ToemsQrProvider>();
    
    private UncServices _unc;
    public UncServices Unc => _unc ??= serviceProvider.GetRequiredService<UncServices>();
    
    
    // Workflows
    private BuildReportSqlQuery _buildReportSqlQuery;
    public BuildReportSqlQuery BuildReportSqlQuery => _buildReportSqlQuery ??= serviceProvider.GetRequiredService<BuildReportSqlQuery>();
    
    private BuildSqlQuery _buildSqlQuery;
    public BuildSqlQuery BuildSqlQuery => _buildSqlQuery ??= serviceProvider.GetRequiredService<BuildSqlQuery>();
    
    private CancelAllImagingTasks _cancelAllImagingTasks;
    public CancelAllImagingTasks CancelAllImagingTasks => _cancelAllImagingTasks ??= serviceProvider.GetRequiredService<CancelAllImagingTasks>();
    
    private CleanTaskBootFiles _cleanTaskBootFiles;
    public CleanTaskBootFiles CleanTaskBootFiles => _cleanTaskBootFiles ??= serviceProvider.GetRequiredService<CleanTaskBootFiles>();
    
    private ClientPartitionScript _clientPartitionScript;
    public ClientPartitionScript ClientPartitionScript => _clientPartitionScript ??= serviceProvider.GetRequiredService<ClientPartitionScript>();
    
    private ClientPolicyJson _clientPolicyJson;
    public ClientPolicyJson ClientPolicyJson => _clientPolicyJson ??= serviceProvider.GetRequiredService<ClientPolicyJson>();
    
    private ComServerFreeSpace _comServerFreeSpace;
    public ComServerFreeSpace ComServerFreeSpace => _comServerFreeSpace ??= serviceProvider.GetRequiredService<ComServerFreeSpace>();
    
    private CopyPxeBinaries _copyPxeBinaries;
    public CopyPxeBinaries CopyPxeBinaries => _copyPxeBinaries ??= serviceProvider.GetRequiredService<CopyPxeBinaries>();
    
    private CreateTaskArguments _createTaskArguments;
    public CreateTaskArguments CreateTaskArguments => _createTaskArguments ??= serviceProvider.GetRequiredService<CreateTaskArguments>();
    
    private DataCleanup _dataCleanup;
    public DataCleanup DataCleanup => _dataCleanup ??= serviceProvider.GetRequiredService<DataCleanup>();
    
    private DbUpdater _dbUpdater;
    public DbUpdater DbUpdater => _dbUpdater ??= serviceProvider.GetRequiredService<DbUpdater>();
    
    private DefaultBootMenu _defaultBootMenu;
    public DefaultBootMenu DefaultBootMenu => _defaultBootMenu ??= serviceProvider.GetRequiredService<DefaultBootMenu>();
    
    private DownloadKernel _downloadKernel;
    public DownloadKernel DownloadKernel => _downloadKernel ??= serviceProvider.GetRequiredService<DownloadKernel>();
    
    private ExportPolicy _exportPolicy;
    public ExportPolicy ExportPolicy => _exportPolicy ??= serviceProvider.GetRequiredService<ExportPolicy>();
    
    private FolderSync _folderSync;
    public FolderSync FolderSync => _folderSync ??= serviceProvider.GetRequiredService<FolderSync>();
    
    private GenerateClientGroupPolicy _generateClientGroupPolicy;
    public GenerateClientGroupPolicy GenerateClientGroupPolicy => _generateClientGroupPolicy ??= serviceProvider.GetRequiredService<GenerateClientGroupPolicy>();
    
    private GenerateWie _generateWie;
    public GenerateWie GenerateWie => _generateWie ??= serviceProvider.GetRequiredService<GenerateWie>();
    
    private GetBestCompImageServer _getBestCompImageServer;
    public GetBestCompImageServer GetBestCompImageServer => _getBestCompImageServer ??= serviceProvider.GetRequiredService<GetBestCompImageServer>();
    
    private GetBootImages _getBootImages;
    public GetBootImages GetBootImages => _getBootImages ??= serviceProvider.GetRequiredService<GetBootImages>();
    
    private GetClientPolicies _getClientPolicies;
    public GetClientPolicies GetClientPolicies => _getClientPolicies ??= serviceProvider.GetRequiredService<GetClientPolicies>();
    
    private GetCompEmServers _getCompEmServers;
    public GetCompEmServers GetCompEmServers => _getCompEmServers ??= serviceProvider.GetRequiredService<GetCompEmServers>();
    
    private GetCompImagingServers _getCompImagingServers;
    public GetCompImagingServers GetCompImagingServers => _getCompImagingServers ??= serviceProvider.GetRequiredService<GetCompImagingServers>();
    
    private GetCompTftpServers _getCompTftpServers;
    public GetCompTftpServers GetCompTftpServers => _getCompTftpServers ??= serviceProvider.GetRequiredService<GetCompTftpServers>();
    
    private GetKernels _getKernels;
    public GetKernels GetKernels => _getKernels ??= serviceProvider.GetRequiredService<GetKernels>();
    
    private GetMulticastServer _getMulticastServer;
    public GetMulticastServer GetMulticastServer => _getMulticastServer ??= serviceProvider.GetRequiredService<GetMulticastServer>();
    
    private ImageSync _imageSync;
    public ImageSync ImageSync => _imageSync ??= serviceProvider.GetRequiredService<ImageSync>();
    
    private ImportPolicy _importPolicy;
    public ImportPolicy ImportPolicy => _importPolicy ??= serviceProvider.GetRequiredService<ImportPolicy>();
    
    private IsoGenerator _isoGenerator;
    public IsoGenerator IsoGenerator => _isoGenerator ??= serviceProvider.GetRequiredService<IsoGenerator>();
    
    private LdapSync _ldapSync;
    public LdapSync LdapSync => _ldapSync ??= serviceProvider.GetRequiredService<LdapSync>();
    
    private Multicast _multicast;
    public Multicast Multicast => _multicast ??= serviceProvider.GetRequiredService<Multicast>();
    
    private MulticastArguments _multicastArguments;
    public MulticastArguments MulticastArguments => _multicastArguments ??= serviceProvider.GetRequiredService<MulticastArguments>();
    
    private PowerManagement _powerManagement;
    public PowerManagement PowerManagement => _powerManagement ??= serviceProvider.GetRequiredService<PowerManagement>();
    
    private ProvisionCompleteTasks _provisionCompleteTasks;
    public ProvisionCompleteTasks ProvisionCompleteTasks => _provisionCompleteTasks ??= serviceProvider.GetRequiredService<ProvisionCompleteTasks>();
    
    private ScheduleRunner _scheduleRunner;
    public ScheduleRunner ScheduleRunner => _scheduleRunner ??= serviceProvider.GetRequiredService<ScheduleRunner>();
    
    private SubmitInventory _submitInventory;
    public SubmitInventory SubmitInventory => _submitInventory ??= serviceProvider.GetRequiredService<SubmitInventory>();
    
    private TaskBootMenu _taskBootMenu;
    public TaskBootMenu TaskBootMenu => _taskBootMenu ??= serviceProvider.GetRequiredService<TaskBootMenu>();
    
    private ToecRemoteInstaller _toecRemoteInstaller;
    public ToecRemoteInstaller ToecRemoteInstaller => _toecRemoteInstaller ??= serviceProvider.GetRequiredService<ToecRemoteInstaller>();
    
    private Unicast _unicast;
    public Unicast Unicast => _unicast ??= serviceProvider.GetRequiredService<Unicast>();
    
    private UploadImage _uploadImage;
    public UploadImage UploadImage => _uploadImage ??= serviceProvider.GetRequiredService<UploadImage>();
    
    private ValidatePolicy _validatePolicy;
    public ValidatePolicy ValidatePolicy => _validatePolicy ??= serviceProvider.GetRequiredService<ValidatePolicy>();
    
    private WinGetManifestImporter _winGetManifestImporter;
    public WinGetManifestImporter WinGetManifestImporter => _winGetManifestImporter ??= serviceProvider.GetRequiredService<WinGetManifestImporter>();
    
    private WolRelayTask _wolRelayTask;
    public WolRelayTask WolRelayTask => _wolRelayTask ??= serviceProvider.GetRequiredService<WolRelayTask>();
    
}