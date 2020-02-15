using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("groups")]
    public class EntityGroup
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("group_id", Order = 1)]
        public int Id { get; set; }

        [Column("group_name", Order = 2)]
        public string Name { get; set; }

        [Column("group_description", Order = 3)]
        public string Description { get; set; }

        [Column("group_dn", Order = 4)]
        public string Dn { get; set; }

        [Column("is_ou")]
        public bool IsOu { get; set; }

        [Column("group_parent_ou")]
        public string ParentOu { get; set; }

        [Column("parent_id")]
        public string ParentId { get; set; }

        [Column("group_type")]
        public string Type { get; set; }

        [Column("cluster_id")]
        public int ClusterId { get; set; }

        [Column("wakeup_schedule_id")]
        public int WakeupScheduleId { get; set; }

        [Column("shutdown_schedule_id")]
        public int ShutdownScheduleId { get; set; }

        [Column("prevent_shutdown")]
        public bool PreventShutdown { get; set; }

        [Column("image_id")]
        public int ImageId { get; set; }

        [Column("image_profile_id")]
        public int ImageProfileId { get; set; }

        [Column("proxy_bootloader")]
        public string ProxyBootloader { get; set; }


    }
}