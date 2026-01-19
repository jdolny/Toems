using Newtonsoft.Json;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_DataModel;
using Toems_Service;
using Toems_Service.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceImage(EntityContext ectx, GroupService groupService, ServiceComputer computerService, UncServices UncService, ServiceUser userService)
    {
        public DtoActionResult Add(EntityImage image)
        {
            var actionResult = new DtoActionResult();

            var validationResult = Validate(image,true);
            if (validationResult.Success)
            {
                ectx.Uow.ImageRepository.Insert(image);
                ectx.Uow.Save();
                actionResult.Success = true;
                actionResult.Id = image.Id;
                var defaultProfile = SeedDefaultImageProfile(image.Id);
                defaultProfile.ImageId = image.Id;
                new ServiceImageProfile().Add(defaultProfile);
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

            ectx.Uow.ImageRepository.Delete(imageId);
            ectx.Uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.Id;

            //Check if image name is empty or null, return if so or something will be deleted that shouldn't be
            if (string.IsNullOrEmpty(u.Name)) return actionResult;

            var computers = ectx.Uow.ComputerRepository.Get(x => x.ImageId == imageId);
          
            foreach (var computer in computers)
            {
                computer.ImageId = -1;
                computer.ImageProfileId = -1;
                computerService.UpdateComputer(computer);
            }

            var groups = ectx.Uow.GroupRepository.Get(x => x.ImageId == imageId);
           
            foreach (var group in groups)
            {
                group.ImageId = -1;
                group.ImageProfileId = -1;
                groupService.UpdateGroup(group);
            }

            var delDirectoryResult = new FilesystemServices().DeleteImageFolders(u.Name);

            return actionResult;
        }

        public List<DtoServerImageRepStatus> GetReplicationStatus(int imageId)
        {
            var list = new List<DtoServerImageRepStatus>();
            var image = GetImage(imageId);
            var comServers = ectx.Uow.ClientComServerRepository.Get(x => x.IsImagingServer);
            var intercomKey = ectx.Settings.GetSettingValue(SettingStrings.IntercomKeyEncrypted);
            var decryptedKey = ectx.Encryption.DecryptText(intercomKey);

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
            var basePath = ectx.Settings.GetSettingValue(SettingStrings.StoragePath);
           
                if (UncService.NetUseWithCredentials() || UncService.LastError == 1219)
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
                ectx.Uow.AuditLogRepository.Get(x => x.ObjectType == "Image" && x.ObjectId == imageId)
                    .OrderByDescending(x => x.Id)
                    .Take(limit)
                    .ToList();
            foreach (var log in logs)
            {
                if (string.IsNullOrEmpty(log.UserName))
                    log.UserName = userService.GetUserName(log.UserId);
            }
            return logs;
        }


        public List<DtoImageFileInfo> GetPartitionImageFileInfoForGridView(int imageId, string selectedHd,
         string selectedPartition)
        {
            var image = GetImage(imageId);
            return new FilesystemServices().GetPartitionFileSize(image.Name, selectedHd, selectedPartition);
        }

        public string ImageSizeOnServerForGridView(string imageName, string hdNumber)
        {
            return new FilesystemServices().GetHdFileSize(imageName, hdNumber);
        }

        public EntityImage GetImage(int imageId)
        {
            return ectx.Uow.ImageRepository.GetById(imageId);
        }

        public List<ImageWithDate> Search(DtoSearchFilterCategories filter, int userId)
        {
            if(filter.Limit == 0)
                filter.Limit = Int32.MaxValue;
            
            var images = ectx.Uow.ImageRepository.Get(x => x.Name.Contains(filter.SearchText)).OrderBy(x => x.Name).ToList();

            var categoryFilterIds = new List<int>();
            foreach (var catName in filter.Categories)
            {
                var category = ectx.Uow.CategoryRepository.GetFirstOrDefault(x => x.Name.Equals(catName));
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
                var auditLog = ectx.Uow.AuditLogRepository.Get(
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

            var imageAcl = userService.GetAllowedImages(userId);
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
            var images = ectx.Uow.ImageRepository.Get();

            var imageAcl = userService.GetAllowedImages(userId);
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
            return ectx.Uow.ImageRepository.Count();
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
                ectx.Uow.ImageRepository.Update(image, u.Id);
                ectx.Uow.Save();

                actionResult.Id = image.Id;
                if (updateFolderName)
                {
                    actionResult.Success = new FilesystemServices().RenameImageFolder(oldName, image.Name);
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
            return ectx.Uow.ImageCategoryRepository.Get(x => x.ImageId == imageId);
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
                if (ectx.Uow.ImageRepository.Exists(h => h.Name == image.Name))
                {
                    validationResult.Success = false;
                    validationResult.ErrorMessage = "A Image With This Name Already Exists";
                    return validationResult;
                }
            }
            else
            {
                var original = ectx.Uow.ImageRepository.GetById(image.Id);
                if (original.Name != image.Name)
                {
                    if (ectx.Uow.ImageRepository.Exists(h => h.Name == image.Name))
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
                    new ServiceImageProfileTemplate().GetTemplate(EnumProfileTemplate.TemplateType.LinuxBlock);
                var json = JsonConvert.SerializeObject(template);
                return JsonConvert.DeserializeObject<EntityImageProfile>(json);

            }
            else if (image.Environment.Equals("linux") && image.Type.Equals("File"))
            {
                var template =
                   new ServiceImageProfileTemplate().GetTemplate(EnumProfileTemplate.TemplateType.LinuxFile);
                var json = JsonConvert.SerializeObject(template);
                return JsonConvert.DeserializeObject<EntityImageProfile>(json);
            }

            else //winpe
            {
                var template =
                   new ServiceImageProfileTemplate().GetTemplate(EnumProfileTemplate.TemplateType.WinPE);
                var json = JsonConvert.SerializeObject(template);
                return JsonConvert.DeserializeObject<EntityImageProfile>(json);
            }


        }

        public List<EntityImage> GetOnDemandImageList(string task, int userId = 0)
        {
            var images = ectx.Uow.ImageRepository.Get(i => i.IsVisible && i.Enabled, q => q.OrderBy(p => p.Name));
            if (userId == 0)
                return images;

            var imageAcl = userService.GetAllowedImages(userId);
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
            return ectx.Uow.ImageReplicationServerRepository.Get(x => x.ImageId == imageId);
        }

        public DtoActionResult AddOrUpdateImageReplicationServers(List<EntityImageReplicationServer> imageComServers)
        {
            var first = imageComServers.FirstOrDefault();
            if (first == null) return new DtoActionResult { ErrorMessage = "No Images Were In The List", Id = 0 };
            var allSame = imageComServers.All(x => x.ImageId == first.ImageId);
            if (!allSame) return new DtoActionResult { ErrorMessage = "The List Must Be For A Single Image.", Id = 0 };
            var actionResult = new DtoActionResult();
            var pToRemove = ectx.Uow.ImageReplicationServerRepository.Get(x => x.ImageId == first.ImageId);
            foreach (var imageComServer in imageComServers)
            {
                var existing = ectx.Uow.ImageReplicationServerRepository.GetFirstOrDefault(x => x.ImageId == imageComServer.ImageId && x.ComServerId == imageComServer.ComServerId);
                if (existing == null)
                {
                    ectx.Uow.ImageReplicationServerRepository.Insert(imageComServer);
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
                ectx.Uow.ImageReplicationServerRepository.Delete(p.Id);
            }

            ectx.Uow.Save();
            actionResult.Success = true;
            return actionResult;
        }


    }
}