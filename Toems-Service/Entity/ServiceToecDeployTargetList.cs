using System.Collections.Generic;
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
            var u = GetToecDeployJob(toecTargetListId);
            if (u == null) return new DtoActionResult { ErrorMessage = "Deploy Job Not Found", Id = 0 };

            _uow.ToecTargetListRepository.Delete(toecTargetListId);
            _uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.Id;
            return actionResult;
        }

        public EntityToecTargetList GetToecDeployJob(int toecTargetListId)
        {
            return _uow.ToecTargetListRepository.GetById(toecTargetListId);
        }

        public List<EntityToecTargetList> Search(DtoSearchFilter filter)
        {
            return _uow.ToecTargetListRepository.Get(x => x.Name.Contains(filter.SearchText));
        }

        public string TotalCount()
        {
            return _uow.ToecTargetListRepository.Count();
        }

        public DtoActionResult Update(EntityToecTargetList toecTargetList)
        {
            var u = GetToecDeployJob(toecTargetList.Id);
            if (u == null) return new DtoActionResult {ErrorMessage = "Target List Not Found", Id = 0};

            var actionResult = new DtoActionResult();

               var validationResult = Validate(toecTargetList, false);
            if (validationResult.Success)
            {
                _uow.ToecTargetListRepository.Update(toecTargetList, u.Id);
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
                var originalDeployJob = _uow.ToecDeployJobRepository.GetById(toecDeployJob.Id);
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