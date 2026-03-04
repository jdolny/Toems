using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Toems_ApiCalls;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;
using Toems_Service.Entity;
using Toems_ServiceCore.EntityServices;
using Toems_ServiceCore.Infrastructure;

namespace Toems_Service.Workflows
{
    public class DownloadKernel(InfrastructureContext ictx, ServiceClientComServer serviceClientComServer)
    {
        private EntityClientComServer _thisComServer;
        
        public bool RunAllServers(DtoOnlineKernel onlineKernel)
        {
            var uow = new UnitOfWork();
            var comServers = uow.ClientComServerRepository.Get(x => x.IsTftpServer);

            var intercomKey = ictx.Settings.GetSettingValue(SettingStrings.IntercomKeyEncrypted);
            var decryptedKey = ictx.Encryption.DecryptText(intercomKey);
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
            var guid = ictx.Config["ComServerUniqueId"];
            _thisComServer = serviceClientComServer.GetServerByGuid(guid);
            if (_thisComServer == null)
            {
                ictx.Log.Error($"Com Server With Guid {guid} Not Found");
                return false;
            }

            if (string.IsNullOrEmpty(_thisComServer.TftpPath))
            {
                ictx.Log.Error($"Com Server With Guid {guid} Does Not Have A Valid Tftp Path");
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
                    ictx.Log.Error("Could Not Download Kernel On Com Server: " + _thisComServer.DisplayName);
                    ictx.Log.Error(ex.Message);
                    return false;
                }
            }
        }
    }
}
