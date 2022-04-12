using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceSysprepAnswerFile
    {
        private readonly UnitOfWork _uow;

        public ServiceSysprepAnswerFile()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult Add(EntitySysprepAnswerfile answerFile)
        {
            var actionResult = new DtoActionResult();

            var validationResult = Validate(answerFile, true);
            if (validationResult.Success)
            {
                _uow.SysprepAnswerFileRepository.Insert(answerFile);
                _uow.Save();
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

            _uow.SysprepAnswerFileRepository.Delete(answerFileId);
            _uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.Id;
            return actionResult;
        }

        public EntitySysprepAnswerfile GetAnswerFile(int answerFileId)
        {
            return _uow.SysprepAnswerFileRepository.GetById(answerFileId);
        }

        public List<EntitySysprepAnswerfile> GetAll()
        {
            return _uow.SysprepAnswerFileRepository.Get().OrderBy(x => x.Name).ToList();
        }

        public EntitySysprepAnswerfile Get(int id)
        {
            return _uow.SysprepAnswerFileRepository.GetById(id);
        }

        public DtoActionResult Update(EntitySysprepAnswerfile answerFile)
        {
            var u = GetAnswerFile(answerFile.Id);
            if (u == null) return new DtoActionResult { ErrorMessage = "Answer Not Found", Id = 0 };

            var actionResult = new DtoActionResult();

            var validationResult = Validate(answerFile, false);
            if (validationResult.Success)
            {
                _uow.SysprepAnswerFileRepository.Update(answerFile, u.Id);
                _uow.Save();
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
                if (_uow.SysprepAnswerFileRepository.Exists(h => h.Name == answerFile.Name))
                {
                    validationResult.Success = false;
                    validationResult.ErrorMessage = "An Answer File With This Name Already Exists";
                    return validationResult;
                }
            }
            else
            {
                var original = _uow.SysprepAnswerFileRepository.GetById(answerFile.Id);
                if (original.Name != answerFile.Name)
                {
                    if (_uow.SysprepAnswerFileRepository.Exists(h => h.Name == answerFile.Name))
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
