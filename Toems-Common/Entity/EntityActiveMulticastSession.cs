using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toems_Common.Entity
{
    [Table("active_multicast_sessions")]
    public class EntityActiveMulticastSession
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("multicast_session_id")]
        public int Id { get; set; }

        [Column("ond_image_profile_id")]
        public int ImageProfileId { get; set; }

        [Column("multicast_name")]
        public string Name { get; set; }

        [Column("multicast_pid")]
        public int Pid { get; set; }

        [Column("multicast_port")]
        public int Port { get; set; }

        [Column("com_server_id")]
        public int ComServerId { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("upload_task_id")]
        public int? UploadTaskId { get; set; }
    }
}
