using System;
using System.Collections.Generic;
using Toems_Common.Dto.client;
using Toems_Common.Enum;

namespace Toems_Common.Dto
{
    public class DtoClientPolicy
    {
        public DtoClientPolicy()
        {
            CommandModules = new List<DtoClientCommandModule>();
            FileCopyModules = new List<DtoClientFileCopyModule>();
            PrinterModules = new List<DtoClientPrinterModule>();
            ScriptModules = new List<DtoClientScriptModule>();
            SoftwareModules = new List<DtoClientSoftwareModule>();
            WuModules = new List<DtoClientWuModule>();
            MessageModules = new List<DtoClientMessageModule>();
            WinPeModules = new List<DtoClientWinPeModule>();
            Condition = new DtoClientModuleCondition();
        }
        public string Name { get; set; }
        public string Guid { get; set; }
        public string Hash { get; set; }
        public int Id { get; set; }
        public EnumPolicy.Frequency Frequency { get; set; }
        public EnumPolicy.Trigger Trigger { get; set; }
        public int SubFrequency { get; set; }
        public DateTime StartDate { get; set; }
        public EnumPolicy.CompletedAction CompletedAction { get; set; }
        public bool RemoveInstallCache { get; set; }
        public EnumPolicy.ExecutionType ExecutionType { get; set; }
        public EnumPolicy.ErrorAction ErrorAction { get; set; }
        public EnumPolicy.InventoryAction IsInventory { get; set; }
        public EnumPolicy.RemoteAccess RemoteAccess { get; set; }
        public bool IsLoginTracker { get; set; }
        public bool IsApplicationMonitor { get; set; }
        public bool JoinDomain { get; set; }
        public string DomainOU { get; set; }
        public bool ImagePrepCleanup { get; set; }
        public int Order { get; set; }
        public EnumPolicy.FrequencyMissedAction FrequencyMissedAction { get; set; }
        public EnumPolicy.LogLevel LogLevel { get; set; }
        public bool ReRunExisting { get; set; }
        public bool SkipServerResult { get; set; }
        public int StartWindowScheduleId { get; set; }
        public int EndWindowScheduleId { get; set; }
        public List<DtoClientCommandModule> CommandModules { get; set; }
        public List<DtoClientFileCopyModule> FileCopyModules { get; set; }
        public List<DtoClientPrinterModule> PrinterModules { get; set; }
        public List<DtoClientScriptModule> ScriptModules { get; set; }
        public List<DtoClientSoftwareModule> SoftwareModules { get; set; } 
        public List<DtoClientWuModule> WuModules { get; set; }
        public List<DtoClientMessageModule> MessageModules { get; set; }
        public List<DtoClientWinPeModule> WinPeModules { get; set; }
        public EnumPolicy.WuType WuType { get; set; }
        public EnumPolicy.PolicyComCondition PolicyComCondition { get; set; }
        public EnumCondition.FailedAction ConditionFailedAction { get; set; }
        public DtoClientModuleCondition Condition { get; set; }
    }
}
