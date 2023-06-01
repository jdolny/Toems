using Toems_Common.Dto;

namespace Toems_ApiCalls
{
    public class APICall : IAPICall
    {
        private readonly DtoCustomApiCall _cApiDto;

        public APICall(DtoCustomApiCall cApi)
        {
            _cApiDto = cApi;
        }

        public APICall()
        {

        }

        public MessageModuleAPI MessageModuleApi
        {
            get { return new MessageModuleAPI("MessageModule"); }
        }

        public CertificateInventoryAPI CertificateInventoryApi
        {
            get { return new CertificateInventoryAPI("CertificateInventory"); }
        }

        public PinnedGroupAPI PinnedGroupApi
        {
            get { return new PinnedGroupAPI("PinnedGroup");}
        }

        public AssetCategoryAPI AssetCategoryApi
        {
            get { return new AssetCategoryAPI("AssetCategory");}
        }

        public AssetGroupMemberAPI AssetGroupMemberApi
        {
            get { return new AssetGroupMemberAPI("AssetGroupMember");}
        }

        public AssetGroupAPI AssetGroupApi
        {
            get { return new AssetGroupAPI("AssetGroup");}
        }

        public SoftwareAssetSoftwareAPI SoftwareAssetSoftwareApi
        {
            get { return new SoftwareAssetSoftwareAPI("SoftwareAssetSoftware");}
        }

        public AttachmentAPI AttachmentApi
        {
            get { return new AttachmentAPI("Attachment");}
        }

        public AssetAttributeAPI AssetAttributeApi
        {
            get { return new AssetAttributeAPI("AssetAttribute");}
        }

        public AssetAPI AssetApi
        {
            get { return new AssetAPI("Asset");}
        }

        public CustomAssetTypeAPI CustomAssetTypeApi
        {
            get { return new CustomAssetTypeAPI("CustomAssetType");}
        }

        public CustomComputerAttributeAPI CustomComputerAttributeApi
        {
            get { return new CustomComputerAttributeAPI("CustomComputerAttribute");}
        }

        public CustomAttributeAPI CustomAttributeApi
        {
            get { return new CustomAttributeAPI("CustomAttribute");}
        }

        public ExternalDownloadAPI ExternalDownloadApi
        {
            get { return new ExternalDownloadAPI("ExternalDownload");}
        }

        public GroupCategoryAPI GroupCategoryApi
        {
            get { return new GroupCategoryAPI("GroupCategory"); }
        }

        public ModuleCategoryAPI ModuleCategoryApi
        {
            get { return new ModuleCategoryAPI("ModuleCategory"); }
        }

        public WolRelayAPI WolRelayApi
        {
            get { return new WolRelayAPI("WolRelay");}
        }

        public ReportAPI ReportApi
        {
            get { return new ReportAPI("Report");}
        }

        public ApprovalRequestAPI ApprovalRequestApi
        {
            get { return new ApprovalRequestAPI("ApprovalRequest"); }

        }
        public ResetRequestAPI ResetRequestApi
        {
            get { return new ResetRequestAPI("ResetRequest");}
        }

        public PinnedPolicyAPI PinnedPolicyApi
        {
            get { return new PinnedPolicyAPI("PinnedPolicy");}
        }

        public ComClusterServerAPI ComClusterServerApi
        {
            get { return new ComClusterServerAPI("ComClusterServer");}
        }

        public ComServerClusterAPI ComServerClusterApi
        {
            get { return new ComServerClusterAPI("ComServerCluster");}
        }

        public ImpersonationAccountAPI ImpersonationAccountApi
        {
            get { return new ImpersonationAccountAPI("ImpersonationAccount"); }
        }

        public ClientComServerAPI ClientComServerApi
        {
            get { return new ClientComServerAPI("ClientComServer");}
        }

        public ModuleAPI ModuleApi
        {
            get { return new ModuleAPI("Module"); }
        }

        public GroupMembershipAPI GroupMembershipApi
        {
            get { return new GroupMembershipAPI("GroupMembership");}
        }

        public UserGroupMembershipAPI UserGroupMembershipApi
        {
            get { return new UserGroupMembershipAPI("UserGroupMembership"); }
        }

        public ComputerAPI ComputerApi
        {
            get { return new ComputerAPI("Computer");}
        }
        public CommandModuleAPI CommandModuleApi
        {
            get { return new CommandModuleAPI("CommandModule"); }
        }

        public FileCopyModuleAPI FileCopyModuleApi
        {
            get { return new FileCopyModuleAPI("FileCopyModule"); }
        }

        public WinPeModuleAPI WinPeModuleApi
        {
            get { return new WinPeModuleAPI("WinPeModule"); }
        }

        public ScriptModuleAPI ScriptModuleApi
        {
            get { return new ScriptModuleAPI("ScriptModule"); }
        }

        public CategoryAPI CategoryApi
        {
            get { return new CategoryAPI("Category");}
        }

        public PolicyCategoryAPI PolicyCategoryApi
        {
            get { return new PolicyCategoryAPI("PolicyCategory");}
        }

        public UploadedFileAPI UploadedFileApi
        {
            get { return new UploadedFileAPI("UploadedFile");}
        }

        public SoftwareModuleAPI SoftwareModuleApi
        {
            get { return new SoftwareModuleAPI("SoftwareModule"); }
        }

        public PolicyComServerAPI PolicyComServerApi
        {
            get { return new PolicyComServerAPI("PolicyComServer");}
        }

        public WuModuleAPI WuModuleApi
        {
            get { return new WuModuleAPI("WuModule");}
        }

