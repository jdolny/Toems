using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("image_profile_sysprep")]
    public class EntityImageProfileSysprepTag
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("image_profile_sysprep_tag_id")]
        public int Id { get; set; }

        [Column("priority")]
        public int Priority { get; set; }

        [Column("image_profile_id")]
        public int ProfileId { get; set; }

        [Column("sysprep_module_id")]
        public int SysprepModuleId { get; set; }
    }
}
