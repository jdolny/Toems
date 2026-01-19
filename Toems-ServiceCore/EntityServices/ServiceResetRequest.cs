using System.Text;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_Service;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceResetRequest(EntityContext ectx, ServiceUser userService, ServiceComputer computerService, ServiceCertificate certificateService)
    {

        public DtoActionResult CreateRequest(EntityResetRequest request)
        {
            var existing =
                ectx.Uow.ResetRequestRepository.GetFirstOrDefault(
                    x => x.ComputerName == request.ComputerName && x.InstallationId == request.InstallationId);

            if (existing != null)
                return new DtoActionResult() {Id = existing.Id, Success = true};

            var actionResult = new DtoActionResult();

            ectx.Uow.ResetRequestRepository.Insert(request);
            ectx.Uow.Save();
            actionResult.Success = true;
            actionResult.Id = request.Id;

            return actionResult;
        }

        public List<EntityResetRequest> Search(DtoSearchFilter filter)
        {
            return ectx.Uow.ResetRequestRepository.Get(x => x.ComputerName.Contains(filter.SearchText));
        }

        public bool SendResetRequestReport()
        {
            if (ectx.Settings.GetSettingValue(SettingStrings.SmtpEnabled) != "1")
                return true;

            var resetRequests = ectx.Uow.ResetRequestRepository.Get();
            if (resetRequests.Count == 0) return true;
            var sb = new StringBuilder();
            sb.Append("The Following Computers Are Pending Reset Approval:\r\n\r\n");
            foreach (var resetRequest in resetRequests)
            {
                sb.Append(resetRequest.ComputerName + "\t" + resetRequest.RequestTime + "\t" + resetRequest.IpAddress +
                          Environment.NewLine);
            }

            var emailList = new List<string>();
            var users = ectx.Uow.UserRepository.Get();
            foreach (var user in users)
            {
                if (user.Membership.Equals("Administrator"))
                {
                    if(!string.IsNullOrEmpty(user.Email))
                        emailList.Add(user.Email);
                }
                else
                {
                    var rights = userService.GetUserRights(user.Id).Select(right => right.Right).ToList();
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
            return ectx.Uow.ResetRequestRepository.Count();
        }

        public DtoActionResult ApproveRequest(int requestId)
        {
            var request = ectx.Uow.ResetRequestRepository.GetById(requestId);
            if (request == null) return new DtoActionResult() { ErrorMessage = "Reset Request Id Not Found" };
            var computer = computerService.GetByNameForReset(request.ComputerName);
            computer.ProvisionStatus = EnumProvisionStatus.Status.Reset;
            computerService.UpdateComputer(computer);
            certificateService.DeleteCertificate(computer.CertificateId);
            Delete(requestId);
            return new DtoActionResult() {Success = true, Id = requestId};
        }

        public DtoActionResult Delete(int requestId)
        {
            var u = ectx.Uow.ResetRequestRepository.GetById(requestId);
            if (u == null) return new DtoActionResult { ErrorMessage = "Reset Request Id Not Found", Id = 0 };
            ectx.Uow.ResetRequestRepository.Delete(u.Id);
            ectx.Uow.Save();
            return new DtoActionResult() { Success = true, Id = requestId };
        }

      
        
    }
}