using System.Text;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceModule(ServiceContext ctx)
    {
        public EntityModule GetGenericModule(string moduleGuid)
        {
            return ctx.Uow.ModuleRepository.Get(x => x.Guid.Equals(moduleGuid)).FirstOrDefault();
        }

        public List<EntityModuleCategory> GetModuleCategories(string moduleGuid)
        {
            return ctx.Uow.ModuleCategoryRepository.Get(x => x.ModuleGuid == moduleGuid);
        }

        public List<EntityPolicy> GetModulePolicies(string moduleGuid)
        {
            return ctx.Uow.ModuleRepository.GetModulePolicies(moduleGuid);
        }

        public List<EntityGroup> GetModuleGroups(string moduleGuid)
        {
            return ctx.Uow.ModuleRepository.GetModuleGroups(moduleGuid);
        }

        public List<EntityComputer> GetModuleComputers(string moduleGuid)
        {
            return ctx.Uow.ModuleRepository.GetModuleComputers(moduleGuid);
        }

        public List<EntityImage> GetModuleImages(string moduleGuid)
        {
            var typeMapping = GetModuleIdFromGuid(moduleGuid);
            if(typeMapping.moduleType == EnumModule.ModuleType.FileCopy)
            return ctx.Uow.ModuleRepository.GetModuleImagesFileCopy(moduleGuid);
            if (typeMapping.moduleType == EnumModule.ModuleType.Script)
                return ctx.Uow.ModuleRepository.GetModuleImagesScript(moduleGuid);
            if (typeMapping.moduleType == EnumModule.ModuleType.Sysprep)
                return ctx.Uow.ModuleRepository.GetModuleImagesSysprep(moduleGuid);

            return new List<EntityImage>();
        }

        public DtoGuidTypeMapping GetModuleIdFromGuid(string moduleGuid)
        {
            var softwareModule = ctx.Uow.SoftwareModuleRepository.GetFirstOrDefault(x => x.Guid == moduleGuid);
            if (softwareModule != null)
                return new DtoGuidTypeMapping()
                {
                    moduleId = softwareModule.Id,
                    moduleType = EnumModule.ModuleType.Software
                };
            var fileModule = ctx.Uow.FileCopyModuleRepository.GetFirstOrDefault(x => x.Guid == moduleGuid);
            if (fileModule != null)
                return new DtoGuidTypeMapping()
                {
                    moduleId = fileModule.Id,
                    moduleType = EnumModule.ModuleType.FileCopy
                };
            var printerModule = ctx.Uow.PrinterModuleRepository.GetFirstOrDefault(x => x.Guid == moduleGuid);
            if (printerModule != null)
                return new DtoGuidTypeMapping()
                {
                    moduleId = printerModule.Id,
                    moduleType = EnumModule.ModuleType.Printer
                };
            var scriptModule = ctx.Uow.ScriptModuleRepository.GetFirstOrDefault(x => x.Guid == moduleGuid);
            if (scriptModule != null)
                return new DtoGuidTypeMapping()
                {
                    moduleId = scriptModule.Id,
                    moduleType = EnumModule.ModuleType.Script
                };
            var commandModule = ctx.Uow.CommandModuleRepository.GetFirstOrDefault(x => x.Guid == moduleGuid);
            if (commandModule != null)
                return new DtoGuidTypeMapping()
                {
                    moduleId = commandModule.Id,
                    moduleType = EnumModule.ModuleType.Command
                };
            var wuModule = ctx.Uow.WindowsUpdateModuleRepository.GetFirstOrDefault(x => x.Guid == moduleGuid);
            if (wuModule != null)
                return new DtoGuidTypeMapping()
                {
                    moduleId = wuModule.Id,
                    moduleType = EnumModule.ModuleType.Wupdate
                };
            var messageModule = ctx.Uow.MessageModuleRepository.GetFirstOrDefault(x => x.Guid == moduleGuid);
            if (messageModule != null)
                return new DtoGuidTypeMapping()
                {
                    moduleId = messageModule.Id,
                    moduleType = EnumModule.ModuleType.Message
                };
            var sysprepModule = ctx.Uow.SysprepModuleRepository.GetFirstOrDefault(x => x.Guid == moduleGuid);
            if (sysprepModule != null)
                return new DtoGuidTypeMapping()
                {
                    moduleId = sysprepModule.Id,
                    moduleType = EnumModule.ModuleType.Sysprep
                };
            var winPeModule = ctx.Uow.WinPeModuleRepository.GetFirstOrDefault(x => x.Guid == moduleGuid);
            if (winPeModule != null)
                return new DtoGuidTypeMapping()
                {
                    moduleId = winPeModule.Id,
                    moduleType = EnumModule.ModuleType.WinPE
                };
            var wingetModule = ctx.Uow.WingetModuleRepository.GetFirstOrDefault(x => x.Guid == moduleGuid);
            if (wingetModule != null)
                return new DtoGuidTypeMapping()
                {
                    moduleId = wingetModule.Id,
                    moduleType = EnumModule.ModuleType.Winget
                };
            return null;
        }

        public DtoActionResult RestoreModule(int moduleId, EnumModule.ModuleType moduleType)
        {
            
            switch (moduleType)
            {
                case EnumModule.ModuleType.Command:
                    var cModule = ctx.Uow.CommandModuleRepository.GetById(moduleId);
                    if (cModule != null)
                    {
                        cModule.Archived = false;
                        cModule.Name = cModule.Name.Split('#').First();
                        cModule.ArchiveDateTime = null;
                        if (ctx.Uow.CommandModuleRepository.Exists(x => x.Name.Equals(cModule.Name)))
                            return new DtoActionResult()
                            {
                                ErrorMessage = "Could Not Restore Module.  A Module With Name " + cModule.Name +
                                               " Already Exists"
                            };
                        ctx.Uow.CommandModuleRepository.Update(cModule, cModule.Id);
                        ctx.Uow.Save();
                        return new DtoActionResult() {Id = cModule.Id, Success = true};
                    }

                    break;
                case EnumModule.ModuleType.FileCopy:
                    var fModule = ctx.Uow.FileCopyModuleRepository.GetById(moduleId);
                    if (fModule != null)
                    {
                        fModule.Archived = false;
                        fModule.Name = fModule.Name.Split('#').First();
                        fModule.ArchiveDateTime = null;
                        if (ctx.Uow.FileCopyModuleRepository.Exists(x => x.Name.Equals(fModule.Name)))
                            return new DtoActionResult()
                            {
                                ErrorMessage = "Could Not Restore Module.  A Module With Name " + fModule.Name +
                                               " Already Exists"
                            };
                        ctx.Uow.FileCopyModuleRepository.Update(fModule, fModule.Id);
                        ctx.Uow.Save();
                        return new DtoActionResult() { Id = fModule.Id, Success = true };
                    }

                    break;
                case EnumModule.ModuleType.WinPE:
                    var winPeModule = ctx.Uow.WinPeModuleRepository.GetById(moduleId);
                    if (winPeModule != null)
                    {
                        winPeModule.Archived = false;
                        winPeModule.Name = winPeModule.Name.Split('#').First();
                        winPeModule.ArchiveDateTime = null;
                        if (ctx.Uow.WinPeModuleRepository.Exists(x => x.Name.Equals(winPeModule.Name)))
                            return new DtoActionResult()
                            {
                                ErrorMessage = "Could Not Restore Module.  A Module With Name " + winPeModule.Name +
                                               " Already Exists"
                            };
                        ctx.Uow.WinPeModuleRepository.Update(winPeModule, winPeModule.Id);
                        ctx.Uow.Save();
                        return new DtoActionResult() { Id = winPeModule.Id, Success = true };
                    }

                    break;
                case EnumModule.ModuleType.Printer:
                    var pModule = ctx.Uow.PrinterModuleRepository.GetById(moduleId);
                    if (pModule != null)
                    {
                        pModule.Archived = false;
                        pModule.Name = pModule.Name.Split('#').First();
                        pModule.ArchiveDateTime = null;
                        if (ctx.Uow.PrinterModuleRepository.Exists(x => x.Name.Equals(pModule.Name)))
                            return new DtoActionResult()
                            {
                                ErrorMessage = "Could Not Restore Module.  A Module With Name " + pModule.Name +
                                               " Already Exists"
                            };
                        ctx.Uow.PrinterModuleRepository.Update(pModule, pModule.Id);
                        ctx.Uow.Save();
                        return new DtoActionResult() { Id = pModule.Id, Success = true };
                    }

                    break;
                case EnumModule.ModuleType.Script:
                    var scModule = ctx.Uow.ScriptModuleRepository.GetById(moduleId);
                    if (scModule != null)
                    {
                        scModule.Archived = false;
                        scModule.Name = scModule.Name.Split('#').First();
                        scModule.ArchiveDateTime = null;
                        if (ctx.Uow.ScriptModuleRepository.Exists(x => x.Name.Equals(scModule.Name)))
                            return new DtoActionResult()
                            {
                                ErrorMessage = "Could Not Restore Module.  A Module With Name " + scModule.Name +
                                               " Already Exists"
                            };
                        ctx.Uow.ScriptModuleRepository.Update(scModule, scModule.Id);
                        ctx.Uow.Save();
                        return new DtoActionResult() { Id = scModule.Id, Success = true };
                    }

                    break;
                case EnumModule.ModuleType.Software:
                    var sModule = ctx.Uow.SoftwareModuleRepository.GetById(moduleId);
                    if (sModule != null)
                    {
                        sModule.Archived = false;
                        sModule.Name = sModule.Name.Split('#').First();
                        sModule.ArchiveDateTime = null;
                        if (ctx.Uow.SoftwareModuleRepository.Exists(x => x.Name.Equals(sModule.Name)))
                            return new DtoActionResult()
                            {
                                ErrorMessage = "Could Not Restore Module.  A Module With Name " + sModule.Name +
                                               " Already Exists"
                            };
                        ctx.Uow.SoftwareModuleRepository.Update(sModule, sModule.Id);
                        ctx.Uow.Save();
                        return new DtoActionResult() { Id = sModule.Id, Success = true };
                    }

                    break;
                case EnumModule.ModuleType.Wupdate:
                    var wModule = ctx.Uow.WindowsUpdateModuleRepository.GetById(moduleId);
                    if (wModule != null)
                    {
                        wModule.Archived = false;
                        wModule.Name = wModule.Name.Split('#').First();
                        wModule.ArchiveDateTime = null;
                        if (ctx.Uow.WindowsUpdateModuleRepository.Exists(x => x.Name.Equals(wModule.Name)))
                            return new DtoActionResult()
                            {
                                ErrorMessage = "Could Not Restore Module.  A Module With Name " + wModule.Name +
                                               " Already Exists"
                            };
                        ctx.Uow.WindowsUpdateModuleRepository.Update(wModule, wModule.Id);
                        ctx.Uow.Save();
                        return new DtoActionResult() { Id = wModule.Id, Success = true };
                    }

                    break;
                case EnumModule.ModuleType.Message:
                    var messageModule = ctx.Uow.MessageModuleRepository.GetById(moduleId);
                    if (messageModule != null)
                    {
                        messageModule.Archived = false;
                        messageModule.Name = messageModule.Name.Split('#').First();
                        messageModule.ArchiveDateTime = null;
                        if (ctx.Uow.MessageModuleRepository.Exists(x => x.Name.Equals(messageModule.Name)))
                            return new DtoActionResult()
                            {
                                ErrorMessage = "Could Not Restore Module.  A Module With Name " + messageModule.Name +
                                               " Already Exists"
                            };
                        ctx.Uow.MessageModuleRepository.Update(messageModule, messageModule.Id);
                        ctx.Uow.Save();
                        return new DtoActionResult() { Id = messageModule.Id, Success = true };
                    }

                    break;
                case EnumModule.ModuleType.Sysprep:
                    var sysprepModule = ctx.Uow.SysprepModuleRepository.GetById(moduleId);
                    if (sysprepModule != null)
                    {
                        sysprepModule.Archived = false;
                        sysprepModule.Name = sysprepModule.Name.Split('#').First();
                        sysprepModule.ArchiveDateTime = null;
                        if (ctx.Uow.SysprepModuleRepository.Exists(x => x.Name.Equals(sysprepModule.Name)))
                            return new DtoActionResult()
                            {
                                ErrorMessage = "Could Not Restore Module.  A Module With Name " + sysprepModule.Name +
                                               " Already Exists"
                            };
                        ctx.Uow.SysprepModuleRepository.Update(sysprepModule, sysprepModule.Id);
                        ctx.Uow.Save();
                        return new DtoActionResult() { Id = sysprepModule.Id, Success = true };
                    }

                    break;
                case EnumModule.ModuleType.Winget:
                    var wingetModule = ctx.Uow.WingetModuleRepository.GetById(moduleId);
                    if (wingetModule != null)
                    {
                        wingetModule.Archived = false;
                        wingetModule.Name = wingetModule.Name.Split('#').First();
                        wingetModule.ArchiveDateTime = null;
                        if (ctx.Uow.WingetModuleRepository.Exists(x => x.Name.Equals(wingetModule.Name)))
                            return new DtoActionResult()
                            {
                                ErrorMessage = "Could Not Restore Module.  A Module With Name " + wingetModule.Name +
                                               " Already Exists"
                            };
                        ctx.Uow.WingetModuleRepository.Update(wingetModule, wingetModule.Id);
                        ctx.Uow.Save();
                        return new DtoActionResult() { Id = wingetModule.Id, Success = true };
                    }

                    break;
            }

            return new DtoActionResult()
                {ErrorMessage = "Unknown Error While Looking Up Module", Id = 0, Success = false};
        }

        public DtoActionResult ArchiveModule(int moduleId, EnumModule.ModuleType moduleType)
        {
            var isActiveResult = IsModuleActive(moduleId, moduleType);
            if (isActiveResult != null)
                return new DtoActionResult() {ErrorMessage = isActiveResult, Id = 0, Success = false};

            switch (moduleType)
            {
                case EnumModule.ModuleType.Command:
                    var cModule = ctx.Uow.CommandModuleRepository.GetById(moduleId);
                    if (cModule != null)
                    {
                        if (cModule.Archived) return new DtoActionResult() { Id = moduleId, Success = true };
                        cModule.Archived = true;
                        cModule.Name = cModule.Name + "#" + DateTime.Now.ToString("MM-dd-yyyy_HH:mm");
                        cModule.ArchiveDateTime = DateTime.Now;
                        ctx.Uow.CommandModuleRepository.Update(cModule,cModule.Id);
                        ctx.Uow.Save();
                        return new DtoActionResult() {Id = moduleId, Success = true };
                    }
                    break;
                case EnumModule.ModuleType.FileCopy:
                    var fModule = ctx.Uow.FileCopyModuleRepository.GetById(moduleId);
                    if (fModule != null)
                    {
                        if (fModule.Archived) return new DtoActionResult() { Id = moduleId, Success = true };
                        fModule.Archived = true;
                        fModule.Name = fModule.Name + "#" + DateTime.Now.ToString("MM-dd-yyyy_HH:mm");
                        fModule.ArchiveDateTime = DateTime.Now;
                        ctx.Uow.FileCopyModuleRepository.Update(fModule,fModule.Id);
                        ctx.Uow.Save();
                        return new DtoActionResult() {Id = moduleId, Success = true };
                    }
                    break;
                case EnumModule.ModuleType.WinPE:
                    var winPeModule = ctx.Uow.WinPeModuleRepository.GetById(moduleId);
                    if (winPeModule != null)
                    {
                        if (winPeModule.Archived) return new DtoActionResult() { Id = moduleId, Success = true };
                        winPeModule.Archived = true;
                        winPeModule.Name = winPeModule.Name + "#" + DateTime.Now.ToString("MM-dd-yyyy_HH:mm");
                        winPeModule.ArchiveDateTime = DateTime.Now;
                        ctx.Uow.WinPeModuleRepository.Update(winPeModule, winPeModule.Id);
                        ctx.Uow.Save();
                        return new DtoActionResult() { Id = moduleId, Success = true };
                    }
                    break;
                case EnumModule.ModuleType.Printer:
                     var pModule = ctx.Uow.PrinterModuleRepository.GetById(moduleId);
                    if (pModule != null)
                    {
                        if (pModule.Archived) return new DtoActionResult() { Id = moduleId, Success = true };
                        pModule.Archived = true;
                        pModule.Name = pModule.Name + "#" + DateTime.Now.ToString("MM-dd-yyyy_HH:mm");
                        pModule.ArchiveDateTime = DateTime.Now;
                        ctx.Uow.PrinterModuleRepository.Update(pModule,pModule.Id);
                        ctx.Uow.Save();
                        return new DtoActionResult() {Id = moduleId, Success = true };
                    }
                    break;
                case EnumModule.ModuleType.Script:
                     var scModule = ctx.Uow.ScriptModuleRepository.GetById(moduleId);
                    if (scModule != null)
                    {
                        if (scModule.Archived) return new DtoActionResult() { Id = moduleId, Success = true };
                        scModule.Archived = true;
                        scModule.Name = scModule.Name + "#" + DateTime.Now.ToString("MM-dd-yyyy_HH:mm");
                        scModule.ArchiveDateTime = DateTime.Now;
                       
                        ctx.Uow.ScriptModuleRepository.Update(scModule,scModule.Id);
                        ctx.Uow.Save();
                        return new DtoActionResult() {Id = moduleId, Success = true };
                    }
                    break;
                case EnumModule.ModuleType.Software:
                     var sModule = ctx.Uow.SoftwareModuleRepository.GetById(moduleId);
                    if (sModule != null)
                    {
                        if (sModule.Archived) return new DtoActionResult() { Id = moduleId, Success = true };
                        sModule.Archived = true;
                        sModule.Name = sModule.Name + "#" + DateTime.Now.ToString("MM-dd-yyyy_HH:mm");
                        sModule.ArchiveDateTime = DateTime.Now;
                        ctx.Uow.SoftwareModuleRepository.Update(sModule,sModule.Id);
                        ctx.Uow.Save();
                        return new DtoActionResult() {Id = moduleId, Success = true };
                    }
                    break;
                case EnumModule.ModuleType.Wupdate:
                    var uModule = ctx.Uow.WindowsUpdateModuleRepository.GetById(moduleId);
                    if (uModule != null)
                    {
                        if (uModule.Archived) return new DtoActionResult() { Id = moduleId, Success = true };
                        uModule.Archived = true;
                        uModule.Name = uModule.Name + "#" + DateTime.Now.ToString("MM-dd-yyyy_HH:mm");
                        uModule.ArchiveDateTime = DateTime.Now;
                        ctx.Uow.WindowsUpdateModuleRepository.Update(uModule, uModule.Id);
                        ctx.Uow.Save();
                        return new DtoActionResult() { Id = moduleId, Success = true };
                    }
                    break;
                case EnumModule.ModuleType.Message:
                    var messageModule = ctx.Uow.MessageModuleRepository.GetById(moduleId);
                    if (messageModule != null)
                    {
                        if (messageModule.Archived) return new DtoActionResult() { Id = moduleId, Success = true };
                        messageModule.Archived = true;
                        messageModule.Name = messageModule.Name + "#" + DateTime.Now.ToString("MM-dd-yyyy_HH:mm");
                        messageModule.ArchiveDateTime = DateTime.Now;
                        ctx.Uow.MessageModuleRepository.Update(messageModule, messageModule.Id);
                        ctx.Uow.Save();
                        return new DtoActionResult() { Id = moduleId, Success = true };
                    }
                    break;
                case EnumModule.ModuleType.Sysprep:
                    var sysprepModule = ctx.Uow.SysprepModuleRepository.GetById(moduleId);
                    if (sysprepModule != null)
                    {
                        if (sysprepModule.Archived) return new DtoActionResult() { Id = moduleId, Success = true };
                        sysprepModule.Archived = true;
                        sysprepModule.Name = sysprepModule.Name + "#" + DateTime.Now.ToString("MM-dd-yyyy_HH:mm");
                        sysprepModule.ArchiveDateTime = DateTime.Now;
                        ctx.Uow.SysprepModuleRepository.Update(sysprepModule, sysprepModule.Id);
                        ctx.Uow.Save();
                        return new DtoActionResult() { Id = moduleId, Success = true };
                    }
                    break;
                case EnumModule.ModuleType.Winget:
                    var wingetModule = ctx.Uow.WingetModuleRepository.GetById(moduleId);
                    if (wingetModule != null)
                    {
                        if (wingetModule.Archived) return new DtoActionResult() { Id = moduleId, Success = true };
                        wingetModule.Archived = true;
                        wingetModule.Name = wingetModule.Name + "#" + DateTime.Now.ToString("MM-dd-yyyy_HH:mm");
                        wingetModule.ArchiveDateTime = DateTime.Now;
                        ctx.Uow.WingetModuleRepository.Update(wingetModule, wingetModule.Id);
                        ctx.Uow.Save();
                        return new DtoActionResult() { Id = moduleId, Success = true };
                    }
                    break;
            }

            return new DtoActionResult() { ErrorMessage = "Unknown Error While Looking Up Module", Id = 0, Success = false };
        }

        public string IsModuleActive(int moduleId, EnumModule.ModuleType moduleType)
        {
            var activeModulePolicies = ctx.Uow.PolicyRepository.ActivePoliciesWithModule(moduleId, moduleType);
            if (activeModulePolicies == null)
            {
                return null;
            }

            if (activeModulePolicies.Count > 0)
            {
                var result = new StringBuilder();
                result.Append("This Module Cannot Be Changed.  It Is Currently Part Of The Following Active Policies:  ");
                foreach (var policy in activeModulePolicies)
                    result.Append(policy.Name + ",");
                return result.ToString().Trim(',');
            }
            else
            {
                return null;
            }
        }

        public List<DtoModuleFile> GetModuleFiles(string moduleGuid)
        {
            var listOfFiles = new List<DtoModuleFile>();
            var uploadedFiles = ctx.Uow.UploadedFileRepository.Get(s => s.Guid == moduleGuid).OrderBy(x => x.Name).ToList();
            foreach (var file in uploadedFiles)
            {
                var dtoFile = new DtoModuleFile();
                dtoFile.FileName = file.Name;
                dtoFile.Md5Hash = file.Hash;
                listOfFiles.Add(dtoFile);
            }

            var externalFiles = ctx.Uow.ExternalDownloadRepository.Get(s => s.ModuleGuid == moduleGuid && s.Status == EnumFileDownloader.DownloadStatus.Complete).OrderBy(x => x.FileName).ToList();
            foreach (var file in externalFiles)
            {
                var dtoFile = new DtoModuleFile();
                dtoFile.FileName = file.FileName;
                dtoFile.Md5Hash = file.Md5Hash;
                listOfFiles.Add(dtoFile);
            }

            return listOfFiles;
        }
    }
}