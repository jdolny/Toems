using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toems_Common.Entity;

namespace Toems_Common.Dto
{
    public class DtoTaskBootFile
    {
        public EntityComputer Computer { get; set; }
        public EntityImageProfile ImageProfile { get; set; }
    }
}
