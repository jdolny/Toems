using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Toems_Common.Enum;

namespace Toems_Common.Entity
{
    [Table("toec_deploy_target_list_ous")]
    public class EntityToecTargetListOu
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("toec_deploy_target_list_ou_id")]
        public int Id { get; set; }

        [Column("toec_deploy_target_list_group_id")]
        public int GroupId { get; set; }

        [Column("toec_deploy_target_list_id")]
        public int TargetListId { get; set; }




    }
}