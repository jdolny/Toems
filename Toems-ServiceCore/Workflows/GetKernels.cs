using Toems_ApiCalls;
using Toems_Common;
using Toems_Common.Entity;
using Toems_DataModel;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.Workflows
{
    public class GetKernels(ServiceContext ctx)
    {
        public List<string> Run()
        {
            var uow = new UnitOfWork();
            var tftpComServers = uow.ClientComServerRepository.Get(x => x.IsTftpServer);
            EntityClientComServer tftpInfoServer;
            if (tftpComServers.Count == 0)
            {
                ctx.Log.Error("No Tftp Servers Are Currently Enabled To Retrieve Kernel Listing");
                return null;
            }
            if (tftpComServers.Count > 1)
            {
                tftpInfoServer = tftpComServers.Where(x => x.IsTftpInfoServer).FirstOrDefault();
                if (tftpInfoServer == null)
                {
                    ctx.Log.Error("No Tftp Servers Are Currently Set As The Information Server.  Unable To Retrieve Kernel Listing");
                    return null;
                }
            }
            else
                tftpInfoServer = tftpComServers.First();

            //Connect To Client Com Server

            var intercomKey = ctx.Setting.GetSettingValue(SettingStrings.IntercomKeyEncrypted);
            var decryptedKey = ctx.Encryption.DecryptText(intercomKey);

            return new APICall().ClientComServerApi.GetKernels(tftpInfoServer.Url, "", decryptedKey);

        }
    }
}
