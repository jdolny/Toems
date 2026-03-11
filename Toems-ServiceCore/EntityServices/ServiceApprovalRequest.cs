using System.Text;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_DataModel;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceApprovalRequest(ServiceContext ctx)
    {
        public DtoActionResult CreateRequest(EntityApprovalRequest request)
        {
            var existing =
                ctx.Uow.ApprovalRequestRepository.GetFirstOrDefault(
                    x => x.ComputerName == request.ComputerName && x.InstallationId == request.InstallationId);

            if (existing != null)
                return new DtoActionResult() {Id = existing.Id, Success = true};

            var actionResult = new DtoActionResult();

            ctx.Uow.ApprovalRequestRepository.Insert(request);
            ctx.Uow.Save();
            actionResult.Success = true;
            actionResult.Id = request.Id;

            return actionResult;
        }

        public List<EntityApprovalRequest> Search(DtoSearchFilter filter)
        {
            return ctx.Uow.ApprovalRequestRepository.Get(x => x.ComputerName.Contains(filter.SearchText));
        }

        public string TotalCount()
        {
            return ctx.Uow.ApprovalRequestRepository.Count();
        }

        public DtoActionResult ApproveRequest(int requestId)
        {
            var request = ctx.Uow.ApprovalRequestRepository.GetById(requestId);
            if (request == null) return new DtoActionResult() { ErrorMessage = "Request Id Not Found" };
            var computer = ctx.Computer.GetByName(request.ComputerName);
            if (computer == null) //should always be the case, approvals are only for new computers
            {
                computer = new EntityComputer();
                computer.Name = request.ComputerName;
                computer.ProvisionStatus = EnumProvisionStatus.Status.ProvisionApproved;
                computer.LastIp = request.IpAddress;
                ctx.Computer.AddComputer(computer);
            }
            else
            {
                computer.ProvisionStatus = EnumProvisionStatus.Status.ProvisionApproved;
                ctx.Computer.UpdateComputer(computer);
                ctx.Certificate.DeleteCertificate(computer.CertificateId);
            }
                 
            Delete(requestId);
            return new DtoActionResult() { Success = true, Id = requestId };
        }

        public DtoActionResult Delete(int requestId)
        {
            var u = ctx.Uow.ApprovalRequestRepository.GetById(requestId);
            if (u == null) return new DtoActionResult { ErrorMessage = "Request Id Not Found", Id = 0 };
            ctx.Uow.ApprovalRequestRepository.Delete(u.Id);
            ctx.Uow.Save();
            return new DtoActionResult() { Success = true, Id = requestId };
        }

        public async Task<bool> SendApprovalRequestReport()
        {
            if (ctx.Setting.GetSettingValue(SettingStrings.SmtpEnabled) != "1")
                return true;

            var approvalRequests = ctx.Uow.ApprovalRequestRepository.Get();
            if (approvalRequests.Count == 0) return true;
            var sb = new StringBuilder();
            sb.Append("The Following Computers Are Pending Approval:\r\n\r\n");
            foreach (var approvalRequest in approvalRequests)
            {
                sb.Append(approvalRequest.ComputerName + "\t" + approvalRequest.RequestTime + "\t" + approvalRequest.IpAddress +
                          Environment.NewLine);
            }

            var emailList = new List<string>();
            var users = ctx.Uow.UserRepository.Get();
            foreach (var user in users)
            {
                if (user.Membership.Equals("Administrator"))
                {
                    if (!string.IsNullOrEmpty(user.Email))
                        emailList.Add(user.Email);
                }
                else
                {
                    var rights = ctx.User.GetUserRights(user.Id).Select(right => right.Right).ToList();
                    if (rights.Any(right => right == AuthorizationStrings.EmailReset))
                    {
                        if (!string.IsNullOrEmpty(user.Email))
                            emailList.Add(user.Email);
                    }
                }
            }

            foreach (var email in emailList)
            {
                await ctx.Mail.SendMailAsync(sb.ToString(),email,"Approval Request Report");
            }

            return true;
        }

      
        
    }
}