using System.Collections.Generic;
using Toems_Common.Dto.client;
using Toems_Common.Enum;

namespace Toems_Common.Dto.exports
{
    public class DtoPolicyExport
    {
        public DtoPolicyExport()
        {
            CommandModules = new List<DtoCommandModuleExport>();
            FileCopyModules = new List<DtoFileCopyModuleExport>();
            PrinterModules = new List<DtoPrinterModuleExport>();
            ScriptModules = new List<DtoScriptModuleExport>();
            SoftwareModules = new List<DtoSoftwareModuleExport>();
            WuModules = new List<DtoWuModuleExport>();

        }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Instructions { get; set; }
        public string Requirements { get; set; }
        public string Guid { get; set; }
        public string Hash { get; set; }
        public EnumPolicy.Frequency Frequency { get; set; }
        public EnumPolicy.Trigger Trigger { get; set; }
        public int SubFrequency { get; set; }
        public EnumPolicy.CompletedAction CompletedAction { get; set; }
        public bool RemoveInstallCache { get; set; }
        public EnumPolicy.ExecutionType ExecutionType { get; set; }
        public EnumPolicy.ErrorAction ErrorAction { get; set; }
        public EnumPolicy.InventoryAction IsInventory { get; set; }
        public bool IsLoginTracker { get; set; }
        public bool IsApplicationMonitor { get; set; }
        public EnumPolicy.FrequencyMissedAction FrequencyMissedAction { get; set; }
        public EnumPolicy.LogLevel LogLevel { get; set; }
        public bool SkipServerResult { get; set; }
        public List<DtoCommandModuleExport> CommandModules { get; set; }
        public List<DtoFileCopyModuleExport> FileCopyModules { get; set; }
        public List<DtoPrinterModuleExport> PrinterModules { get; set; }
        public List<DtoScriptModuleExport> ScriptModules { get; set; }
        public List<DtoSoftwareModuleExport> SoftwareModules { get; set; }
        public List<DtoWuModuleExport> WuModules { get; set; }
        public EnumPolicy.WuType WuType { get; set; }
    }
}
