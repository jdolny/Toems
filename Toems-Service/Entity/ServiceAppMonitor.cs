using System;
using System.Collections.Generic;
using System.Globalization;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    class ServiceAppMonitor
    {
        private readonly UnitOfWork _uow;

        public ServiceAppMonitor()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult AddOrUpdate(List<DtoAppMonitor> appMonitors, int computerId)
        {
            //Add Process Inventory
            foreach (var app in appMonitors)
            {
                var localApp = app;
                var existing = _uow.ProcessInventoryRepository.GetFirstOrDefault(x => x.Name == localApp.Name && x.Path == localApp.Path);
                if (existing == null)
                {
                    var process = new EntityProcessInventory();
                    process.Name = localApp.Name;
                    process.Path = localApp.Path;
                    _uow.ProcessInventoryRepository.Insert(process);
                } 
                //must save after each one or will end up with duplicates
                _uow.Save();
            }
          
            //Add Computer Process
            foreach (var app in appMonitors)
            {
                var localApp = app;
                var s = _uow.ProcessInventoryRepository.GetFirstOrDefault(x => x.Name == localApp.Name && x.Path == localApp.Path);
                //check if process exists before adding computer relationship
                if (s == null)
                    continue;

                var computerProcess = new EntityComputerProcess();
                computerProcess.ComputerId = computerId;
                computerProcess.ProcessId = s.Id;
                computerProcess.StartTimeUtc = Convert.ToDateTime(localApp.StartDateTime, CultureInfo.InvariantCulture);
                computerProcess.CloseTimeUtc = Convert.ToDateTime(localApp.EndDateTime, CultureInfo.InvariantCulture);
                computerProcess.Username = localApp.UserName;
                _uow.ComputerProcessRepository.Insert(computerProcess);
            }
            _uow.Save();

            return new DtoActionResult() {Success = true};

        }
    }
}
