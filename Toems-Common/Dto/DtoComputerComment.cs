using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toems_Common.Dto
{
    public class DtoComputerComment
    {
        public string Comment { get; set; }
        public int ComputerId { get; set; }
        public string Username { get; set; }
        public DateTime CommentTime { get; set; }
    }
}
