using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceToecDeployJob(EntityContext ectx)
    {
        public DtoActionResult Add(EntityToecDeployJob toecDeployJob)
        {
            var actionResult = new DtoActionResult();

            var validationResult = Validate(toecDeployJob,true);
            if (validationResult.Success)
            {
                toecDeployJob.PasswordEncrypted = ectx.Encryption.EncryptText(toecDeployJob.PasswordEncrypted);
                ectx.Uow.ToecDeployJobRepository.Insert(toecDeployJob);
                ectx.Uow.Save();
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

            ectx.Uow.ToecDeployJobRepository.Delete(toecDeployJobId);
            ectx.Uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.Id;
            return actionResult;
        }

        public EntityToecDeployJob GetToecDeployJob(int toecDeployJobId)
        {
            return ectx.Uow.ToecDeployJobRepository.GetById(toecDeployJobId);
        }

        public List<EntityToecDeployJob> Search(DtoSearchFilter filter)
        {
            return ectx.Uow.ToecDeployJobRepository.Get(x => x.Name.Contains(filter.SearchText));
        }
        public List<EntityToecTargetListComputer> GetTargetComputers(int toecDeployJobId)
        {
            var u = GetToecDeployJob(toecDeployJobId);
            if(u != null)
            {
                return ectx.Uow.ToecTargetListComputerRepository.Get(x => x.TargetListId == u.TargetListId);
            }
            return null;
        }

        public bool RestartDeployJobService()
        {
            ectx.Uow.ToecDeployThreadRepository.DeleteRange(x => x.Id > 0);
            ectx.Uow.Save();
            return true;
        }

        public bool ResetComputerStatus(int computerId)
        {
            var computer = ectx.Uow.ToecTargetListComputerRepository.GetById(computerId);
            if (computer == null) return false;
            computer.Status = Toems_Common.Enum.EnumToecDeployTargetComputer.TargetStatus.AwaitingAction;
            computer.LastStatusDate = null;
            computer.LastUpdateDetails = null;
            ectx.Uow.ToecTargetListComputerRepository.Update(computer, computer.Id);

            ectx.Uow.Save();
            return true;
        }
        public string TotalCount()
        {
            return ectx.Uow.ToecDeployJobRepository.Count();
        }

        public DtoActionResult Update(EntityToecDeployJob toecDeployJob)
        {
            var u = GetToecDeployJob(toecDeployJob.Id);
            if (u == null) return new DtoActionResult {ErrorMessage = "Toec Deploy Job Not Found", Id = 0};
            if (string.IsNullOrEmpty(toecDeployJob.PasswordEncrypted))
                toecDeployJob.PasswordEncrypted = u.PasswordEncrypted;
            else
                toecDeployJob.PasswordEncrypted = ectx.Encryption.EncryptText(toecDeployJob.PasswordEncrypted);

            var actionResult = new DtoActionResult();

               var validationResult = Validate(toecDeployJob,false);
            if (validationResult.Success)
            {
                ectx.Uow.ToecDeployJobRepository.Update(toecDeployJob, u.Id);
                ectx.Uow.Save();
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
                if (ectx.Uow.ToecDeployJobRepository.Exists(h => h.Name == toecDeployJob.Name))
                {
                    validationResult.Success = false;
                    validationResult.ErrorMessage = "A Toec Deploy Job With This Name Already Exists.";
                    return validationResult;
                }
            }
            else
            {
                var originalDeployJob = ectx.Uow.ToecDeployJobRepository.GetById(toecDeployJob.Id);
                if (originalDeployJob.Name != toecDeployJob.Name)
                {
                    if (ectx.Uow.ToecDeployJobRepository.Exists(h => h.Name == toecDeployJob.Name))
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