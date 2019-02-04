using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Toems_Common.Enum;

namespace Toems_Common.Entity
{
    [Table("modules")]
    public class EntityModule
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("module_id")]
        public int Id { get; set; }

        [Column("module_guid")]
        public string Guid { get; set; }

        [Column("module_type")]
        public EnumModule.ModuleType ModuleType { get; set; }

    }
}