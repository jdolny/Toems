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
    public class ServiceApprovalRequest
    {
        private readonly UnitOfWork _uow;

        public ServiceApprovalRequest()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult CreateRequest(EntityApprovalRequest request)
        {
            var existing =
                _uow.ApprovalRequestRepository.GetFirstOrDefault(
                    x => x.ComputerName == request.ComputerName && x.InstallationId == request.InstallationId);

            if (existing != null)
                return new DtoActionResult() {Id = existing.Id, Success = true};

            var actionResult = new DtoActionResult();

            _uow.ApprovalRequestRepository.Insert(request);
            _uow.Save();
            actionResult.Success = true;
            actionResult.Id = request.Id;

            return actionResult;
        }

        public List<EntityApprovalRequest> Search(DtoSearchFilter filter)
        {
            return _uow.ApprovalRequestRepository.Get(x => x.ComputerName.Contains(filter.SearchText));
        }

        public string TotalCount()
        {
            return _uow.ApprovalRequestRepository.Count();
        }

        public DtoActionResult ApproveRequest(int requestId)
        {
            var request = _uow.ApprovalRequestRepository.GetById(requestId);
            if (request == null) return new DtoActionResult() { ErrorMessage = "Request Id Not Found" };
            var computer = new ServiceComputer().GetByName(request.ComputerName);
            if (computer == null) //should always be the case, approvals are only for new computers
            {
                computer = new EntityComputer();
                computer.Name = request.ComputerName;
                computer.ProvisionStatus = EnumProvisionStatus.Status.ProvisionApproved;
                computer.LastIp = request.IpAddress;
                new ServiceComputer().AddComputer(computer);
            }
            else
            {
                computer.ProvisionStatus = EnumProvisionStatus.Status.ProvisionApproved;
                new ServiceComputer().UpdateComputer(computer);
                new ServiceCertificate().DeleteCertificate(computer.CertificateId);
            }
                 
            Delete(requestId);
            return new DtoActionResult() { Success = true, Id = requestId };
        }

        public DtoActionResult Delete(int requestId)
        {
            var u = _uow.ApprovalRequestRepository.GetById(requestId);
            if (u == null) return new DtoActionResult { ErrorMessage = "Request Id Not Found", Id = 0 };
            _uow.ApprovalRequestRepository.Delete(u.Id);
            _uow.Save();
            return new DtoActionResult() { Success = true, Id = requestId };
        }

        public bool SendApprovalRequestReport()
        {
            if (ServiceSetting.GetSettingValue(SettingStrings.SmtpEnabled) != "1")
                return true;

            var approvalRequests = _uow.ApprovalRequestRepository.Get();
            if (approvalRequests.Count == 0) return true;
            var sb = new StringBuilder();
            sb.Append("The Following Computers Are Pending Approval:\r\n\r\n");
            foreach (var approvalRequest in approvalRequests)
            {
                sb.Append(approvalRequest.ComputerName + "\t" + approvalRequest.RequestTime + "\t" + approvalRequest.IpAddress +
                          Environment.NewLine);
            }

            var emailList = new List<string>();
            var users = _uow.UserRepository.Get();
            foreach (var user in users)
            {
                if (user.Membership.Equals("Administrator"))
                {
                    if (!string.IsNullOrEmpty(user.Email))
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
                    Subject = "Approval Request Report",
                    Body = sb.ToString(),
                    MailTo = email
                };

                mail.Send();
            }

            return true;
        }

      
        
    }
}