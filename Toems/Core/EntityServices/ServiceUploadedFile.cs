using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceUploadedFile(ServiceContext ctx)
    {
        public DtoActionResult AddFile(EntityUploadedFile file)
        {
            var actionResult = new DtoActionResult();
            file.DateUploaded = DateTime.UtcNow;

            var existingFile = ctx.Uow.UploadedFileRepository.GetFirstOrDefault(x => x.Name == file.Name && x.Guid == file.Guid);
            if (existingFile == null)
            {
                ctx.Uow.UploadedFileRepository.Insert(file);
                ctx.Uow.Save();
                actionResult.Success = true;
                actionResult.Id = file.Id;
            }
            else
            {
                file.Id = existingFile.Id;
                ctx.Uow.UploadedFileRepository.Update(file, file.Id);
                ctx.Uow.Save();
                actionResult.Success = true;
                actionResult.Id = file.Id;
            }

            return actionResult;
        }

        public DtoActionResult DeleteFile(int fileId)
        {
            var u = GetFile(fileId);
            if (u == null) return new DtoActionResult {ErrorMessage = "File Not Found", Id = 0};

            var f = ctx.Module.GetModuleIdFromGuid(u.Guid);
            if (f != null)
            {
                var isActive = ctx.Module.IsModuleActive(f.moduleId, f.moduleType);
                if (!string.IsNullOrEmpty(isActive)) return new DtoActionResult() { ErrorMessage = isActive, Id = 0 };
            }

            ctx.Uow.UploadedFileRepository.Delete(fileId);
            ctx.Uow.Save();

            ctx.Filessystem.DeleteModuleFile(u);
            var module = ctx.Uow.SoftwareModuleRepository.Get(x => x.Guid == u.Guid).FirstOrDefault();
            //arguments may need changed now that file is deleted, update arguments.
            if (module != null)
                ctx.SoftwareModule.GenerateArguments(module.Id);
            
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.Id;
            return actionResult;
        }

        public EntityUploadedFile GetFile(int fileId)
        {
            return ctx.Uow.UploadedFileRepository.GetById(fileId);
        }

        public List<EntityUploadedFile> SearchFiles(DtoSearchFilter filter)
        {
            return ctx.Uow.UploadedFileRepository.Get(s => s.Name.Contains(filter.SearchText)).OrderBy(x => x.Name).Take(filter.Limit).ToList();
        }

        public string TotalCount()
        {
            return ctx.Uow.UploadedFileRepository.Count();
        }

        public List<EntityUploadedFile> GetFilesForModule(string moduleGuid)
        {
             var files = ctx.Uow.UploadedFileRepository.Get(s => s.Guid == moduleGuid).OrderBy(x => x.Name).ToList();
            foreach (var file in files)
            {
                file.DateUploaded = file.DateUploaded.ToLocalTime();
            }
            return files;
        }
        

        
    }
}