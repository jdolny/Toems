using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceSchedule(ServiceContext ctx)
    {

        public DtoActionResult Add(EntitySchedule schedule)
        {
            var actionResult = new DtoActionResult();

            var validationResult = Validate(schedule,true);
            if (validationResult.Success)
            {
                ctx.Uow.ScheduleRepository.Insert(schedule);
                ctx.Uow.Save();
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

            var wakeUpGroups = ctx.Uow.GroupRepository.Get(x => x.WakeupScheduleId == scheduleId);
            var shutdownGroups = ctx.Uow.GroupRepository.Get(x => x.ShutdownScheduleId == scheduleId);

            foreach (var wuGroup in wakeUpGroups)
            {
                wuGroup.WakeupScheduleId = -1;
                ctx.Uow.GroupRepository.Update(wuGroup,wuGroup.Id);
            }

            foreach (var sdGroup in shutdownGroups)
            {
                sdGroup.ShutdownScheduleId = -1;
                ctx.Uow.GroupRepository.Update(sdGroup, sdGroup.Id);
            }

            var policyStartWindows = ctx.Uow.PolicyRepository.Get(x => x.WindowStartScheduleId == scheduleId);
            var policyEndWindows = ctx.Uow.PolicyRepository.Get(x => x.WindowEndScheduleId == scheduleId);

            foreach (var policy in policyStartWindows)
            {
                policy.WindowStartScheduleId = -1;
                ctx.Uow.PolicyRepository.Update(policy,policy.Id);
            }

            foreach (var policy in policyEndWindows)
            {
                policy.WindowEndScheduleId = -1;
                ctx.Uow.PolicyRepository.Update(policy, policy.Id);
            }

            ctx.Uow.ScheduleRepository.Delete(scheduleId);
            ctx.Uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.Id;
            return actionResult;
        }

        public EntitySchedule GetSchedule(int scheduleId)
        {
            return ctx.Uow.ScheduleRepository.GetById(scheduleId);
        }

        public List<EntitySchedule> GetAll()
        {
            return ctx.Uow.ScheduleRepository.Get();
        }

        public List<EntitySchedule> Search(DtoSearchFilter filter)
        {
            return ctx.Uow.ScheduleRepository.Get(x => x.Name.Contains(filter.SearchText));
        }

        public string TotalCount()
        {
            return ctx.Uow.ScheduleRepository.Count();
        }

        public DtoActionResult Update(EntitySchedule schedule)
        {
            var u = GetSchedule(schedule.Id);
            if (u == null) return new DtoActionResult {ErrorMessage = "Schedule Not Found", Id = 0};

            var actionResult = new DtoActionResult();

               var validationResult = Validate(schedule,false);
            if (validationResult.Success)
            {
                ctx.Uow.ScheduleRepository.Update(schedule, u.Id);
                ctx.Uow.Save();
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
                if (ctx.Uow.ScheduleRepository.Exists(h => h.Name == schedule.Name))
                {
                    validationResult.Success = false;
                    validationResult.ErrorMessage = "A Schedule With This Name Already Exists";
                    return validationResult;
                }
            }
            else
            {
                var original = ctx.Uow.ScheduleRepository.GetById(schedule.Id);
                if (original.Name != schedule.Name)
                {
                    if (ctx.Uow.ScheduleRepository.Exists(h => h.Name == schedule.Name))
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
                return ctx.Uow.PolicyRepository.Get(x => x.WindowStartScheduleId == scheduleId);
            }
            else if(type.Equals("window end"))
            {
                return ctx.Uow.PolicyRepository.Get(x => x.WindowEndScheduleId == scheduleId);
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
                return ctx.Uow.GroupRepository.Get(x => x.WakeupScheduleId == scheduleId);
            }
            else if (type.Equals("shutdown"))
            {
                 return ctx.Uow.GroupRepository.Get(x => x.ShutdownScheduleId == scheduleId);
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
                groups = ctx.Uow.GroupRepository.Get(x => x.WakeupScheduleId == scheduleId);
            }
            else if (type.Equals("shutdown"))
            {
                groups = ctx.Uow.GroupRepository.Get(x => x.ShutdownScheduleId == scheduleId);
            }
            else
            {
                return new List<EntityComputer>();
            }

            var computers = new List<EntityComputer>();
            foreach (var group in groups)
            {
                computers.AddRange(ctx.Group.GetGroupMembers(group.Id));
            }

            return computers.GroupBy(x => x.Id).Select(y => y.First()).ToList();
        }


    }
}