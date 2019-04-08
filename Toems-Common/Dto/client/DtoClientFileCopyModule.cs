using System.Collections.Generic;
using Toems_Common.Enum;

namespace Toems_Common.Dto.client
{
    public class DtoClientFileCopyModule
    {
         public DtoClientFileCopyModule()
        {
            Files = new List<DtoClientFileHash>();
            Condition = new DtoClientModuleCondition();
        }
        public string Guid { get; set; }
        public string DisplayName { get; set; }
        public string Destination { get; set; }
        public int Order { get; set; }
        public bool Unzip { get; set; }
        public List<DtoClientFileHash> Files { get; set; }
        public EnumCondition.FailedAction ConditionFailedAction { get; set; }
        public int ConditionNextOrder { get; set; }
        public DtoClientModuleCondition Condition { get; set; }
    }
}
