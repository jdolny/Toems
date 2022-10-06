using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Toems_Common.Enum;

namespace Toems_Common.Entity
{
    [Table("toec_deploy_target_list_computers")]
    public class EntityToecTargetListComputer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("toec_deploy_target_list_computer_id")]
        public int Id { get; set; }

        [Column("toec_deploy_target_list_computer_name")]
        public string Name { get; set; }

        [Column("toec_deploy_target_list_computer_status")]
        public EnumToecDeployTargetComputer.TargetStatus Status { get; set; }

        [Column("toec_deploy_target_list_id")]
        public int TargetListId { get; set; }




    }
}