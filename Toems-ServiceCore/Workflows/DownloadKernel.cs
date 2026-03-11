using System.Net;
using Toems_ApiCalls;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;
using Toems_ServiceCore.EntityServices;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.Workflows
{
    public class DownloadKernel(ServiceContext ctx)
    {
        private EntityClientComServer _thisComServer;
        
        public bool RunAllServers(DtoOnlineKernel onlineKernel)
        {
            var uow = new UnitOfWork();
            var comServers = uow.ClientComServerRepository.Get(x => x.IsTftpServer);

            var intercomKey = ctx.Setting.GetSettingValue(SettingStrings.IntercomKeyEncrypted);
            var decryptedKey = ctx.Encryption.DecryptText(intercomKey);
            var NoErrors = true;
            foreach (var com in comServers)
            {
                if (!new APICall().ClientComServerApi.DownloadKernel(com.Url, "", decryptedKey, onlineKernel))
                    NoErrors = false;
            }

            return NoErrors;

        }

        public bool Download(DtoOnlineKernel onlineKernel)
        {
            var guid = ctx.Config["ComServerUniqueId"];
            _thisComServer = ctx.ClientComServer.GetServerByGuid(guid);
            if (_thisComServer == null)
            {
                ctx.Log.Error($"Com Server With Guid {guid} Not Found");
                return false;
            }

            if (string.IsNullOrEmpty(_thisComServer.TftpPath))
            {
                ctx.Log.Error($"Com Server With Guid {guid} Does Not Have A Valid Tftp Path");
                return false;
            }

            var baseUrl = "http://files.theopenem.com/kernels/";
            using (var wc = new WebClient())
            {
                try
                {
                    wc.DownloadFile(new Uri(baseUrl + onlineKernel.BaseVersion + "/" + onlineKernel.FileName),
                        _thisComServer.TftpPath + "kernels" +
                        Path.DirectorySeparatorChar + onlineKernel.FileName);
                    return true;
                }
                catch (Exception ex)
                {
                    ctx.Log.Error("Could Not Download Kernel On Com Server: " + _thisComServer.DisplayName);
                    ctx.Log.Error(ex.Message);
                    return false;
                }
            }
        }
    }
}
