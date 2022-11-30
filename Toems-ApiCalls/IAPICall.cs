namespace Toems_ApiCalls
{
    public interface IAPICall
    {
        GroupAPI GroupApi { get; }
        GroupPolicyAPI GroupPolicyApi { get; }
        PolicyModulesAPI PolicyModulesApi { get; }
        PrinterModuleAPI PrinterModuleApi { get; }
        AuthorizationAPI AuthorizationApi { get; }
        PolicyAPI PolicyApi { get; }
        VersionAPI VersionApi { get; }
        UserAPI ToemsUserApi { get; }
        SoftwareModuleAPI SoftwareModuleApi { get; }
        UploadedFileAPI UploadedFileApi { get; }
        CommandModuleAPI CommandModuleApi { get; }
        FileCopyModuleAPI FileCopyModuleApi { get; }
        ScriptModuleAPI ScriptModuleApi { get; }
        SettingAPI SettingApi { get; }
        ComputerAPI ComputerApi { get; }
        TokenApi TokenApi { get; }
        UserGroupAPI UserGroupApi { get; }
        GroupMembershipAPI GroupMembershipApi { get; }
        WuModuleAPI WuModuleApi { get; }
        UserGroupRightAPI UserGroupRightApi { get; }
        GroupCategoryAPI GroupCategoryApi { get; }
        UserRightAPI UserRightApi { get; }
        FilesystemAPI FilesystemApi { get; }
        SmartGroupQueryAPI SmartGroupQueryApi { get; }
        ModuleAPI ModuleApi { get; }
        ClientComServerAPI ClientComServerApi { get; }
        ImpersonationAccountAPI ImpersonationAccountApi { get; }
        ComServerClusterAPI ComServerClusterApi { get; }
        ComClusterServerAPI ComClusterServerApi { get; }
        PinnedPolicyAPI PinnedPolicyApi { get; }
        ResetRequestAPI ResetRequestApi { get; }
        ApprovalRequestAPI ApprovalRequestApi { get; }
        ScheduleAPI ScheduleApi { get; }
        WolRelayAPI WolRelayApi { get; }
        ProcessInventoryAPI ProcessInventoryApi { get; }
        SoftwareInventoryAPI SoftwareInventoryApi { get; }
        PolicyComServerAPI PolicyComServerApi { get; }
        CategoryAPI CategoryApi { get; }
        PolicyCategoryAPI PolicyCategoryApi { get; }
        ModuleCategoryAPI ModuleCategoryApi { get; }
        ExternalDownloadAPI ExternalDownloadApi { get; }
        CustomAttributeAPI CustomAttributeApi { get; }
        CustomComputerAttributeAPI CustomComputerAttributeApi { get; }
        CustomAssetTypeAPI CustomAssetTypeApi { get; }
        AssetAPI AssetApi { get; }
        AssetAttributeAPI AssetAttributeApi { get; }
        AttachmentAPI AttachmentApi { get; }
        SoftwareAssetSoftwareAPI SoftwareAssetSoftwareApi { get; }
        AssetGroupAPI AssetGroupApi { get; }
        AssetGroupMemberAPI AssetGroupMemberApi { get; }
        AssetCategoryAPI AssetCategoryApi { get; }
        PinnedGroupAPI PinnedGroupApi { get; }
        CertificateInventoryAPI CertificateInventoryApi { get; }
        MessageModuleAPI MessageModuleApi { get; }
        WinPeModuleAPI WinPeModuleApi { get; }
    }
}