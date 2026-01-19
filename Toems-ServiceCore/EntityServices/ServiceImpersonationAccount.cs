using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceImpersonationAccount(EntityContext ectx)
    {
        public DtoActionResult Add(EntityImpersonationAccount account)
        {
            account.Password = ectx.Encryption.EncryptText(account.Password);
            var actionResult = new DtoActionResult();

            var validationResult = Validate(account, true);
            if (validationResult.Success)
            {
                ectx.Uow.ImpersonationAccountRepository.Insert(account);
                ectx.Uow.Save();
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
            var commandModules = ectx.Uow.CommandModuleRepository.Get(x => x.ImpersonationId == u.Id);
            foreach (var module in commandModules)
            {
                module.ImpersonationId = -1;
                ectx.Uow.CommandModuleRepository.Update(module,module.Id);
            }
            var scriptModules = ectx.Uow.ScriptModuleRepository.Get(x => x.ImpersonationId == u.Id);
            foreach (var module in scriptModules)
            {
                module.ImpersonationId = -1;
                ectx.Uow.ScriptModuleRepository.Update(module, module.Id);
            }
            var softwareModules = ectx.Uow.SoftwareModuleRepository.Get(x => x.ImpersonationId == u.Id);
            foreach (var module in softwareModules)
            {
                module.ImpersonationId = -1;
                ectx.Uow.SoftwareModuleRepository.Update(module, module.Id);
            }
            ectx.Uow.ImpersonationAccountRepository.Delete(accountId);
            ectx.Uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.Id;
            return actionResult;
        }

        public EntityImpersonationAccount GetAccount(int accountId)
        {
            return ectx.Uow.ImpersonationAccountRepository.GetById(accountId);
        }

        public List<EntityImpersonationAccount> GetAll()
        {
            return ectx.Uow.ImpersonationAccountRepository.Get();
        }

        public List<EntityImpersonationAccount> GetForDropDown()
        {
            var list = new List<EntityImpersonationAccount>();
            var accounts = ectx.Uow.ImpersonationAccountRepository.Get();
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
            return ectx.Uow.ImpersonationAccountRepository.Get(x => x.Username.Contains(filter.SearchText));
        }

        public string TotalCount()
        {
            return ectx.Uow.ImpersonationAccountRepository.Count();
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
                    account.Password = ectx.Encryption.EncryptText(account.Password);
                else
                    account.Password = u.Password;
                

                ectx.Uow.ImpersonationAccountRepository.Update(account, u.Id);
                ectx.Uow.Save();
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
            var account = ectx.Uow.ImpersonationAccountRepository.GetById(id);
            if (account != null)
                return account.Guid;

            return string.Empty;
        }

        public DtoImpersonationAccount GetForClient(string impersonationGuid, string clientGuid)
        {
            //todo:  verify client has a task that warrants the use of this impersonation account
            var account = ectx.Uow.ImpersonationAccountRepository.GetFirstOrDefault(x => x.Guid.Equals(impersonationGuid));
            var client = new DtoImpersonationAccount();
            if (account != null)
            {
                client.Username = account.Username;
                client.Password = ectx.Encryption.DecryptText(account.Password);
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
                if (ectx.Uow.ImpersonationAccountRepository.Exists(h => h.Username == account.Username))
                {
                    validationResult.Success = false;
                    validationResult.ErrorMessage = "An Account With This Name Already Exists";
                    return validationResult;
                }
            }
            else
            {
                var original = ectx.Uow.ImpersonationAccountRepository.GetById(account.Id);
                if (original.Username != account.Username)
                {
                    if (ectx.Uow.ImpersonationAccountRepository.Exists(h => h.Username == account.Username))
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