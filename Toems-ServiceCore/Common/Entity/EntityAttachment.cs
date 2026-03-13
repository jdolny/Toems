using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("attachments")]
    public class EntityAttachment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("attachment_id")]
        public int Id { get; set; }

        [Column("attachment_name")]
        public string Name { get; set; }

        [Column("attachment_time_local")]
        public DateTime AttachmentTime { get; set; }

        [Column("directory_guid")]
        public string DirectoryGuid { get; set; }

        [Column("username")]
        public string UserName { get; set; }



    }
}