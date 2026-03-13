using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Toems_Common.Enum;

namespace Toems_Common.Entity
{
    [Table("wu_modules")]
    public class EntityWuModule
    {
        public EntityWuModule()
        {
            Guid = System.Guid.NewGuid().ToString();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("wu_module_id")]
        public int Id { get; set; }

        [Column("wu_module_guid")]
        public string Guid { get; set; }

        [Column("wu_module_name")]
        public string Name { get; set; }

        [Column("wu_module_description")]
        public string Description { get; set; }

        [Column("wu_module_addarguments")]
        public string AdditionalArguments { get; set; }

        [Column("wu_module_timeout")]
        public int Timeout { get; set; }

        [Column("redirect_stdout")]
        public bool RedirectStdOut { get; set; }

        [Column("redirect_stderror")]
        public bool RedirectStdError { get; set; }

        [Column("success_codes")]
        public string SuccessCodes { get; set; }

        [Column("is_archived")]
        public bool Archived { get; set; }

        [Column("datetime_archived_local")]
        public DateTime? ArchiveDateTime { get; set; }
    }
}