using System;
using System.Collections.Generic;
using System.Linq;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceAsset
    {
        private readonly UnitOfWork _uow;

        public ServiceAsset()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult Archive(int assetId)
        {
            var u = GetAsset(assetId);
            if (u == null) return new DtoActionResult {ErrorMessage = "Asset Not Found", Id = 0};
            if (u.IsArchived) return new DtoActionResult() {ErrorMessage = "Asset Is Already Archived"};
            var actionResult = new DtoActionResult();

            u.IsArchived = true;
            u.DisplayName = u.DisplayName + "#" + DateTime.Now.ToString("MM-dd-yyyy_HH:mm");
            u.ArchivedDateTime = DateTime.Now;
            _uow.AssetRepository.Update(u, u.Id);
            _uow.Save();
            actionResult.Success = true;
            actionResult.Id = u.Id;

            return actionResult;
        }

        public DtoActionResult Restore(int assetId)
        {
            var u = GetAsset(assetId);
            if (u == null) return new DtoActionResult { ErrorMessage = "Asset Not Found", Id = 0 };

            var actionResult = new DtoActionResult();

            u.IsArchived = false;
            u.ArchivedDateTime = null;
            u.DisplayName = u.DisplayName.Split('#').First();
            if (_uow.AssetRepository.Exists(x => x.DisplayName.Equals(u.DisplayName)))
            {
                return new DtoActionResult()
                    { ErrorMessage = "Could Not Restore Asset.  An Asset With The Name " + u.DisplayName + " Already Exists" };
            }
            _uow.AssetRepository.Update(u, u.Id);
            _uow.Save();
            actionResult.Success = true;
            actionResult.Id = u.Id;

            return actionResult;
        }

        public DtoActionResult Add(EntityAsset assetType)
        {
            var actionResult = new DtoActionResult();

            var validationResult = Validate(assetType,true);
            if (validationResult.Success)
            {
                _uow.AssetRepository.Insert(assetType);
                _uow.Save();
                actionResult.Success = true;
                actionResult.Id = assetType.Id;
            }
            else
            {
                return new DtoActionResult() {ErrorMessage = validationResult.ErrorMessage};
            }

            return actionResult;
        }

        public DtoActionResult Delete(int assetTypeId)
        {
            var u = GetAsset(assetTypeId);
            if (u == null) return new DtoActionResult { ErrorMessage = "Asset Not Found", Id = 0 };

            _uow.AssetRepository.Delete(assetTypeId);
            _uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.Id;
            return actionResult;
        }

        public EntityAsset GetAsset(int assetTypeId)
        {
            return _uow.AssetRepository.GetById(assetTypeId);
        }

        public List<EntityAsset> GetAll()
        {
            return _uow.AssetRepository.Get().OrderBy(x => x.DisplayName).ToList();
        }

        public List<DtoAssetWithType> SearchArchived(DtoSearchFilterCategories filter)
        {
            var list = new List<DtoAssetWithType>();
            var assets = _uow.AssetRepository.GetAssetWithType();

            if (filter.Limit == 0) filter.Limit = int.MaxValue;
            if (string.IsNullOrEmpty(filter.SearchText)) filter.SearchText = "";
            if (filter.AssetType.Equals("Any Asset Type"))
            {
                list = assets.Where(x => x.Name.ToLower().Contains(filter.SearchText.ToLower()) && x.IsArchived)
                    .OrderBy(x => x.Name).ToList();
            }
            else
            {
                list = assets.Where(x => x.Name.ToLower().Contains(filter.SearchText.ToLower()) && x.IsArchived && x.AssetType.Equals(filter.AssetType))
                    .OrderBy(x => x.Name).ToList();
            }

            return list;
        }

        public string GetArchivedCount()
        {
            return _uow.AssetRepository.Count(x => x.IsArchived);
        }

        public List<DtoAssetWithType> Search(DtoSearchFilterCategories filter)
        {
            var list = new List<DtoAssetWithType>();
            var assets = _uow.AssetRepository.GetAssetWithType();
          
            if (filter.Limit == 0) filter.Limit = int.MaxValue;
            if (string.IsNullOrEmpty(filter.SearchText)) filter.SearchText = "";
            if (filter.AssetType.Equals("Any Asset Type"))
            {
                list = assets.Where(x => x.Name.ToLower().Contains(filter.SearchText.ToLower()) && !x.IsArchived)
                    .OrderBy(x => x.Name).ToList();
            }
            else
            {
                list = assets.Where(x => x.Name.ToLower().Contains(filter.SearchText.ToLower()) && !x.IsArchived && x.AssetType.Equals(filter.AssetType))
                    .OrderBy(x => x.Name).ToList();
            }

            if (list.Count == 0) return list;

            var categoryFilterIds = new List<int>();
            foreach (var catName in filter.Categories)
            {
                var category = _uow.CategoryRepository.GetFirstOrDefault(x => x.Name.Equals(catName));
                if (category != null)
                    categoryFilterIds.Add(category.Id);
            }

            var toRemove = new List<DtoAssetWithType>();
            if (filter.CategoryType.Equals("Any Category"))
                return list.Take(filter.Limit).ToList();
            else if (filter.CategoryType.Equals("And Category"))
            {
                foreach (var asset in list)
                {
                    var aCategories = GetAssetCategories(asset.AssetId);
                    if (aCategories == null) continue;

                    if (filter.Categories.Count == 0)
                    {
                        if (aCategories.Count > 0)
                        {
                            toRemove.Add(asset);
                            continue;
                        }
                    }

                    foreach (var id in categoryFilterIds)
                    {
                        if (aCategories.Any(x => x.CategoryId == id)) continue;
                        toRemove.Add(asset);
                        break;
                    }
                }
            }
            else if (filter.CategoryType.Equals("Or Category"))
            {
                foreach (var asset in list)
                {
                    var aCategories = GetAssetCategories(asset.AssetId);
                    if (aCategories == null) continue;
                    if (filter.Categories.Count == 0)
                    {
                        if (aCategories.Count > 0)
                        {
                            toRemove.Add(asset);
                            continue;
                        }
                    }

                    var catFound = false;
                    foreach (var id in categoryFilterIds)
                    {
                        if (aCategories.Any(x => x.CategoryId == id))
                        {
                            catFound = true;
                            break;
                        }

                    }

                    if (!catFound)
                        toRemove.Add(asset);
                }
            }

            foreach (var p in toRemove)
            {
                list.Remove(p);
            }

            return list.Take(filter.Limit).ToList();

        }



        public string TotalCount()
        {
            return _uow.AssetRepository.Count();
        }

        public DtoActionResult Update(EntityAsset asset)
        {
            var u = GetAsset(asset.Id);
            if (u == null) return new DtoActionResult {ErrorMessage = "Asset Not Found", Id = 0};

            var actionResult = new DtoActionResult();

               var validationResult = Validate(asset,false);
            if (validationResult.Success)
            {
                _uow.AssetRepository.Update(asset, u.Id);
                _uow.Save();
                actionResult.Success = true;
                actionResult.Id = asset.Id;
            }
            else
            {
                return new DtoActionResult() {ErrorMessage = validationResult.ErrorMessage};
            }
            return actionResult;
        }

        public DtoValidationResult Validate(EntityAsset asset, bool isNew)
        {
            var validationResult = new DtoValidationResult { Success = true };

            if (string.IsNullOrEmpty(asset.DisplayName) || !asset.DisplayName.All(c => char.IsLetterOrDigit(c) || c == '_' || c == '-' || c == ' ' || c == '.'))
            {
                validationResult.Success = false;
                validationResult.ErrorMessage = "Asset Name Is Not Valid";
                return validationResult;
            }

            if (isNew)
            {
                if (_uow.AssetRepository.Exists(h => h.DisplayName == asset.DisplayName))
                {
                    validationResult.Success = false;
                    validationResult.ErrorMessage = "An Asset With This Name Already Exists";
                    return validationResult;
                }
            }
            else
            {
                var original = _uow.AssetRepository.GetById(asset.Id);
                if (original.DisplayName != asset.DisplayName)
                {
                    if (_uow.AssetRepository.Exists(h => h.DisplayName == asset.DisplayName))
                    {
                        validationResult.Success = false;
                        validationResult.ErrorMessage = "An Asset With This Name Already Exists";
                        return validationResult;
                    }
                }
            }

            return validationResult;


        }

        public List<EntityAssetAttribute> GetAttributes(int assetId)
        {
            return _uow.AssetAttributeRepository.Get(x => x.AssetId == assetId);
        }


        public DtoActionResult AddComment(DtoAssetComment comment, int userId)
        {
            if (string.IsNullOrEmpty(comment.Comment))
            {
                return new DtoActionResult() {ErrorMessage = "Comments Cannot Be Empty"};
            }

            var user = new ServiceUser().GetUser(userId);
            if(user == null)
                return new DtoActionResult() { ErrorMessage = "Could Not Determine Current User" };

            var entityComment = new EntityComment();
            entityComment.CommentText = comment.Comment;
            entityComment.CommentTime = DateTime.Now;
            entityComment.Username = user.Name;
            _uow.CommentRepository.Insert(entityComment);
            _uow.Save();

            var assetComment = new EntityAssetComment();
            assetComment.AssetId = comment.AssetId;
            assetComment.CommentId = entityComment.Id;
            _uow.AssetCommentRepository.Insert(assetComment);
            _uow.Save();

            return new DtoActionResult() {Success = true,Id=assetComment.Id};
        }

        public List<EntityAttachment> GetAttachments(int assetId)
        {
            return _uow.AssetAttachmentRepository.GetAssetAttachments(assetId);
        }

        public List<EntityComputer> GetAssetSoftwareComputers(int assetId)
        {
            return _uow.AssetRepository.GetAssetSoftwareComputers(assetId);
        }

        public List<DtoAssetComment> GetComments(int assetId)
        {
            var commentIds = _uow.AssetCommentRepository.Get(x => x.AssetId == assetId).Select(x => x.CommentId).ToList();
            if(commentIds.Count == 0) return new List<DtoAssetComment>();

            var list = new List<DtoAssetComment>();
            foreach (var commentId in commentIds)
            {
                var comment = _uow.CommentRepository.GetById(commentId);
                if (comment == null) continue;
                var assetComment = new DtoAssetComment();
                assetComment.Comment = comment.CommentText;
                assetComment.CommentTime = comment.CommentTime;
                assetComment.Username = comment.Username;
                list.Add(assetComment);
            }

            return list.OrderByDescending(x => x.CommentTime).ToList();
        }

        public List<DtoAssetSoftware> GetAssetSoftware(int assetId)
        {
            var sas = _uow.SoftwareAssetSoftwareRepository.Get(x => x.AssetId == assetId);
            if(sas == null) return new List<DtoAssetSoftware>();
            if(sas.Count == 0) return  new List<DtoAssetSoftware>();

            var list = new List<DtoAssetSoftware>();
            foreach (var sa in sas)
            {
                var assetSoftware = new DtoAssetSoftware();
                assetSoftware.MatchType = sa.MatchType;
                assetSoftware.SoftwareId = sa.SoftwareInventoryId;
                assetSoftware.SoftwareAssetSoftwareId = sa.Id;
                var softwareInventory = _uow.SoftwareInventoryRepository.GetById(sa.SoftwareInventoryId);
                if (softwareInventory == null) continue;
                assetSoftware.Name = softwareInventory.Name;
                assetSoftware.Version = softwareInventory.Version;
                assetSoftware.Major = softwareInventory.Major;
                assetSoftware.Minor = softwareInventory.Minor;
                assetSoftware.Build = softwareInventory.Build;
                list.Add(assetSoftware);

            }

            return list;
        }

        public List<EntityAssetCategory> GetAssetCategories(int assetId)
        {
            return _uow.AssetCategoryRepository.Get(x => x.AssetId == assetId);
        }
    }
}