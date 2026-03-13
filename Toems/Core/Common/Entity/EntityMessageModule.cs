using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Toems_Common.Enum;

namespace Toems_Common.Entity
{
    [Table("message_modules")]
    public class EntityMessageModule
    {
        public EntityMessageModule()
        {
            Guid = System.Guid.NewGuid().ToString();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("message_module_id")]
        public int Id { get; set; }

        [Column("message_module_guid")]
        public string Guid { get; set; }

        [Column("message_module_name")]
        public string Name { get; set; }

        [Column("message_module_description")]
        public string Description { get; set; }

        [Column("message_module_title")]
        public string Title { get; set; }

        [Column("message_module_message")]
        public string Message { get; set; }

        [Column("message_module_timeout")]
        public int Timeout { get; set; }

        [Column("is_archived")]
        public bool Archived { get; set; }

        [Column("datetime_archived_local")]
        public DateTime? ArchiveDateTime { get; set; }
    }
}