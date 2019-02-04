using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceAttachment
    {
        private readonly UnitOfWork _uow;

        public ServiceAttachment()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult Add(EntityAttachment attachment)
        {
            var actionResult = new DtoActionResult();


            _uow.AttachmentRepository.Insert(attachment);
            _uow.Save();
            actionResult.Success = true;
            actionResult.Id = attachment.Id;


            return actionResult;
        }

        public EntityAttachment Get(int id)
        {
            return _uow.AttachmentRepository.GetById(id);
        }

        public DtoActionResult Delete(int id)
        {
            var u = Get(id);
            if (u == null) return new DtoActionResult { ErrorMessage = "Attachment Not Found", Id = 0 };

            _uow.AttachmentRepository.Delete(id);
            _uow.Save();

            using (var unc = new UncServices())
            {
                if (unc.NetUseWithCredentials() || unc.LastError == 1219)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(u.DirectoryGuid) || string.IsNullOrEmpty(u.Name) || u.DirectoryGuid == Path.DirectorySeparatorChar.ToString() || u.Name == Path.DirectorySeparatorChar.ToString())
                        {
                            //ignored
                        }
                        else
                        {
                            var storagePath = ServiceSetting.GetSettingValue(SettingStrings.StoragePath);
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
            }

            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.Id;
            return actionResult;
        }
    }
}