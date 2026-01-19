using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceSysprepAnswerFile(EntityContext ectx)
    {
        public DtoActionResult Add(EntitySysprepAnswerfile answerFile)
        {
            var actionResult = new DtoActionResult();

            var validationResult = Validate(answerFile, true);
            if (validationResult.Success)
            {
                ectx.Uow.SysprepAnswerFileRepository.Insert(answerFile);
                ectx.Uow.Save();
                actionResult.Success = true;
                actionResult.Id = answerFile.Id;
            }
            else
            {
                return new DtoActionResult() { ErrorMessage = validationResult.ErrorMessage };
            }

            return actionResult;
        }

        public DtoActionResult Delete(int answerFileId)
        {
            var u = GetAnswerFile(answerFileId);
            if (u == null) return new DtoActionResult { ErrorMessage = "Answer File Not Found", Id = 0 };

            ectx.Uow.SysprepAnswerFileRepository.Delete(answerFileId);
            ectx.Uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.Id;
            return actionResult;
        }

        public EntitySysprepAnswerfile GetAnswerFile(int answerFileId)
        {
            return ectx.Uow.SysprepAnswerFileRepository.GetById(answerFileId);
        }

        public List<EntitySysprepAnswerfile> GetAll()
        {
            return ectx.Uow.SysprepAnswerFileRepository.Get().OrderBy(x => x.Name).ToList();
        }

        public EntitySysprepAnswerfile Get(int id)
        {
            return ectx.Uow.SysprepAnswerFileRepository.GetById(id);
        }

        public DtoActionResult Update(EntitySysprepAnswerfile answerFile)
        {
            var u = GetAnswerFile(answerFile.Id);
            if (u == null) return new DtoActionResult { ErrorMessage = "Answer Not Found", Id = 0 };

            var actionResult = new DtoActionResult();

            var validationResult = Validate(answerFile, false);
            if (validationResult.Success)
            {
                ectx.Uow.SysprepAnswerFileRepository.Update(answerFile, u.Id);
                ectx.Uow.Save();
                actionResult.Success = true;
                actionResult.Id = answerFile.Id;
            }
            else
            {
                return new DtoActionResult() { ErrorMessage = validationResult.ErrorMessage };
            }
            return actionResult;
        }

        public DtoValidationResult Validate(EntitySysprepAnswerfile answerFile, bool isNew)
        {
            var validationResult = new DtoValidationResult { Success = true };

            if (string.IsNullOrEmpty(answerFile.Name) || !answerFile.Name.All(c => char.IsLetterOrDigit(c) || c == '_' || c == '-' || c == ' ' || c == '.'))
            {
                validationResult.Success = false;
                validationResult.ErrorMessage = "Answer File Name Is Not Valid";
                return validationResult;
            }

            if (isNew)
            {
                if (ectx.Uow.SysprepAnswerFileRepository.Exists(h => h.Name == answerFile.Name))
                {
                    validationResult.Success = false;
                    validationResult.ErrorMessage = "An Answer File With This Name Already Exists";
                    return validationResult;
                }
            }
            else
            {
                var original = ectx.Uow.SysprepAnswerFileRepository.GetById(answerFile.Id);
                if (original.Name != answerFile.Name)
                {
                    if (ectx.Uow.SysprepAnswerFileRepository.Exists(h => h.Name == answerFile.Name))
                    {
                        validationResult.Success = false;
                        validationResult.ErrorMessage = "An Answer File With This Name Already Exists";
                        return validationResult;
                    }
                }
            }

            return validationResult;


        }
    }
}
