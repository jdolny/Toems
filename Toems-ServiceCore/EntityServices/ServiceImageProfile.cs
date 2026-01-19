using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;
using Toems_Service;
using Toems_Service.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceImageProfile(EntityContext ectx, GroupService groupService, ServiceComputer computerService)
    {
        public DtoActionResult Add(EntityImageProfile imageProfile)
        {
            var actionResult = new DtoActionResult();

            var validationResult = Validate(imageProfile,true);
            if (validationResult.Success)
            {
                ectx.Uow.ImageProfileRepository.Insert(imageProfile);
                ectx.Uow.Save();
                actionResult.Success = true;
                actionResult.Id = imageProfile.Id;
            }
            else
            {
                return new DtoActionResult() {ErrorMessage = validationResult.ErrorMessage};
            }

            return actionResult;
        }

        public void CloneProfile(int imageProfileId)
        {
            var imageProfile = ectx.Uow.ImageProfileRepository.GetById(imageProfileId);
            var originalName = imageProfile.Name;
            using (var uow = new UnitOfWork())
            {
                for (var c = 1; c <= 100; c++)
                {
                    var newProfileName = imageProfile.Name + "_" + c;
                    if (uow.ImageProfileRepository.Exists(h => h.Name == newProfileName))
                        continue;

                    var clonedProfile = imageProfile;
                    clonedProfile.Name = newProfileName;
                    clonedProfile.Description = "Cloned From " + originalName;
                    clonedProfile.ModelMatch = "";
                    clonedProfile.ModelMatchType = "Disabled";

                    var result = Add(clonedProfile);

                    foreach (var file in GetImageProfileFileCopy(imageProfileId))
                    {
                        file.ProfileId = result.Id;
                        new ServiceImageProfileFileCopy().AddImageProfileFileCopy(file);
                    }

                    foreach (var script in GetImageProfileScripts(imageProfileId))
                    {
                        script.ProfileId = result.Id;
                        new ServiceImageProfileScript().AddImageProfileScript(script);
                    }
                

                    foreach (var sysprep in GetImageProfileSysprep(imageProfileId))
                    {
                        sysprep.ProfileId = result.Id;
                        new ServiceImageProfileSysprep().AddImageProfileSysprep(sysprep);
                    }
                    break;
                }
            }
        }

        public DtoActionResult Delete(int imageProfileId)
        {
            var u = GetImageProfile(imageProfileId);
            if (u == null) return new DtoActionResult { ErrorMessage = "ImageProfile Not Found", Id = 0 };
            ectx.Uow.ImageProfileRepository.Delete(imageProfileId);
            ectx.Uow.Save();

            var computers = ectx.Uow.ComputerRepository.Get(x => x.ImageProfileId == imageProfileId);
            foreach (var computer in computers)
            {
                computer.ImageProfileId = -1;
                computerService.UpdateComputer(computer);
            }

            var groups = ectx.Uow.GroupRepository.Get(x => x.ImageProfileId == imageProfileId);
          
            foreach (var group in groups)
            {
                group.ImageProfileId = -1;
                groupService.UpdateGroup(group);
            }

            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.Id;
            return actionResult;
        }

        public EntityImageProfile GetImageProfile(int imageProfileId)
        {
            return ectx.Uow.ImageProfileRepository.GetById(imageProfileId);
        }

        public ImageProfileWithImage ReadProfile(int profileId)
        {
            return ectx.Uow.ImageProfileRepository.GetImageProfileWithImage(profileId);
        }

        public List<EntityImageProfile> Search(DtoSearchFilter filter)
        {
            if(filter.Limit == 0)
                filter.Limit = Int32.MaxValue;
            
            return ectx.Uow.ImageProfileRepository.Get(x => x.Name.Contains(filter.SearchText)).Take(filter.Limit).ToList();
        }

        public List<ImageProfileWithImage> GetAll()
        {
            return ectx.Uow.ImageProfileRepository.GetImageProfilesWithImages();
        }

        public string TotalCount()
        {
            return ectx.Uow.ImageProfileRepository.Count();
        }

        public DtoActionResult Update(EntityImageProfile imageProfile)
        {
            var u = GetImageProfile(imageProfile.Id);
            if (u == null) return new DtoActionResult {ErrorMessage = "ImageProfile Not Found", Id = 0};

            var actionResult = new DtoActionResult();

               var validationResult = Validate(imageProfile,false);
            if (validationResult.Success)
            {
                if (!string.IsNullOrEmpty(imageProfile.ModelMatch))
                    imageProfile.ModelMatch = imageProfile.ModelMatch.ToLower();
                ectx.Uow.ImageProfileRepository.Update(imageProfile, u.Id);
                ectx.Uow.Save();
                actionResult.Success = true;
                actionResult.Id = imageProfile.Id;
            }
            else
            {
                return new DtoActionResult() {ErrorMessage = validationResult.ErrorMessage};
            }
            return actionResult;
        }

        public EntityImageProfile GetModelMatch(string model, string environment)
        {
            if (string.IsNullOrEmpty(model)) return null;
            var profiles =
                ectx.Uow.ImageProfileRepository.Get(
                    x => !string.IsNullOrEmpty(x.ModelMatch) && !x.ModelMatchType.Equals("Disabled"));
            if (!profiles.Any()) return null;
            model = model.ToLower();

            var environmentProfiles = new List<EntityImageProfile>();
            foreach (var profile in profiles)
            {
                var image = ectx.Uow.ImageRepository.GetById(profile.ImageId);
                if (image.Environment.Equals(environment))
                    environmentProfiles.Add(profile);
            }


            //done in seperate loops to match most specific first
            foreach (var profile in environmentProfiles)
            {
                if (profile.ModelMatchType.Equals("Equals") && model.Equals(profile.ModelMatch))
                    return profile;
            }
            foreach (var profile in environmentProfiles)
            {
                if (profile.ModelMatchType.Equals("Starts With") && model.StartsWith(profile.ModelMatch))
                    return profile;
            }
            foreach (var profile in environmentProfiles)
            {
                if (profile.ModelMatchType.Equals("Ends With") && model.EndsWith(profile.ModelMatch))
                    return profile;
            }
            foreach (var profile in environmentProfiles)
            {
                if (profile.ModelMatchType.Equals("Contains") && model.Contains(profile.ModelMatch))
                    return profile;
            }
            return null;
        }

        public DtoValidationResult Validate(EntityImageProfile imageProfile, bool isNew)
        {
            var validationResult = new DtoValidationResult { Success = true };

            if (string.IsNullOrEmpty(imageProfile.Name) || !imageProfile.Name.All(c => char.IsLetterOrDigit(c) || c == '_' || c == '-' || c == ' '))
            {
                validationResult.Success = false;
                validationResult.ErrorMessage = "Image Profile Name Is Not Valid";
                return validationResult;
            }

            if (isNew)
            {
                if (ectx.Uow.ImageProfileRepository.Exists(h => h.Name == imageProfile.Name && h.ImageId == imageProfile.ImageId))
                {
                    validationResult.Success = false;
                    validationResult.ErrorMessage = "An Image Profile With This Name Already Exists";
                    return validationResult;
                }
            }
            else
            {
                var original = ectx.Uow.ImageProfileRepository.GetById(imageProfile.Id);
                if (original.Name != imageProfile.Name)
                {
                    if (ectx.Uow.ImageProfileRepository.Exists(h => h.Name == imageProfile.Name && h.ImageId == imageProfile.ImageId))
                    {
                        validationResult.Success = false;
                        validationResult.ErrorMessage = "An Image Profile With This Name Already Exists";
                        return validationResult;
                    }
                }
            }

            if (!string.IsNullOrEmpty(imageProfile.ModelMatch))
            {
                var profilesWithModelMatch =
                    ectx.Uow.ImageProfileRepository.Get(x => x.ModelMatch.Equals(imageProfile.ModelMatch.ToLower()) && x.Id != imageProfile.Id);
                if (profilesWithModelMatch.Any())
                {
                    var image = ectx.Uow.ImageRepository.GetById(profilesWithModelMatch.First().ImageId);
                    validationResult.Success = false;
                    validationResult.ErrorMessage = "This Model Match Already Exists On Image: " + image.Name;
                }
            }

            return validationResult;
        }

        public string MinimumClientSizeForGridView(int profileId, int hdNumber)
        {
            try
            {
                var profile = ReadProfile(profileId);
                var fltClientSize = new ServiceClientPartition(profile).HardDrive(hdNumber, 1) / 1024f / 1024f / 1024f;
                return Math.Abs(fltClientSize) < 0.1f ? "< 100M" : fltClientSize.ToString("#.##") + " GB";
            }
            catch
            {
                return "N/A";
            }
        }

        public List<EntityImageProfileScript> GetImageProfileScripts(int profileId)
        {
            return ectx.Uow.ImageProfileScriptRepository.Get(x => x.ProfileId == profileId, q => q.OrderBy(t => t.Priority));

        }

        public List<EntityImageProfileSysprepTag> GetImageProfileSysprep(int profileId)
        {
            return ectx.Uow.ImageProfileSysprepRepository.Get(x => x.ProfileId == profileId, q => q.OrderBy(t => t.Priority));

        }

        public List<EntityImageProfileFileCopy> GetImageProfileFileCopy(int profileId)
        {
            return ectx.Uow.ImageProfileFileCopyRepository.Get(x => x.ProfileId == profileId, q => q.OrderBy(t => t.Priority));

        }

        public bool DeleteImageProfileFileCopy(int profileId)
        {
            ectx.Uow.ImageProfileFileCopyRepository.DeleteRange(x => x.ProfileId == profileId);
            ectx.Uow.Save();
            return true;
        }

        public bool DeleteImageProfileScripts(int profileId)
        {
            ectx.Uow.ImageProfileScriptRepository.DeleteRange(x => x.ProfileId == profileId);
            ectx.Uow.Save();
            return true;
        }

        public bool DeleteImageProfileSysprepTags(int profileId)
        {
            ectx.Uow.ImageProfileSysprepRepository.DeleteRange(x => x.ProfileId == profileId);
            ectx.Uow.Save();
            return true;
        }



    }
}