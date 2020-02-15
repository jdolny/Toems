using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Toems_Common.Entity
{
    [Table("image_profile_scripts")]
    public class EntityImageProfileScript
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("image_profile_script_id")]
        public int Id { get; set; }

        [Column("priority")]
        public int Priority { get; set; }

        [Column("image_profile_id")]
        public int ProfileId { get; set; }

        [Column("run_when")]
        public string RunWhen { get; set; }

        [Column("script_module_id")]
        public int ScriptModuleId { get; set; }
    }
}
