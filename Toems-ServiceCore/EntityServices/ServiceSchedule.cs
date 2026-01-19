using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceSchedule(EntityContext ectx, GroupService groupService)
    {

        public DtoActionResult Add(EntitySchedule schedule)
        {
            var actionResult = new DtoActionResult();

            var validationResult = Validate(schedule,true);
            if (validationResult.Success)
            {
                ectx.Uow.ScheduleRepository.Insert(schedule);
                ectx.Uow.Save();
                actionResult.Success = true;
                actionResult.Id = schedule.Id;
            }
            else
            {
                return new DtoActionResult() {ErrorMessage = validationResult.ErrorMessage};
            }

            return actionResult;
        }

        public DtoActionResult Delete(int scheduleId)
        {
            var u = GetSchedule(scheduleId);
            if (u == null) return new DtoActionResult { ErrorMessage = "Schedule Not Found", Id = 0 };

            var wakeUpGroups = ectx.Uow.GroupRepository.Get(x => x.WakeupScheduleId == scheduleId);
            var shutdownGroups = ectx.Uow.GroupRepository.Get(x => x.ShutdownScheduleId == scheduleId);

            foreach (var wuGroup in wakeUpGroups)
            {
                wuGroup.WakeupScheduleId = -1;
                ectx.Uow.GroupRepository.Update(wuGroup,wuGroup.Id);
            }

            foreach (var sdGroup in shutdownGroups)
            {
                sdGroup.ShutdownScheduleId = -1;
                ectx.Uow.GroupRepository.Update(sdGroup, sdGroup.Id);
            }

            var policyStartWindows = ectx.Uow.PolicyRepository.Get(x => x.WindowStartScheduleId == scheduleId);
            var policyEndWindows = ectx.Uow.PolicyRepository.Get(x => x.WindowEndScheduleId == scheduleId);

            foreach (var policy in policyStartWindows)
            {
                policy.WindowStartScheduleId = -1;
                ectx.Uow.PolicyRepository.Update(policy,policy.Id);
            }

            foreach (var policy in policyEndWindows)
            {
                policy.WindowEndScheduleId = -1;
                ectx.Uow.PolicyRepository.Update(policy, policy.Id);
            }

            ectx.Uow.ScheduleRepository.Delete(scheduleId);
            ectx.Uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.Id;
            return actionResult;
        }

        public EntitySchedule GetSchedule(int scheduleId)
        {
            return ectx.Uow.ScheduleRepository.GetById(scheduleId);
        }

        public List<EntitySchedule> GetAll()
        {
            return ectx.Uow.ScheduleRepository.Get();
        }

        public List<EntitySchedule> Search(DtoSearchFilter filter)
        {
            return ectx.Uow.ScheduleRepository.Get(x => x.Name.Contains(filter.SearchText));
        }

        public string TotalCount()
        {
            return ectx.Uow.ScheduleRepository.Count();
        }

        public DtoActionResult Update(EntitySchedule schedule)
        {
            var u = GetSchedule(schedule.Id);
            if (u == null) return new DtoActionResult {ErrorMessage = "Schedule Not Found", Id = 0};

            var actionResult = new DtoActionResult();

               var validationResult = Validate(schedule,false);
            if (validationResult.Success)
            {
                ectx.Uow.ScheduleRepository.Update(schedule, u.Id);
                ectx.Uow.Save();
                actionResult.Success = true;
                actionResult.Id = schedule.Id;
            }
            else
            {
                return new DtoActionResult() {ErrorMessage = validationResult.ErrorMessage};
            }
            return actionResult;
        }

        public DtoValidationResult Validate(EntitySchedule schedule, bool isNew)
        {
            if (schedule.Hour < 0 || schedule.Hour > 23)
                return new DtoValidationResult() {ErrorMessage = "Hour Was Not Valid", Success = false};
            if (schedule.Minute != 0 && schedule.Minute != 15 && schedule.Minute != 30 && schedule.Minute != 45)
                return new DtoValidationResult() {ErrorMessage = "Minute Was Not Valid", Success = false};

            var validationResult = new DtoValidationResult { Success = true };

            if (string.IsNullOrEmpty(schedule.Name) || !schedule.Name.All(c => char.IsLetterOrDigit(c) || c == '_' || c == '-' || c == ' ' || c == '.'))
            {
                validationResult.Success = false;
                validationResult.ErrorMessage = "Schedule Name Is Not Valid";
                return validationResult;
            }

            if (isNew)
            {
                if (ectx.Uow.ScheduleRepository.Exists(h => h.Name == schedule.Name))
                {
                    validationResult.Success = false;
                    validationResult.ErrorMessage = "A Schedule With This Name Already Exists";
                    return validationResult;
                }
            }
            else
            {
                var original = ectx.Uow.ScheduleRepository.GetById(schedule.Id);
                if (original.Name != schedule.Name)
                {
                    if (ectx.Uow.ScheduleRepository.Exists(h => h.Name == schedule.Name))
                    {
                        validationResult.Success = false;
                        validationResult.ErrorMessage = "A Schedule With This Name Already Exists";
                        return validationResult;
                    }
                }
            }

            return validationResult;
        }

        public List<EntityPolicy> GetSchedulePolicies(int scheduleId, string type)
        {
            if (type.Equals("window start"))
            {
                return ectx.Uow.PolicyRepository.Get(x => x.WindowStartScheduleId == scheduleId);
            }
            else if(type.Equals("window end"))
            {
                return ectx.Uow.PolicyRepository.Get(x => x.WindowEndScheduleId == scheduleId);
            }
            else
            {
                return new List<EntityPolicy>();
            }
        }

        public List<EntityGroup> GetScheduleGroups(int scheduleId, string type)
        {
            if (type.Equals("wakeup"))
            {
                return ectx.Uow.GroupRepository.Get(x => x.WakeupScheduleId == scheduleId);
            }
            else if (type.Equals("shutdown"))
            {
                 return ectx.Uow.GroupRepository.Get(x => x.ShutdownScheduleId == scheduleId);
            }
            else
            {
                return new List<EntityGroup>();
            }
        }

        public List<EntityComputer> GetScheduleComputers(int scheduleId, string type)
        {
            var groups = new List<EntityGroup>();
            if (type.Equals("wakeup"))
            {
                groups = ectx.Uow.GroupRepository.Get(x => x.WakeupScheduleId == scheduleId);
            }
            else if (type.Equals("shutdown"))
            {
                groups = ectx.Uow.GroupRepository.Get(x => x.ShutdownScheduleId == scheduleId);
            }
            else
            {
                return new List<EntityComputer>();
            }

            var computers = new List<EntityComputer>();
            foreach (var group in groups)
            {
                computers.AddRange(groupService.GetGroupMembers(group.Id));
            }

            return computers.GroupBy(x => x.Id).Select(y => y.First()).ToList();
        }


    }
}