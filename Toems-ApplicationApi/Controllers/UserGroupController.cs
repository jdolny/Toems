using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Toems_ApplicationApi.Controllers.Authorization;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Service.Entity;

namespace Toems_ApplicationApi.Controllers
{
    public class UserGroupController : ApiController
    {
        private readonly ServiceUserGroup _userGroupServices;

        public UserGroupController()
        {
            _userGroupServices = new ServiceUserGroup();
        }


       [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public DtoActionResult Delete(int id)
        {
            var result = _userGroupServices.DeleteUserGroup(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }



       [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public DtoApiBoolResponse DeleteRights(int id)
        {
            return new DtoApiBoolResponse {Value = _userGroupServices.DeleteUserGroupRights(id)};
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public EntityToemsUserGroup Get(int id)
        {
            var result = _userGroupServices.GetUserGroup(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        [HttpPost]
        public IEnumerable<EntityToemsUserGroup> Get(DtoSearchFilter filter)
        {
            return _userGroupServices.SearchUserGroups(filter);
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        [HttpPost]
        public IEnumerable<EntityToemsUserGroup> Search(DtoSearchFilter filter)
        {
            return _userGroupServices.SearchUserGroups(filter);
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public DtoApiStringResponse GetCount()
        {
            return new DtoApiStringResponse {Value = _userGroupServices.TotalCount()};
        }



       [CustomAuth(Permission = AuthorizationStrings.Administrator)]
       [HttpPost]
        public IEnumerable<EntityToemsUser> GetGroupMembers(int id, DtoSearchFilter filter)
        {
            return _userGroupServices.GetGroupMembers(id, filter);
        }

        [HttpGet]
        [CustomAuth(Permission = AuthorizationStrings.GroupUpdate)]
        public DtoApiBoolResponse RemoveGroupMember(int id, int userId)
        {
            return new DtoApiBoolResponse { Value = _userGroupServices.RemoveMembership(userId, id) };
        }


        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public DtoApiStringResponse GetMemberCount(int id)
        {
            return new DtoApiStringResponse {Value = _userGroupServices.MemberCount(id)};
        }

       [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public IEnumerable<EntityUserGroupRight> GetRights(int id)
        {
            return _userGroupServices.GetUserGroupRights(id);
        }

       [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public DtoActionResult Post(EntityToemsUserGroup userGroup)
        {
            return _userGroupServices.AddUserGroup(userGroup);
        }

       [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public DtoActionResult Put(int id, EntityToemsUserGroup userGroup)
        {
            userGroup.Id = id;
            var result = _userGroupServices.UpdateUserGroup(userGroup);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public DtoActionResult UpdateImageManagement(int id, List<EntityUserGroupImages> userGroupImages)
        {
            return new ServiceUserGroupImagesMembership().AddOrUpdate(userGroupImages,id);
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public IEnumerable<int> GetManagedImageIds(int id)
        {
            return _userGroupServices.GetManagedImageIds(id);
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public DtoActionResult UpdateGroupManagement(int id, List<EntityUserGroupComputerGroups> userGroupGroups)
        {
            return new ServiceUserGroupComputerGroupMembership().AddOrUpdate(userGroupGroups, id);
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public IEnumerable<int> GetManagedGroupIds(int id)
        {
            return _userGroupServices.GetManagedGroupIds(id);
        }

    }
}