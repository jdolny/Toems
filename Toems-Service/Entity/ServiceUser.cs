using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceUser
    {
        private readonly UnitOfWork _uow;

        public ServiceUser()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult AddUser(EntityToemsUser user)
        {
            user.ImagingToken = Guid.NewGuid().ToString("N").ToUpper() + Guid.NewGuid().ToString("N").ToUpper();
            var validationResult = ValidateUser(user, true);
            var actionResult = new DtoActionResult();
            if (validationResult.Success)
            {
                _uow.UserRepository.Insert(user);
                _uow.Save();
                actionResult.Success = true;
                actionResult.Id = user.Id;
            }
            else
            {
                actionResult.ErrorMessage = validationResult.ErrorMessage;
            }

            return actionResult;
        }

        public DtoActionResult DeleteUser(int userId)
        {
            var u = GetUser(userId);
            if (u == null) return new DtoActionResult { ErrorMessage = "User Not Found", Id = 0 };
            if (GetAdminCount() == 1 && u.Membership == "Administrator")
                return new DtoActionResult() { ErrorMessage = "There Must Be At Least 1 Administrator" };
            if (u.Membership.Equals("Administrator"))
                return new DtoActionResult()
                    {ErrorMessage = "Administrators Must Be Changed To The User Role Before They Can Be Removed"};
            _uow.UserRepository.Delete(userId);
            _uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.Id;
            return actionResult;
        }

        public bool DeleteUserRights(int userId)
        {
            _uow.UserRightRepository.DeleteRange(x => x.UserId == userId);
            _uow.Save();
            return true;
        }

        public int GetAdminCount()
        {
            return Convert.ToInt32(_uow.UserRepository.Count(u => u.Membership == "Administrator"));
        }

        public EntityToemsUser GetUser(int userId)
        {
            return _uow.UserRepository.GetById(userId);
        }

        public EntityToemsUser GetUser(string userName)
        {
            return _uow.UserRepository.GetFirstOrDefault(u => u.Name == userName);
        }

        public string GetUserName(int userId)
        {
            return _uow.UserRepository.GetUserName(userId);
        }

        public List<EntityAuditLog> GetUserAuditLogs(int userId, DtoSearchFilter filter)
        {
            if (filter.SearchText != "Select Filter")
                return
                _uow.AuditLogRepository.Get(x => x.UserId == userId && x.AuditType.ToString() == filter.SearchText).OrderByDescending(x => x.Id).Take(filter.Limit).ToList();
            else
            {
                return
                    _uow.AuditLogRepository.Get(x => x.UserId == userId).OrderByDescending(x => x.Id).Take(filter.Limit).ToList();
            }
        }

        public EntityToemsUser GetUserFromToken(string imagingToken)
        {
            var user = _uow.UserRepository.Get(x => x.ImagingToken.Equals(imagingToken)).FirstOrDefault();
            if (user == null)
                return null;
            user.Password = string.Empty;
            user.Salt = string.Empty;

            return user;
        }
      

        public DtoApiObjectResponse GetUserForLogin(int userId)
        {
            var result = new DtoApiObjectResponse();
            var user = _uow.UserRepository.GetById(userId);
            if (user != null)
            {
               
                user.Password = string.Empty;
                user.Salt = string.Empty;
                result.Success = true;
                result.Id = user.Id;
                result.ObjectJson = JsonConvert.SerializeObject(user);
            }

            return result;
        }

        public List<DtoPinnedPolicy> GetPinnedPolicyCounts(int userId)
        {
            return _uow.UserRepository.GetUserPinnedPolicyCounts(userId);
        }

        public List<DtoPinnedGroup> GetPinnedGroupCounts(int userId)
        {

            return _uow.UserRepository.GetUserPinnedGroupCounts(userId);
        }

       
        public List<EntityUserRight> GetUserRights(int userId)
        {
            return _uow.UserRightRepository.Get(x => x.UserId == userId);
        }

      
        public bool IsAdmin(int userId)
        {
            var user = GetUser(userId);
            return user.Membership == "Administrator";
        }

        public List<EntityToemsUser> GetAll()
        {
            return _uow.UserRepository.Get();
        }

        public List<UserWithUserGroup> SearchUsers(DtoSearchFilter filter)
        {
            return _uow.UserRepository.Search(filter.SearchText);
        }

        public void SendLockOutEmail(int userId)
        {
            //Mail not enabled
            if (ServiceSetting.GetSettingValue(SettingStrings.SmtpEnabled) == "0") return;

            var lockedUser = GetUser(userId);
            foreach (var user in SearchUsers(new DtoSearchFilter()).Where(x => !string.IsNullOrEmpty(x.Email)))
            {
                if (user.Membership != "Administrator" && user.Id != userId) continue;
                var mail = new MailServices
                {
                    MailTo = user.Email,
                    Body = lockedUser.Name + " Has Been Locked For 15 Minutes Because Of Too Many Failed Login Attempts",
                    Subject = "User Locked"
                };
                mail.Send();
            }
        }

      

        public string TotalCount()
        {
            return _uow.UserRepository.Count();
        }

        public DtoActionResult UpdateUser(EntityToemsUser user)
        {
            var u = GetUser(user.Id);
            if (u == null) return new DtoActionResult { ErrorMessage = "User Not Found", Id = 0 };
            if (GetAdminCount() == 1 && user.Membership != "Administrator" && u.Membership.Equals("Administrator"))
                return new DtoActionResult() {ErrorMessage = "There Must Be At Least 1 Administrator"};
            var validationResult = ValidateUser(user, false);
            var actionResult = new DtoActionResult();
            if (validationResult.Success)
            {
                user.ImagingToken = Guid.NewGuid().ToString("N").ToUpper() + Guid.NewGuid().ToString("N").ToUpper(); //create new token each time user is updated
                _uow.UserRepository.Update(user, user.Id);
                _uow.Save();
                actionResult.Success = true;
                actionResult.Id = user.Id;
            }
            else
            {
                actionResult.ErrorMessage = validationResult.ErrorMessage;
            }

            return actionResult;
        }

        private DtoValidationResult ValidateUser(EntityToemsUser user, bool isNewUser)
        {
            var validationResult = new DtoValidationResult { Success = true };

            if (string.IsNullOrEmpty(user.Name) || !user.Name.All(c => char.IsLetterOrDigit(c) || c == '_' || c == '-'))
            {
                validationResult.Success = false;
                validationResult.ErrorMessage = "User Name Is Not Valid";
                return validationResult;
            }

            if (isNewUser)
            {
                if (string.IsNullOrEmpty(user.Password))
                {
                    validationResult.Success = false;
                    validationResult.ErrorMessage = "Password Is Not Valid";
                    return validationResult;
                }

                if (_uow.UserRepository.Exists(h => h.Name == user.Name))
                {
                    validationResult.Success = false;
                    validationResult.ErrorMessage = "A User With This Name Already Exists";
                    return validationResult;
                }
            }
            else
            {
                var originalUser = _uow.UserRepository.GetById(user.Id);
                if (originalUser.Name != user.Name)
                {
                    if (_uow.UserRepository.Exists(h => h.Name == user.Name))
                    {
                        validationResult.Success = false;
                        validationResult.ErrorMessage = "A User With This Name Already Exists";
                        return validationResult;
                    }
                }
            }

            return validationResult;
        }

        public List<DtoProcessWithTime> GetUserProcessTimes(DateTime dateCutoff, int limit, string userName)
        {
            return new ReportRepository().GetTopProcessTimesForUser(dateCutoff, limit, userName);
        }

        public List<DtoProcessWithCount> GetUserProcessCounts(DateTime dateCutoff, int limit, string userName)
        {
            return new ReportRepository().GetTopProcessCountsForUser(dateCutoff, limit, userName);
        }

        public List<DtoProcessWithUser> GetAllProcessForUser(DateTime dateCutoff, int limit, string userName)
        {
            return new ReportRepository().GetAllProcessForUser(dateCutoff, limit, userName);
        }
    }
}