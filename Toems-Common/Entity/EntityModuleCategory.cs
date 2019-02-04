using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("module_categories")]
    public class EntityModuleCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("module_category_id")]
        public int Id { get; set; }

        [Column("module_guid")]
        public string ModuleGuid { get; set; }

        [Column("category_id")]
        public int CategoryId { get; set; }

    }
}