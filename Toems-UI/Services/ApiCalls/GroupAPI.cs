using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{
    public class GroupAPI : BaseAPI<EntityGroup>
    {

        public GroupAPI(string resource, ProtectedLocalStorage protectedLocalStorage) : base(resource, protectedLocalStorage)
        {
            
        }
        public new List<DtoGroupWithCount> Search(DtoSearchFilterCategories filter)
        {
            Request.Method = Method.Post;
            Request.Resource = $"{Resource}/Search";
            Request.AddParameter("application/json", JsonConvert.SerializeObject(filter), ParameterType.RequestBody);
            return _apiRequest.Execute<List<DtoGroupWithCount>>(Request);
        }

        public new List<DtoGroupWithCount> Get()
        {
            Request.Method = Method.Get;
            Request.Resource = $"{Resource}/Get";
            return _apiRequest.Execute<List<DtoGroupWithCount>>(Request);
        }

        public IEnumerable<GroupPolicyDetailed> GetAssignedPolicies(int id, DtoSearchFilter filter)
        {
            Request.Method = Method.Post;
            Request.Resource = string.Format("{0}/GetAssignedPolicies/{1}", Resource, id);
            Request.AddParameter("application/json", JsonConvert.SerializeObject(filter), ParameterType.RequestBody);
            return _apiRequest.Execute<List<GroupPolicyDetailed>>(Request);
        }

        public IEnumerable<EntityComputer> GetGroupMembers(int id, DtoSearchFilter filter)
        {
            Request.Method = Method.Post;
            Request.Resource = string.Format("{0}/GetGroupMembers/{1}", Resource, id);
            Request.AddParameter("application/json", JsonConvert.SerializeObject(filter), ParameterType.RequestBody);
            return _apiRequest.Execute<List<EntityComputer>>(Request);
        }

        public IEnumerable<EntityGroup> GetAdGroups()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetAdGroups/", Resource);
            return _apiRequest.Execute<List<EntityGroup>>(Request);
        }

        public IEnumerable<EntityGroup> GetOuGroups()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetOuGroups/", Resource);
            return _apiRequest.Execute<List<EntityGroup>>(Request);
        }

        public IEnumerable<EntityGroup> GetAdSecurityGroups()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetAdSecurityGroups/", Resource);
            return _apiRequest.Execute<List<EntityGroup>>(Request);
        }

        public IEnumerable<EntityGroupCategory> GetGroupCategories(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetGroupCategories/{1}", Resource, id);
            return _apiRequest.Execute<List<EntityGroupCategory>>(Request);
        }

        public DtoActionResult ClearImagingIds(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/ClearImagingIds/{1}", Resource, id);
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

        public IEnumerable<EntitySmartGroupQuery> GetDynamicQuery(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetDynamicQuery/{1}", Resource,id);
            return _apiRequest.Execute<List<EntitySmartGroupQuery>>(Request);
        }

        public DataSet GetDynamic(List<EntitySmartGroupQuery> queries)
        {
            Request.Method = Method.Post;
            Request.AddParameter("application/json", JsonConvert.SerializeObject(queries), ParameterType.RequestBody);
            Request.Resource = string.Format("{0}/GetDynamic/", Resource);
            var response = _apiRequest.Execute<DtoApiStringResponse>(Request);
            if (response != null)
                return JsonConvert.DeserializeObject<DataSet>(response.Value);
            return null;
        }

        public bool UpdateDynamicQuery(List<EntitySmartGroupQuery> queries)
        {
            Request.Method = Method.Post;
            Request.AddParameter("application/json", JsonConvert.SerializeObject(queries), ParameterType.RequestBody);
            Request.Resource = string.Format("{0}/UpdateDynamicQuery/", Resource);
            var response = _apiRequest.Execute<DtoApiBoolResponse>(Request);
            if (response != null) return response.Value;
                return false;
        }

        public string GetMemberCount(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetMemberCount/{1}", Resource, id);
            var response = _apiRequest.Execute<DtoApiStringResponse>(Request);
            return response != null ? response.Value : string.Empty;
        }

        public bool RemoveGroupMember(int id, int computerId)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/RemoveGroupMember/{1}", Resource, id);
            Request.AddParameter("computerId", computerId);
            var response = _apiRequest.Execute<DtoApiBoolResponse>(Request);
            return response != null && response.Value;
        }

        public void SendMessage(int id, DtoMessage message)
        {
            Request.Method = Method.Post;
            Request.Resource = string.Format("{0}/SendMessage/{1}", Resource, id);
            Request.AddParameter("application/json", JsonConvert.SerializeObject(message), ParameterType.RequestBody);
#pragma warning disable CS4014
            _apiRequest.ExecuteAsync<DtoApiBoolResponse>(Request);
#pragma warning restore CS4014
        }

        public void Reboot(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/Reboot/{1}", Resource, id);
#pragma warning disable CS4014
            _apiRequest.ExecuteAsync<DtoApiBoolResponse>(Request);
#pragma warning restore CS4014
        }

        public void Shutdown(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/Shutdown/{1}", Resource, id);
#pragma warning disable CS4014
            _apiRequest.ExecuteAsync<DtoApiBoolResponse>(Request);
#pragma warning restore CS4014
        }

        public void Wakeup(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/Wakeup/{1}", Resource, id);
#pragma warning disable CS4014
            _apiRequest.ExecuteAsync<DtoApiBoolResponse>(Request);
#pragma warning restore CS4014
        }

        public void ForceCheckin(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/ForceCheckin/{1}", Resource, id);
#pragma warning disable CS4014
            _apiRequest.ExecuteAsync<DtoApiBoolResponse>(Request);
#pragma warning restore CS4014
        }

        public void CollectInventory(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/CollectInventory/{1}", Resource, id);
#pragma warning disable CS4014
            _apiRequest.ExecuteAsync<DtoApiBoolResponse>(Request);
#pragma warning restore CS4014
        }

        public List<DtoProcessWithTime> GroupProcessTimes(DateTime dateCutoff, int limit, int groupId)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GroupProcessTimes", Resource);
            Request.AddParameter("dateCutoff", dateCutoff);
            Request.AddParameter("limit", limit);
            Request.AddParameter("groupId", groupId);
            return _apiRequest.Execute<List<DtoProcessWithTime>>(Request);
        }

        public List<DtoProcessWithCount> GroupProcessCounts(DateTime dateCutoff, int limit, int groupId)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GroupProcessCounts", Resource);
            Request.AddParameter("dateCutoff", dateCutoff);
            Request.AddParameter("limit", limit);
            Request.AddParameter("groupId", groupId);
            return _apiRequest.Execute<List<DtoProcessWithCount>>(Request);
        }

        public int StartGroupUnicast(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/StartGroupUnicast/{1}", Resource, id);
            var response = _apiRequest.Execute<DtoApiIntResponse>(Request);
            return response != null ? response.Value : 0;
        }

        public bool StartGroupWinPe(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/StartGroupWinPe/{1}", Resource, id);
            var response = _apiRequest.Execute<DtoApiBoolResponse>(Request);
            return response != null ? response.Value : false;
        }

        public string StartMulticast(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/StartMulticast/{1}", Resource, id);
            var response = _apiRequest.Execute<DtoApiStringResponse>(Request);
            return response != null ? response.Value : string.Empty;
        }
    }
}