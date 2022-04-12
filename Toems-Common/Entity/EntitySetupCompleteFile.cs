using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toems_Common.Entity
{
    [Table("setupcomplete_files")]
    public class EntitySetupCompleteFile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("setupcomplete_file_id")]
        public int Id { get; set; }

        [Column("setupcomplete_file_name")]

        public string Name { get; set; }

        [Column("setupcomplete_file_description")]

        public string Description { get; set; }

        [Column("setupcomplete_file_contents")]
        public string Contents { get; set; }

    }
}
