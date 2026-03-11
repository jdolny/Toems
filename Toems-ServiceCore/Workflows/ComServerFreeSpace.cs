using Toems_ApiCalls;
using Toems_Common;
using Toems_Common.Dto;
using Toems_DataModel;
using Toems_ServiceCore.Infrastructure;

//https://code.msdn.microsoft.com/windowsdesktop/File-Sync-with-Simple-c497bf87/sourcecode?fileId=19013&pathId=1314099233

namespace Toems_ServiceCore.Workflows
{
    public class ComServerFreeSpace(ServiceContext ctx)
    {
        public List<DtoFreeSpace> RunAllServers()
        {
            var uow = new UnitOfWork();
            var comServers = uow.ClientComServerRepository.Get();
           
            var intercomKey = ctx.Setting.GetSettingValue(SettingStrings.IntercomKeyEncrypted);
            var decryptedKey = ctx.Encryption.DecryptText(intercomKey);

            var list = new List<DtoFreeSpace>();
            foreach (var com in comServers)
            {
                var free = new DtoFreeSpace();
                free = new APICall().ClientComServerApi.GetFreeSpace(com.Url, "", decryptedKey);

                if (free == null) {
                    ctx.Log.Error("Com server returned null for status. Check your com server URL!");
                }
                else
                {
                    list.Add(free);
                }
            }

            return list;

        }

        public DtoFreeSpace GetFreeSpace()
        {
            return ctx.Filessystem.GetComServerFreeSpace();
        }

      

       
    }
}
