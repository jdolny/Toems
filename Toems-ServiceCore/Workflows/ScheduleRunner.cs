using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Workflows
{
    public class ScheduleRunner
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly UnitOfWork _uow;
        public ScheduleRunner()
        {
            _uow = new UnitOfWork();
        }

        public void Run()
        {
            Logger.Debug("Starting Schedule Runner");

            var currentDateTime = DateTime.Now;
            var currentHour = currentDateTime.Hour;
            var currentMinute = currentDateTime.Minute;
            var currentDayOfWeek = (int)DateTime.Now.DayOfWeek;

            Logger.Debug("Current Hour: " + currentHour);
            Logger.Debug("Current Minute: " + currentMinute);
            Logger.Debug("Current Day: " + currentDayOfWeek);


            Logger.Debug("Checking For Schedule That Should Run Now");
            //Find any schedules set to run on this day
            var dayMatches = new List<EntitySchedule>();
            switch (currentDayOfWeek)
            {
                case 0:
                    dayMatches = _uow.ScheduleRepository.Get(x => x.IsActive && x.Sunday);
                    break;
                case 1:
                    dayMatches = _uow.ScheduleRepository.Get(x => x.IsActive && x.Monday);
                    break;
                case 2:
                    dayMatches = _uow.ScheduleRepository.Get(x => x.IsActive && x.Tuesday);
                    break;
                case 3:
                    dayMatches = _uow.ScheduleRepository.Get(x => x.IsActive && x.Wednesday);
                    break;
                case 4:
                    dayMatches = _uow.ScheduleRepository.Get(x => x.IsActive && x.Thursday);
                    break;
                case 5:
                    dayMatches = _uow.ScheduleRepository.Get(x => x.IsActive && x.Friday);
                    break;
                case 6:
                    dayMatches = _uow.ScheduleRepository.Get(x => x.IsActive && x.Saturday);
                    break;
            }

            if (dayMatches.Count == 0)
            {
                Logger.Debug("No Schedules Are Set For This Day");
                return;
            }

            //find hour matches
            var hourMatches = new List<EntitySchedule>();
            hourMatches = dayMatches.Where(x => x.Hour == currentHour).ToList();

            if (hourMatches.Count == 0)
            {
                Logger.Debug("No Schedules Are Set To Run This Hour");
                return;
            }

            //find minute matches, add a 5 minute window in case the 15 minute checkin window ran a little late

            var minuteMatches = new List<EntitySchedule>();
            minuteMatches =
                hourMatches.Where(
                    x =>
                        x.Minute == currentMinute || x.Minute + 1 == currentMinute || x.Minute + 2 == currentMinute ||
                        x.Minute + 3 == currentMinute || x.Minute + 4 == currentMinute || x.Minute + 5 == currentMinute).ToList();

            if (minuteMatches.Count == 0)
            {
                Logger.Debug("No Schedules Are Set To Run This Minute");
                return;
            }

            //Schedules have been found that should run now.  Find any groups with wakeup or shutdown tasks that should run.

            var wakeUpgroups = new List<EntityGroup>();
            var shutdownGroups = new List<EntityGroup>();
            foreach (var schedule in minuteMatches)
            {
                var localSchedule = schedule;
                wakeUpgroups.AddRange(_uow.GroupRepository.Get(x => x.WakeupScheduleId == localSchedule.Id));
                shutdownGroups.AddRange(_uow.GroupRepository.Get(x => x.ShutdownScheduleId == localSchedule.Id));
            }

            if (wakeUpgroups.Count == 0 && shutdownGroups.Count == 0)
            {
                Logger.Debug("No Groups Are Assigned To This Schedule");
                return;
            }
            
            if (wakeUpgroups.Count > 0)
            {
                Logger.Debug("Found Groups To Wakeup For This Schedule.  Starting Now. ");
                new Workflows.PowerManagement().WakeupGroups(wakeUpgroups);
            }

            if (shutdownGroups.Count > 0)
            {
                Logger.Debug("Found Groups To Shutdown For This Schedule.  Starting Now. ");
                new Workflows.PowerManagement().ShutdownGroups(shutdownGroups);
            }
        }     
    }
}
