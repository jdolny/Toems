using System.Text;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceResetRequest(ServiceContext ctx)
    {

        public DtoActionResult CreateRequest(EntityResetRequest request)
        {
            var existing =
                ctx.Uow.ResetRequestRepository.GetFirstOrDefault(
                    x => x.ComputerName == request.ComputerName && x.InstallationId == request.InstallationId);

            if (existing != null)
                return new DtoActionResult() {Id = existing.Id, Success = true};

            var actionResult = new DtoActionResult();

            ctx.Uow.ResetRequestRepository.Insert(request);
            ctx.Uow.Save();
            actionResult.Success = true;
            actionResult.Id = request.Id;

            return actionResult;
        }

        public List<EntityResetRequest> Search(DtoSearchFilter filter)
        {
            return ctx.Uow.ResetRequestRepository.Get(x => x.ComputerName.Contains(filter.SearchText));
        }

        public async Task<bool> SendResetRequestReport()
        {
            if (ctx.Setting.GetSettingValue(SettingStrings.SmtpEnabled) != "1")
                return true;

            var resetRequests = ctx.Uow.ResetRequestRepository.Get();
            if (resetRequests.Count == 0) return true;
            var sb = new StringBuilder();
            sb.Append("The Following Computers Are Pending Reset Approval:\r\n\r\n");
            foreach (var resetRequest in resetRequests)
            {
                sb.Append(resetRequest.ComputerName + "\t" + resetRequest.RequestTime + "\t" + resetRequest.IpAddress +
                          Environment.NewLine);
            }

            var emailList = new List<string>();
            var users = ctx.Uow.UserRepository.Get();
            foreach (var user in users)
            {
                if (user.Membership.Equals("Administrator"))
                {
                    if(!string.IsNullOrEmpty(user.Email))
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
                await ctx.Mail.SendMailAsync(sb.ToString(),email,"Reset Request Report");
            }

            return true;
        }

        public string TotalCount()
        {
            return ctx.Uow.ResetRequestRepository.Count();
        }

        public DtoActionResult ApproveRequest(int requestId)
        {
            var request = ctx.Uow.ResetRequestRepository.GetById(requestId);
            if (request == null) return new DtoActionResult() { ErrorMessage = "Reset Request Id Not Found" };
            var computer = ctx.Computer.GetByNameForReset(request.ComputerName);
            computer.ProvisionStatus = EnumProvisionStatus.Status.Reset;
            ctx.Computer.UpdateComputer(computer);
            ctx.Certificate.DeleteCertificate(computer.CertificateId);
            Delete(requestId);
            return new DtoActionResult() {Success = true, Id = requestId};
        }

        public DtoActionResult Delete(int requestId)
        {
            var u = ctx.Uow.ResetRequestRepository.GetById(requestId);
            if (u == null) return new DtoActionResult { ErrorMessage = "Reset Request Id Not Found", Id = 0 };
            ctx.Uow.ResetRequestRepository.Delete(u.Id);
            ctx.Uow.Save();
            return new DtoActionResult() { Success = true, Id = requestId };
        }

      
        
    }
}