using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toems_ApiCalls;
using Toems_Common;
using Toems_Common.Entity;
using Toems_DataModel;
using Toems_Service.Entity;

namespace Toems_Service.Workflows
{
    public class GetBootImages
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public List<string> Run()
        {
            var uow = new UnitOfWork();
            var tftpComServers = uow.ClientComServerRepository.Get(x => x.IsTftpServer);
            EntityClientComServer tftpInfoServer;
            if (tftpComServers.Count == 0)
            {
                Logger.Error("No Tftp Servers Are Currently Enabled To Retrieve Kernel Listing");
                return null;
            }
            if (tftpComServers.Count > 1)
            {
                tftpInfoServer = tftpComServers.Where(x => x.IsTftpInfoServer).FirstOrDefault();
                if (tftpInfoServer == null)
                {
                    Logger.Error("No Tftp Servers Are Currently Set As The Information Server.  Unable To Retrieve Kernel Listing");
                    return null;
                }
            }
            else
                tftpInfoServer = tftpComServers.First();

            //Connect To Client Com Server

            var intercomKey = ServiceSetting.GetSettingValue(SettingStrings.IntercomKeyEncrypted);
            var decryptedKey = new EncryptionServices().DecryptText(intercomKey);

            return new APICall().ClientComServerApi.GetBootImages(tftpInfoServer.Url, "", decryptedKey);

        }
    }
}
