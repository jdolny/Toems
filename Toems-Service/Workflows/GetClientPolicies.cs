using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using log4net;
using Newtonsoft.Json;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_DataModel;
using Toems_Service.Entity;

namespace Toems_Service.Workflows
{
    public class GetClientPolicies
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly UnitOfWork _uow;

        public GetClientPolicies()
        {
            _uow = new UnitOfWork();
        }

        public DtoTriggerResponse Execute(DtoPolicyRequest policyRequest, int? computerId=null)
        {
            var triggerResponse = new DtoTriggerResponse();
           
            var list = new List<DtoClientPolicy>();

            EntityComputer computer = null;
            if(computerId != null)
                computer = _uow.ComputerRepository.GetFirstOrDefault(x => x.Id == computerId);
            else if(policyRequest.ClientIdentity.Guid != null)
                computer = _uow.ComputerRepository.GetFirstOrDefault(x => x.Guid == policyRequest.ClientIdentity.Guid);

            if (computer == null) return null;

            if (string.IsNullOrEmpty(policyRequest.CurrentComServer))
            {
                Logger.Debug("Could Not Determine The Client's Policy.  A Com Server Was Not Provided.");
                return null;
            }

            //without this, viewing the computer's effective policy, clears these values
            if (!string.IsNullOrEmpty(policyRequest.ClientVersion) && !string.IsNullOrEmpty(policyRequest.PushURL))
            {
                computer.LastCheckinTime = DateTime.Now;
                computer.ClientVersion = policyRequest.ClientVersion;
                computer.PushUrl = policyRequest.PushURL;
                computer.LastIp = IpServices.GetIPAddress();
                new ServiceComputer().UpdateComputer(computer);
            }

            var groupMemberships = _uow.GroupRepository.GetMembershipsForClientPolicy(computer.Id);
            if (policyRequest.UserLogins != null)
            {
                var userLoginsResult = new ServiceUserLogins().AddOrUpdate(policyRequest.UserLogins, computer.Id);
                if (userLoginsResult != null)
                    triggerResponse.UserLoginsSubmitted = userLoginsResult.Success;
            }

            if (policyRequest.AppMonitors != null)
            {
                var appMonitorResult = new ServiceAppMonitor().AddOrUpdate(policyRequest.AppMonitors, computer.Id);
                if (appMonitorResult != null)
                    triggerResponse.AppMonitorSubmitted = appMonitorResult.Success;
            }

            foreach (var membership in groupMemberships)
            {
                var clientPoliciesJson = new ServiceGroup().GetActiveGroupPolicy(membership.GroupId);
                if (clientPoliciesJson == null)
                    continue;
                if(policyRequest.Trigger == EnumPolicy.Trigger.Startup)
                    list.AddRange(JsonConvert.DeserializeObject<List<DtoClientPolicy>>(clientPoliciesJson.PolicyJson).Where(x => x.StartDate <= DateTime.UtcNow && (x.Trigger == EnumPolicy.Trigger.Startup || x.Trigger == EnumPolicy.Trigger.StartupOrCheckin)));
                else if (policyRequest.Trigger == EnumPolicy.Trigger.Checkin)
                    list.AddRange(JsonConvert.DeserializeObject<List<DtoClientPolicy>>(clientPoliciesJson.PolicyJson).Where(x => x.StartDate <= DateTime.UtcNow && (x.Trigger == EnumPolicy.Trigger.Checkin || x.Trigger == EnumPolicy.Trigger.StartupOrCheckin)));
                else
                    list.AddRange(JsonConvert.DeserializeObject<List<DtoClientPolicy>>(clientPoliciesJson.PolicyJson).Where(x => x.StartDate <= DateTime.UtcNow && x.Trigger == policyRequest.Trigger));

            }

            var distinctList = list.GroupBy(dto => dto.Guid).Select(y => y.First()).ToList();
            var toRemoveByComServer = new List<DtoClientPolicy>();
            //refine list to only designated com servers
            foreach (var p in distinctList)
            {
                if (p.PolicyComCondition == EnumPolicy.PolicyComCondition.Any)
                    continue;

                var policyComServers = _uow.PolicyRepository.GetPolicyComServerUrls(p.Id);
                if (!policyComServers.Contains(policyRequest.CurrentComServer.ToLower()))
                    toRemoveByComServer.Add(p);
            }

            foreach (var p in toRemoveByComServer)
            {
                distinctList.Remove(p);
            }

            //refine list to designated schedules
            var currentDayOfWeek = (int)DateTime.Now.DayOfWeek;
            var currentTimeOfDay = DateTime.Now.TimeOfDay;
            var serviceSchedule = new ServiceSchedule();

            var toRemoveBySchedule = new List<DtoClientPolicy>();

            foreach (var p in distinctList.Where(x => x.StartWindowScheduleId != -1 && x.EndWindowScheduleId != -1 && x.Trigger != EnumPolicy.Trigger.Login))
            {
                //items in here have a more refined schedule.  Check if they should be removed from the current run list.
                var start = serviceSchedule.GetSchedule(p.StartWindowScheduleId);
                var end = serviceSchedule.GetSchedule(p.EndWindowScheduleId);
                if (start == null || end == null)
                    continue;
                if (!start.IsActive || !end.IsActive)
                    continue;

                if (p.Frequency != EnumPolicy.Frequency.OncePerWeek && p.Frequency != EnumPolicy.Frequency.OncePerMonth) //day of week is already defined in weekly frequency and day of month is already defined
                {
                    if (currentDayOfWeek == 0 && !start.Sunday)
                        toRemoveBySchedule.Add(p);
                    else if (currentDayOfWeek == 1 && !start.Monday)
                        toRemoveBySchedule.Add(p);
                    else if (currentDayOfWeek == 2 && !start.Tuesday)
                        toRemoveBySchedule.Add(p);
                    else if (currentDayOfWeek == 3 && !start.Wednesday)
                        toRemoveBySchedule.Add(p);
                    else if (currentDayOfWeek == 4 && !start.Thursday)
                        toRemoveBySchedule.Add(p);
                    else if (currentDayOfWeek == 5 && !start.Friday)
                        toRemoveBySchedule.Add(p);
                    else if (currentDayOfWeek == 6 && !start.Saturday)
                        toRemoveBySchedule.Add(p);
                }

                try
                {
                    var startMinute = start.Minute == 0 ? "00" : start.Minute.ToString();
                    var endMinute = end.Minute == 0 ? "00" : end.Minute.ToString();
                    var startTimeOfDay = DateTime.ParseExact(start.Hour + " " + startMinute, "H mm", CultureInfo.InvariantCulture).TimeOfDay;
                    var endTimeOfDay = DateTime.ParseExact(end.Hour + " " + endMinute, "H mm", CultureInfo.InvariantCulture).TimeOfDay;
                    if (currentTimeOfDay >= startTimeOfDay && currentTimeOfDay <= endTimeOfDay)
                    {
                        //ignored
                    }
                    else
                    {
                        toRemoveBySchedule.Add(p);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("Could Not Parse Schedule Times For " + start.Name + " " + end.Name);
                    Logger.Error(ex.Message);
                    continue;
                }                  
            }

            foreach (var p in toRemoveBySchedule)
            {
                distinctList.Remove(p);
            }

            triggerResponse.Policies = distinctList;
            //without this, viewing the computer's effective policy, runs the unicast task
            if (!string.IsNullOrEmpty(policyRequest.ClientVersion) && !string.IsNullOrEmpty(policyRequest.PushURL))
            {
                if (triggerResponse.Policies.Any(x => x.WinPeModules.Any()))
                {
                    new Unicast(Convert.ToInt32(computer.Id), "deploy", 0).Start();
                }

            }
            triggerResponse.CheckinTime = Convert.ToInt32(ServiceSetting.GetSettingValue(SettingStrings.CheckinInterval));
            triggerResponse.ShutdownDelay = Convert.ToInt32(ServiceSetting.GetSettingValue(SettingStrings.ShutdownDelay));
            return triggerResponse;
            
        }
    }
}
