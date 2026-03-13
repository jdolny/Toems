using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("schedules")]
    public class EntitySchedule
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("schedule_id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("monday")]
        public bool Monday { get; set; }

        [Column("tuesday")]
        public bool Tuesday { get; set; }

        [Column("wednesday")]
        public bool Wednesday { get; set; }

        [Column("thursday")]
        public bool Thursday { get; set; }

        [Column("friday")]
        public bool Friday { get; set; }

        [Column("saturday")]
        public bool Saturday { get; set; }

        [Column("sunday")]
        public bool Sunday { get; set; }

        [Column("hour")]
        public int Hour { get; set; }

        [Column("minute")]
        public int Minute { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; }


    }
}