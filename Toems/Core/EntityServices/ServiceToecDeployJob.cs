using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceToecDeployJob(ServiceContext ctx)
    {
        public DtoActionResult Add(EntityToecDeployJob toecDeployJob)
        {
            var actionResult = new DtoActionResult();

            var validationResult = Validate(toecDeployJob,true);
            if (validationResult.Success)
            {
                toecDeployJob.PasswordEncrypted = ctx.Encryption.EncryptText(toecDeployJob.PasswordEncrypted);
                ctx.Uow.ToecDeployJobRepository.Insert(toecDeployJob);
                ctx.Uow.Save();
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

            ctx.Uow.ToecDeployJobRepository.Delete(toecDeployJobId);
            ctx.Uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.Id;
            return actionResult;
        }

        public EntityToecDeployJob GetToecDeployJob(int toecDeployJobId)
        {
            return ctx.Uow.ToecDeployJobRepository.GetById(toecDeployJobId);
        }

        public List<EntityToecDeployJob> Search(DtoSearchFilter filter)
        {
            return ctx.Uow.ToecDeployJobRepository.Get(x => x.Name.Contains(filter.SearchText));
        }
        public List<EntityToecTargetListComputer> GetTargetComputers(int toecDeployJobId)
        {
            var u = GetToecDeployJob(toecDeployJobId);
            if(u != null)
            {
                return ctx.Uow.ToecTargetListComputerRepository.Get(x => x.TargetListId == u.TargetListId);
            }
            return null;
        }

        public bool RestartDeployJobService()
        {
            ctx.Uow.ToecDeployThreadRepository.DeleteRange(x => x.Id > 0);
            ctx.Uow.Save();
            return true;
        }

        public bool ResetComputerStatus(int computerId)
        {
            var computer = ctx.Uow.ToecTargetListComputerRepository.GetById(computerId);
            if (computer == null) return false;
            computer.Status = Toems_Common.Enum.EnumToecDeployTargetComputer.TargetStatus.AwaitingAction;
            computer.LastStatusDate = null;
            computer.LastUpdateDetails = null;
            ctx.Uow.ToecTargetListComputerRepository.Update(computer, computer.Id);

            ctx.Uow.Save();
            return true;
        }
        public string TotalCount()
        {
            return ctx.Uow.ToecDeployJobRepository.Count();
        }

        public DtoActionResult Update(EntityToecDeployJob toecDeployJob)
        {
            var u = GetToecDeployJob(toecDeployJob.Id);
            if (u == null) return new DtoActionResult {ErrorMessage = "Toec Deploy Job Not Found", Id = 0};
            if (string.IsNullOrEmpty(toecDeployJob.PasswordEncrypted))
                toecDeployJob.PasswordEncrypted = u.PasswordEncrypted;
            else
                toecDeployJob.PasswordEncrypted = ctx.Encryption.EncryptText(toecDeployJob.PasswordEncrypted);

            var actionResult = new DtoActionResult();

               var validationResult = Validate(toecDeployJob,false);
            if (validationResult.Success)
            {
                ctx.Uow.ToecDeployJobRepository.Update(toecDeployJob, u.Id);
                ctx.Uow.Save();
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
                if (ctx.Uow.ToecDeployJobRepository.Exists(h => h.Name == toecDeployJob.Name))
                {
                    validationResult.Success = false;
                    validationResult.ErrorMessage = "A Toec Deploy Job With This Name Already Exists.";
                    return validationResult;
                }
            }
            else
            {
                var originalDeployJob = ctx.Uow.ToecDeployJobRepository.GetById(toecDeployJob.Id);
                if (originalDeployJob.Name != toecDeployJob.Name)
                {
                    if (ctx.Uow.ToecDeployJobRepository.Exists(h => h.Name == toecDeployJob.Name))
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