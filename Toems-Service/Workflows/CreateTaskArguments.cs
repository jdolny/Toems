using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_Service.Entity;

namespace Toems_Service.Workflows
{
    public class CreateTaskArguments
    {
        private readonly StringBuilder _activeTaskArguments;
        private readonly EntityComputer _computer;
        private readonly string _direction;
        private readonly ImageProfileWithImage _imageProfile;
        private readonly ServiceImageProfile _imageProfileServices;
        private readonly int _comServerId;
        public CreateTaskArguments(EntityComputer computer, ImageProfileWithImage imageProfile, string direction)
        {
            _computer = computer;
            _imageProfile = imageProfile;
            _direction = direction;
            _activeTaskArguments = new StringBuilder();
            _imageProfileServices = new ServiceImageProfile();
        }

        //used for when multicasting
        public CreateTaskArguments(EntityComputer computer, ImageProfileWithImage imageProfile, string direction, int comServerId)
        {
            _computer = computer;
            _imageProfile = imageProfile;
            _direction = direction;
            _activeTaskArguments = new StringBuilder();
            _imageProfileServices = new ServiceImageProfile();
            _comServerId = comServerId;
        }

        private void AppendString(string value)
        {
            _activeTaskArguments.Append(value);
            _activeTaskArguments.Append(_imageProfile.Image.Environment == "winpe" ? "\r\n" : " ");
        }

        public string Execute(string multicastPort = "")
        {
            var preScripts = "\"";
            var beforeFileScripts = "\"";
            var afterFileScripts = "\"";
            foreach (var script in _imageProfileServices.GetImageProfileScripts(_imageProfile.Id))
            {
                if (script.RunWhen == EnumProfileScript.RunWhen.BeforeImaging)
                    preScripts += script.ScriptModuleId + " ";
                else if (script.RunWhen == EnumProfileScript.RunWhen.BeforeFileCopy)
                    beforeFileScripts += script.ScriptModuleId + " ";
                else if (script.RunWhen == EnumProfileScript.RunWhen.AfterFileCopy)
                    afterFileScripts += script.ScriptModuleId + " ";
            }
            beforeFileScripts += "\"";
            afterFileScripts += "\"";
            preScripts += "\"";

            var sysprepTags = "\"";
            foreach (var sysprepTag in _imageProfileServices.GetImageProfileSysprep(_imageProfile.Id))
                sysprepTags += sysprepTag.SysprepModuleId + " ";

            sysprepTags = sysprepTags.Trim();
            sysprepTags += "\"";

            var areFilesToCopy = _imageProfileServices.GetImageProfileFileCopy(_imageProfile.Id).Any();

            //On demand computer may be null if not registered
            if (_computer != null)
            {
                AppendString("computer_name=" + _computer.Name.Split(':').First());
                //AppendString("computer_id=" + _computer.Id);
            }

            AppendString("image_name=" + _imageProfile.Image.Name);
            AppendString("profile_id=" + _imageProfile.Id);
            AppendString("pre_scripts=" + preScripts);
            AppendString("before_file_scripts=" + beforeFileScripts);
            AppendString("after_file_scripts=" + afterFileScripts);
            AppendString("file_copy=" + areFilesToCopy);
            AppendString("sysprep_tags=" + sysprepTags);
            AppendString("image_type=" + _imageProfile.Image.Type);
            AppendString("set_bootmgr=" + _imageProfile.SetBootmgrFirst);
            AppendString("display_sleep_time=" + ServiceSetting.GetSettingValue(SettingStrings.LieSleepTime));
            if (Convert.ToBoolean(_imageProfile.WebCancel))
                AppendString("web_cancel=true");
            AppendString("task_completed_action=" + "\"" + _imageProfile.TaskCompletedAction + "\"");

            if (ServiceSetting.GetSettingValue(SettingStrings.ImageDirectSmb).Equals("True"))
                AppendString("direct_smb=true");

            if (_direction.Contains("upload"))
            {
                if (Convert.ToBoolean(_imageProfile.RemoveGPT)) AppendString("remove_gpt_structures=true");
                if (Convert.ToBoolean(_imageProfile.SkipShrinkVolumes)) AppendString("skip_shrink_volumes=true");
                if (Convert.ToBoolean(_imageProfile.SkipShrinkLvm)) AppendString("skip_shrink_lvm=true");
                AppendString("compression_algorithm=" + _imageProfile.Compression);
                AppendString("compression_level=-" + _imageProfile.CompressionLevel);
                if (Convert.ToBoolean(_imageProfile.UploadSchemaOnly)) AppendString("upload_schema_only=true");
                if (Convert.ToBoolean(_imageProfile.SimpleUploadSchema)) AppendString("simple_upload_schema=true");

                if (!string.IsNullOrEmpty(_imageProfile.CustomUploadSchema))
                {
                    AppendString("custom_upload_schema=true");
                    SetCustomSchemaUpload();
                }
                if (Convert.ToBoolean(_imageProfile.SkipBitlockerCheck)) AppendString("skip_bitlocker_check=true");
                if (Convert.ToBoolean(_imageProfile.SkipHibernationCheck)) AppendString("skip_hibernation_check=true");
            }
            else // push or multicast
            {
                //check for null in case of on demand
                if (_computer != null)
                {
                    var computerAttributes = new ServiceComputer().GetCustomAttributes(_computer.Id);
                    var serviceAttribute = new ServiceCustomAttribute();
                    foreach(var attribute in computerAttributes)
                    {
                        var compAttrib = serviceAttribute.GetCustomAttribute(attribute.CustomAttributeId);
                        if(!string.IsNullOrEmpty(attribute.Value))
                        {
                            if(compAttrib.ClientImagingAvailable)
                                AppendString( compAttrib.Name + "=" + "\"" + attribute.Value + "\"");
                        }
                    }
                }


                if (Convert.ToBoolean(_imageProfile.ChangeName)) AppendString("change_computer_name=true");
                if (Convert.ToBoolean(_imageProfile.SkipExpandVolumes)) AppendString("skip_expand_volumes=true");
                if (Convert.ToBoolean(_imageProfile.FixBcd)) AppendString("fix_bcd=true");
                if (Convert.ToBoolean(_imageProfile.RandomizeGuids)) AppendString("randomize_guids=true");
                if (Convert.ToBoolean(_imageProfile.ForceStandardLegacy)) AppendString("force_legacy_layout=true");
                if (Convert.ToBoolean(_imageProfile.ForceStandardEfi)) AppendString("force_efi_layout=true");
                if (Convert.ToBoolean(_imageProfile.SkipNvramUpdate)) AppendString("skip_nvram=true");
                if (Convert.ToBoolean(_imageProfile.FixBootloader)) AppendString("fix_bootloader=true");

                if (Convert.ToBoolean(_imageProfile.ForceDynamicPartitions))
                    AppendString("force_dynamic_partitions=true");
                AppendString(SetPartitionMethod());
                if (!string.IsNullOrEmpty(_imageProfile.CustomSchema))
                {
                    AppendString("custom_deploy_schema=true");
                    SetCustomSchemaDeploy();
                }
                if (_direction.Contains("multicast"))
                {
                    var comServer = new ServiceClientComServer().GetServer(_comServerId);
                    if (comServer.DecompressImageOn == "client")
                        AppendString("decompress_multicast_on_client=true");
                    if (string.IsNullOrEmpty(_imageProfile.ReceiverArguments))
                        AppendString("client_receiver_args=" + "\"" + comServer.MulticastReceiverArguments + "\"");
                    else
                    {
                        AppendString("client_receiver_args=" + "\"" + _imageProfile.ReceiverArguments + "\"");
                    }

                    AppendString("multicast_port=" + multicastPort);
                    AppendString("multicast_server_ip=" + comServer.MulticastInterfaceIp);
                }
            }

            return _activeTaskArguments.ToString();
        }

