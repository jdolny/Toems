using System;
using System.Collections.Generic;
using System.Web.Http;
using log4net;
using Toems_ClientApi.Controllers.Authorization;
using Toems_ClientApi.Hubs;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Dto.client;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_Service;
using Toems_Service.Entity;
using Toems_Service.Workflows;

namespace Toems_ClientApi.Controllers
{
    public class ProvisionController : ApiController
    {
        private static readonly ILog Logger =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public DtoApiStringResponse GetToecApiVersion()
        {
            return new DtoApiStringResponse() { Value = ToecApiStrings.ToecApiVersion };
        }

        [HttpGet]
        public string VerifyDb()
        {
            return new ServiceSetting().GetSetting(SettingStrings.CheckinInterval).Value;
        }

        [ProvisionAuth]
        [HttpGet]
        public DtoApiBoolResponse ComConnectionTest()
        {
            return new DtoApiBoolResponse() {Value = true};
        }

        [ProvisionAuth]
        [HttpGet]
        public List<DtoComServerConnection> ComConnections()
        {
            return new ServiceCurrentDownload().GetUsageCounts();
        }

        [ProvisionAuth]
        [HttpGet]
        public DtoClientStartupInfo GetStartupInfo()
        {
            var settingService = new ServiceSetting();
            var startupInfo = new DtoClientStartupInfo();
            startupInfo.DelayType =
                (EnumStartupDelay.DelayType)
                    Convert.ToInt16(settingService.GetSetting(SettingStrings.StartupDelayType).Value);
            startupInfo.SubDelay = settingService.GetSetting(SettingStrings.StartupDelaySub).Value;
            startupInfo.ThresholdWindow = settingService.GetSetting(SettingStrings.ThresholdWindow).Value;
            
            var versions = new ServiceVersion().GetVersions();
            if (versions == null)
            {
                startupInfo.IsError = true;
                startupInfo.ErrorMessage = "Could Not Determine Server Version";
                return startupInfo;
            }

            if (!versions.ExpectedToecApiVersion.Equals(ToecApiStrings.ToecApiVersion))
            {
                startupInfo.IsError = true;
                startupInfo.ErrorMessage = "Toec Api Version And Database Version Do Not Match.  The Toec Api Server May Need Updated.";
                return startupInfo;
            }

     
            startupInfo.ExpectedClientVersion = versions.LatestClientVersion;
            return startupInfo;
        }

        [ProvisionAuth]
        [HttpGet]
        public DtoProvisionResponse GetIntermediateCert()
        {
            var rawCert = new ServiceCertificate().GetIntermediatePublic();
            var response = new DtoProvisionResponse();
            if (rawCert != null)
            {
                response.Certificate = Convert.ToBase64String(rawCert);
                response.ProvisionStatus = EnumProvisionStatus.Status.IntermediateInstalled;

            }
            else
            {
                response.Certificate = null;
                response.ProvisionStatus = EnumProvisionStatus.Status.Error;
                response.Message = "Could Not Locate Intermediate Certificate On Server.";
            }
            return response;
        }

        [ProvisionAuth]
        [HttpPost]
        public DtoProvisionResponse ProvisionClient(DtoProvisionRequest request)
        {
            return new ServiceProvision().ProvisionClient(request);
        }

        [HttpPost]
        [CertificateAuth]
        public DtoProvisionResponse ConfirmProvisionRequest(DtoConfirmProvisionRequest request)
        {
            var computerService = new ServiceComputer();
            var computer = computerService.GetByGuid(request.Guid);
            if (computer == null)
                return new DtoProvisionResponse() { ProvisionStatus = EnumProvisionStatus.Status.Error };
            if (computer.ProvisionStatus == EnumProvisionStatus.Status.Reset)
                return new DtoProvisionResponse() { ProvisionStatus = EnumProvisionStatus.Status.Reset };
            computer.ProvisionStatus = EnumProvisionStatus.Status.Provisioned;
            computerService.UpdateComputer(computer);
            var response = new DtoProvisionResponse();
            var clientComServers = new GetCompEmServers().Run(request.Guid);
            if (clientComServers != null)
                response.ComServers = clientComServers;
            response.ProvisionStatus = EnumProvisionStatus.Status.Provisioned;
            return response;
            
        }

