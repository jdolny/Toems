using Toems_Common.Entity;
using Toems_DataModel;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.Workflows
{
    public class ScheduleRunner(ServiceContext ctx, PowerManagement powerManagement)
    {
        public void Run()
        {
            ctx.Log.Debug("Starting Schedule Runner");

            var currentDateTime = DateTime.Now;
            var currentHour = currentDateTime.Hour;
            var currentMinute = currentDateTime.Minute;
            var currentDayOfWeek = (int)DateTime.Now.DayOfWeek;

            ctx.Log.Debug("Current Hour: " + currentHour);
            ctx.Log.Debug("Current Minute: " + currentMinute);
            ctx.Log.Debug("Current Day: " + currentDayOfWeek);


            ctx.Log.Debug("Checking For Schedule That Should Run Now");
            //Find any schedules set to run on this day
            var dayMatches = new List<EntitySchedule>();
            switch (currentDayOfWeek)
            {
                case 0:
                    dayMatches = ctx.Uow.ScheduleRepository.Get(x => x.IsActive && x.Sunday);
                    break;
                case 1:
                    dayMatches = ctx.Uow.ScheduleRepository.Get(x => x.IsActive && x.Monday);
                    break;
                case 2:
                    dayMatches = ctx.Uow.ScheduleRepository.Get(x => x.IsActive && x.Tuesday);
                    break;
                case 3:
                    dayMatches = ctx.Uow.ScheduleRepository.Get(x => x.IsActive && x.Wednesday);
                    break;
                case 4:
                    dayMatches = ctx.Uow.ScheduleRepository.Get(x => x.IsActive && x.Thursday);
                    break;
                case 5:
                    dayMatches = ctx.Uow.ScheduleRepository.Get(x => x.IsActive && x.Friday);
                    break;
                case 6:
                    dayMatches = ctx.Uow.ScheduleRepository.Get(x => x.IsActive && x.Saturday);
                    break;
            }

            if (dayMatches.Count == 0)
            {
                ctx.Log.Debug("No Schedules Are Set For This Day");
                return;
            }

            //find hour matches
            var hourMatches = new List<EntitySchedule>();
            hourMatches = dayMatches.Where(x => x.Hour == currentHour).ToList();

            if (hourMatches.Count == 0)
            {
                ctx.Log.Debug("No Schedules Are Set To Run This Hour");
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
                ctx.Log.Debug("No Schedules Are Set To Run This Minute");
                return;
            }

            //Schedules have been found that should run now.  Find any groups with wakeup or shutdown tasks that should run.

            var wakeUpgroups = new List<EntityGroup>();
            var shutdownGroups = new List<EntityGroup>();
            foreach (var schedule in minuteMatches)
            {
                var localSchedule = schedule;
                wakeUpgroups.AddRange(ctx.Uow.GroupRepository.Get(x => x.WakeupScheduleId == localSchedule.Id));
                shutdownGroups.AddRange(ctx.Uow.GroupRepository.Get(x => x.ShutdownScheduleId == localSchedule.Id));
            }

            if (wakeUpgroups.Count == 0 && shutdownGroups.Count == 0)
            {
                ctx.Log.Debug("No Groups Are Assigned To This Schedule");
                return;
            }
            
            if (wakeUpgroups.Count > 0)
            {
                ctx.Log.Debug("Found Groups To Wakeup For This Schedule.  Starting Now. ");
                powerManagement.WakeupGroups(wakeUpgroups);
            }

            if (shutdownGroups.Count > 0)
            {
                ctx.Log.Debug("Found Groups To Shutdown For This Schedule.  Starting Now. ");
                powerManagement.ShutdownGroups(shutdownGroups);
            }
        }     
    }
}
