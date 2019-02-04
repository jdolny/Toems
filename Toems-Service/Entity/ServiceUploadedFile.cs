using System;
using System.Collections.Generic;
using System.Linq;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceUploadedFile
    {
        private readonly UnitOfWork _uow;

        public ServiceUploadedFile()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult AddFile(EntityUploadedFile file)
        {
            var actionResult = new DtoActionResult();
            file.DateUploaded = DateTime.UtcNow;

            var existingFile = _uow.UploadedFileRepository.GetFirstOrDefault(x => x.Name == file.Name && x.Guid == file.Guid);
            if (existingFile == null)
            {
                _uow.UploadedFileRepository.Insert(file);
                _uow.Save();
                actionResult.Success = true;
                actionResult.Id = file.Id;
            }
            else
            {
                file.Id = existingFile.Id;
                _uow.UploadedFileRepository.Update(file, file.Id);
                _uow.Save();
                actionResult.Success = true;
                actionResult.Id = file.Id;
            }

            return actionResult;
        }

        public DtoActionResult DeleteFile(int fileId)
        {
            var u = GetFile(fileId);
            if (u == null) return new DtoActionResult {ErrorMessage = "File Not Found", Id = 0};

            var f = new ServiceModule().GetModuleIdFromGuid(u.Guid);
            if (f != null)
            {
                var isActive = new ServiceModule().IsModuleActive(f.moduleId, f.moduleType);
                if (!string.IsNullOrEmpty(isActive)) return new DtoActionResult() { ErrorMessage = isActive, Id = 0 };
            }

            _uow.UploadedFileRepository.Delete(fileId);
            _uow.Save();

            new FilesystemServices().DeleteModuleFile(u);
            var module = _uow.SoftwareModuleRepository.Get(x => x.Guid == u.Guid).FirstOrDefault();
            //arguments may need changed now that file is deleted, update arguments.
            if (module != null)
                new ServiceSoftwareModule().GenerateArguments(module.Id);
            
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.Id;
            return actionResult;
        }

        public EntityUploadedFile GetFile(int fileId)
        {
            return _uow.UploadedFileRepository.GetById(fileId);
        }

        public List<EntityUploadedFile> SearchFiles(DtoSearchFilter filter)
        {
            return _uow.UploadedFileRepository.Get(s => s.Name.Contains(filter.SearchText)).OrderBy(x => x.Name).Take(filter.Limit).ToList();
        }

        public string TotalCount()
        {
            return _uow.UploadedFileRepository.Count();
        }

        public List<EntityUploadedFile> GetFilesForModule(string moduleGuid)
        {
             var files = _uow.UploadedFileRepository.Get(s => s.Guid == moduleGuid).OrderBy(x => x.Name).ToList();
            foreach (var file in files)
            {
                file.DateUploaded = file.DateUploaded.ToLocalTime();
            }
            return files;
        }
        

        
    }
}