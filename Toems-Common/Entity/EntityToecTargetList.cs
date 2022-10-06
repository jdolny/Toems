using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Toems_Common.Enum;

namespace Toems_Common.Entity
{
    [Table("toec_deploy_target_lists")]
    public class EntityToecTargetList
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("toec_deploy_target_list_id")]
        public int Id { get; set; }

        [Column("toec_deploy_target_list_name")]
        public string Name { get; set; }

        [Column("toec_deploy_target_list_type")]
        public EnumToecDeployTargetList.ListType Type { get; set; }




    }
}