using System.Collections.Generic;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{
    public class UserGroupAPI : BaseAPI<EntityToemsUserGroup>
    {


        public UserGroupAPI(string resource, ProtectedLocalStorage protectedLocalStorage) : base(resource, protectedLocalStorage)
        {

        }

        public bool AddNewMember(int id, int userId)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/AddNewMember/{1}", Resource, id);
            Request.AddParameter("userId", userId);
            var response = _apiRequest.Execute<DtoApiBoolResponse>(Request);
            return response != null && response.Value;
        }

      

       

        public bool DeleteRights(int id)
        {
            Request.Method = Method.Delete;
            Request.Resource = string.Format("{0}/DeleteRights/{1}", Resource, id);
            var response = _apiRequest.Execute<DtoApiBoolResponse>(Request);
            return response != null && response.Value;
        }

       

    

     

      

        public IEnumerable<EntityToemsUser> GetGroupMembers(int id, DtoSearchFilter filter)
        {
            Request.Method = Method.Post;
            Request.Resource = string.Format("{0}/GetGroupMembers/{1}", Resource, id);
            Request.AddParameter("application/json", JsonConvert.SerializeObject(filter), ParameterType.RequestBody);
            return _apiRequest.Execute<List<EntityToemsUser>>(Request);
        }

        public bool RemoveGroupMember(int id, int userId)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/RemoveGroupMember/{1}", Resource, id);
            Request.AddParameter("userId", userId);
            var response = _apiRequest.Execute<DtoApiBoolResponse>(Request);
            return response != null && response.Value;
        }

        public string GetMemberCount(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetMemberCount/{1}", Resource, id);
            var response = _apiRequest.Execute<DtoApiStringResponse>(Request);
            return response != null ? response.Value : string.Empty;
        }

        public IEnumerable<EntityUserGroupRight> GetRights(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetRights/{1}", Resource, id);
            return _apiRequest.Execute<List<EntityUserGroupRight>>(Request);
        }

        public IEnumerable<int> GetManagedImageIds(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetManagedImageIds/{1}", Resource, id);
            return _apiRequest.Execute<List<int>>(Request);
        }

        public IEnumerable<int> GetManagedGroupIds(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetManagedGroupIds/{1}", Resource, id);
            return _apiRequest.Execute<List<int>>(Request);
        }





        public bool UpdateMemberAcls(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/UpdateMemberAcls/{1}", Resource, id);
            var response = _apiRequest.Execute<DtoApiBoolResponse>(Request);
            return response != null && response.Value;
        }

        public DtoActionResult UpdateImageManagement(List<EntityUserGroupImages> images, int userGroupId)
        {
            Request.Method = Method.Post;
            Request.Resource = string.Format("{0}/UpdateImageManagement/{1}", Resource,userGroupId);
            Request.AddParameter("application/json", JsonConvert.SerializeObject(images), ParameterType.RequestBody);
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

        public DtoActionResult UpdateGroupManagement(List<EntityUserGroupComputerGroups> groups, int userGroupId)
        {
            Request.Method = Method.Post;
            Request.Resource = string.Format("{0}/UpdateGroupManagement/{1}", Resource, userGroupId);
            Request.AddParameter("application/json", JsonConvert.SerializeObject(groups), ParameterType.RequestBody);
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