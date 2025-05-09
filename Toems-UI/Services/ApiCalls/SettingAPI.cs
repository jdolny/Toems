﻿using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{
    public class SettingAPI : BaseAPI<EntitySetting>
    {

        public SettingAPI(string resource, ProtectedLocalStorage protectedLocalStorage) : base(resource, protectedLocalStorage)
        {

        }


        public bool CheckExpiredToken()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/CheckExpiredToken/", Resource);
            var result = _apiRequest.ExecuteExpired<DtoApiBoolResponse>(Request);
            return result;
        }

        public string GetSharedLibraryVersion()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetSharedLibraryVersion/", Resource);
            var result = _apiRequest.Execute<DtoApiStringResponse>(Request);
            if (result != null)
                return result.Value;
            else
                return "";
        }

        public string GetApplicationVersion()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetApplicationVersion/", Resource);
            var result = _apiRequest.Execute<DtoApiStringResponse>(Request);
            if (result != null)
                return result.Value;
            else
                return "";
        }

        public string CheckMfaEnabled()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/CheckMfaEnabled/", Resource);
            var result = _apiRequest.Execute<DtoApiStringResponse>(Request);
            if (result != null)
                return result.Value;
            else
                return "";
        }



        public EntitySetting GetSetting(string name)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetSetting/", Resource);
            Request.AddParameter("name", name);
            return _apiRequest.Execute<EntitySetting>(Request);
        }

        public bool SendEmailTest()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/SendEmailTest/", Resource);
            var response = _apiRequest.Execute<DtoApiBoolResponse>(Request);
            return response != null && response.Value;
        }

        public bool IsStorageRemote()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/IsStorageRemote/", Resource);
            var response = _apiRequest.Execute<DtoApiBoolResponse>(Request);
            return response != null && response.Value;
        }

        public bool TestADBind()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/TestADBind/", Resource);
            var response = _apiRequest.Execute<DtoApiBoolResponse>(Request);
            return response != null && response.Value;
        }

        public bool UpdateSettings(List<EntitySetting> listSettings)
        {
            Request.Method = Method.Post;
            Request.Resource = string.Format("{0}/UpdateSettings/", Resource);
            Request.AddParameter("application/json", JsonConvert.SerializeObject(listSettings), ParameterType.RequestBody);
            var response = _apiRequest.Execute<DtoApiBoolResponse>(Request);
            return response != null && response.Value;
        }

        public bool UpdatePxeSettings(List<EntitySetting> listSettings)
        {
            Request.Method = Method.Post;
            Request.Resource = string.Format("{0}/UpdatePxeSettings/", Resource);
            Request.AddParameter("application/json", JsonConvert.SerializeObject(listSettings), ParameterType.RequestBody);
            var response = _apiRequest.Execute<DtoApiBoolResponse>(Request);
            return response != null && response.Value;
        }

        public IEnumerable<EntityCertificate> GetCAInt()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetCAInt/", Resource);
            return _apiRequest.Execute<List<EntityCertificate>>(Request);
        }

        public bool GenerateCAInt()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GenerateCAInt/", Resource);
            var response = _apiRequest.Execute<DtoApiBoolResponse>(Request);
            return response != null && response.Value;
        }

        public byte[] ExportCert(int id)
        {
            Request.Method = Method.Post;
            Request.Resource = string.Format("{0}/ExportCert/{1}", Resource,id);
            return _apiRequest.ExecuteRaw(Request);
        }

        public string GetClientInstallArgs()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetClientInstallArgs/", Resource);

            var response = _apiRequest.Execute<DtoApiStringResponse>(Request);
            if (response != null)
            {
                if (response.Value != null)
                    return response.Value;
                else
                {
                    return string.Empty;
                }
            }
            return string.Empty;
        }

        public string GetMeshAdminPass()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetMeshAdminPass/", Resource);
            var response = _apiRequest.Execute<DtoApiStringResponse>(Request);
            if (response != null)
            {
                if (response.Value != null)
                    return response.Value;
                else
                {
                    return string.Empty;
                }
            }
            return string.Empty;
        }

        public string GetMeshControlPass()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetMeshControlPass/", Resource);
            var response = _apiRequest.Execute<DtoApiStringResponse>(Request);
            if (response != null)
            {
                if (response.Value != null)
                    return response.Value;
                else
                {
                    return string.Empty;
                }
            }
            return string.Empty;
        }

        public string GetMeshViewPass()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetMeshViewPass/", Resource);
            var response = _apiRequest.Execute<DtoApiStringResponse>(Request);
            if (response != null)
            {
                if (response.Value != null)
                    return response.Value;
                else
                {
                    return string.Empty;
                }
            }
            return string.Empty;
        }

        public bool CopyPxeBinaries()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/CopyPxeBinaries/", Resource);
            var response = _apiRequest.Execute<DtoApiBoolResponse>(Request);
            return response != null && response.Value;
        }

        public bool CreateDefaultBootMenu(DtoBootMenuGenOptions defaultMenuOptions)
        {
            Request.Method = Method.Post;
            Request.Resource = string.Format("{0}/CreateDefaultBootMenu/", Resource);
            Request.AddParameter("application/json", JsonConvert.SerializeObject(defaultMenuOptions), ParameterType.RequestBody);
            var response = _apiRequest.Execute<DtoApiBoolResponse>(Request);
            return response != null && response.Value;
        }

        public bool CopyToecUpgrade()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/CopyToecUpgrade/", Resource);
            var response = _apiRequest.Execute<DtoApiBoolResponse>(Request);
            return response != null && response.Value;
        }

        public byte[] GenerateIso(DtoIsoGenOptions isoOptions)
        {
            Request.Method = Method.Post;
            Request.Resource = string.Format("{0}/GenerateISO/", Resource);
            Request.AddParameter("application/json", JsonConvert.SerializeObject(isoOptions), ParameterType.RequestBody);
            return _apiRequest.ExecuteRaw(Request);
        }

        public DtoActionResult GenerateWie(DtoWieConfig wieConfig)
        {
            Request.Method = Method.Post;
            Request.Resource = string.Format("{0}/GenerateWie/", Resource);
            Request.AddParameter("application/json", JsonConvert.SerializeObject(wieConfig), ParameterType.RequestBody);
            var response = _apiRequest.Execute<DtoActionResult>(Request);
            if (response == null)
            {
                return new DtoActionResult()
                {
                    ErrorMessage = "Unknown Exception.  Check The Exception Logs For More Info.",
                    Success = false
                };
            }
            return response;
        }

        public byte[] ExportMsi(bool is64bit)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/ExportMsi/", Resource);
            Request.AddParameter("is64bit", is64bit);
            return _apiRequest.ExecuteRaw(Request);

        }

        public string GetMsiFileName(bool is64bit)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetMsiFileName/", Resource);
            Request.AddParameter("is64bit", is64bit);
            var response = _apiRequest.Execute<DtoApiStringResponse>(Request);
            if (response != null)
            {
                if (response.Value != null)
                    return response.Value;
                else
                {
                    return string.Empty;
                }
            }
            return string.Empty;

        }

        public IEnumerable<EntityDefaultImageReplicationServer> GetDefaultImageReplicationComServers()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetDefaultImageReplicationComServers/", Resource);
            return _apiRequest.Execute<List<EntityDefaultImageReplicationServer>>(Request);
        }

        public DtoActionResult UpdateDefaultReplicationServers(List<EntityDefaultImageReplicationServer> imageReplicationServers)
        {
            Request.Method = Method.Post;
            Request.Resource = string.Format("{0}/UpdateDefaultReplicationServers/", Resource);
            Request.AddParameter("application/json", JsonConvert.SerializeObject(imageReplicationServers), ParameterType.RequestBody);
            var response = _apiRequest.Execute<DtoActionResult>(Request);
            if (response != null)
            {
                if (response.Id == 0)
                    response.Success = false;
            }
            else
            {
                return new DtoActionResult()
                {
                    ErrorMessage = "Unknown Exception.  Check The Exception Logs For More Info.",
                    Success = false
                };
            }
            return response;
        }


    }
}