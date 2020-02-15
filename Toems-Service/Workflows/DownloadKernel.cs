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

namespace Toems_Service.Workflows
{
    public class DownloadKernel
    {
        private EntityClientComServer _thisComServer;

        private readonly ILog log = LogManager.GetLogger(typeof(DefaultBootMenu));

        public bool RunAllServers(DtoOnlineKernel onlineKernel)
        {
            var uow = new UnitOfWork();
            var comServers = uow.ClientComServerRepository.Get(x => x.IsTftpServer);

            var intercomKey = ServiceSetting.GetSettingValue(SettingStrings.IntercomKeyEncrypted);
            var decryptedKey = new EncryptionServices().DecryptText(intercomKey);
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
            var guid = ConfigurationManager.AppSettings["ComServerUniqueId"];
            _thisComServer = new ServiceClientComServer().GetServerByGuid(guid);
            if (_thisComServer == null)
            {
                log.Error($"Com Server With Guid {guid} Not Found");
                return false;
            }

            if (string.IsNullOrEmpty(_thisComServer.TftpPath))
            {
                log.Error($"Com Server With Guid {guid} Does Not Have A Valid Tftp Path");
                return false;
            }

            var baseUrl = "http://files.clonedeploy.org/kernels/";
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
                    log.Error(ex.Message);
                    return false;
                }
            }
        }
    }
}
