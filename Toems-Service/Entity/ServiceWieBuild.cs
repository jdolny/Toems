using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceWieBuild
    {
        private readonly UnitOfWork _uow;

        public ServiceWieBuild()
        {
            _uow = new UnitOfWork();
        }

        public EntityWieBuild GetLastBuildOptions()
        {
            return _uow.WieBuildRepository.Get(x => x.Status == "Complete").OrderByDescending(x => x.Id).FirstOrDefault();
        }

        public void UpdateProcessStatus()
        {
            var basePath = Path.Combine(HttpContext.Current.Server.MapPath("~"), "private", "wie_builder", "Status");
            var incompleteProcesses = _uow.WieBuildRepository.Get(x => x.Status != "Complete");
            foreach(var process in incompleteProcesses)
            {
                var path = Path.Combine(basePath, process.WieGuid + ".complete");
                if(File.Exists(path))
                {
                    process.Status = "Complete";
                    process.EndTime = DateTime.Now;
                    _uow.WieBuildRepository.Update(process, process.Id);
                }
                else if(DateTime.Now.AddMinutes(-20) > process.StartTime)
                {
                    process.Status = "Error";
                    process.EndTime = DateTime.Now;
                    _uow.WieBuildRepository.Update(process, process.Id);
                }
            }
            _uow.Save();
        }

        public List<DtoReplicationProcess> GetWieProcess()
        {
            var processes = _uow.WieBuildRepository.Get(x => x.Pid != null && x.Status != "Complete").OrderByDescending(x => x.Id);
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
    }
}