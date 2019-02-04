using System.Collections.Generic;
using System.Web.Http;
using Toems_ApplicationApi.Controllers.Authorization;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Service.Entity;

namespace Toems_ApplicationApi.Controllers
{
    public class AssetGroupMemberController : ApiController
    {
        private readonly ServiceAssetGroupMember _assetGroupMemberService;

        public AssetGroupMemberController()
        {
            _assetGroupMemberService = new ServiceAssetGroupMember();
        }

        [CustomAuth(Permission = AuthorizationStrings.AssetUpdate)]
        public DtoActionResult Post(List<EntityAssetGroupMember> assetGroupMembers)
        {
            return _assetGroupMemberService.AddMembership(assetGroupMembers);
        }

      
    }
}