        private void SetCustomSchemaDeploy()
        {
            var imageSchemaRequest = new DtoImageSchemaRequest();
            imageSchemaRequest.imageProfile = _imageProfile;
            imageSchemaRequest.schemaType = "deploy";
            var customSchema = new ServiceImageSchemaFE(imageSchemaRequest).GetImageSchema();
            var customHardDrives = new StringBuilder();
            customHardDrives.Append("custom_hard_drives=\"");

            foreach (var hd in customSchema.HardDrives.Where(x => x.Active && !string.IsNullOrEmpty(x.Destination)))
                customHardDrives.Append(hd.Destination + " ");

            customHardDrives.Append("\"");
            AppendString(customHardDrives.ToString());
        }

        private void SetCustomSchemaUpload()
        {
            var imageSchemaRequest = new DtoImageSchemaRequest();
            imageSchemaRequest.imageProfile = _imageProfile;
            imageSchemaRequest.schemaType = "upload";
            var customSchema = new ServiceImageSchemaFE(imageSchemaRequest).GetImageSchema();
            var customHardDrives = new StringBuilder();
            customHardDrives.Append("custom_hard_drives=\"");
            var customPartitions = new StringBuilder();
            customPartitions.Append("custom_partitions=\"");
            var customFixedPartitions = new StringBuilder();
            customFixedPartitions.Append("custom_fixed_partitions=\"");
            var customLogicalVolumes = new StringBuilder();
            customLogicalVolumes.Append("custom_logical_volumes=\"");
            var customFixedLogicalVolumes = new StringBuilder();
            customFixedLogicalVolumes.Append("custom_fixed_logical_volumes=\"");
            foreach (var hd in customSchema.HardDrives.Where(x => x.Active))
            {
                customHardDrives.Append(hd.Name + " ");
                foreach (var partition in hd.Partitions.Where(x => x.Active))
                {
                    customPartitions.Append(hd.Name + partition.Prefix + partition.Number + " ");
                    if (partition.ForceFixedSize)
                        customFixedPartitions.Append(hd.Name + partition.Prefix + partition.Number + " ");

                    if (partition.VolumeGroup.LogicalVolumes != null)
                    {
                        foreach (
                            var logicalVolume in partition.VolumeGroup.LogicalVolumes.Where(x => x.Active))
                        {
                            var vgName = partition.VolumeGroup.Name.Replace("-", "--");
                            var lvName = logicalVolume.Name.Replace("-", "--");
                            customLogicalVolumes.Append(vgName + "-" + lvName + " ");
                            if (logicalVolume.ForceFixedSize)
                                customFixedLogicalVolumes.Append(vgName + "-" + lvName + " ");
                        }
                    }
                }
            }
            customHardDrives.Append("\"");
            customPartitions.Append("\"");
            customFixedPartitions.Append("\"");
            customLogicalVolumes.Append("\"");
            customFixedLogicalVolumes.Append("\"");
            AppendString(customHardDrives.ToString());
            AppendString(customPartitions.ToString());
            AppendString(customFixedPartitions.ToString());
            AppendString(customLogicalVolumes.ToString());
            AppendString(customFixedLogicalVolumes.ToString());
        }

        private string SetPartitionMethod()
        {
            switch (_imageProfile.PartitionMethod)
            {
                case "Use Original MBR / GPT":
                    return "partition_method=original";

                case "Dynamic":
                    return "partition_method=dynamic";

                case "Custom Script":
                    return "partition_method=script";

                case "Custom Layout":
                    return "partition_method=layout";

                case "Standard":
                    return "partition_method=standard";

                default:
                    return "";
            }
        }
    }
}