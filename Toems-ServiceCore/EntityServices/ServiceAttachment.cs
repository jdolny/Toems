using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceAttachment(ServiceContext ctx)
    {
        public DtoActionResult Add(EntityAttachment attachment)
        {
            var actionResult = new DtoActionResult();


            ctx.Uow.AttachmentRepository.Insert(attachment);
            ctx.Uow.Save();
            actionResult.Success = true;
            actionResult.Id = attachment.Id;


            return actionResult;
        }

        public EntityAttachment Get(int id)
        {
            return ctx.Uow.AttachmentRepository.GetById(id);
        }

        public DtoActionResult Delete(int id)
        {
            var u = Get(id);
            if (u == null) return new DtoActionResult { ErrorMessage = "Attachment Not Found", Id = 0 };

            ctx.Uow.AttachmentRepository.Delete(id);
            ctx.Uow.Save();


                if (ctx.Unc.NetUseWithCredentials() || ctx.Unc.LastError == 1219)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(u.DirectoryGuid) || string.IsNullOrEmpty(u.Name) || u.DirectoryGuid == Path.DirectorySeparatorChar.ToString() || u.Name == Path.DirectorySeparatorChar.ToString())
                        {
                            //ignored
                        }
                        else
                        {
                            var storagePath = ctx.Setting.GetSettingValue(SettingStrings.StoragePath);
                            File.Delete(Path.Combine(storagePath, "attachments", u.DirectoryGuid, u.Name));
                            var dirFiles = Directory.GetFiles(Path.Combine(storagePath, "attachments", u.DirectoryGuid));
                            if (dirFiles.Length == 0)
                                Directory.Delete(Path.Combine(storagePath, "attachments", u.DirectoryGuid));
                        }
                      
                    }
                    catch
                    {
                       //ignored
                    }
                }
            

            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.Id;
            return actionResult;
        }
    }
}