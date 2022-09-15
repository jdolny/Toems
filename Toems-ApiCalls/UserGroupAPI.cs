using System.Collections.Generic;
using Newtonsoft.Json;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{
    public class UserGroupAPI : BaseAPI<EntityToemsUserGroup>
    {
        private readonly ApiRequest _apiRequest;

        public UserGroupAPI(string resource) : base(resource)
        {
            _apiRequest = new ApiRequest();
        }

        public bool AddNewMember(int id, int userId)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/AddNewMember/{1}", Resource, id);
            Request.AddParameter("userId", userId);
            var response = _apiRequest.Execute<DtoApiBoolResponse>(Request);
            return response != null && response.Value;
        }

      

       

        public bool DeleteRights(int id)
        {
            Request.Method = Method.DELETE;
            Request.Resource = string.Format("{0}/DeleteRights/{1}", Resource, id);
            var response = _apiRequest.Execute<DtoApiBoolResponse>(Request);
            return response != null && response.Value;
        }

       

    

     

      

        public IEnumerable<EntityToemsUser> GetGroupMembers(int id, DtoSearchFilter filter)
        {
            Request.Method = Method.POST;
            Request.Resource = string.Format("{0}/GetGroupMembers/{1}", Resource, id);
            Request.AddParameter("application/json", JsonConvert.SerializeObject(filter), ParameterType.RequestBody);
            return _apiRequest.Execute<List<EntityToemsUser>>(Request);
        }

      

        public string GetMemberCount(int id)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetMemberCount/{1}", Resource, id);
            var response = _apiRequest.Execute<DtoApiStringResponse>(Request);
            return response != null ? response.Value : string.Empty;
        }

        public IEnumerable<EntityUserGroupRight> GetRights(int id)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetRights/{1}", Resource, id);
            return _apiRequest.Execute<List<EntityUserGroupRight>>(Request);
        }

      

      

        public bool UpdateMemberAcls(int id)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/UpdateMemberAcls/{1}", Resource, id);
            var response = _apiRequest.Execute<DtoApiBoolResponse>(Request);
            return response != null && response.Value;
        }

      

       
    }
}