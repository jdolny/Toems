using System.Collections.Generic;
using Toems_Common.Enum;

namespace Toems_Common.Dto.client
{
    public class DtoClientMessageModule
    {
       public DtoClientMessageModule()
        {
            Condition = new DtoClientModuleCondition();
        }
        public string Guid { get; set; }
        public string DisplayName { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public int Timeout { get; set; }
        public int Order { get; set; }
        public EnumCondition.FailedAction ConditionFailedAction { get; set; }
        public int ConditionNextOrder { get; set; }
        public DtoClientModuleCondition Condition { get; set; }

    }
}