        [HttpPost]
        [CertificateAuth]
        public DtoProvisionResponse RenewSymmKey(DtoRenewKeyRequest request)
        {
            var response = new DtoProvisionResponse();
            var computerService = new ServiceComputer();
            var computer = computerService.GetByGuid(request.Guid);
            if(computer == null)
                return new DtoProvisionResponse() { ProvisionStatus = EnumProvisionStatus.Status.Error };
            if (computer.ProvisionStatus == EnumProvisionStatus.Status.Reset)
                return new DtoProvisionResponse() { ProvisionStatus = EnumProvisionStatus.Status.Reset };
            computer.ProvisionStatus = EnumProvisionStatus.Status.Provisioned;
            computer.SymmKeyEncrypted = new EncryptionServices().EncryptText(request.SymmKey);
            
            //computer may have been renamed, update the name
            var originalName = computer.Name;
            request.Name = request.Name.ToUpper();
            if (!request.Name.Equals(computer.Name))
            {
                computer.Name = request.Name;
                Logger.Debug("SymmKey Update Requires Computer Name Update");
                Logger.Debug("Old Name: " + originalName);
                Logger.Debug("New Name: " + request.Name);

                var doesExist = computerService.GetByName(request.Name);
                //a computer already exists with this name, what next
                if (doesExist != null)
                {
                    if (doesExist.ProvisionStatus == EnumProvisionStatus.Status.Provisioned)
                    {
                        Logger.Debug("An Active Computer With This Name Already Exists.  Cannot Renew SymmKey");
                        return new DtoProvisionResponse() { ProvisionStatus = EnumProvisionStatus.Status.Error };
                    }
                    else if (doesExist.ProvisionStatus == EnumProvisionStatus.Status.PreProvisioned)

                    {
                        Logger.Debug("Updating Name, Removing PreProvisioned Computer");
                        //new computer has preprovisioned with this name, allow provison to occur
                        //delete the doesExist entity, allowing the archived computer to be restored
                        new ServiceComputer().DeleteComputer(doesExist.Id); //requires a new instance or delete will fail

                    }
                    else if (doesExist.ProvisionStatus == EnumProvisionStatus.Status.ProvisionApproved ||
                                doesExist.ProvisionStatus == EnumProvisionStatus.Status.Reset)
                    {
                        Logger.Debug("Updating Name, Archiving Existing Computer");
                        //new computer has preprovisioned with this name, allow provison to occur
                        //delete the doesExist entity, allowing the archived computer to be restored
                        new ServiceComputer().ArchiveComputerKeepGroups(doesExist.Id); //requires a new instance or delete will fail
                    }
                    else
                    {
                        return new DtoProvisionResponse() { ProvisionStatus = EnumProvisionStatus.Status.Error };
                    }
                }
            }

            var result = computerService.UpdateComputer(computer);
            if (result == null)
            {
                Logger.Debug("Could Not Renew SymmKey.  Computer Name May Have Been Updated With An Existing Computer");
                return new DtoProvisionResponse() { ProvisionStatus = EnumProvisionStatus.Status.Error };
            }

            if (!result.Success)
            {
                Logger.Debug("Could Not Renew SymmKey.  Computer Name May Have Been Updated With An Existing Computer");
                return new DtoProvisionResponse() { ProvisionStatus = EnumProvisionStatus.Status.Error };
            }

            
            var clientComServers = new GetCompEmServers().Run(request.Guid);
            if (clientComServers != null)
                response.ComServers = clientComServers;
            response.ProvisionStatus = EnumProvisionStatus.Status.Provisioned;
            return response;
           
        }
    }
}
