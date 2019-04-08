using System.Collections.Generic;
using Toems_Common.Dto.client;
using Toems_Common.Enum;

namespace Toems_Common.Dto.exports
{
    public class DtoMessageModuleExport
    {
        public string DisplayName { get; set; }
        public int Order { get; set; }
        public int Timeout { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string Description { get; set; }
        public string Guid { get; set; }
    }
}
