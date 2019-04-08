using Toems_Common.Enum;

namespace Toems_Common.Dto.client
{
    public class DtoClientPrinterModule
    {
        public DtoClientPrinterModule()
        {
            Condition = new DtoClientModuleCondition();
        }
        public string Guid { get; set; }
        public string DisplayName { get; set; }
        public string PrinterPath { get; set; }
        public int Order { get; set; }
        public bool IsDefault { get; set; }
        public bool RestartSpooler { get; set; }
        public EnumPrinterModule.ActionType PrinterAction { get; set; }
        public bool WaitForEnumeration { get; set; }
        public EnumCondition.FailedAction ConditionFailedAction { get; set; }
        public int ConditionNextOrder { get; set; }
        public DtoClientModuleCondition Condition { get; set; }
    }
}
