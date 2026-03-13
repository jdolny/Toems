using System.Collections.Generic;

namespace Toems_Common.Dto
{
    public class DtoSocketRequest
    {
        public DtoSocketRequest()
        {
            connectionIds = new List<string>();
        }
        public List<string> connectionIds { get; set; }
        public string action { get; set; }
        public string message { get; set; }
    }
}