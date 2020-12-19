using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toems_ApiCalls;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity.Remotely;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceRemoteAccess
    {
        private readonly UnitOfWork _uow;

        public ServiceRemoteAccess()
        {
            _uow = new UnitOfWork();
        }

        public bool VerifyRemoteAccessInstalled(int comServerId)
        {
            var comServer = _uow.ClientComServerRepository.GetById(comServerId);
            var intercomKey = ServiceSetting.GetSettingValue(SettingStrings.IntercomKeyEncrypted);
            var decryptedKey = new EncryptionServices().DecryptText(intercomKey);

            return new APICall().ClientComServerApi.VerifyRemoteAccessInstalled(comServer.Url, "", decryptedKey);
        }

        public DtoActionResult InitializeRemotelyServer(int comServerId)
        {
            var comServer = _uow.ClientComServerRepository.GetById(comServerId);
            if(comServer == null)
            {
                return new DtoActionResult() { Success = false, ErrorMessage = "Com Server Not Found" };
            }

            if(string.IsNullOrEmpty(comServer.RemoteAccessUrl))
            {
                return new DtoActionResult { Success = false, ErrorMessage = "Could Not Initialize Remote Access.  The Url Was Empty" };
            }

            var servicePassGen = new PasswordGenerator();
            var remotelyUser = new RemotelyUser();
            remotelyUser.Username = servicePassGen.GeneratePassword(true, true, true, false, 10);

            for (int i = 0; i < 1000; i++)
            {
                remotelyUser.Password = servicePassGen.GeneratePassword(true, true, true, true, 16);
                if (servicePassGen.ValidatePassword(remotelyUser.Password, true))
                    break;
            }
                
            var response = new APICall().RemoteAccessApi.CreateRemotelyFirstUser(comServer.RemoteAccessUrl,remotelyUser);

            if(response == null)
            {
                return new DtoActionResult { Success = false, ErrorMessage = "Unknown Error While Initializing The Remote Access Server.  Check The Logs." };
            }

            var trimmed = response.Replace("\"", "");
            var final = trimmed.Replace("\\", "");

            if (final.Equals("The Remote Access Server Has Already Been Initialized"))
                return new DtoActionResult() { Success = false, ErrorMessage = "The Remote Access Server Has Already Been Initialized" };

            comServer.RaUsername = remotelyUser.Username;
            comServer.RaPasswordEncrypted = new EncryptionServices().EncryptText(remotelyUser.Password);
            comServer.RaAuthHeaderEncrypted = new EncryptionServices().EncryptText(final);

            var result = new ServiceClientComServer().Update(comServer);
            if (result != null)
            {
                if (result.Success)
                    return new DtoActionResult { Success = true };
                else
                    return new DtoActionResult { Success = false, ErrorMessage = result.ErrorMessage };
            }
            return new DtoActionResult { Success = false, ErrorMessage = "Unknown Error" };
        }
    }
}
