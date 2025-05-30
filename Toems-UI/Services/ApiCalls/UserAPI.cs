﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{
    /// <summary>
    ///     Summary description for User
    /// </summary>
    public class UserAPI : BaseAPI<EntityToemsUser>
    {

        public UserAPI(string resource, ProtectedLocalStorage protectedLocalStorage) : base(resource, protectedLocalStorage)
        {

        }

        public DtoActionResult ChangePassword(EntityToemsUser tObject)
        {
            Request.Method = Method.Put;
            Request.AddParameter("application/json", JsonConvert.SerializeObject(tObject), ParameterType.RequestBody);
            Request.Resource = string.Format("{0}/ChangePassword/", Resource);
            var response = _apiRequest.Execute<DtoActionResult>(Request);
            if (response.Id == 0)
                response.Success = false;
            return response;
        }

      

        public bool DeleteRights(int id)
        {
            Request.Method = Method.Delete;
            Request.Resource = string.Format("{0}/DeleteRights/{1}", Resource, id);
            var response = _apiRequest.Execute<DtoApiBoolResponse>(Request);
            return response != null && response.Value;
        }

     

        public EntityToemsUser GetSelf()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetSelf/", Resource);
            return _apiRequest.Execute<EntityToemsUser>(Request);
        }

        public new List<EntityToemsUser> Search(DtoSearchFilter filter)
        {
            Request.Method = Method.Post;
            Request.Resource = $"{Resource}/Search";
            Request.AddParameter("application/json", JsonConvert.SerializeObject(filter), ParameterType.RequestBody);
            return _apiRequest.Execute<List<EntityToemsUser>>(Request);
        }

        public int GetAdminCount()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetAdminCount/", Resource);
            var response = _apiRequest.Execute<DtoApiIntResponse>(Request);
            return response != null ? response.Value : 0;
        }

        public EntityToemsUser GetByName(string username)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetByName/", Resource);
            Request.AddParameter("username", username);
            return _apiRequest.Execute<EntityToemsUser>(Request);
        }

       

        public DtoApiObjectResponse GetForLogin(string username)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetForLogin/", Resource);
            Request.AddParameter("username", username);
            var response = _apiRequest.Execute<DtoApiObjectResponse>(Request);

            if (response.Id == 0)
                response.Success = false;
            return response;
        }

        public IEnumerable<EntityUserRight> GetRights(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetRights/{1}", Resource, id);
            return _apiRequest.Execute<List<EntityUserRight>>(Request);
        }

        public IEnumerable<DtoPinnedPolicy> GetPinnedPolicies()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetPinnedPolicies/", Resource);
            return _apiRequest.Execute<List<DtoPinnedPolicy>>(Request);
        }

        public IEnumerable<DtoPinnedGroup> GetPinnedGroups()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetPinnedGroups/", Resource);
            return _apiRequest.Execute<List<DtoPinnedGroup>>(Request);
        }

        public IEnumerable<EntityAuditLog> GetUserAuditLogs(int id, DtoSearchFilter filter)
        {
            Request.Method = Method.Post;
            Request.Resource = string.Format("{0}/GetUserAuditLogs/{1}", Resource, id);
            Request.AddParameter("application/json", JsonConvert.SerializeObject(filter), ParameterType.RequestBody);
            return _apiRequest.Execute<List<EntityAuditLog>>(Request);
        }

        public List<DtoProcessWithTime> UserProcessTimes(DateTime dateCutoff, int limit, string userName)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/UserProcessTimes", Resource);
            Request.AddParameter("dateCutoff", dateCutoff);
            Request.AddParameter("limit", limit);
            Request.AddParameter("userName", userName);
            return _apiRequest.Execute<List<DtoProcessWithTime>>(Request);
        }

        public List<DtoProcessWithCount> UserProcessCounts(DateTime dateCutoff, int limit, string userName)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/UserProcessCounts", Resource);
            Request.AddParameter("dateCutoff", dateCutoff);
            Request.AddParameter("limit", limit);
            Request.AddParameter("userName", userName);
            return _apiRequest.Execute<List<DtoProcessWithCount>>(Request);
        }

        public List<DtoProcessWithUser> UserProcess(DateTime dateCutoff, int limit, string userName)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/UserProcess", Resource);
            Request.AddParameter("dateCutoff", dateCutoff);
            Request.AddParameter("limit", limit);
            Request.AddParameter("userName", userName);
            return _apiRequest.Execute<List<DtoProcessWithUser>>(Request);
        }

        public bool IsAdmin(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/IsAdmin/{1}", Resource, id);
            var response = _apiRequest.Execute<DtoApiBoolResponse>(Request);
            return response != null && response.Value;
        }

        public string GetUserComputerView()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetUserComputerView/", Resource);
            var response = _apiRequest.Execute<DtoApiStringResponse>(Request);
            if (response == null)
                return "";
            else
                return response.Value;
        }

        public EntityToemsUserOptions GetUserComputerOptions(int userId)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetUserComputerOptions/{1}", Resource,userId);
            return _apiRequest.Execute<EntityToemsUserOptions>(Request);
        }

        public DtoActionResult UpdateUserComputerOptions(EntityToemsUserOptions entityToemsUserOptions)
        {
            Request.Method = Method.Post;
            Request.AddParameter("application/json", JsonConvert.SerializeObject(entityToemsUserOptions), ParameterType.RequestBody);
            Request.Resource = $"{Resource}/UpdateOrInsertUserComputerOptions/";
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

        public string GetUserComputerSort()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetUserComputerSort/}", Resource);
            var response = _apiRequest.Execute<DtoApiStringResponse>(Request);
            if (response == null)
                return "";
            else
                return response.Value;
        }

        public string GetUserLoginPage()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetUserLoginPage/", Resource);
            var response = _apiRequest.Execute<DtoApiStringResponse>(Request);
            if (response == null)
                return "";
            else
                return response.Value;
        }

        public bool ResetUserMfaData(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/ResetUserMfaData/{1}", Resource, id);
            var response = _apiRequest.Execute<DtoApiBoolResponse>(Request);
            if (response == null)
                return false;
            else
                return response.Value;
        }

        public bool RemoveUserLegacyGroup(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/RemoveUserLegacyGroup/{1}", Resource,id);
            var response = _apiRequest.Execute<DtoApiBoolResponse>(Request);
            if (response == null)
                return false;
            else
                return response.Value;
        }

        public bool CheckMfaSetupComplete()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/CheckMfaSetupComplete/", Resource);
            var response = _apiRequest.Execute<DtoApiBoolResponse>(Request);
            if (response == null)
                return false;
            else
                return response.Value;
        }

        public string GenerateTempMfaSecret()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GenerateTempMfaSecret/", Resource);
            var response = _apiRequest.Execute<DtoApiStringResponse>(Request);
            if (response == null)
                return "";
            else
                return response.Value;
        }

        public bool VerifyMfaSecret(string code)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/VerifyMfaSecret/", Resource);
            Request.AddParameter("code", code);
            var response = _apiRequest.Execute<DtoApiBoolResponse>(Request);
            if (response == null)
                return false;
            else
                return response.Value;
        }

    }
}