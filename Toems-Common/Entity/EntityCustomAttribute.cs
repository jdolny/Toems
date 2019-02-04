using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Toems_Common.Enum;

namespace Toems_Common.Entity
{
    [Table("custom_attributes")]
    public class EntityCustomAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("custom_attribute_id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("textmode")]
        public EnumCustomAttribute.TextMode TextMode { get; set; }

        [Column("usage_type")]
        public int UsageType { get; set; }

    }
}