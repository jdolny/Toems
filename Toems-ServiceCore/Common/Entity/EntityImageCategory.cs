using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("image_categories")]
    public class EntityImageCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("image_category_id")]
        public int Id { get; set; }

        [Column("image_id")]
        public int ImageId { get; set; }

        [Column("category_id")]
        public int CategoryId { get; set; }

    }
}