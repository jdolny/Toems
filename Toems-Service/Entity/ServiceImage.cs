using Newtonsoft.Json;
using Org.BouncyCastle.Math.EC.Rfc7748;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Toems_ApiCalls;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceImage
    {
        private readonly UnitOfWork _uow;

        public ServiceImage()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult Add(EntityImage image)
        {
            var actionResult = new DtoActionResult();

            var validationResult = Validate(image,true);
            if (validationResult.Success)
            {
                _uow.ImageRepository.Insert(image);
                _uow.Save();
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

            _uow.ImageRepository.Delete(imageId);
            _uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.Id;

            //Check if image name is empty or null, return if so or something will be deleted that shouldn't be
            if (string.IsNullOrEmpty(u.Name)) return actionResult;

            var computers = _uow.ComputerRepository.Get(x => x.ImageId == imageId);
            var computerService = new ServiceComputer();
            foreach (var computer in computers)
            {
                computer.ImageId = -1;
                computer.ImageProfileId = -1;
                computerService.UpdateComputer(computer);
            }

            var groups = _uow.GroupRepository.Get(x => x.ImageId == imageId);
            var groupService = new ServiceGroup();
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
            var comServers = _uow.ClientComServerRepository.Get(x => x.IsImagingServer);
            var intercomKey = ServiceSetting.GetSettingValue(SettingStrings.IntercomKeyEncrypted);
            var decryptedKey = new EncryptionServices().DecryptText(intercomKey);

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
            var basePath = ServiceSetting.GetSettingValue(SettingStrings.StoragePath);
            using (var unc = new UncServices())
            {
                if (unc.NetUseWithCredentials() || unc.LastError == 1219)
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
            }

            return list;
        }


        public List<EntityAuditLog> GetImageAuditLogs(int imageId, int limit)
        {
            if (limit == 0) limit = int.MaxValue;
            var logs =
                _uow.AuditLogRepository.Get(x => x.ObjectType == "Image" && x.ObjectId == imageId)
                    .OrderByDescending(x => x.Id)
                    .Take(limit)
                    .ToList();
            foreach (var log in logs)
            {
                if (string.IsNullOrEmpty(log.UserName))
                    log.UserName = new ServiceUser().GetUserName(log.UserId);
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
            return _uow.ImageRepository.GetById(imageId);
        }

        public List<ImageWithDate> Search(DtoSearchFilterCategories filter, int userId)
        {
            if(filter.Limit == 0)
                filter.Limit = Int32.MaxValue;
            
            var images = _uow.ImageRepository.Get(x => x.Name.Contains(filter.SearchText)).OrderBy(x => x.Name).ToList();

            var categoryFilterIds = new List<int>();
            foreach (var catName in filter.Categories)
            {
                var category = _uow.CategoryRepository.GetFirstOrDefault(x => x.Name.Equals(catName));
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
                var auditLog = _uow.AuditLogRepository.Get(
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

            var imageAcl = new ServiceUser().GetAllowedImages(userId);
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
            var images = _uow.ImageRepository.Get();

            var imageAcl = new ServiceUser().GetAllowedImages(userId);
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
            return _uow.ImageRepository.Count();
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
                _uow.ImageRepository.Update(image, u.Id);
                _uow.Save();

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
            return _uow.ImageCategoryRepository.Get(x => x.ImageId == imageId);
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
                if (_uow.ImageRepository.Exists(h => h.Name == image.Name))
                {
                    validationResult.Success = false;
                    validationResult.ErrorMessage = "A Image With This Name Already Exists";
                    return validationResult;
                }
            }
            else
            {
                var original = _uow.ImageRepository.GetById(image.Id);
                if (original.Name != image.Name)
                {
                    if (_uow.ImageRepository.Exists(h => h.Name == image.Name))
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
            var images = _uow.ImageRepository.Get(i => i.IsVisible && i.Enabled, q => q.OrderBy(p => p.Name));
            if (userId == 0)
                return images;

            var imageAcl = new ServiceUser().GetAllowedImages(userId);
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
            return _uow.ImageReplicationServerRepository.Get(x => x.ImageId == imageId);
        }

        public DtoActionResult AddOrUpdateImageReplicationServers(List<EntityImageReplicationServer> imageComServers)
        {
            var first = imageComServers.FirstOrDefault();
            if (first == null) return new DtoActionResult { ErrorMessage = "No Images Were In The List", Id = 0 };
            var allSame = imageComServers.All(x => x.ImageId == first.ImageId);
            if (!allSame) return new DtoActionResult { ErrorMessage = "The List Must Be For A Single Image.", Id = 0 };
            var actionResult = new DtoActionResult();
            var pToRemove = _uow.ImageReplicationServerRepository.Get(x => x.ImageId == first.ImageId);
            foreach (var imageComServer in imageComServers)
            {
                var existing = _uow.ImageReplicationServerRepository.GetFirstOrDefault(x => x.ImageId == imageComServer.ImageId && x.ComServerId == imageComServer.ComServerId);
                if (existing == null)
                {
                    _uow.ImageReplicationServerRepository.Insert(imageComServer);
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
                _uow.ImageReplicationServerRepository.Delete(p.Id);
            }

            _uow.Save();
            actionResult.Success = true;
            return actionResult;
        }


    }
}