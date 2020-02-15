using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Toems_Common.Enum;

namespace Toems_Common.Entity
{
    [Table("image_profile_templates")]
    public class EntityImageProfileTemplate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("image_profile_template_id")]
        public int Id { get; set; }

        [Column("profile_boot_image")]
        public string BootImage { get; set; }

        [Column("change_name")]
        public bool ChangeName { get; set; }

        [Column("compression_algorithm")]
        public string Compression { get; set; }

        [Column("compression_level")]
        public string CompressionLevel { get; set; }

        [Column("custom_partition_script")]
        public string CustomPartitionScript { get; set; }

        [Column("custom_image_schema")]
        public string CustomSchema { get; set; }

        [Column("custom_upload_schema")]
        public string CustomUploadSchema { get; set; }

        [Column("profile_description")]
        public string Description { get; set; }

        [Column("fix_bcd")]
        public bool FixBcd { get; set; }

        [Column("fix_bootloader")]
        public bool FixBootloader { get; set; }

        [Column("force_dynamic_partitions")]
        public bool ForceDynamicPartitions { get; set; }

        [Column("profile_kernel")]
        public string Kernel { get; set; }

        [Column("profile_kernel_arguments")]
        public string KernelArguments { get; set; }

        [Column("profile_name")]
        public string Name { get; set; }

        [Column("partition_method")]
        public string PartitionMethod { get; set; }

        [Column("multicast_receiver_arguments")]
        public string ReceiverArguments { get; set; }

        [Column("remove_gpt_structures")]
        public bool RemoveGPT { get; set; }

        [Column("multicast_sender_arguments")]
        public string SenderArguments { get; set; }

        [Column("skip_core_download")]
        public bool SkipCore { get; set; }

        [Column("skip_volume_expand")]
        public bool SkipExpandVolumes { get; set; }

        [Column("skip_lvm_shrink")]
        public bool SkipShrinkLvm { get; set; }

        [Column("skip_volume_shrink")]
        public bool SkipShrinkVolumes { get; set; }

        [Column("task_completed_action")]
        public string TaskCompletedAction { get; set; }

        [Column("upload_schema_only")]
        public bool UploadSchemaOnly { get; set; }

        [Column("web_cancel")]
        public bool WebCancel { get; set; }

        [Column("skip_nvram")]
        public bool SkipNvramUpdate { get; set; }

        [Column("randomize_guids")]
        public bool RandomizeGuids { get; set; }

        [Column("force_standard_efi")]
        public bool ForceStandardEfi { get; set; }

        [Column("force_standard_legacy")]
        public bool ForceStandardLegacy { get; set; }

        [Column("simple_upload_schema")]
        public bool SimpleUploadSchema { get; set; }

        [Column("template_type")]
        public EnumProfileTemplate.TemplateType TemplateType { get; set; }

        [Column("skip_hibernation_check")]
        public bool SkipHibernationCheck { get; set; }

        [Column("skip_bitlocker_check")]
        public bool SkipBitlockerCheck { get; set; }
    }
}
