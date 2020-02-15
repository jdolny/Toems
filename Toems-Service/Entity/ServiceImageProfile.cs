using System;
using System.Collections.Generic;
using System.Linq;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceImageProfile
    {
        private readonly UnitOfWork _uow;

        public ServiceImageProfile()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult Add(EntityImageProfile imageProfile)
        {
            var actionResult = new DtoActionResult();

            var validationResult = Validate(imageProfile,true);
            if (validationResult.Success)
            {
                _uow.ImageProfileRepository.Insert(imageProfile);
                _uow.Save();
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
            var imageProfile = _uow.ImageProfileRepository.GetById(imageProfileId);
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
                    Add(clonedProfile);
                    break;
                }
            }
        }

        public DtoActionResult Delete(int imageProfileId)
        {
            var u = GetImageProfile(imageProfileId);
            if (u == null) return new DtoActionResult { ErrorMessage = "ImageProfile Not Found", Id = 0 };
            _uow.ImageProfileRepository.Delete(imageProfileId);
            _uow.Save();

            var computers = _uow.ComputerRepository.Get(x => x.ImageProfileId == imageProfileId);
            var computerService = new ServiceComputer();
            foreach (var computer in computers)
            {
                computer.ImageProfileId = -1;
                computerService.UpdateComputer(computer);
            }

            var groups = _uow.GroupRepository.Get(x => x.ImageProfileId == imageProfileId);
            var groupService = new ServiceGroup();
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
            return _uow.ImageProfileRepository.GetById(imageProfileId);
        }

        public ImageProfileWithImage ReadProfile(int profileId)
        {
            return _uow.ImageProfileRepository.GetImageProfileWithImage(profileId);
        }

        public List<EntityImageProfile> Search(DtoSearchFilter filter)
        {
            if(filter.Limit == 0)
                filter.Limit = Int32.MaxValue;
            
            return _uow.ImageProfileRepository.Get(x => x.Name.Contains(filter.SearchText)).Take(filter.Limit).ToList();
        }

        public List<ImageProfileWithImage> GetAll()
        {
            return _uow.ImageProfileRepository.GetImageProfilesWithImages();
        }

        public string TotalCount()
        {
            return _uow.ImageProfileRepository.Count();
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
                _uow.ImageProfileRepository.Update(imageProfile, u.Id);
                _uow.Save();
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
                _uow.ImageProfileRepository.Get(
                    x => !string.IsNullOrEmpty(x.ModelMatch) && !x.ModelMatchType.Equals("Disabled"));
            if (!profiles.Any()) return null;
            model = model.ToLower();

            var environmentProfiles = new List<EntityImageProfile>();
            foreach (var profile in profiles)
            {
                var image = _uow.ImageRepository.GetById(profile.ImageId);
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
                if (_uow.ImageProfileRepository.Exists(h => h.Name == imageProfile.Name && h.ImageId == imageProfile.ImageId))
                {
                    validationResult.Success = false;
                    validationResult.ErrorMessage = "An Image Profile With This Name Already Exists";
                    return validationResult;
                }
            }
            else
            {
                var original = _uow.ImageProfileRepository.GetById(imageProfile.Id);
                if (original.Name != imageProfile.Name)
                {
                    if (_uow.ImageProfileRepository.Exists(h => h.Name == imageProfile.Name && h.ImageId == imageProfile.ImageId))
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
                    _uow.ImageProfileRepository.Get(x => x.ModelMatch.Equals(imageProfile.ModelMatch.ToLower()) && x.Id != imageProfile.Id);
                if (profilesWithModelMatch.Any())
                {
                    var image = _uow.ImageRepository.GetById(profilesWithModelMatch.First().ImageId);
                    validationResult.Success = false;
                    validationResult.ErrorMessage = "This Model Match Already Exists On Image: " + image.Name;
                }
            }

            return validationResult;
        }

       
        


    }
}