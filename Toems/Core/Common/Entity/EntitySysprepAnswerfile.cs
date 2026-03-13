using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toems_Common.Entity
{
    [Table("sysprep_answer_files")]
    public class EntitySysprepAnswerfile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("sysprep_answer_file_id", Order = 1)]
        public int Id { get; set; }

        [Column("sysprep_answer_file_name", Order = 2)]
        public string Name { get; set; }

        [Column("sysprep_answer_file_description", Order = 3)]
        public string Description { get; set; }

        [Column("sysprep_answer_file_contents", Order = 4)]
        public string Contents { get; set; }

    }
}
