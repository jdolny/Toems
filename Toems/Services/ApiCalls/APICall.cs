
namespace Toems_ApiCalls
{
    public class APICall(ApiRequest apiRequest)
    {
        public ComputerAPI ComputerApi => new("Computer", apiRequest);
        public CategoryAPI CategoryApi => new("Category",apiRequest);
        public AttachmentAPI AttachmentApi => new("Attachment",apiRequest);
        public BrowserTokenAPI BrowserTokenApi => new("BrowserToken",apiRequest);
        public CustomAttributeAPI CustomAttributeApi => new("CustomAttribute",apiRequest);
        public CustomComputerAttributeAPI CustomComputerAttributeApi => new("CustomComputerAttribute",apiRequest);
        public ClientComServerAPI ClientComServerApi => new("ClientComServer",apiRequest);
        public ComputerLogAPI ComputerLogApi => new("ComputerLog",apiRequest);
        public ImageAPI ImageApi => new("Image",apiRequest);
        public WinPeModuleAPI WinPeModuleApi => new("WinPeModule",apiRequest);
        public PolicyAPI PolicyApi => new("Policy",apiRequest);
        public RemoteAccessApi RemoteAccessApi => new("RemoteAccess",apiRequest);
        public ImageProfileAPI ImageProfileApi => new("ImageProfile",apiRequest);
        public ImageCategoryAPI ImageCategoryApi => new("ImageCategory",apiRequest);
        public ImageSchemaAPI ImageSchemaApi => new("ImageSchema",apiRequest);
        /*
        public MessageModuleAPI MessageModuleApi
        {
            get { return new MessageModuleAPI("MessageModule",_protectedSessionStorage); }
        }

        public CertificateInventoryAPI CertificateInventoryApi
        {
            get { return new CertificateInventoryAPI("CertificateInventory",_protectedSessionStorage); }
        }

        public PinnedGroupAPI PinnedGroupApi
        {
            get { return new PinnedGroupAPI("PinnedGroup",_protectedSessionStorage);}
        }

        public AssetCategoryAPI AssetCategoryApi
        {
            get { return new AssetCategoryAPI("AssetCategory",_protectedSessionStorage);}
        }

        public AssetGroupMemberAPI AssetGroupMemberApi
        {
            get { return new AssetGroupMemberAPI("AssetGroupMember",_protectedSessionStorage);}
        }

        public AssetGroupAPI AssetGroupApi
        {
            get { return new AssetGroupAPI("AssetGroup",_protectedSessionStorage);}
        }

        public SoftwareAssetSoftwareAPI SoftwareAssetSoftwareApi
        {
            get { return new SoftwareAssetSoftwareAPI("SoftwareAssetSoftware",_protectedSessionStorage);}
        }

       

        public AssetAttributeAPI AssetAttributeApi
        {
            get { return new AssetAttributeAPI("AssetAttribute",_protectedSessionStorage);}
        }

        public AssetAPI AssetApi
        {
            get { return new AssetAPI("Asset",_protectedSessionStorage);}
        }

        public CustomAssetTypeAPI CustomAssetTypeApi
        {
            get { return new CustomAssetTypeAPI("CustomAssetType",_protectedSessionStorage);}
        }

      

       

        public ExternalDownloadAPI ExternalDownloadApi
        {
            get { return new ExternalDownloadAPI("ExternalDownload",_protectedSessionStorage);}
        }

        public GroupCategoryAPI GroupCategoryApi
        {
            get { return new GroupCategoryAPI("GroupCategory",_protectedSessionStorage); }
        }

        public ModuleCategoryAPI ModuleCategoryApi
        {
            get { return new ModuleCategoryAPI("ModuleCategory",_protectedSessionStorage); }
        }

        public WolRelayAPI WolRelayApi
        {
            get { return new WolRelayAPI("WolRelay",_protectedSessionStorage);}
        }

        public ReportAPI ReportApi
        {
            get { return new ReportAPI("Report",_protectedSessionStorage);}
        }

        public ApprovalRequestAPI ApprovalRequestApi
        {
            get { return new ApprovalRequestAPI("ApprovalRequest",_protectedSessionStorage); }

        }
        public ResetRequestAPI ResetRequestApi
        {
            get { return new ResetRequestAPI("ResetRequest",_protectedSessionStorage);}
        }

        public PinnedPolicyAPI PinnedPolicyApi
        {
            get { return new PinnedPolicyAPI("PinnedPolicy",_protectedSessionStorage);}
        }

        public ComClusterServerAPI ComClusterServerApi
        {
            get { return new ComClusterServerAPI("ComClusterServer",_protectedSessionStorage);}
        }

        public ComServerClusterAPI ComServerClusterApi
        {
            get { return new ComServerClusterAPI("ComServerCluster",_protectedSessionStorage);}
        }

        public ImpersonationAccountAPI ImpersonationAccountApi
        {
            get { return new ImpersonationAccountAPI("ImpersonationAccount",_protectedSessionStorage); }
        }

        public ClientComServerAPI ClientComServerApi
        {
            get { return new ClientComServerAPI("ClientComServer",_protectedSessionStorage);}
        }

        public ModuleAPI ModuleApi
        {
            get { return new ModuleAPI("Module",_protectedSessionStorage); }
        }

        public GroupMembershipAPI GroupMembershipApi
        {
            get { return new GroupMembershipAPI("GroupMembership",_protectedSessionStorage);}
        }

        public UserGroupMembershipAPI UserGroupMembershipApi
        {
            get { return new UserGroupMembershipAPI("UserGroupMembership",_protectedSessionStorage); }
        }

      
        public CommandModuleAPI CommandModuleApi
        {
            get { return new CommandModuleAPI("CommandModule",_protectedSessionStorage); }
        }

        public FileCopyModuleAPI FileCopyModuleApi
        {
            get { return new FileCopyModuleAPI("FileCopyModule",_protectedSessionStorage); }
        }

      

        public ScriptModuleAPI ScriptModuleApi
        {
            get { return new ScriptModuleAPI("ScriptModule",_protectedSessionStorage); }
        }

       

        public PolicyCategoryAPI PolicyCategoryApi
        {
            get { return new PolicyCategoryAPI("PolicyCategory",_protectedSessionStorage);}
        }

        public UploadedFileAPI UploadedFileApi
        {
            get { return new UploadedFileAPI("UploadedFile",_protectedSessionStorage);}
        }

        public SoftwareModuleAPI SoftwareModuleApi
        {
            get { return new SoftwareModuleAPI("SoftwareModule",_protectedSessionStorage); }
        }

        public PolicyComServerAPI PolicyComServerApi
        {
            get { return new PolicyComServerAPI("PolicyComServer",_protectedSessionStorage);}
        }

        public WuModuleAPI WuModuleApi
        {
            get { return new WuModuleAPI("WuModule",_protectedSessionStorage);}
        }

        public GroupAPI GroupApi
        {
            get { return new GroupAPI("Group",_protectedSessionStorage); }
        }

        public GroupPolicyAPI GroupPolicyApi
        {
            get { return new GroupPolicyAPI("GroupPolicy",_protectedSessionStorage); }
        }


        public PolicyModulesAPI PolicyModulesApi
        {
            get { return new PolicyModulesAPI("PolicyModules",_protectedSessionStorage); }
        }

     

        public PrinterModuleAPI PrinterModuleApi
        {
            get { return new PrinterModuleAPI("PrinterModule",_protectedSessionStorage); }
        }
      

        public AuthorizationAPI AuthorizationApi
        {
            get { return new AuthorizationAPI("Authorization",_protectedSessionStorage); }
        }


        public FilesystemAPI FilesystemApi
        {
            get { return new FilesystemAPI("FileSystem",_protectedSessionStorage); }
        }

       

        public VersionAPI VersionApi
        {
            get { return new VersionAPI("Version",_protectedSessionStorage); }
        }

        public UserAPI ToemsUserApi
        {
            get { return new UserAPI("User",_protectedSessionStorage); }
        }

        public SmartGroupQueryAPI SmartGroupQueryApi
        {
            get { return new SmartGroupQueryAPI("SmartGroupQuery",_protectedSessionStorage);}
        }

        public HangfireTriggerAPI HangfireTriggerApi
        {
            get { return new HangfireTriggerAPI("HangfireTrigger",_protectedSessionStorage); }
        }

        public CustomBootMenuAPI CustomBootMenuApi
        {
            get { return new CustomBootMenuAPI("CustomBootMenu",_protectedSessionStorage); }
        }

        public ImageProfileTemplateAPI ImageProfileTemplateApi
        {
            get { return new ImageProfileTemplateAPI("ImageProfileTemplate",_protectedSessionStorage); }
        }


        public SettingAPI SettingApi
        {
            get { return new SettingAPI("Setting",_protectedSessionStorage); }
        }


        public ProcessInventoryAPI ProcessInventoryApi
        {
            get { return new ProcessInventoryAPI("ProcessInventory",_protectedSessionStorage);}
        }

        public SoftwareInventoryAPI SoftwareInventoryApi
        {
            get { return new SoftwareInventoryAPI("SoftwareInventory",_protectedSessionStorage);}
        }

        public TokenApi TokenApi
        {
            get { return _cApiDto != null ? new TokenApi(_cApiDto.BaseUrl, "Token") : new TokenApi("Token"); }
        }

        public UserGroupAPI UserGroupApi
        {
            get { return new UserGroupAPI("UserGroup",_protectedSessionStorage); }
        }

        public ClientAPI ClientApi
        {
            get { return new ClientAPI("Push");}
        }


        public ScheduleAPI ScheduleApi
        {
            get { return new ScheduleAPI("Schedule",_protectedSessionStorage);}
        }
   

        public UserGroupRightAPI UserGroupRightApi
        {
            get { return new UserGroupRightAPI("UserGroupRight",_protectedSessionStorage); }
        }

      

        public UserRightAPI UserRightApi
        {
            get { return new UserRightAPI("UserRight",_protectedSessionStorage); }
        }

        public OnlineKernelAPI OnlineKernelApi
        {
            get { return new OnlineKernelAPI("OnlineKernel",_protectedSessionStorage); }
        }

        public SysprepModuleAPI SysprepModuleApi
        {
            get { return new SysprepModuleAPI("SysprepModule",_protectedSessionStorage); }
        }


       

   

     

       

        public ImageProfileScriptAPI ImageProfileScriptApi
        {
            get { return new ImageProfileScriptAPI("ImageProfileScript",_protectedSessionStorage); }
        }

        public ImageProfileSysprepAPI ImageProfileSysprepApi
        {
            get { return new ImageProfileSysprepAPI("ImageProfileSysprep",_protectedSessionStorage); }
        }

        public ImageProfileFileCopyAPI ImageProfileFileCopyApi
        {
            get { return new ImageProfileFileCopyAPI("ImageProfileFileCopy",_protectedSessionStorage); }
        }

        public ActiveImagingTaskAPI ActiveImagingTaskApi
        {
            get { return new ActiveImagingTaskAPI("ActiveImagingTask",_protectedSessionStorage); }
        }

        public ActiveMulticastSessionAPI ActiveMulticastSessionApi
        {
            get { return new ActiveMulticastSessionAPI("ActiveMulticastSession",_protectedSessionStorage); }
        }

        

       

        public SysprepAnswerFileAPI SysprepAnswerFileApi
        {
            get { return new SysprepAnswerFileAPI("SysprepAnswerFile",_protectedSessionStorage); }
        }

        public SetupCompleteFileAPI SetupCompleteFileApi
        {
            get { return new SetupCompleteFileAPI("SetupCompleteFile",_protectedSessionStorage); }
        }
        public ToecDeployJobAPI ToecDeployJobApi
        {
            get { return new ToecDeployJobAPI("ToecDeployJob",_protectedSessionStorage); }
        }
        public ToecDeployTargetListAPI ToecTargetListApi
        {
            get { return new ToecDeployTargetListAPI("ToecDeployTargetList",_protectedSessionStorage); }
        }

        public WieBuildAPI WieBuildApi
        {
            get { return new WieBuildAPI("WieBuild",_protectedSessionStorage); }
        }

        public WingetModuleAPI WingetModuleApi
        {
            get { return new WingetModuleAPI("WingetModule",_protectedSessionStorage); }
        }*/

    }
}