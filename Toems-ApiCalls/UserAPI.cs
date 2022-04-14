using System;
using System.Collections.Generic;
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
        private readonly ApiRequest _apiRequest;

        public UserAPI(string resource) : base(resource)
        {
            _apiRequest = new ApiRequest();
        }

        public DtoActionResult ChangePassword(EntityToemsUser tObject)
        {
            Request.Method = Method.PUT;
            Request.AddJsonBody(tObject);
            Request.Resource = string.Format("{0}/ChangePassword/", Resource);
            var response = _apiRequest.Execute<DtoActionResult>(Request);
            if (response.Id == 0)
                response.Success = false;
            return response;
        }

      

        public bool DeleteRights(int id)
        {
            Request.Method = Method.DELETE;
            Request.Resource = string.Format("{0}/DeleteRights/{1}", Resource, id);
            var response = _apiRequest.Execute<DtoApiBoolResponse>(Request);
            return response != null && response.Value;
        }

     

        public EntityToemsUser GetSelf()
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetSelf/", Resource);
            return _apiRequest.Execute<EntityToemsUser>(Request);
        }

        public new List<UserWithUserGroup> Search(DtoSearchFilter filter)
        {
            Request.Method = Method.POST;
            Request.Resource = $"{Resource}/Search";
            Request.AddJsonBody(filter);
            return new ApiRequest().Execute<List<UserWithUserGroup>>(Request);
        }

        public int GetAdminCount()
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetAdminCount/", Resource);
            var response = _apiRequest.Execute<DtoApiIntResponse>(Request);
            return response != null ? response.Value : 0;
        }

        public EntityToemsUser GetByName(string username)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetByName/", Resource);
            Request.AddParameter("username", username);
            return _apiRequest.Execute<EntityToemsUser>(Request);
        }

       

        public DtoApiObjectResponse GetForLogin(string username)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetForLogin/", Resource);
            Request.AddParameter("username", username);
            var response = _apiRequest.Execute<DtoApiObjectResponse>(Request);

            if (response.Id == 0)
                response.Success = false;
            return response;
        }

        public IEnumerable<EntityUserRight> GetRights(int id)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetRights/{1}", Resource, id);
            return _apiRequest.Execute<List<EntityUserRight>>(Request);
        }

        public IEnumerable<DtoPinnedPolicy> GetPinnedPolicies()
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetPinnedPolicies/", Resource);
            return _apiRequest.Execute<List<DtoPinnedPolicy>>(Request);
        }

        public IEnumerable<DtoPinnedGroup> GetPinnedGroups()
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetPinnedGroups/", Resource);
            return _apiRequest.Execute<List<DtoPinnedGroup>>(Request);
        }

        public IEnumerable<EntityAuditLog> GetUserAuditLogs(int id, DtoSearchFilter filter)
        {
            Request.Method = Method.POST;
            Request.Resource = string.Format("{0}/GetUserAuditLogs/{1}", Resource, id);
            Request.AddJsonBody(filter);
            return _apiRequest.Execute<List<EntityAuditLog>>(Request);
        }

        public List<DtoProcessWithTime> UserProcessTimes(DateTime dateCutoff, int limit, string userName)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/UserProcessTimes", Resource);
            Request.AddParameter("dateCutoff", dateCutoff);
            Request.AddParameter("limit", limit);
            Request.AddParameter("userName", userName);
            return new ApiRequest().Execute<List<DtoProcessWithTime>>(Request);
        }

        public List<DtoProcessWithCount> UserProcessCounts(DateTime dateCutoff, int limit, string userName)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/UserProcessCounts", Resource);
            Request.AddParameter("dateCutoff", dateCutoff);
            Request.AddParameter("limit", limit);
            Request.AddParameter("userName", userName);
            return new ApiRequest().Execute<List<DtoProcessWithCount>>(Request);
        }

        public List<DtoProcessWithUser> UserProcess(DateTime dateCutoff, int limit, string userName)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/UserProcess", Resource);
            Request.AddParameter("dateCutoff", dateCutoff);
            Request.AddParameter("limit", limit);
            Request.AddParameter("userName", userName);
            return new ApiRequest().Execute<List<DtoProcessWithUser>>(Request);
        }

        public bool IsAdmin(int id)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/IsAdmin/{1}", Resource, id);
            var response = _apiRequest.Execute<DtoApiBoolResponse>(Request);
            return response != null && response.Value;
        }

        public string GetUserComputerView()
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetUserComputerView/", Resource);
            var response = _apiRequest.Execute<DtoApiStringResponse>(Request);
            if (response == null)
                return "";
            else
                return response.Value;
        }

        public string GetUserComputerSort()
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetUserComputerSort/}", Resource);
            var response = _apiRequest.Execute<DtoApiStringResponse>(Request);
            if (response == null)
                return "";
            else
                return response.Value;
        }

        public string GetUserLoginPage()
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetUserLoginPage/", Resource);
            var response = _apiRequest.Execute<DtoApiStringResponse>(Request);
            if (response == null)
                return "";
            else
                return response.Value;
        }

    }
}