using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toems_Common.Entity.Remotely
{
    [Table("Organizations")]
    public class Organization
    {
        [Key]
        public string ID { get; set; }
        public string OrganizationName { get; set; }
    }
}
