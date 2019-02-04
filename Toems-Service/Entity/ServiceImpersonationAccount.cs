using System.Collections.Generic;
using System.Linq;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceImpersonationAccount
    {
        private readonly UnitOfWork _uow;

        public ServiceImpersonationAccount()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult Add(EntityImpersonationAccount account)
        {
            account.Password = new EncryptionServices().EncryptText(account.Password);
            var actionResult = new DtoActionResult();

            var validationResult = Validate(account, true);
            if (validationResult.Success)
            {
                _uow.ImpersonationAccountRepository.Insert(account);
                _uow.Save();
                actionResult.Success = true;
                actionResult.Id = account.Id;
            }
            else
            {
                return new DtoActionResult() { ErrorMessage = validationResult.ErrorMessage };
            }

            return actionResult;
        }

        public DtoActionResult Delete(int accountId)
        {
            var u = GetAccount(accountId);
            if (u == null) return new DtoActionResult { ErrorMessage = "Account Not Found", Id = 0 };
            var commandModules = _uow.CommandModuleRepository.Get(x => x.ImpersonationId == u.Id);
            foreach (var module in commandModules)
            {
                module.ImpersonationId = -1;
                _uow.CommandModuleRepository.Update(module,module.Id);
            }
            var scriptModules = _uow.ScriptModuleRepository.Get(x => x.ImpersonationId == u.Id);
            foreach (var module in scriptModules)
            {
                module.ImpersonationId = -1;
                _uow.ScriptModuleRepository.Update(module, module.Id);
            }
            var softwareModules = _uow.SoftwareModuleRepository.Get(x => x.ImpersonationId == u.Id);
            foreach (var module in softwareModules)
            {
                module.ImpersonationId = -1;
                _uow.SoftwareModuleRepository.Update(module, module.Id);
            }
            _uow.ImpersonationAccountRepository.Delete(accountId);
            _uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.Id;
            return actionResult;
        }

        public EntityImpersonationAccount GetAccount(int accountId)
        {
            return _uow.ImpersonationAccountRepository.GetById(accountId);
        }

        public List<EntityImpersonationAccount> GetAll()
        {
            return _uow.ImpersonationAccountRepository.Get();
        }

        public List<EntityImpersonationAccount> GetForDropDown()
        {
            var list = new List<EntityImpersonationAccount>();
            var accounts = _uow.ImpersonationAccountRepository.Get();
            foreach (var account in accounts)
            {
                var imp = new EntityImpersonationAccount();
                imp.Id = account.Id;
                imp.Username = account.Username;
                list.Add(imp);
            }
            return list;
        }

        public List<EntityImpersonationAccount> Search(DtoSearchFilter filter)
        {
            return _uow.ImpersonationAccountRepository.Get(x => x.Username.Contains(filter.SearchText));
        }

        public string TotalCount()
        {
            return _uow.ImpersonationAccountRepository.Count();
        }

        public DtoActionResult Update(EntityImpersonationAccount account)
        {
            var u = GetAccount(account.Id);
            if (u == null) return new DtoActionResult {ErrorMessage = "Account Not Found", Id = 0};
            var actionResult = new DtoActionResult();
            var validationResult = Validate(account, false);
            if (validationResult.Success)
            {
                if (!string.IsNullOrEmpty(account.Password))
                    account.Password = new EncryptionServices().EncryptText(account.Password);
                else
                    account.Password = u.Password;
                

                _uow.ImpersonationAccountRepository.Update(account, u.Id);
                _uow.Save();
                actionResult.Success = true;
                actionResult.Id = account.Id;
            }
            else
            {
                return new DtoActionResult() { ErrorMessage = validationResult.ErrorMessage };
            }


            return actionResult;
        }

        public string GetGuid(int id)
        {
            var account = _uow.ImpersonationAccountRepository.GetById(id);
            if (account != null)
                return account.Guid;

            return string.Empty;
        }

        public DtoImpersonationAccount GetForClient(string impersonationGuid, string clientGuid)
        {
            //todo:  verify client has a task that warrants the use of this impersonation account
            var account = _uow.ImpersonationAccountRepository.GetFirstOrDefault(x => x.Guid.Equals(impersonationGuid));
            var client = new DtoImpersonationAccount();
            if (account != null)
            {
                client.Username = account.Username;
                client.Password = new EncryptionServices().DecryptText(account.Password);
            }

            return client;
        }

        public DtoValidationResult Validate(EntityImpersonationAccount account, bool isNew)
        {
            var validationResult = new DtoValidationResult { Success = true };

            if (string.IsNullOrEmpty(account.Username))
            {
                validationResult.Success = false;
                validationResult.ErrorMessage = "Account Name Is Not Valid";
                return validationResult;
            }

            if (isNew)
            {
                if (_uow.ImpersonationAccountRepository.Exists(h => h.Username == account.Username))
                {
                    validationResult.Success = false;
                    validationResult.ErrorMessage = "An Account With This Name Already Exists";
                    return validationResult;
                }
            }
            else
            {
                var original = _uow.ImpersonationAccountRepository.GetById(account.Id);
                if (original.Username != account.Username)
                {
                    if (_uow.ImpersonationAccountRepository.Exists(h => h.Username == account.Username))
                    {
                        validationResult.Success = false;
                        validationResult.ErrorMessage = "An Account With This Name Already Exists";
                        return validationResult;
                    }
                }
            }

            return validationResult;


        }


    }
}