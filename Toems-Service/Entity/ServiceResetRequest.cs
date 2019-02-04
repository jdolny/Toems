using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceResetRequest
    {
        private readonly UnitOfWork _uow;

        public ServiceResetRequest()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult CreateRequest(EntityResetRequest request)
        {
            var existing =
                _uow.ResetRequestRepository.GetFirstOrDefault(
                    x => x.ComputerName == request.ComputerName && x.InstallationId == request.InstallationId);

            if (existing != null)
                return new DtoActionResult() {Id = existing.Id, Success = true};

            var actionResult = new DtoActionResult();

            _uow.ResetRequestRepository.Insert(request);
            _uow.Save();
            actionResult.Success = true;
            actionResult.Id = request.Id;

            return actionResult;
        }

        public List<EntityResetRequest> Search(DtoSearchFilter filter)
        {
            return _uow.ResetRequestRepository.Get(x => x.ComputerName.Contains(filter.SearchText));
        }

        public bool SendResetRequestReport()
        {
            if (ServiceSetting.GetSettingValue(SettingStrings.SmtpEnabled) != "1")
                return true;

            var resetRequests = _uow.ResetRequestRepository.Get();
            if (resetRequests.Count == 0) return true;
            var sb = new StringBuilder();
            sb.Append("The Following Computers Are Pending Reset Approval:\r\n\r\n");
            foreach (var resetRequest in resetRequests)
            {
                sb.Append(resetRequest.ComputerName + "\t" + resetRequest.RequestTime + "\t" + resetRequest.IpAddress +
                          Environment.NewLine);
            }

            var emailList = new List<string>();
            var users = _uow.UserRepository.Get();
            foreach (var user in users)
            {
                if (user.Membership.Equals("Administrator"))
                {
                    if(!string.IsNullOrEmpty(user.Email))
                        emailList.Add(user.Email);
                }
                else
                {
                    var rights = new ServiceUser().GetUserRights(user.Id).Select(right => right.Right).ToList();
                    if (rights.Any(right => right == AuthorizationStrings.EmailReset))
                    {
                        if (!string.IsNullOrEmpty(user.Email))
                            emailList.Add(user.Email);
                    }
                }
            }

            foreach (var email in emailList)
            {
                var mail = new MailServices
                {
                    Subject = "Reset Request Report",
                    Body = sb.ToString(),
                    MailTo = email
                };

                mail.Send();
            }

            return true;
        }

        public string TotalCount()
        {
            return _uow.ResetRequestRepository.Count();
        }

        public DtoActionResult ApproveRequest(int requestId)
        {
            var request = _uow.ResetRequestRepository.GetById(requestId);
            if (request == null) return new DtoActionResult() { ErrorMessage = "Reset Request Id Not Found" };
            var computer = new ServiceComputer().GetByNameForReset(request.ComputerName);
            computer.ProvisionStatus = EnumProvisionStatus.Status.Reset;
            new ServiceComputer().UpdateComputer(computer);
            new ServiceCertificate().DeleteCertificate(computer.CertificateId);
            Delete(requestId);
            return new DtoActionResult() {Success = true, Id = requestId};
        }

        public DtoActionResult Delete(int requestId)
        {
            var u = _uow.ResetRequestRepository.GetById(requestId);
            if (u == null) return new DtoActionResult { ErrorMessage = "Reset Request Id Not Found", Id = 0 };
            _uow.ResetRequestRepository.Delete(u.Id);
            _uow.Save();
            return new DtoActionResult() { Success = true, Id = requestId };
        }

      
        
    }
}