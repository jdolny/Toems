using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("image_profile_file_copy")]
    public class EntityImageProfileFileCopy
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("image_profile_file_copy_id")]
        public int Id { get; set; }

        [Column("destination_folder")]
        public string DestinationFolder { get; set; }

        [Column("destination_partition")]
        public string DestinationPartition { get; set; }

        [Column("filecopy_module_id")]
        public int FileCopyModuleId { get; set; }     

        [Column("priority")]
        public int Priority { get; set; }

        [Column("image_profile_id")]
        public int ProfileId { get; set; }
    }
}
