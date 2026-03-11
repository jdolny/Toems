using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceSetupCompleteFile(ServiceContext ctx)
    {

        public DtoActionResult Add(EntitySetupCompleteFile setupCompleteFile)
        {
            var actionResult = new DtoActionResult();

            var validationResult = Validate(setupCompleteFile, true);
            if (validationResult.Success)
            {
                ctx.Uow.SetupCompleteFileRepository.Insert(setupCompleteFile);
                ctx.Uow.Save();
                actionResult.Success = true;
                actionResult.Id = setupCompleteFile.Id;
            }
            else
            {
                return new DtoActionResult() { ErrorMessage = validationResult.ErrorMessage };
            }

            return actionResult;
        }

        public DtoActionResult Delete(int setupCompleteFileId)
        {
            var u = GetSetupCompleteFile(setupCompleteFileId);
            if (u == null) return new DtoActionResult { ErrorMessage = "Setup Complete File Not Found", Id = 0 };

            ctx.Uow.SetupCompleteFileRepository.Delete(setupCompleteFileId);
            ctx.Uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.Id;
            return actionResult;
        }

        public EntitySetupCompleteFile GetSetupCompleteFile(int setupCompleteFileId)
        {
            return ctx.Uow.SetupCompleteFileRepository.GetById(setupCompleteFileId);
        }

        public List<EntitySetupCompleteFile> GetAll()
        {
            return ctx.Uow.SetupCompleteFileRepository.Get().OrderBy(x => x.Name).ToList();
        }

        public EntitySetupCompleteFile Get(int id)
        {
            return ctx.Uow.SetupCompleteFileRepository.GetById(id);
        }

        public DtoActionResult Update(EntitySetupCompleteFile setupCompleteFile)
        {
            var u = GetSetupCompleteFile(setupCompleteFile.Id);
            if (u == null) return new DtoActionResult { ErrorMessage = "Setup Complete File Not Found", Id = 0 };

            var actionResult = new DtoActionResult();

            var validationResult = Validate(setupCompleteFile, false);
            if (validationResult.Success)
            {
                ctx.Uow.SetupCompleteFileRepository.Update(setupCompleteFile, u.Id);
                ctx.Uow.Save();
                actionResult.Success = true;
                actionResult.Id = setupCompleteFile.Id;
            }
            else
            {
                return new DtoActionResult() { ErrorMessage = validationResult.ErrorMessage };
            }
            return actionResult;
        }

        public DtoValidationResult Validate(EntitySetupCompleteFile setupCompleteFile, bool isNew)
        {
            var validationResult = new DtoValidationResult { Success = true };

            if (string.IsNullOrEmpty(setupCompleteFile.Name) || !setupCompleteFile.Name.All(c => char.IsLetterOrDigit(c) || c == '_' || c == '-' || c == ' ' || c == '.'))
            {
                validationResult.Success = false;
                validationResult.ErrorMessage = "File Name Is Not Valid";
                return validationResult;
            }

            if (isNew)
            {
                if (ctx.Uow.SetupCompleteFileRepository.Exists(h => h.Name == setupCompleteFile.Name))
                {
                    validationResult.Success = false;
                    validationResult.ErrorMessage = "A File With This Name Already Exists";
                    return validationResult;
                }
            }
            else
            {
                var original = ctx.Uow.SetupCompleteFileRepository.GetById(setupCompleteFile.Id);
                if (original.Name != setupCompleteFile.Name)
                {
                    if (ctx.Uow.SetupCompleteFileRepository.Exists(h => h.Name == setupCompleteFile.Name))
                    {
                        validationResult.Success = false;
                        validationResult.ErrorMessage = "A File With This Name Already Exists";
                        return validationResult;
                    }
                }
            }

            return validationResult;


        }
    }
}
