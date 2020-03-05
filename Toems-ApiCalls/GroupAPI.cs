using System;
using System.Collections.Generic;
using System.Data;
using Newtonsoft.Json;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{
    public class GroupAPI : BaseAPI<EntityGroup>
    {

        public GroupAPI(string resource) : base(resource)
        {
            
        }

        public IEnumerable<GroupPolicyDetailed> GetAssignedPolicies(int id, DtoSearchFilter filter)
        {
            Request.Method = Method.POST;
            Request.Resource = string.Format("{0}/GetAssignedPolicies/{1}", Resource, id);
            Request.AddJsonBody(filter);
            return new ApiRequest().Execute<List<GroupPolicyDetailed>>(Request);
        }

        public IEnumerable<EntityComputer> GetGroupMembers(int id, DtoSearchFilter filter)
        {
            Request.Method = Method.POST;
            Request.Resource = string.Format("{0}/GetGroupMembers/{1}", Resource, id);
            Request.AddJsonBody(filter);
            return new ApiRequest().Execute<List<EntityComputer>>(Request);
        }

        public IEnumerable<EntityGroup> GetAdGroups()
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetAdGroups/", Resource);
            return new ApiRequest().Execute<List<EntityGroup>>(Request);
        }

        public IEnumerable<EntityGroupCategory> GetGroupCategories(int id)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetGroupCategories/{1}", Resource, id);
            return new ApiRequest().Execute<List<EntityGroupCategory>>(Request);
        }

        public DtoActionResult ClearImagingIds(int id)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/ClearImagingIds/{1}", Resource, id);
            var response = new ApiRequest().Execute<DtoActionResult>(Request);
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
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetDynamicQuery/{1}", Resource,id);
            return new ApiRequest().Execute<List<EntitySmartGroupQuery>>(Request);
        }

        public DataSet GetDynamic(List<EntitySmartGroupQuery> queries)
        {
            Request.Method = Method.POST;
            Request.AddJsonBody(queries);
            Request.Resource = string.Format("{0}/GetDynamic/", Resource);
            var response = new ApiRequest().Execute<DtoApiStringResponse>(Request);
            if (response != null)
                return JsonConvert.DeserializeObject<DataSet>(response.Value);
            return null;
        }

        public bool UpdateDynamicQuery(List<EntitySmartGroupQuery> queries)
        {
            Request.Method = Method.POST;
            Request.AddJsonBody(queries);
            Request.Resource = string.Format("{0}/UpdateDynamicQuery/", Resource);
            var response = new ApiRequest().Execute<DtoApiBoolResponse>(Request);
            if (response != null) return response.Value;
                return false;
        }

        public string GetMemberCount(int id)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetMemberCount/{1}", Resource, id);
            var response = new ApiRequest().Execute<DtoApiStringResponse>(Request);
            return response != null ? response.Value : string.Empty;
        }

        public bool RemoveGroupMember(int id, int computerId)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/RemoveGroupMember/{1}", Resource, id);
            Request.AddParameter("computerId", computerId);
            var response = new ApiRequest().Execute<DtoApiBoolResponse>(Request);
            return response != null && response.Value;
        }

        public void SendMessage(int id, DtoMessage message)
        {
            Request.Method = Method.POST;
            Request.Resource = string.Format("{0}/SendMessage/{1}", Resource, id);
            Request.AddJsonBody(message);
            new ApiRequest().ExecuteAsync<DtoApiBoolResponse>(Request);
        }

        public void Reboot(int id)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/Reboot/{1}", Resource, id);
            new ApiRequest().ExecuteAsync<DtoApiBoolResponse>(Request);
        }

        public void Shutdown(int id)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/Shutdown/{1}", Resource, id);
            new ApiRequest().ExecuteAsync<DtoApiBoolResponse>(Request);
        }

        public void Wakeup(int id)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/Wakeup/{1}", Resource, id);
            new ApiRequest().ExecuteAsync<DtoApiBoolResponse>(Request);
        }

        public void ForceCheckin(int id)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/ForceCheckin/{1}", Resource, id);
            new ApiRequest().ExecuteAsync<DtoApiBoolResponse>(Request);
        }

        public void CollectInventory(int id)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/CollectInventory/{1}", Resource, id);
            new ApiRequest().ExecuteAsync<DtoApiBoolResponse>(Request);
        }

        public List<DtoProcessWithTime> GroupProcessTimes(DateTime dateCutoff, int limit, int groupId)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GroupProcessTimes", Resource);
            Request.AddParameter("dateCutoff", dateCutoff);
            Request.AddParameter("limit", limit);
            Request.AddParameter("groupId", groupId);
            return new ApiRequest().Execute<List<DtoProcessWithTime>>(Request);
        }

        public List<DtoProcessWithCount> GroupProcessCounts(DateTime dateCutoff, int limit, int groupId)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GroupProcessCounts", Resource);
            Request.AddParameter("dateCutoff", dateCutoff);
            Request.AddParameter("limit", limit);
            Request.AddParameter("groupId", groupId);
            return new ApiRequest().Execute<List<DtoProcessWithCount>>(Request);
        }

        public int StartGroupUnicast(int id)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/StartGroupUnicast/{1}", Resource, id);
            var response = new ApiRequest().Execute<DtoApiIntResponse>(Request);
            return response != null ? response.Value : 0;
        }

        public string StartMulticast(int id)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/StartMulticast/{1}", Resource, id);
            var response = new ApiRequest().Execute<DtoApiStringResponse>(Request);
            return response != null ? response.Value : string.Empty;
        }
    }
}