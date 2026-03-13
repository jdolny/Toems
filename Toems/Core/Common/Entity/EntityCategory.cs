using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("categories")]
    public class EntityCategory
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("category_id")]
        public int Id { get; set; }

        [Column("category_name")]
        public string Name { get; set; }

        [Column("category_description")]
        public string Description { get; set; }

    }
}