using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toems_Common.Dto.clientimaging
{
    public class AuthResponseDto
    {
        public string UserType { get; set; }
        public int Id { get; set; }
        public bool IsAuthorized { get; set; }
    }
}
