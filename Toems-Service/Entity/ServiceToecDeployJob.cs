using System.Collections.Generic;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceToecDeployJob
    {
        private readonly UnitOfWork _uow;

        public ServiceToecDeployJob()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult Add(EntityToecDeployJob toecDeployJob)
        {
            var actionResult = new DtoActionResult();

            var validationResult = Validate(toecDeployJob,true);
            if (validationResult.Success)
            {
                _uow.ToecDeployJobRepository.Insert(toecDeployJob);
                _uow.Save();
                actionResult.Success = true;
                actionResult.Id = toecDeployJob.Id;
            }
            else
            {
                return new DtoActionResult() {ErrorMessage = validationResult.ErrorMessage};
            }

            return actionResult;
        }

        public DtoActionResult Delete(int toecDeployJobId)
        {
            var u = GetToecDeployJob(toecDeployJobId);
            if (u == null) return new DtoActionResult { ErrorMessage = "Deploy Job Not Found", Id = 0 };

            _uow.ToecDeployJobRepository.Delete(toecDeployJobId);
            _uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.Id;
            return actionResult;
        }

        public EntityToecDeployJob GetToecDeployJob(int toecDeployJobId)
        {
            return _uow.ToecDeployJobRepository.GetById(toecDeployJobId);
        }

        public List<EntityToecDeployJob> Search(DtoSearchFilter filter)
        {
            return _uow.ToecDeployJobRepository.Get(x => x.Name.Contains(filter.SearchText));
        }

        public string TotalCount()
        {
            return _uow.ToecDeployJobRepository.Count();
        }

        public DtoActionResult Update(EntityToecDeployJob toecDeployJob)
        {
            var u = GetToecDeployJob(toecDeployJob.Id);
            if (u == null) return new DtoActionResult {ErrorMessage = "Toec Deploy Job Not Found", Id = 0};

            var actionResult = new DtoActionResult();

               var validationResult = Validate(toecDeployJob,false);
            if (validationResult.Success)
            {
                _uow.ToecDeployJobRepository.Update(toecDeployJob, u.Id);
                _uow.Save();
                actionResult.Success = true;
                actionResult.Id = toecDeployJob.Id;
            }
            else
            {
                return new DtoActionResult() {ErrorMessage = validationResult.ErrorMessage};
            }
            return actionResult;
        }

        public DtoValidationResult Validate(EntityToecDeployJob toecDeployJob, bool isNew)
        {         
             
            var validationResult = new DtoValidationResult();
            if (isNew)
            {
                if (_uow.ToecDeployJobRepository.Exists(h => h.Name == toecDeployJob.Name))
                {
                    validationResult.Success = false;
                    validationResult.ErrorMessage = "A Toec Deploy Job With This Name Already Exists.";
                    return validationResult;
                }
            }
            else
            {
                var originalDeployJob = _uow.ToecDeployJobRepository.GetById(toecDeployJob.Id);
                if (originalDeployJob.Name != toecDeployJob.Name)
                {
                    if (_uow.ToecDeployJobRepository.Exists(h => h.Name == toecDeployJob.Name))
                    {
                        validationResult.Success = false;
                        validationResult.ErrorMessage = "A Toec Deploy Job With This Name Already Exists.";
                        return validationResult;
                    }
                }
            }

            return new DtoValidationResult() {Success = true};
        }


    }
}