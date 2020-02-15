using System.Collections.Generic;
using System.Configuration;
using log4net;
using RoboSharp;
using Toems_ApiCalls;
using Toems_Common;
using Toems_Common.Dto;
using Toems_DataModel;
using Toems_Service.Entity;

//https://code.msdn.microsoft.com/windowsdesktop/File-Sync-with-Simple-c497bf87/sourcecode?fileId=19013&pathId=1314099233

namespace Toems_Service.Workflows
{
    public class ComServerFreeSpace
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public List<DtoFreeSpace> RunAllServers()
        {
            var uow = new UnitOfWork();
            var comServers = uow.ClientComServerRepository.Get();
           
            var intercomKey = ServiceSetting.GetSettingValue(SettingStrings.IntercomKeyEncrypted);
            var decryptedKey = new EncryptionServices().DecryptText(intercomKey);

            var list = new List<DtoFreeSpace>();
            foreach (var com in comServers)
            {
                var free = new DtoFreeSpace();
                free = new APICall().ClientComServerApi.GetFreeSpace(com.Url, "", decryptedKey);
                list.Add(free);
            }

            return list;

        }

        public DtoFreeSpace GetFreeSpace()
        {
            return new FilesystemServices().GetComServerFreeSpace();
        }

      

       
    }
}
