using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("toems_users_options")]
    public class EntityToemsUserOptions
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("toems_users_options_id")]
        public int Id { get; set; }

        [Column("toems_user_id")]
        public int ToemsUserId { get; set; }
        [Column("description_enabled")]
        public bool DescriptionEnabled { get; set; }
        [Column("description_order")]
        public int DescriptionOrder { get; set; }
        [Column("last_checkin_enabled")]
        public bool LastCheckinEnabled { get; set; }
        
        [Column("last_checkin_order")]
        public int LastCheckinOrder { get; set; }
        
        [Column("last_ip_enabled")]
        public bool LastIpEnabled { get; set; }
        
        [Column("last_ip_order")]
        public int LastIpOrder { get; set;}
        
        [Column("client_version_enabled")]
        public bool ClientVersionEnabled { get; set; }
        
        [Column("client_version_order")]
        public int ClientVersionOrder { get; set;}
        
        [Column("last_user_enabled")]
        public bool LastUserEnabled { get; set; }
        
        [Column("last_user_order")]
        public int LastUserOrder { get; set;}
        
        [Column("provision_date_enabled")]
        public bool ProvisionDateEnabled { get; set; }
        
        [Column("provision_date_order")]
        public int ProvisionDateOrder { get;set; }
        
        [Column("status_enabled")]
        public bool StatusEnabled { get; set; }
        
        [Column("status_order")]
        public int StatusOrder { get; set; }
        
        [Column("current_image_enabled")]
        public bool CurrentImageEnabled { get; set; }
       
        [Column("current_image_order")]
        public int CurrentImageOrder { get; set;}
        
        [Column("manufacturer_enabled")]
        public bool ManufacturerEnabled { get; set; }
        
        [Column("manufacturer_order")]
        public int ManufacturerOrder { get; set;}
        
        [Column("model_enabled")]
        public bool ModelEnabled { get; set; }
        
        [Column("model_order")]
        public int ModelOrder { get; set; }
        
        [Column("os_name_enabled")]
        public bool OsNameEnabled { get; set; }
        
        [Column("os_name_order")]
        public int OsNameOrder { get; set;}
        
        [Column("os_version_enabled")]
        public bool OsVersionEnabled { get; set; }
        
        [Column("os_version_order")]
        public int OsVersionOrder { get; set; }
        
        [Column("os_build_enabled")]
        public bool OsBuildEnabled { get; set; }
        
        [Column("os_build_order")]
        public int OsBuildOrder { get; set; }
        
        [Column("domain_enabled")]
        public bool DomainEnabled { get; set; }
        
        [Column("domain_order")]
        public int DomainOrder { get; set; }
        
        [Column("force_checkin_enabled")]
        public bool ForceCheckinEnabled { get; set; }
        
        [Column("force_checkin_order")]
        public int ForceCheckinOrder { get; set; }
        
        [Column("collect_inventory_enabled")]
        public bool CollectInventoryEnabled { get; set; }
        
        [Column("collect_inventory_order")]
        public int CollectInventoryOrder { get; set;}
        
        [Column("remote_control_enabled")]
        public bool RemoteControlEnabled { get; set; }
        
        [Column("remote_control_order")]
        public int RemoteControlOrder { get; set;}
        
        [Column("service_log_enabled")]
        public bool ServiceLogEnabled { get; set; }
        
        [Column("service_log_order")]
        public int ServiceLogOrder { get; set; }
    }
}