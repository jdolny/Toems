using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceToecDeployTargetList(EntityContext ectx, GroupService groupService)
    {
        public DtoActionResult Add(EntityToecTargetList toecTargetList)
        {
            var actionResult = new DtoActionResult();

            var validationResult = Validate(toecTargetList, true);
            if (validationResult.Success)
            {
                ectx.Uow.ToecTargetListRepository.Insert(toecTargetList);
                ectx.Uow.Save();
                if(toecTargetList.Type == Toems_Common.Enum.EnumToecDeployTargetList.ListType.CustomList)
                {
                    foreach(var comp in toecTargetList.ComputerNames.Distinct())
                    {
                        if(ectx.Uow.ToecTargetListComputerRepository.Exists(x => x.Name == comp && x.TargetListId == toecTargetList.Id))
                                continue;
                        var targetComputer = new EntityToecTargetListComputer() { Name = comp, TargetListId = toecTargetList.Id };
                        ectx.Uow.ToecTargetListComputerRepository.Insert(targetComputer);
                        
                    }
                }
                else if (toecTargetList.Type == Toems_Common.Enum.EnumToecDeployTargetList.ListType.AdOU || toecTargetList.Type == Toems_Common.Enum.EnumToecDeployTargetList.ListType.AdGroup)
                {
                    var computerList = new List<string>();
                    foreach(var groupId in toecTargetList.GroupIds.Distinct())
                    {
                        var computers = new ServiceGroup().GetGroupMembers(groupId);
                        foreach(var computer in computers)
                            computerList.Add(computer.Name);
                        if (ectx.Uow.ToecTargetListOuRepository.Exists(x => x.GroupId == groupId && x.TargetListId == toecTargetList.Id))
                            continue;
                        var targetGroup = new EntityToecTargetListOu() { GroupId = groupId, TargetListId = toecTargetList.Id };
                        ectx.Uow.ToecTargetListOuRepository.Insert(targetGroup);
                    }

                    foreach(var comp in computerList.Distinct())
                    {
                        if (ectx.Uow.ToecTargetListComputerRepository.Exists(x => x.Name == comp && x.TargetListId == toecTargetList.Id))
                            continue;
                        var targetComputer = new EntityToecTargetListComputer() { Name = comp, TargetListId = toecTargetList.Id };
                        ectx.Uow.ToecTargetListComputerRepository.Insert(targetComputer);
                    }
                }


                ectx.Uow.Save();
                actionResult.Success = true;
                actionResult.Id = toecTargetList.Id;
            }
            else
            {
                return new DtoActionResult() {ErrorMessage = validationResult.ErrorMessage};
            }

            return actionResult;
        }

        public DtoActionResult Delete(int toecTargetListId)
        {
            var u = GetToecTargetList(toecTargetListId);
            if (u == null) return new DtoActionResult { ErrorMessage = "Target List Not Found", Id = 0 };

            ectx.Uow.ToecTargetListRepository.Delete(toecTargetListId);
            ectx.Uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.Id;
            return actionResult;
        }

        public EntityToecTargetList GetToecTargetList(int toecTargetListId)
        {
            var tl = ectx.Uow.ToecTargetListRepository.GetById(toecTargetListId);
            tl.GroupIds = ectx.Uow.ToecTargetListOuRepository.Get(x => x.TargetListId == toecTargetListId).Select(x => x.GroupId).ToList();
            return tl;
        }

        public List<EntityToecTargetList> Search(DtoSearchFilter filter)
        {
            return ectx.Uow.ToecTargetListRepository.Get(x => x.Name.Contains(filter.SearchText));
        }

        public List<EntityToecTargetListComputer> GetMembers(int toecTargetListId)
        {
            return ectx.Uow.ToecTargetListComputerRepository.Get(x => x.TargetListId.Equals(toecTargetListId));
        }

        public string TotalCount()
        {
            return ectx.Uow.ToecTargetListRepository.Count();
        }

        public DtoActionResult Update(EntityToecTargetList toecTargetList)
        {
            var u = GetToecTargetList(toecTargetList.Id);
            if (u == null) return new DtoActionResult {ErrorMessage = "Target List Not Found", Id = 0};

            var actionResult = new DtoActionResult();

            var validationResult = Validate(toecTargetList, false);
            if (validationResult.Success)
            {
                ectx.Uow.ToecTargetListRepository.Update(toecTargetList, u.Id);
                ectx.Uow.Save();

                if (toecTargetList.Type == Toems_Common.Enum.EnumToecDeployTargetList.ListType.CustomList)
                {
                    foreach (var comp in toecTargetList.ComputerNames.Distinct())
                    {
                        if (ectx.Uow.ToecTargetListComputerRepository.Exists(x => x.Name == comp && x.TargetListId == toecTargetList.Id))
                            continue;
                        var targetComputer = new EntityToecTargetListComputer() { Name = comp, TargetListId = toecTargetList.Id };
                        ectx.Uow.ToecTargetListComputerRepository.Insert(targetComputer);

                    }
                }
                else if (toecTargetList.Type == Toems_Common.Enum.EnumToecDeployTargetList.ListType.AdOU || toecTargetList.Type == Toems_Common.Enum.EnumToecDeployTargetList.ListType.AdGroup)
                {
                    var currentGroups = ectx.Uow.ToecTargetListOuRepository.Get(x => x.TargetListId == toecTargetList.Id);
                    foreach(var group in currentGroups)
                    {
                        if (toecTargetList.GroupIds.Contains(group.GroupId))
                            continue;
                        ectx.Uow.ToecTargetListOuRepository.Delete(group.Id);

                    }
                    
                    foreach (var groupId in toecTargetList.GroupIds.Distinct())
                    {
                        var computers = groupService.GetGroupMembers(groupId);
                        foreach (var computer in computers)
                            toecTargetList.ComputerNames.Add(computer.Name);
                        if (ectx.Uow.ToecTargetListOuRepository.Exists(x => x.GroupId == groupId && x.TargetListId == toecTargetList.Id))
                            continue;
                        var targetGroup = new EntityToecTargetListOu() { GroupId = groupId, TargetListId = toecTargetList.Id };
                        ectx.Uow.ToecTargetListOuRepository.Insert(targetGroup);
                    }

                    foreach (var comp in toecTargetList.ComputerNames.Distinct())
                    {
                        if (ectx.Uow.ToecTargetListComputerRepository.Exists(x => x.Name == comp && x.TargetListId == toecTargetList.Id))
                            continue;
                        var targetComputer = new EntityToecTargetListComputer() { Name = comp, TargetListId = toecTargetList.Id };
                        ectx.Uow.ToecTargetListComputerRepository.Insert(targetComputer);
                    }
                }

              
                ectx.Uow.Save();
                var currentTargetListComputers = ectx.Uow.ToecTargetListComputerRepository.Get(x => x.TargetListId.Equals(toecTargetList.Id));

                foreach (var computer in currentTargetListComputers)
                {
                    if (toecTargetList.ComputerNames.Contains(computer.Name))
                        continue;
                    ectx.Uow.ToecTargetListComputerRepository.Delete(computer.Id);
                }

                ectx.Uow.Save();
                actionResult.Success = true;
                actionResult.Id = toecTargetList.Id;
            }
            else
            {
                return new DtoActionResult() {ErrorMessage = validationResult.ErrorMessage};
            }
            return actionResult;
        }

        public DtoValidationResult Validate(EntityToecTargetList toecDeployJob, bool isNew)
        {         
             
            var validationResult = new DtoValidationResult();
            if (isNew)
            {
                if (ectx.Uow.ToecTargetListRepository.Exists(h => h.Name == toecDeployJob.Name))
                {
                    validationResult.Success = false;
                    validationResult.ErrorMessage = "A Target List With This Name Already Exists.";
                    return validationResult;
                }
            }
            else
            {
                var originalDeployJob = ectx.Uow.ToecTargetListRepository.GetById(toecDeployJob.Id);
                if (originalDeployJob.Name != toecDeployJob.Name)
                {
                    if (ectx.Uow.ToecTargetListRepository.Exists(h => h.Name == toecDeployJob.Name))
                    {
                        validationResult.Success = false;
                        validationResult.ErrorMessage = "A Target List With This Name Already Exists.";
                        return validationResult;
                    }
                }
            }

            return new DtoValidationResult() {Success = true};
        }


    }
}