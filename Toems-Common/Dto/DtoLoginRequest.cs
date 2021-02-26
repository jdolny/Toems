using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toems_Common.Dto
{
    public class DtoLoginRequest
    {
        [Required]
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
