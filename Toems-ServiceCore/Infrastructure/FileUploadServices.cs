using Newtonsoft.Json.Linq;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.EntityServices;

namespace Toems_ServiceCore.Infrastructure
{
    public class FileUploadServices(InfrastructureContext ictx, 
        UncServices uncService, 
        ServiceUploadedFile uploadedFileService, 
        ServiceAttachment attachmentService,
        ServiceComputerAttachment computerAttachmentService)
    {
        private DtoFileUpload _upload;
        
        public void SetUpload(DtoFileUpload upload)
        {
            _upload = upload;
        }

        public string Upload(string type)
        {
            if (_upload.TotalParts > 0)
            {
                if (_upload.UploadMethod == "alternative")
                    return SaveBlobAlternate();
                else

                    return SaveBlob();
            }
            else
            {
                var createDirResult = CreateDirectory();
                if (createDirResult != null) return createDirResult;
                return SaveAs(type);
            }
        }


        private string SaveAs(string type)
        {
            var filePath = Path.Combine(_upload.DestinationDirectory, _upload.Filename);
            
            if (uncService.NetUseWithCredentials() || uncService.LastError == 1219)
            {
                try
                {
                    using (var file = new FileStream(filePath, FileMode.Create))
                        _upload.InputStream.CopyTo(file);
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }

                if (type.Equals("module"))
                {
                    var uploadedFile = new EntityUploadedFile();
                    uploadedFile.Name = _upload.Filename;
                    uploadedFile.Guid = _upload.ModuleGuid;
                    uploadedFile.Hash = Utility.GetFileHash(filePath);

                    var result = uploadedFileService.AddFile(uploadedFile);
                    if (!result.Success)
                    {
                        try
                        {
                            File.Delete(filePath);
                        }
                        catch
                        {
                            //ignored                  
                        }

                        return "Could Not Update Database";
                    }
                }
                else if (type.Equals("attachment"))
                {
                    var attachment = new EntityAttachment();
                    attachment.AttachmentTime = DateTime.Now;
                    attachment.DirectoryGuid = _upload.AttachmentGuid;
                    attachment.Name = _upload.Filename;
                    attachment.UserName = _upload.Username;
                    var result = attachmentService.Add(attachment);
                    if (!result.Success) throw new Exception();
                    
                    if (_upload.ComputerId != null)
                    {
                        var computer = new EntityComputerAttachment();
                        computer.ComputerId = Convert.ToInt32(_upload.ComputerId);
                        computer.AttachmentId = attachment.Id;
                        result = computerAttachmentService.Add(computer);
                        if (!result.Success) throw new Exception();
                    }
                }
            }
            else
            {
                return "Could Not Reach Storage Path";
            }

            return null;

        }



        private string SaveBlob()
        {
            if (uncService.NetUseWithCredentials() || uncService.LastError == 1219)
            {
                var filePath = Path.Combine(_upload.DestinationDirectory, _upload.OriginalFilename);
                if (_upload.PartIndex == 0)
                {
                    var createDirResult = CreateDirectory();
                    if (createDirResult != null) return createDirResult;

                    if (File.Exists(filePath))
                        File.Delete(filePath);
                }

                Stream stream = null;
                try
                {
                    stream = new FileStream(filePath, (_upload.PartIndex == 0) ? FileMode.Create : FileMode.Append);
                    _upload.InputStream.CopyTo(stream, 16384);
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
                finally
                {
                    if (stream != null)
                        stream.Dispose();

                }
            }
            else
            {
                return "Could Not Reach Storage Path";
            }
            
            return null;
        }

        private string SaveBlobAlternate()
        {
            var path = Path.Combine(_upload.DestinationDirectory, _upload.PartUuid + "." + _upload.PartIndex);
            SaveAs(path);


            if (uncService.NetUseWithCredentials() || uncService.LastError == 1219)
            {
                if (_upload.PartIndex == (_upload.TotalParts - 1))
                {
                    ulong bytesWritten = 0;
                    using (
                        var output =
                        System.IO.File.OpenWrite(Path.Combine(_upload.DestinationDirectory,
                            _upload.OriginalFilename))
                    )
                    {
                        for (var i = 0; i < _upload.TotalParts; i++)
                        {

                            using (
                                var input =
                                System.IO.File.OpenRead(Path.Combine(_upload.DestinationDirectory,
                                    _upload.PartUuid + "." + i))
                            )
                            {
                                var buff = new byte[1];
                                while (input.Read(buff, 0, 1) > 0)
                                {
                                    output.WriteByte(buff[0]);
                                    bytesWritten++;
                                }

                                input.Close();
                            }

                            output.Flush();
                        }

                        output.Close();

                        if (bytesWritten != _upload.FileSize)
                        {
                            return "Filesize Mismatch";
                        }

                        for (var i = 0; i < _upload.TotalParts; i++)
                        {
                            System.IO.File.Delete(Path.Combine(_upload.DestinationDirectory,
                                _upload.PartUuid + "." + i));
                        }

                    }

                }
            }
            else
            {
                return "Could Not Reach Storage Path";
            }

            return null;

        }

        private string CreateDirectory()
        {

            if (uncService.NetUseWithCredentials() || uncService.LastError == 1219)
            {
                var directory = new DirectoryInfo(_upload.DestinationDirectory);
                try
                {
                    if (!directory.Exists)
                        directory.Create();
                    return null;
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            else
            {
                return "Could Not Reach Storage Path";
            }
        }
    }

    public class FineUploaderResult
    {
        private readonly bool _success;
        private readonly string _error;
        private readonly bool? _preventRetry;
        private readonly JObject _otherData;

        public FineUploaderResult(bool success, object otherData = null, string error = null, bool? preventRetry = null)
        {
            _success = success;
            _error = error;
            _preventRetry = preventRetry;

            if (otherData != null)
                _otherData = JObject.FromObject(otherData);
        }


        public string BuildResponse()
        {
            var response = _otherData ?? new JObject();
            response["success"] = _success;

            if (!string.IsNullOrWhiteSpace(_error))
                response["error"] = _error;

            if (_preventRetry.HasValue)
                response["preventRetry"] = _preventRetry.Value;

            return response.ToString();
        }
    }
}
