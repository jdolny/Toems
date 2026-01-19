using System.Diagnostics;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceWieBuild(EntityContext ectx, IWebHostEnvironment env)
    {
        public EntityWieBuild GetLastBuildOptions()
        {
            return ectx.Uow.WieBuildRepository.Get(x => x.Status == "Complete").OrderByDescending(x => x.Id).FirstOrDefault();
        }

        public void UpdateProcessStatus()
        {
            var basePath = Path.Combine(env.ContentRootPath, "private", "wie_builder", "Status");
            var incompleteProcesses = ectx.Uow.WieBuildRepository.Get(x => x.Status != "Complete");
            foreach(var process in incompleteProcesses)
            {
                var path = Path.Combine(basePath, process.WieGuid + ".complete");
                if(File.Exists(path))
                {
                    process.Status = "Complete";
                    process.EndTime = DateTime.Now;
                    ectx.Uow.WieBuildRepository.Update(process, process.Id);
                }
                else if(DateTime.Now.AddMinutes(-20) > process.StartTime)
                {
                    process.Status = "Error";
                    process.EndTime = DateTime.Now;
                    ectx.Uow.WieBuildRepository.Update(process, process.Id);
                }
            }
            ectx.Uow.Save();
        }

        public List<DtoReplicationProcess> GetWieProcess()
        {
            var processes = ectx.Uow.WieBuildRepository.Get(x => x.Pid != null && x.Status != "Complete").OrderByDescending(x => x.Id);
            var proc = new DtoReplicationProcess();
            var list = new List<DtoReplicationProcess>();
            if (processes == null) return list;
            foreach(var p in processes)
            try
            {
                var wie = Process.GetProcessById(Convert.ToInt32(p.Pid));
                proc.Pid = wie.Id;
                proc.ProcessName = wie.ProcessName;
                if(proc.ProcessName == "cmd")
                    list.Add(proc);

            }
            catch
            {
                //ignored
            }
            return list;
        }

        public byte[] GetWieIso()
        {
            var filePath = Path.Combine(env.ContentRootPath, "private", "wie_builder","Builds","ISO", "WinPE 10 x64 WinPE.iso");

            return File.ReadAllBytes(filePath);

        }

        public bool CheckIsoExists()
        {
            var filePath = Path.Combine(env.ContentRootPath, "private", "wie_builder", "Builds", "ISO", "WinPE 10 x64 WinPE.iso");
            if (File.Exists(filePath))
                return true;
            return false;
        }
    }
}