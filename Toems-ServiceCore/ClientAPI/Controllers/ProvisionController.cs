using System;
using System.Collections.Generic;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Toems_ClientApi.Controllers.Authorization;
using Toems_ClientApi.Hubs;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Dto.client;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_ServiceCore.EntityServices;
using Toems_ServiceCore.Infrastructure;
using Toems_ServiceCore.Workflows;


namespace Toems_ClientApi.Controllers
{
    [ApiController]
    [Route("api/client/[controller]")]
    public class ProvisionController(ServiceContext ctx) : ControllerBase
    {
        
        public DtoApiStringResponse GetToecApiVersion()
        {
            return new DtoApiStringResponse() { Value = ToecApiStrings.ToecApiVersion };
        }

        [HttpGet]
        public string VerifyDb()
        {
            return ctx.Setting.GetSetting(SettingStrings.CheckinInterval).Value;
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
            return ctx.CurrentDownload.GetUsageCounts();
        }

        [ProvisionAuth]
        [HttpGet]
        public DtoClientStartupInfo GetStartupInfo()
        {
            var startupInfo = new DtoClientStartupInfo();
            startupInfo.DelayType =
                (EnumStartupDelay.DelayType)
                    Convert.ToInt16(ctx.Setting.GetSetting(SettingStrings.StartupDelayType).Value);
            startupInfo.SubDelay = ctx.Setting.GetSetting(SettingStrings.StartupDelaySub).Value;
            startupInfo.ThresholdWindow = ctx.Setting.GetSetting(SettingStrings.ThresholdWindow).Value;
            
            var versions = ctx.Version.GetVersions();
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
            var rawCert = ctx.Certificate.GetIntermediatePublic();
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
            return ctx.Provision.ProvisionClient(request);
        }

        [HttpPost]
        [CertificateAuth]
        public DtoProvisionResponse ConfirmProvisionRequest(DtoConfirmProvisionRequest request)
        {
            var computer = ctx.Computer.GetByGuid(request.Guid);
            if (computer == null)
                return new DtoProvisionResponse() { ProvisionStatus = EnumProvisionStatus.Status.Error };
            if (computer.ProvisionStatus == EnumProvisionStatus.Status.Reset)
                return new DtoProvisionResponse() { ProvisionStatus = EnumProvisionStatus.Status.Reset };
            computer.ProvisionStatus = EnumProvisionStatus.Status.Provisioned;
            ctx.Computer.UpdateComputer(computer);
            var response = new DtoProvisionResponse();
            var clientComServers = ctx.GetCompEmServers.Run(request.Guid);
            if (clientComServers != null)
                response.ComServers = clientComServers;
            response.ProvisionStatus = EnumProvisionStatus.Status.Provisioned;
            ctx.ProvisionCompleteTasks.Run(computer);
            return response;
            
        }

        [HttpPost]
        [CertificateAuth]
        public DtoProvisionResponse RenewSymmKey(DtoRenewKeyRequest request)
        {
            var response = new DtoProvisionResponse();
            var computer = ctx.Computer.GetByGuid(request.Guid);
            if(computer == null)
                return new DtoProvisionResponse() { ProvisionStatus = EnumProvisionStatus.Status.Error };
            if (computer.ProvisionStatus == EnumProvisionStatus.Status.Reset)
                return new DtoProvisionResponse() { ProvisionStatus = EnumProvisionStatus.Status.Reset };
            computer.ProvisionStatus = EnumProvisionStatus.Status.Provisioned;
            computer.SymmKeyEncrypted = ctx.Encryption.EncryptText(request.SymmKey);
            
            //computer may have been renamed, update the name
            var originalName = computer.Name;
            request.Name = request.Name.ToUpper();
            if (!request.Name.Equals(computer.Name))
            {
                computer.Name = request.Name;
                ctx.Log.Debug("SymmKey Update Requires Computer Name Update");
                ctx.Log.Debug("Old Name: " + originalName);
                ctx.Log.Debug("New Name: " + request.Name);

                var doesExist = ctx.Computer.GetByName(request.Name);
                //a computer already exists with this name, what next
                if (doesExist != null)
                {
                    if (doesExist.ProvisionStatus == EnumProvisionStatus.Status.Provisioned)
                    {
                        ctx.Log.Debug("An Active Computer With This Name Already Exists.  Cannot Renew SymmKey");
                        return new DtoProvisionResponse() { ProvisionStatus = EnumProvisionStatus.Status.Error };
                    }
                    else if (doesExist.ProvisionStatus == EnumProvisionStatus.Status.PreProvisioned)

                    {
                        ctx.Log.Debug("Updating Name, Removing PreProvisioned Computer");
                        //new computer has preprovisioned with this name, allow provison to occur
                        //delete the doesExist entity, allowing the archived computer to be restored
                        ctx.Computer.DeleteComputer(doesExist.Id); //requires a new instance or delete will fail

                    }
                    else if (doesExist.ProvisionStatus == EnumProvisionStatus.Status.ProvisionApproved ||
                                doesExist.ProvisionStatus == EnumProvisionStatus.Status.Reset)
                    {
                        ctx.Log.Debug("Updating Name, Archiving Existing Computer");
                        //new computer has preprovisioned with this name, allow provison to occur
                        //delete the doesExist entity, allowing the archived computer to be restored
                        ctx.Computer.ArchiveComputerKeepGroups(doesExist.Id); //requires a new instance or delete will fail
                    }
                    else
                    {
                        return new DtoProvisionResponse() { ProvisionStatus = EnumProvisionStatus.Status.Error };
                    }
                }
            }

            var result = ctx.Computer.UpdateComputer(computer);
            if (result == null)
            {
                ctx.Log.Debug("Could Not Renew SymmKey.  Computer Name May Have Been Updated With An Existing Computer");
                return new DtoProvisionResponse() { ProvisionStatus = EnumProvisionStatus.Status.Error };
            }

            if (!result.Success)
            {
                ctx.Log.Debug("Could Not Renew SymmKey.  Computer Name May Have Been Updated With An Existing Computer");
                return new DtoProvisionResponse() { ProvisionStatus = EnumProvisionStatus.Status.Error };
            }

            
            var clientComServers = ctx.GetCompEmServers.Run(request.Guid);
            if (clientComServers != null)
                response.ComServers = clientComServers;
            response.ProvisionStatus = EnumProvisionStatus.Status.Provisioned;
            return response;
           
        }
    }
}
