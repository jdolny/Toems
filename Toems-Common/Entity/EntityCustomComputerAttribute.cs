using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("custom_computer_attributes")]
    public class EntityCustomComputerAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("custom_computer_attribute_id")]
        public int Id { get; set; }

        [Column("computer_id")]
        public int ComputerId { get; set; }

        [Column("custom_attribute_id")]
        public int CustomAttributeId { get; set; }

        [Column("value")]
        public string Value { get; set; }

    }
}