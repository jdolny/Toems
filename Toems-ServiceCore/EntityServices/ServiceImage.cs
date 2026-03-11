using Newtonsoft.Json;
using Toems_ApiCalls;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_DataModel;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceImage(ServiceContext ctx)
    {
        public DtoActionResult Add(EntityImage image)
        {
            var actionResult = new DtoActionResult();

            var validationResult = Validate(image,true);
            if (validationResult.Success)
            {
                ctx.Uow.ImageRepository.Insert(image);
                ctx.Uow.Save();
                actionResult.Success = true;
                actionResult.Id = image.Id;
                var defaultProfile = SeedDefaultImageProfile(image.Id);
                defaultProfile.ImageId = image.Id;
                ctx.ImageProfile.Add(defaultProfile);
            }
            else
            {
                return new DtoActionResult() {ErrorMessage = validationResult.ErrorMessage};
            }

            return actionResult;
        }

        public DtoActionResult Delete(int imageId)
        {
            var u = GetImage(imageId);
            if (u == null) return new DtoActionResult { ErrorMessage = "Image Not Found", Id = 0 };

            if (u.Protected)
            {
                return new DtoActionResult { ErrorMessage = "This Image Is Protected And Cannot Be Deleted", Id = u.Id };
            }

            ctx.Uow.ImageRepository.Delete(imageId);
            ctx.Uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.Id;

            //Check if image name is empty or null, return if so or something will be deleted that shouldn't be
            if (string.IsNullOrEmpty(u.Name)) return actionResult;

            var computers = ctx.Uow.ComputerRepository.Get(x => x.ImageId == imageId);
          
            foreach (var computer in computers)
            {
                computer.ImageId = -1;
                computer.ImageProfileId = -1;
                ctx.Computer.UpdateComputer(computer);
            }

            var groups = ctx.Uow.GroupRepository.Get(x => x.ImageId == imageId);
           
            foreach (var group in groups)
            {
                group.ImageId = -1;
                group.ImageProfileId = -1;
                ctx.Uow.GroupRepository.Update(group,group.Id);
            }

            var delDirectoryResult = ctx.Filessystem.DeleteImageFolders(u.Name);

            return actionResult;
        }

        public List<DtoServerImageRepStatus> GetReplicationStatus(int imageId)
        {
            var list = new List<DtoServerImageRepStatus>();
            var image = GetImage(imageId);
            var comServers = ctx.Uow.ClientComServerRepository.Get(x => x.IsImagingServer);
            var intercomKey = ctx.Setting.GetSettingValue(SettingStrings.IntercomKeyEncrypted);
            var decryptedKey = ctx.Encryption.DecryptText(intercomKey);

            foreach(var com in comServers)
            {
                var status = new DtoServerImageRepStatus();
                status.Servername = com.DisplayName;
                var hasImage = new APICall().ClientComServerApi.CheckImageExists(com.Url, "", decryptedKey, imageId);
                if(hasImage)
                    status.Status = "Replicated";
                else
                    status.Status = "Not Replicated";

                list.Add(status);
            }

            //check if images already exist on smb share
            var basePath = ctx.Setting.GetSettingValue(SettingStrings.StoragePath);
           
                if (ctx.Unc.NetUseWithCredentials() || ctx.Unc.LastError == 1219)
                {

                    var imagePath = Path.Combine(basePath, "images", image.Name);

                    var guidPath = Path.Combine(imagePath, "guid");
                    if (File.Exists(guidPath))
                    {
                        using (StreamReader reader = new StreamReader(guidPath))
                        {
                            var fileGuid = reader.ReadLine() ?? "";
                            if (fileGuid.Equals(image.LastUploadGuid))
                            {
                                var status = new DtoServerImageRepStatus();
                                status.Servername = basePath.Replace(@"\", "\\");
                                status.Status = "Replicated";
                                list.Add(status);
                            }
                            else
                            {
                                var status = new DtoServerImageRepStatus();
                                status.Servername = basePath.Replace(@"\", "\\");
                                status.Status = "Not Replicated";
                                list.Add(status);
                            }
                           
                        }
                    }
                    else
                    {
                        var status = new DtoServerImageRepStatus();
                        status.Servername = basePath.Replace(@"\", "\\");
                        status.Status = "Not Replicated";
                        list.Add(status);
                    }
                }
            

            return list;
        }


        public List<EntityAuditLog> GetImageAuditLogs(int imageId, int limit)
        {
            if (limit == 0) limit = int.MaxValue;
            var logs =
                ctx.Uow.AuditLogRepository.Get(x => x.ObjectType == "Image" && x.ObjectId == imageId)
                    .OrderByDescending(x => x.Id)
                    .Take(limit)
                    .ToList();
            foreach (var log in logs)
            {
                if (string.IsNullOrEmpty(log.UserName))
                    log.UserName = ctx.User.GetUserName(log.UserId);
            }
            return logs;
        }


        public List<DtoImageFileInfo> GetPartitionImageFileInfoForGridView(int imageId, string selectedHd,
         string selectedPartition)
        {
            var image = GetImage(imageId);
            return ctx.Filessystem.GetPartitionFileSize(image.Name, selectedHd, selectedPartition);
        }

        public string ImageSizeOnServerForGridView(string imageName, string hdNumber)
        {
            return ctx.Filessystem.GetHdFileSize(imageName, hdNumber);
        }

        public EntityImage GetImage(int imageId)
        {
            return ctx.Uow.ImageRepository.GetById(imageId);
        }

        public List<ImageWithDate> Search(DtoSearchFilterCategories filter, int userId)
        {
            if(filter.Limit == 0)
                filter.Limit = Int32.MaxValue;
            
            var images = ctx.Uow.ImageRepository.Get(x => x.Name.Contains(filter.SearchText)).OrderBy(x => x.Name).ToList();

            var categoryFilterIds = new List<int>();
            foreach (var catName in filter.Categories)
            {
                var category = ctx.Uow.CategoryRepository.GetFirstOrDefault(x => x.Name.Equals(catName));
                if (category != null)
                    categoryFilterIds.Add(category.Id);
            }

            var toRemove = new List<EntityImage>();
            if (filter.CategoryType.Equals("Any Category"))
            {
                //do nothing, keep all
            }
            else if (filter.CategoryType.Equals("And Category"))
            {
                foreach (var image in images)
                {
                    var gCategories = GetImageCategories(image.Id);
                    if (gCategories == null) continue;

                    if (filter.Categories.Count == 0)
                    {
                        if (gCategories.Count > 0)
                        {
                            toRemove.Add(image);
                            continue;
                        }
                    }

                    foreach (var id in categoryFilterIds)
                    {
                        if (gCategories.Any(x => x.CategoryId == id)) continue;
                        toRemove.Add(image);
                        break;
                    }
                }
            }
            else if (filter.CategoryType.Equals("Or Category"))
            {
                foreach (var image in images)
                {
                    var pCategories = GetImageCategories(image.Id);
                    if (pCategories == null) continue;
                    if (filter.Categories.Count == 0)
                    {
                        if (pCategories.Count > 0)
                        {
                            toRemove.Add(image);
                            continue;
                        }
                    }
                    var catFound = false;
                    foreach (var id in categoryFilterIds)
                    {
                        if (pCategories.Any(x => x.CategoryId == id))
                        {
                            catFound = true;
                            break;
                        }

                    }
                    if (!catFound)
                        toRemove.Add(image);
                }
            }

            foreach (var p in toRemove)
            {
                images.Remove(p);
            }




            var listWithDate = new List<ImageWithDate>();
            foreach (var image in images)
            {
                var auditLog = ctx.Uow.AuditLogRepository.Get(
               x =>
                   x.ObjectType == "Image" && x.ObjectId == image.Id &&
                   (x.AuditType.ToString().ToLower().Contains("deploy") || x.AuditType.ToString().ToLower().Contains("upload") || x.AuditType.ToString().ToLower().Contains("push") || x.AuditType.ToString().ToLower().Contains("multicast")))
               .OrderByDescending(x => x.Id)
               .FirstOrDefault();

                var imageWithDate = new ImageWithDate();
                imageWithDate.Id = image.Id;
                imageWithDate.Name = image.Name;
                imageWithDate.Environment = image.Environment;
                imageWithDate.Protected = image.Protected;
                imageWithDate.Enabled = image.Enabled;
                imageWithDate.IsVisible = image.IsVisible;
                if(auditLog != null)
                {
                    if(auditLog.DateTime != null)
                        imageWithDate.LastUsed = auditLog.DateTime;
                }
               
                imageWithDate.LastUploadGuid = image.LastUploadGuid;
                imageWithDate.Type = image.Type;
                imageWithDate.Description = image.Description;
                imageWithDate.SizeOnServer = ImageSizeOnServerForGridView(image.Name, "0");
                listWithDate.Add(imageWithDate);
            }

            var imageAcl = ctx.User.GetAllowedImages(userId);
            if(!imageAcl.ImageManagementEnforced)
                return listWithDate.Take(filter.Limit).ToList();
            else
            {
                var userImages = new List<ImageWithDate>();
                foreach (var image in listWithDate)
                {
                    if (imageAcl.AllowedImageIds.Contains(image.Id))
                        userImages.Add(image);
                }


                return userImages.Take(filter.Limit).ToList();
            }


        }

        public List<EntityImageProfile> SearchProfiles(int imageId)
        {
            using (var uow = new UnitOfWork())
            {
                return uow.ImageProfileRepository.Get(p => p.ImageId == imageId, q => q.OrderBy(p => p.Name));
            }
        }

        public List<EntityImage> GetAll(int userId)
        {
            var images = ctx.Uow.ImageRepository.Get();

            var imageAcl = ctx.User.GetAllowedImages(userId);
            if (!imageAcl.ImageManagementEnforced)
                return images.ToList();
            else
            {
                var userImages = new List<EntityImage>();
                foreach (var image in images)
                {
                    if (imageAcl.AllowedImageIds.Contains(image.Id))
                        userImages.Add(image);
                }
                return userImages.ToList();
            }
        }

        public string TotalCount()
        {
            return ctx.Uow.ImageRepository.Count();
        }

        public DtoActionResult Update(EntityImage image)
        {
            var u = GetImage(image.Id);
            if (u == null) return new DtoActionResult {ErrorMessage = "Image Not Found", Id = 0};

            var actionResult = new DtoActionResult();

            var updateFolderName = u.Name != image.Name;
            var oldName = u.Name;
            var validationResult = Validate(image,false);
            if (validationResult.Success)
            {
                ctx.Uow.ImageRepository.Update(image, u.Id);
                ctx.Uow.Save();

                actionResult.Id = image.Id;
                if (updateFolderName)
                {
                    actionResult.Success = ctx.Filessystem.RenameImageFolder(oldName, image.Name);
                }
                else
                {
                    actionResult.Success = true;
                }
            }
            else
            {
                return new DtoActionResult() { ErrorMessage = validationResult.ErrorMessage };
            }
            return actionResult;
        }

        public List<EntityImageCategory> GetImageCategories(int imageId)
        {
            return ctx.Uow.ImageCategoryRepository.Get(x => x.ImageId == imageId);
        }

        public DtoValidationResult Validate(EntityImage image, bool isNew)
        {
            var validationResult = new DtoValidationResult { Success = true };

            if (string.IsNullOrEmpty(image.Name) || !image.Name.All(c => char.IsLetterOrDigit(c) || c == '_' || c == '-' ))
            {
                validationResult.Success = false;
                validationResult.ErrorMessage = "Image Name Is Not Valid";
                return validationResult;
            }

            if (isNew)
            {
                if (ctx.Uow.ImageRepository.Exists(h => h.Name == image.Name))
                {
                    validationResult.Success = false;
                    validationResult.ErrorMessage = "A Image With This Name Already Exists";
                    return validationResult;
                }
            }
            else
            {
                var original = ctx.Uow.ImageRepository.GetById(image.Id);
                if (original.Name != image.Name)
                {
                    if (ctx.Uow.ImageRepository.Exists(h => h.Name == image.Name))
                    {
                        validationResult.Success = false;
                        validationResult.ErrorMessage = "A Image With This Name Already Exists";
                        return validationResult;
                    }
                }
            }

            return validationResult;
        }


        public EntityImageProfile SeedDefaultImageProfile(int imageId)
        {
            var image = GetImage(imageId);
            if (image.Environment.Equals("linux") && image.Type.Equals("Block"))
            {
                var template =
                    ctx.ImageProfileTemplate.GetTemplate(EnumProfileTemplate.TemplateType.LinuxBlock);
                var json = JsonConvert.SerializeObject(template);
                return JsonConvert.DeserializeObject<EntityImageProfile>(json);

            }
            else if (image.Environment.Equals("linux") && image.Type.Equals("File"))
            {
                var template =
                   ctx.ImageProfileTemplate.GetTemplate(EnumProfileTemplate.TemplateType.LinuxFile);
                var json = JsonConvert.SerializeObject(template);
                return JsonConvert.DeserializeObject<EntityImageProfile>(json);
            }

            else //winpe
            {
                var template =
                   ctx.ImageProfileTemplate.GetTemplate(EnumProfileTemplate.TemplateType.WinPE);
                var json = JsonConvert.SerializeObject(template);
                return JsonConvert.DeserializeObject<EntityImageProfile>(json);
            }


        }

        public List<EntityImage> GetOnDemandImageList(string task, int userId = 0)
        {
            var images = ctx.Uow.ImageRepository.Get(i => i.IsVisible && i.Enabled, q => q.OrderBy(p => p.Name));
            if (userId == 0)
                return images;

            var imageAcl = ctx.User.GetAllowedImages(userId);
            if (!imageAcl.ImageManagementEnforced)
                return images.ToList();
            else
            {
                var userImages = new List<EntityImage>();
                foreach (var image in images)
                {
                    if (imageAcl.AllowedImageIds.Contains(image.Id))
                        userImages.Add(image);
                }
                return userImages.ToList();
            }
        }

        public List<EntityImageReplicationServer> GetImageReplicationComServers(int imageId)
        {
            return ctx.Uow.ImageReplicationServerRepository.Get(x => x.ImageId == imageId);
        }

        public DtoActionResult AddOrUpdateImageReplicationServers(List<EntityImageReplicationServer> imageComServers)
        {
            var first = imageComServers.FirstOrDefault();
            if (first == null) return new DtoActionResult { ErrorMessage = "No Images Were In The List", Id = 0 };
            var allSame = imageComServers.All(x => x.ImageId == first.ImageId);
            if (!allSame) return new DtoActionResult { ErrorMessage = "The List Must Be For A Single Image.", Id = 0 };
            var actionResult = new DtoActionResult();
            var pToRemove = ctx.Uow.ImageReplicationServerRepository.Get(x => x.ImageId == first.ImageId);
            foreach (var imageComServer in imageComServers)
            {
                var existing = ctx.Uow.ImageReplicationServerRepository.GetFirstOrDefault(x => x.ImageId == imageComServer.ImageId && x.ComServerId == imageComServer.ComServerId);
                if (existing == null)
                {
                    ctx.Uow.ImageReplicationServerRepository.Insert(imageComServer);
                }
                else
                {
                    pToRemove.Remove(existing);
                }

                actionResult.Id = 1;
            }

            //anything left in pToRemove does not exist anymore
            foreach (var p in pToRemove)
            {
                ctx.Uow.ImageReplicationServerRepository.Delete(p.Id);
            }

            ctx.Uow.Save();
            actionResult.Success = true;
            return actionResult;
        }


    }
}