using System.Collections.Generic;
using System.Linq;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceSchedule
    {
        private readonly UnitOfWork _uow;

        public ServiceSchedule()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult Add(EntitySchedule schedule)
        {
            var actionResult = new DtoActionResult();

            var validationResult = Validate(schedule,true);
            if (validationResult.Success)
            {
                _uow.ScheduleRepository.Insert(schedule);
                _uow.Save();
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

            var wakeUpGroups = _uow.GroupRepository.Get(x => x.WakeupScheduleId == scheduleId);
            var shutdownGroups = _uow.GroupRepository.Get(x => x.ShutdownScheduleId == scheduleId);

            foreach (var wuGroup in wakeUpGroups)
            {
                wuGroup.WakeupScheduleId = -1;
                _uow.GroupRepository.Update(wuGroup,wuGroup.Id);
            }

            foreach (var sdGroup in shutdownGroups)
            {
                sdGroup.ShutdownScheduleId = -1;
                _uow.GroupRepository.Update(sdGroup, sdGroup.Id);
            }

            var policyStartWindows = _uow.PolicyRepository.Get(x => x.WindowStartScheduleId == scheduleId);
            var policyEndWindows = _uow.PolicyRepository.Get(x => x.WindowEndScheduleId == scheduleId);

            foreach (var policy in policyStartWindows)
            {
                policy.WindowStartScheduleId = -1;
                _uow.PolicyRepository.Update(policy,policy.Id);
            }

            foreach (var policy in policyEndWindows)
            {
                policy.WindowEndScheduleId = -1;
                _uow.PolicyRepository.Update(policy, policy.Id);
            }

            _uow.ScheduleRepository.Delete(scheduleId);
            _uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.Id;
            return actionResult;
        }

        public EntitySchedule GetSchedule(int scheduleId)
        {
            return _uow.ScheduleRepository.GetById(scheduleId);
        }

        public List<EntitySchedule> GetAll()
        {
            return _uow.ScheduleRepository.Get();
        }

        public List<EntitySchedule> Search(DtoSearchFilter filter)
        {
            return _uow.ScheduleRepository.Get(x => x.Name.Contains(filter.SearchText));
        }

        public string TotalCount()
        {
            return _uow.ScheduleRepository.Count();
        }

        public DtoActionResult Update(EntitySchedule schedule)
        {
            var u = GetSchedule(schedule.Id);
            if (u == null) return new DtoActionResult {ErrorMessage = "Schedule Not Found", Id = 0};

            var actionResult = new DtoActionResult();

               var validationResult = Validate(schedule,false);
            if (validationResult.Success)
            {
                _uow.ScheduleRepository.Update(schedule, u.Id);
                _uow.Save();
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
                if (_uow.ScheduleRepository.Exists(h => h.Name == schedule.Name))
                {
                    validationResult.Success = false;
                    validationResult.ErrorMessage = "A Schedule With This Name Already Exists";
                    return validationResult;
                }
            }
            else
            {
                var original = _uow.ScheduleRepository.GetById(schedule.Id);
                if (original.Name != schedule.Name)
                {
                    if (_uow.ScheduleRepository.Exists(h => h.Name == schedule.Name))
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
                return _uow.PolicyRepository.Get(x => x.WindowStartScheduleId == scheduleId);
            }
            else if(type.Equals("window end"))
            {
                return _uow.PolicyRepository.Get(x => x.WindowEndScheduleId == scheduleId);
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
                return _uow.GroupRepository.Get(x => x.WakeupScheduleId == scheduleId);
            }
            else if (type.Equals("shutdown"))
            {
                 return _uow.GroupRepository.Get(x => x.ShutdownScheduleId == scheduleId);
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
                groups = _uow.GroupRepository.Get(x => x.WakeupScheduleId == scheduleId);
            }
            else if (type.Equals("shutdown"))
            {
                groups = _uow.GroupRepository.Get(x => x.ShutdownScheduleId == scheduleId);
            }
            else
            {
                return new List<EntityComputer>();
            }

            var groupService = new ServiceGroup();
            var computers = new List<EntityComputer>();
            foreach (var group in groups)
            {
                computers.AddRange(groupService.GetGroupMembers(group.Id));
            }

            return computers.GroupBy(x => x.Id).Select(y => y.First()).ToList();
        }


    }
}