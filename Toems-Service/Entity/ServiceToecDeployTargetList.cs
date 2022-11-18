using System.Collections.Generic;
using System.Linq;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceToecDeployTargetList
    {
        private readonly UnitOfWork _uow;

        public ServiceToecDeployTargetList()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult Add(EntityToecTargetList toecTargetList)
        {
            var actionResult = new DtoActionResult();

            var validationResult = Validate(toecTargetList, true);
            if (validationResult.Success)
            {
                _uow.ToecTargetListRepository.Insert(toecTargetList);
                _uow.Save();
                if(toecTargetList.Type == Toems_Common.Enum.EnumToecDeployTargetList.ListType.CustomList)
                {
                    foreach(var comp in toecTargetList.ComputerNames.Distinct())
                    {
                        if(_uow.ToecTargetListComputerRepository.Exists(x => x.Name == comp && x.TargetListId == toecTargetList.Id))
                                continue;
                        var targetComputer = new EntityToecTargetListComputer() { Name = comp, TargetListId = toecTargetList.Id };
                        _uow.ToecTargetListComputerRepository.Insert(targetComputer);
                        
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
                        if (_uow.ToecTargetListOuRepository.Exists(x => x.GroupId == groupId && x.TargetListId == toecTargetList.Id))
                            continue;
                        var targetGroup = new EntityToecTargetListOu() { GroupId = groupId, TargetListId = toecTargetList.Id };
                        _uow.ToecTargetListOuRepository.Insert(targetGroup);
                    }

                    foreach(var comp in computerList.Distinct())
                    {
                        if (_uow.ToecTargetListComputerRepository.Exists(x => x.Name == comp && x.TargetListId == toecTargetList.Id))
                            continue;
                        var targetComputer = new EntityToecTargetListComputer() { Name = comp, TargetListId = toecTargetList.Id };
                        _uow.ToecTargetListComputerRepository.Insert(targetComputer);
                    }
                }


                _uow.Save();
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

            _uow.ToecTargetListRepository.Delete(toecTargetListId);
            _uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.Id;
            return actionResult;
        }

        public EntityToecTargetList GetToecTargetList(int toecTargetListId)
        {
            var tl = _uow.ToecTargetListRepository.GetById(toecTargetListId);
            tl.GroupIds = _uow.ToecTargetListOuRepository.Get(x => x.TargetListId == toecTargetListId).Select(x => x.GroupId).ToList();
            return tl;
        }

        public List<EntityToecTargetList> Search(DtoSearchFilter filter)
        {
            return _uow.ToecTargetListRepository.Get(x => x.Name.Contains(filter.SearchText));
        }

        public List<EntityToecTargetListComputer> GetMembers(int toecTargetListId)
        {
            return _uow.ToecTargetListComputerRepository.Get(x => x.TargetListId.Equals(toecTargetListId));
        }

        public string TotalCount()
        {
            return _uow.ToecTargetListRepository.Count();
        }

        public DtoActionResult Update(EntityToecTargetList toecTargetList)
        {
            var u = GetToecTargetList(toecTargetList.Id);
            if (u == null) return new DtoActionResult {ErrorMessage = "Target List Not Found", Id = 0};

            var actionResult = new DtoActionResult();

            var validationResult = Validate(toecTargetList, false);
            if (validationResult.Success)
            {
                _uow.ToecTargetListRepository.Update(toecTargetList, u.Id);
                _uow.Save();

                if (toecTargetList.Type == Toems_Common.Enum.EnumToecDeployTargetList.ListType.CustomList)
                {
                    foreach (var comp in toecTargetList.ComputerNames.Distinct())
                    {
                        if (_uow.ToecTargetListComputerRepository.Exists(x => x.Name == comp && x.TargetListId == toecTargetList.Id))
                            continue;
                        var targetComputer = new EntityToecTargetListComputer() { Name = comp, TargetListId = toecTargetList.Id };
                        _uow.ToecTargetListComputerRepository.Insert(targetComputer);

                    }
                }
                else if (toecTargetList.Type == Toems_Common.Enum.EnumToecDeployTargetList.ListType.AdOU || toecTargetList.Type == Toems_Common.Enum.EnumToecDeployTargetList.ListType.AdGroup)
                {
                    var currentGroups = _uow.ToecTargetListOuRepository.Get(x => x.TargetListId == toecTargetList.Id);
                    foreach(var group in currentGroups)
                    {
                        if (toecTargetList.GroupIds.Contains(group.GroupId))
                            continue;
                        _uow.ToecTargetListOuRepository.Delete(group.Id);

                    }
                    
                    foreach (var groupId in toecTargetList.GroupIds.Distinct())
                    {
                        var computers = new ServiceGroup().GetGroupMembers(groupId);
                        foreach (var computer in computers)
                            toecTargetList.ComputerNames.Add(computer.Name);
                        if (_uow.ToecTargetListOuRepository.Exists(x => x.GroupId == groupId && x.TargetListId == toecTargetList.Id))
                            continue;
                        var targetGroup = new EntityToecTargetListOu() { GroupId = groupId, TargetListId = toecTargetList.Id };
                        _uow.ToecTargetListOuRepository.Insert(targetGroup);
                    }

                    foreach (var comp in toecTargetList.ComputerNames.Distinct())
                    {
                        if (_uow.ToecTargetListComputerRepository.Exists(x => x.Name == comp && x.TargetListId == toecTargetList.Id))
                            continue;
                        var targetComputer = new EntityToecTargetListComputer() { Name = comp, TargetListId = toecTargetList.Id };
                        _uow.ToecTargetListComputerRepository.Insert(targetComputer);
                    }
                }

              
                _uow.Save();
                var currentTargetListComputers = _uow.ToecTargetListComputerRepository.Get(x => x.TargetListId.Equals(toecTargetList.Id));

                foreach (var computer in currentTargetListComputers)
                {
                    if (toecTargetList.ComputerNames.Contains(computer.Name))
                        continue;
                    _uow.ToecTargetListComputerRepository.Delete(computer.Id);
                }

                _uow.Save();
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
                if (_uow.ToecTargetListRepository.Exists(h => h.Name == toecDeployJob.Name))
                {
                    validationResult.Success = false;
                    validationResult.ErrorMessage = "A Target List With This Name Already Exists.";
                    return validationResult;
                }
            }
            else
            {
                var originalDeployJob = _uow.ToecTargetListRepository.GetById(toecDeployJob.Id);
                if (originalDeployJob.Name != toecDeployJob.Name)
                {
                    if (_uow.ToecTargetListRepository.Exists(h => h.Name == toecDeployJob.Name))
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