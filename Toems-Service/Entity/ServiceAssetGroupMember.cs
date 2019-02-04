using System.Collections.Generic;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceAssetGroupMember
    {
        private readonly UnitOfWork _uow;

        public ServiceAssetGroupMember()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult AddMembership(List<EntityAssetGroupMember> members)
        {
            var actionResult = new DtoActionResult();
            foreach (var membership in members)
            {
                if (
                    _uow.AssetGroupMemberRepository.Exists(
                        x => x.AssetId == membership.AssetId && x.AssetGroupId == membership.AssetGroupId))
                    continue;
                _uow.AssetGroupMemberRepository.Insert(membership);
            }

            _uow.Save();
            actionResult.Success = true;
            actionResult.Id = 1;

            return actionResult;
        }

        public bool DeleteByIds(int assetId, int assetGroupId)
        {
            _uow.AssetGroupMemberRepository.DeleteRange(x => x.AssetId == assetId && x.AssetGroupId == assetGroupId);
            _uow.Save();
            return true;
        }

      

     

      
    }
}