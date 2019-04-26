using Toems_Common.Enum;

namespace Toems_Common.Dto.exports
{
    public class DtoPrinterModuleExport
    {
        public string DisplayName { get; set; }
        public string PrinterPath { get; set; }
        public int Order { get; set; }
        public bool IsDefault { get; set; }
        public bool RestartSpooler { get; set; }
        public EnumPrinterModule.ActionType PrinterAction { get; set; }
        public bool WaitForEnumeration { get; set; }
        public string Description { get; set; }
        public string Guid { get; set; }
        public EnumCondition.FailedAction ConditionFailedAction { get; set; }
        public int ConditionNextOrder { get; set; }
        public DtoScriptModuleExport Condition { get; set; }
    }
}