        public GroupAPI GroupApi
        {
            get { return new GroupAPI("Group"); }
        }

        public GroupPolicyAPI GroupPolicyApi
        {
            get { return new GroupPolicyAPI("GroupPolicy"); }
        }


        public PolicyModulesAPI PolicyModulesApi
        {
            get { return new PolicyModulesAPI("PolicyModules"); }
        }

        public PolicyAPI PolicyApi
        {
            get { return new PolicyAPI("Policy"); }
        }

        public PrinterModuleAPI PrinterModuleApi
        {
            get { return new PrinterModuleAPI("PrinterModule"); }
        }
      

        public AuthorizationAPI AuthorizationApi
        {
            get { return new AuthorizationAPI("Authorization"); }
        }


        public FilesystemAPI FilesystemApi
        {
            get { return new FilesystemAPI("FileSystem"); }
        }

       

        public VersionAPI VersionApi
        {
            get { return new VersionAPI("Version"); }
        }

        public UserAPI ToemsUserApi
        {
            get { return new UserAPI("User"); }
        }

        public SmartGroupQueryAPI SmartGroupQueryApi
        {
            get { return new SmartGroupQueryAPI("SmartGroupQuery");}
        }

        public HangfireTriggerAPI HangfireTriggerApi
        {
            get { return new HangfireTriggerAPI("HangfireTrigger"); }
        }

        public CustomBootMenuAPI CustomBootMenuApi
        {
            get { return new CustomBootMenuAPI("CustomBootMenu"); }
        }

        public ImageProfileTemplateAPI ImageProfileTemplateApi
        {
            get { return new ImageProfileTemplateAPI("ImageProfileTemplate"); }
        }


        public SettingAPI SettingApi
        {
            get { return new SettingAPI("Setting"); }
        }


        public ProcessInventoryAPI ProcessInventoryApi
        {
            get { return new ProcessInventoryAPI("ProcessInventory");}
        }

        public SoftwareInventoryAPI SoftwareInventoryApi
        {
            get { return new SoftwareInventoryAPI("SoftwareInventory");}
        }

        public TokenApi TokenApi
        {
            get { return _cApiDto != null ? new TokenApi(_cApiDto.BaseUrl, "Token") : new TokenApi("Token"); }
        }

        public UserGroupAPI UserGroupApi
        {
            get { return new UserGroupAPI("UserGroup"); }
        }

        public ClientAPI ClientApi
        {
            get { return new ClientAPI("Push");}
        }


        public ScheduleAPI ScheduleApi
        {
            get { return new ScheduleAPI("Schedule");}
        }
   

        public UserGroupRightAPI UserGroupRightApi
        {
            get { return new UserGroupRightAPI("UserGroupRight"); }
        }

      

        public UserRightAPI UserRightApi
        {
            get { return new UserRightAPI("UserRight"); }
        }

        public OnlineKernelAPI OnlineKernelApi
        {
            get { return new OnlineKernelAPI("OnlineKernel"); }
        }

        public SysprepModuleAPI SysprepModuleApi
        {
            get { return new SysprepModuleAPI("SysprepModule"); }
        }


        public ImageAPI ImageApi
        {
            get { return new ImageAPI("Image"); }
        }

        public ImageProfileAPI ImageProfileApi
        {
            get { return new ImageProfileAPI("ImageProfile"); }
        }

        public ImageSchemaAPI ImageSchemaApi
        {
            get { return new ImageSchemaAPI("ImageSchema"); }
        }

        public ImageCategoryAPI ImageCategoryApi
        {
            get { return new ImageCategoryAPI("ImageCategory"); }
        }

        public ImageProfileScriptAPI ImageProfileScriptApi
        {
            get { return new ImageProfileScriptAPI("ImageProfileScript"); }
        }

        public ImageProfileSysprepAPI ImageProfileSysprepApi
        {
            get { return new ImageProfileSysprepAPI("ImageProfileSysprep"); }
        }

        public ImageProfileFileCopyAPI ImageProfileFileCopyApi
        {
            get { return new ImageProfileFileCopyAPI("ImageProfileFileCopy"); }
        }

        public ActiveImagingTaskAPI ActiveImagingTaskApi
        {
            get { return new ActiveImagingTaskAPI("ActiveImagingTask"); }
        }

        public ActiveMulticastSessionAPI ActiveMulticastSessionApi
        {
            get { return new ActiveMulticastSessionAPI("ActiveMulticastSession"); }
        }

        public ComputerLogAPI ComputerLogApi
        {
            get { return new ComputerLogAPI("ComputerLog"); }
        }

        public RemoteAccessApi RemoteAccessApi
        {
            get { return new RemoteAccessApi("RemoteAccess"); }
        }

        public SysprepAnswerFileAPI SysprepAnswerFileApi
        {
            get { return new SysprepAnswerFileAPI("SysprepAnswerFile"); }
        }

        public SetupCompleteFileAPI SetupCompleteFileApi
        {
            get { return new SetupCompleteFileAPI("SetupCompleteFile"); }
        }
        public ToecDeployJobAPI ToecDeployJobApi
        {
            get { return new ToecDeployJobAPI("ToecDeployJob"); }
        }
        public ToecDeployTargetListAPI ToecTargetListApi
        {
            get { return new ToecDeployTargetListAPI("ToecDeployTargetList"); }
        }

        public WieBuildAPI WieBuildApi
        {
            get { return new WieBuildAPI("WieBuild"); }
        }

    }
}