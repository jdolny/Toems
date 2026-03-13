using System.Diagnostics;
using System.Management;
using System.Net;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Dto.clientimaging;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_DataModel;
using Toems_ServiceCore.EntityServices;
using Toems_ServiceCore.Workflows;

namespace Toems_ServiceCore.Infrastructure
{
    public class ClientImagingServices(ServiceContext ctx)
        
    {
        public string AddComputer(string name, string mac, string clientIdentifier)
        {
            if(string.IsNullOrEmpty(name))
            {
                return
                   JsonConvert.SerializeObject(new DtoActionResult
                   {
                       Success = false,
                       ErrorMessage = "Name Not Valid.  Cannot Be Empty."
                   });
            }

            if (name.StartsWith(" ") || name.EndsWith(" "))
            {
                return
                   JsonConvert.SerializeObject(new DtoActionResult
                   {
                       Success = false,
                       ErrorMessage = "Name Not Valid.  Cannot Start Or End With A Space."
                   });
            }

            if (name.Length > 15)
            {
                return
                  JsonConvert.SerializeObject(new DtoActionResult
                  {
                      Success = false,
                      ErrorMessage = "Name Not Valid.  Cannot Be More Than 15 characters."
                  });
            }

            //https://support.microsoft.com/en-us/help/909264/naming-conventions-in-active-directory-for-computers-domains-sites-and
            if (!name.All(c => char.IsLetterOrDigit(c) || (c != '\\' && c != '/' && c != ':' && c != '*' && c != '?' && c != '"' && c != '<' && c != '>' && c != '|')))
            {
                return
                 JsonConvert.SerializeObject(new DtoActionResult
                 {
                     Success = false,
                     ErrorMessage = "Name Not Valid.  Contains An Illegal Character"
                 });
            }
            if(name.StartsWith("."))
            {
                return
                 JsonConvert.SerializeObject(new DtoActionResult
                 {
                     Success = false,
                     ErrorMessage = "Name Not Valid.  Cannot Start With A Period"
                 });
            }

            var existingComputer = ctx.Computer.GetComputerFromClientIdentifier(clientIdentifier);
            if (existingComputer != null)
            {
                return
                    JsonConvert.SerializeObject(new DtoActionResult
                    {
                        Success = false,
                        ErrorMessage = "A Computer With This Client Id Already Exists"
                    });
            }
            var computer = new EntityComputer
            {
                Name = name + ":" + DateTime.Now.ToString("MM-dd-yyyy_HH_mm"),
                ImagingClientId = clientIdentifier.ToUpper(),
                ImagingMac = mac.ToUpper(),
                ImageId = -1,
                ImageProfileId = -1,
                ProvisionStatus = EnumProvisionStatus.Status.ImageOnly
            };
            var result = ctx.Computer.AddComputer(computer);
            return JsonConvert.SerializeObject(result);
        }

        public string AddImage(string imageName, string userId)
        {
            var image = new EntityImage
            {
                Name = imageName,
                Environment = "linux",
                Type = "Block",
                Enabled = true,
                IsVisible = true,
                Description = "",
            };
            var result = ctx.Image.Add(image);
            if (result.Success)
            {
                result.Id = image.Id;
                if (userId != null)
                    ctx.User.UpdateUsersImagesList(new EntityToemsUsersImages() { ImageId = result.Id, UserId = Convert.ToInt32(userId) });
            }

            return JsonConvert.SerializeObject(result);
        }
        
        public string AddImageWinPEEnv(string imageName, string userId)
        {
            var image = new EntityImage
            {
                Name = imageName,
                Environment = "winpe",
                Type = "File",
                Enabled = true,
                IsVisible = true,
                Description = "",
            };

            var imageType = ctx.Setting.GetSettingValue(SettingStrings.DefaultWieImageType);
            if(!string.IsNullOrEmpty(imageType))
                image.Type = imageType;

            var result = ctx.Image.Add(image);
            if (result.Success)
            {
                result.Id = image.Id;
                if (userId != null)
                    ctx.User.UpdateUsersImagesList(new EntityToemsUsersImages() { ImageId = result.Id, UserId = Convert.ToInt32(userId) });
            }

            return JsonConvert.SerializeObject(result);
        }

        public string GetRegistrationSettings()
        {
            var regDto = new RegistrationDTO();
            regDto.registrationEnabled = ctx.Setting.GetSettingValue(SettingStrings.RegistrationEnabled);
            regDto.keepNamePrompt = ctx.Setting.GetSettingValue(SettingStrings.DisabledRegNamePrompt);
            return JsonConvert.SerializeObject(regDto);
        }

        public string GetWebTaskToken(string clientId)
        {
            var webTaskRequiresLogin = ctx.Setting.GetSettingValue(SettingStrings.WebTasksRequireLogin);
            if (webTaskRequiresLogin.Equals("True")) return string.Empty;
            var computer = ctx.Computer.GetComputerFromClientIdentifier(clientId);
            if (computer == null) return string.Empty;
            var task = ctx.ActiveImagingTask.GetForComputer(computer.Id);
            if(task == null) return string.Empty;
            if (task.WebTaskToken == null) return string.Empty;
            return task.WebTaskToken;
        }

        public AuthResponseDto AuthorizeApiCall(string token)
        {
            var response = new AuthResponseDto();
            if (string.IsNullOrEmpty(token)) return response;
            

            var user = ctx.User.GetUserFromToken(token);
            if (user != null)
            {
                response.IsAuthorized = true;
                response.Id = user.Id;
                response.UserType = "user";
                return response;
            }

            var task = ctx.ActiveImagingTask.GetFromWebToken(token);
            if (task != null)
            {
                response.IsAuthorized = true;
                response.Id = task.Id;
                response.UserType = "task";
                return response;
            }

            //check global token
            var globalToken = ctx.Setting.GetSettingValue(SettingStrings.GlobalImagingToken);
            if (token.Equals(globalToken) && !string.IsNullOrEmpty(globalToken))
            {
                response.IsAuthorized = true;
                response.UserType = "global";
                return response;
            }


            return response;
        }

        public void ChangeStatusInProgress(int taskId)
        {
            var task = ctx.ActiveImagingTask.GetTask(taskId);
            task.Status = EnumTaskStatus.ImagingStatus.Imaging;
            ctx.ActiveImagingTask.UpdateActiveImagingTask(task);
        }

        public string CheckForCancelledTask(int taskId)
        {
            var task = ctx.ActiveImagingTask.GetTask(taskId);
            if (task == null)
                return "true";
            return "false";
        }

        public string CheckHdRequirements(int profileId, int clientHdNumber, string newHdSize, string imageSchemaDrives,
            int clientLbs)
        {
            var result = new HardDriveSchema();

            var imageProfile = ctx.ImageProfile.ReadProfile(profileId);
            ctx.ClientPartition.SetImageSchema(imageProfile);
            var imageSchema = ctx.ClientPartition.GetImageSchema();

            if (clientHdNumber > imageSchema.HardDrives.Count())
            {
                result.IsValid = "false";
                result.Message = "No Image Exists To Download To This Hard Drive.  There Are More" +
                                 "Hard Drive's Than The Original Image";

                return JsonConvert.SerializeObject(result);
            }

            var listSchemaDrives = new List<int>();
            if (!string.IsNullOrEmpty(imageSchemaDrives))
                listSchemaDrives.AddRange(imageSchemaDrives.Split(' ').Select(hd => Convert.ToInt32(hd)));
            result.SchemaHdNumber = ctx.ClientPartition.NextActiveHardDrive(listSchemaDrives, clientHdNumber);

            if (result.SchemaHdNumber == -1)
            {
                result.IsValid = "false";
                result.Message = "No Active Hard Drive Images Were Found To Deploy.";
                return JsonConvert.SerializeObject(result);
            }

            var newHdBytes = Convert.ToInt64(newHdSize);
            var minimumSize = ctx.ClientPartition.HardDrive(result.SchemaHdNumber, newHdBytes);

            if (clientLbs != 0) //if zero should be from the winpe imaging environment
            {
                if (imageProfile.Image.Type != "File")
                {
                    if (clientLbs != imageSchema.HardDrives[result.SchemaHdNumber].Lbs)
                    {
                        ctx.Log.Error("Error: The Logical Block Size Of This Hard Drive " + clientLbs +
                                  " Does Not Match The Original Image" +
                                  imageSchema.HardDrives[result.SchemaHdNumber].Lbs);

                        result.IsValid = "false";
                        result.Message = "The Logical Block Size Of This Hard Drive " + clientLbs +
                                         " Does Not Match The Original Image" +
                                         imageSchema.HardDrives[result.SchemaHdNumber].Lbs;
                        return JsonConvert.SerializeObject(result);
                    }
                }
            }

            if (minimumSize > newHdBytes)
            {
                ctx.Log.Error("Error:  " + newHdBytes / 1024 / 1024 +
                          " MB Is Less Than The Minimum Required HD Size For This Image(" +
                          minimumSize / 1024 / 1024 + " MB)");

                result.IsValid = "false";
                result.Message = newHdBytes / 1024 / 1024 +
                                 " MB Is Less Than The Minimum Required HD Size For This Image(" +
                                 minimumSize / 1024 / 1024 + " MB)";
                return JsonConvert.SerializeObject(result);
            }
            if (minimumSize == newHdBytes)
            {
                result.IsValid = "original";
                result.PhysicalPartitions = ctx.ClientPartition.GetActivePartitions(result.SchemaHdNumber, imageProfile);
                result.PhysicalPartitionCount = ctx.ClientPartition.GetActivePartitionCount(result.SchemaHdNumber);
                result.PartitionType = imageSchema.HardDrives[result.SchemaHdNumber].Table;
                result.BootPartition = imageSchema.HardDrives[result.SchemaHdNumber].Boot;
                result.UsesLvm = ctx.ClientPartition.CheckForLvm(result.SchemaHdNumber);
                result.Guid = imageSchema.HardDrives[result.SchemaHdNumber].Guid;
                return JsonConvert.SerializeObject(result);
            }

            result.IsValid = "true";
            result.PhysicalPartitions = ctx.ClientPartition.GetActivePartitions(result.SchemaHdNumber, imageProfile);
            result.PhysicalPartitionCount = ctx.ClientPartition.GetActivePartitionCount(result.SchemaHdNumber);
            result.PartitionType = imageSchema.HardDrives[result.SchemaHdNumber].Table;
            result.BootPartition = imageSchema.HardDrives[result.SchemaHdNumber].Boot;
            result.UsesLvm = ctx.ClientPartition.CheckForLvm(result.SchemaHdNumber);
            result.Guid = imageSchema.HardDrives[result.SchemaHdNumber].Guid;
            return JsonConvert.SerializeObject(result);
        }

        public string CheckHdRequirementsFfu(int profileId, int clientHdNumber, string newHdSize, string imageSchemaDrives)
        {
            var result = new HardDriveSchema();

            var imageProfile = ctx.ImageProfile.ReadProfile(profileId);
            ctx.ClientPartition.SetImageSchema(imageProfile);
            var imageSchema = ctx.ClientPartition.GetImageSchema();

            if (clientHdNumber > imageSchema.HardDrives.Count())
            {
                result.IsValid = "false";
                result.Message = "No Image Exists To Download To This Hard Drive.  There Are More" +
                                 "Hard Drive's Than The Original Image";

                return JsonConvert.SerializeObject(result);
            }

            var listSchemaDrives = new List<int>();
            if (!string.IsNullOrEmpty(imageSchemaDrives))
                listSchemaDrives.AddRange(imageSchemaDrives.Split(' ').Select(hd => Convert.ToInt32(hd)));
            result.SchemaHdNumber = ctx.ClientPartition.NextActiveHardDrive(listSchemaDrives, clientHdNumber);

            if (result.SchemaHdNumber == -1)
            {
                result.IsValid = "false";
                result.Message = "No Active Hard Drive Images Were Found To Deploy.";
                return JsonConvert.SerializeObject(result);
            }

            var newHdBytes = Convert.ToInt64(newHdSize);
            var minimumSize = imageSchema.HardDrives[result.SchemaHdNumber].Size * imageSchema.HardDrives[result.SchemaHdNumber].Lbs;
      
            if (minimumSize > newHdBytes)
            {
                ctx.Log.Error("Error:  " + newHdBytes / 1024 / 1024 +
                          " MB Is Less Than The Minimum Required HD Size For This Image(" +
                          minimumSize / 1024 / 1024 + " MB)");

                result.IsValid = "false";
                result.Message = newHdBytes / 1024 / 1024 +
                                 " MB Is Less Than The Minimum Required HD Size For This Image(" +
                                 minimumSize / 1024 / 1024 + " MB)";
                return JsonConvert.SerializeObject(result);
            }


            result.IsValid = "true";
            return JsonConvert.SerializeObject(result);
        }

        public string GetUploadServerIp()
        {
            var guid = ctx.Config["ComServerUniqueId"];
            var thisComServer = ctx.ClientComServer.GetServerByGuid(guid);

            if (thisComServer == null)
            {
                ctx.Log.Error($"Com Server With Guid {guid} Not Found");
                return "0";
            }

            if (!string.IsNullOrEmpty(thisComServer.ImagingIp))
            {
                return thisComServer.ImagingIp;
            }

            else
            {
                //get the ip needed for upload
                var urlHasIp = Regex.Match(thisComServer.Url, @"\b(\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})\b");
                if (urlHasIp.Success)
                {
                    return urlHasIp.Captures[0].ToString();
                }
                else
                {
                    //get from dns
                    var dnsName = thisComServer.Url.Split(new[] { "//" }, StringSplitOptions.None).Last().Split(':').First();
                    var ipaddresses = Dns.GetHostAddresses(dnsName).Where(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToList();
                    if (ipaddresses.Count > 1)
                    {
                        ctx.Log.Error("More Than 1 Ip Address Has Been Resolved For Com Server.  You Must Add an IP Override.");
                        return "0";
                    }
                    else
                    {
                        return ipaddresses.First().ToString();
                    }
                }
            }
        }

        public string CheckIn(string taskId, string comServers)
        {
            var checkIn = new CheckIn();

            var task = ctx.ActiveImagingTask.GetTask(Convert.ToInt32(taskId));

            if (task == null)
            {
                checkIn.Result = "false";
                checkIn.Message = "Could Not Find Task With Id" + taskId;
                return JsonConvert.SerializeObject(checkIn);
            }

            var computer = ctx.Computer.GetComputer(task.ComputerId);
            if (computer == null)
            {
                checkIn.Result = "false";
                checkIn.Message = "The Computer Assigned To This Task Was Not Found";
                return JsonConvert.SerializeObject(checkIn);
            }
            
            var comServerId = ctx.GetBestCompImageServer.Run(computer, task.Type,comServers);

            task.Status = EnumTaskStatus.ImagingStatus.CheckedIn;
            task.ComServerId = comServerId;

            var imageServer = ctx.ClientComServer.GetServer(comServerId);

            ImageProfileWithImage imageProfile = null;
            if (task.Type == "multicast")
            {
                var mcTask = ctx.ActiveMulticastSession.Get(task.MulticastId);
                var group = ctx.Group.GetGroupByName(mcTask.Name);
                imageProfile = ctx.ImageProfile.ReadProfile(group.ImageProfileId);
            }
            else
            {
               imageProfile = ctx.ImageProfile.ReadProfile(Convert.ToInt32(task.ImageProfileId));
            }
            if (imageProfile.Image.Protected && task.Type.Contains("upload"))
            {
                checkIn.Result = "false";
                checkIn.Message = "This Image Is Protected";
                return JsonConvert.SerializeObject(checkIn);
            }

            if (ctx.ActiveImagingTask.UpdateActiveImagingTask(task))
            {
                checkIn.Result = "true";

                if (imageProfile.Image != null)
                {
                    if (imageProfile.Image.Environment == "")
                        imageProfile.Image.Environment = "linux";
                    checkIn.ImageEnvironment = imageProfile.Image.Environment;
                }
                checkIn.TaskArguments = task.Arguments;
                if (imageProfile.Image.Environment == "winpe")
                {
                    checkIn.TaskArguments += "image_server=\"" +
                                            imageServer.Url + "clientimaging/" + "\"\r\n";
                }
                else
                {
                    checkIn.TaskArguments += " image_server=\"" +
                                            imageServer.Url + "clientimaging/" + "\"";
                }

               

                return JsonConvert.SerializeObject(checkIn);
            }
            checkIn.Result = "false";
            checkIn.Message = "Could Not Update Task Status";
            return JsonConvert.SerializeObject(checkIn);
        }

        public void CheckOut(int taskId)
        {
            var task = ctx.ActiveImagingTask.GetTask(taskId);
            if (task.Type.Contains("upload"))
            {
                //protect image by default
                var imageProfile = ctx.ImageProfile.ReadProfile(Convert.ToInt32(task.ImageProfileId));
                imageProfile.Image.Protected = true;
                ctx.Image.Update(imageProfile.Image);
                var replicationTime = ctx.Setting.GetSettingValue(SettingStrings.ImageReplicationTime);
                if(replicationTime.Equals("Immediately"))
                    ctx.ImageSync.RunAllServers();

            }

            if (task.Type.Contains("unreg"))
                ctx.ActiveImagingTask.DeleteUnregisteredOndTask(task.Id);
            else
                ctx.ActiveImagingTask.DeleteActiveImagingTask(task.Id);

            if (task.Type != "multicast" && task.Type != "ondmulticast")
                ctx.ActiveImagingTask.SendTaskCompletedEmail(task);
        }

        public string CheckQueue(int taskId)
        {
            var queueStatus = new QueueStatus();

            var thisComputerTask = ctx.ActiveImagingTask.GetTask(taskId);
            //var computer = new ServiceComputer().GetComputer(thisComputerTask.ComputerId);
            //Check if already part of the queue
            ctx.ActiveImagingTask.CancelTimedOutTasks();
            if (thisComputerTask.Status == EnumTaskStatus.ImagingStatus.InImagingQueue)
            {
                //Delete Any tasks that have passed the timeout value

                //Check if the queue is open yet
                var inUse = ctx.ActiveImagingTask.GetCurrentQueue(thisComputerTask);
                var totalCapacity = 0;
                var comServer = ctx.ClientComServer.GetServer(thisComputerTask.ComServerId);
                totalCapacity = comServer.ImagingMaxClients;
                if (inUse < totalCapacity)
                {
                    //queue is open, is this computer next
                    var firstTaskInQueue = ctx.ActiveImagingTask.GetNextComputerInQueue(thisComputerTask);
                    if (firstTaskInQueue.ComputerId == thisComputerTask.ComputerId)
                    {
                        ChangeStatusInProgress(taskId);
                        queueStatus.Result = "true";
                        queueStatus.Position = "0";
                        return JsonConvert.SerializeObject(queueStatus);
                    }
                    //not time for this computer yet
                    queueStatus.Result = "false";
                    queueStatus.Position = ctx.ActiveImagingTask.GetQueuePosition(thisComputerTask);
                    return JsonConvert.SerializeObject(queueStatus);
                }
                //queue not open yet
                queueStatus.Result = "false";
                queueStatus.Position = ctx.ActiveImagingTask.GetQueuePosition(thisComputerTask);
                return JsonConvert.SerializeObject(queueStatus);
            }
            else
            {
                //New computer checking queue for the first time

                var inUse = ctx.ActiveImagingTask.GetCurrentQueue(thisComputerTask);
                var totalCapacity = 0;
                var comServer = ctx.ClientComServer.GetServer(thisComputerTask.ComServerId);
                totalCapacity = comServer.ImagingMaxClients;
                if (inUse < totalCapacity)
                {
                    ChangeStatusInProgress(taskId);

                    queueStatus.Result = "true";
                    queueStatus.Position = "0";
                    return JsonConvert.SerializeObject(queueStatus);
                }
                //place into queue
                var lastQueuedTask = ctx.ActiveImagingTask.GetLastQueuedTask(thisComputerTask);
                if (lastQueuedTask == null)
                    thisComputerTask.QueuePosition = 1;
                else
                    thisComputerTask.QueuePosition = lastQueuedTask.QueuePosition + 1;
                thisComputerTask.Status = EnumTaskStatus.ImagingStatus.InImagingQueue;
                ctx.ActiveImagingTask.UpdateActiveImagingTask(thisComputerTask);

                queueStatus.Result = "false";
                queueStatus.Position = ctx.ActiveImagingTask.GetQueuePosition(thisComputerTask);
                return JsonConvert.SerializeObject(queueStatus);
            }
        }

        public string CheckTaskAuth(string task, string token)
        {
            return "true";
            //deprecated - just return true;
        }

        public void DeleteImage(int profileId)
        {
            var profile = ctx.ImageProfile.ReadProfile(profileId);
            if (string.IsNullOrEmpty(profile.Image.Name)) return;
            //Remove existing custom deploy schema, it may not match newly updated image
            profile.CustomSchema = string.Empty;
            ctx.ImageProfile.Update(profile);

            profile.Image.LastUploadGuid = string.Empty;
            ctx.Image.Update(profile.Image);

            var delResult = ctx.Filessystem.DeleteImageFolders(profile.Image.Name);

        }

        public string CheckModelMatch(string environment, string systemModel)
        {
            var modelTask = new ModelTaskDTO();
            //Check for model match
            var modelMatchProfile = ctx.ImageProfile.GetModelMatch(systemModel, environment);
            if (modelMatchProfile != null)
            {
                var image = ctx.Image.GetImage(modelMatchProfile.ImageId);
                if (image != null)
                    modelTask.imageName = image.Name;
                modelTask.imageProfileId = modelMatchProfile.Id.ToString();
                modelTask.imageProfileName = modelMatchProfile.Name;
                return JsonConvert.SerializeObject(modelTask);
            }
            return JsonConvert.SerializeObject(new ModelTaskDTO());
        }

        public string GetComputerNameForPe(string id)
        {
            var determineTaskDto = new DetermineTaskDTO();

            EntityComputer computer;

            computer = ctx.Computer.GetComputerFromClientIdentifier(id);

            if (computer == null)
            {
                determineTaskDto.task = "ond";
                determineTaskDto.computerId = "false";
                return JsonConvert.SerializeObject(determineTaskDto);
            }


            determineTaskDto.computerId = computer.Id.ToString();
            determineTaskDto.computerName = computer.Name.Split(':').First(); //imaging only computers have a : in the name to avoid duplicates, just take beginning


            return JsonConvert.SerializeObject(determineTaskDto);
        }

        public string DetermineTask(string id)
        {
            var determineTaskDto = new DetermineTaskDTO();

            EntityComputer computer;


            computer = ctx.Computer.GetComputerFromClientIdentifier(id);
            

            if (computer == null)
            {
                determineTaskDto.task = "ond";
                determineTaskDto.computerId = "false";
                return JsonConvert.SerializeObject(determineTaskDto);
            }

            var computerTask = ctx.Computer.GetTaskForComputerCheckin(computer.Id);
            if (computerTask == null)
            {
                determineTaskDto.computerId = computer.Id.ToString();
                determineTaskDto.task = "ond";
                determineTaskDto.computerName = computer.Name.Split(':').First(); //imaging only computers have a : in the name to avoid duplicates, just take beginning
            }
            else
            {
                determineTaskDto.computerId = computer.Id.ToString();
                determineTaskDto.task = computerTask.Type;
                determineTaskDto.taskId = computerTask.Id.ToString();
                determineTaskDto.computerName = computer.Name.Split(':').First(); //imaging only computers have a : in the name to avoid duplicates, just take beginning
            }

            return JsonConvert.SerializeObject(determineTaskDto);
        }



    

        public void ErrorEmail(int taskId, string error)
        {
            var task = ctx.ActiveImagingTask.GetTask(taskId);
            ctx.ActiveImagingTask.SendTaskErrorEmail(task, error);
        }

        public string GetAllClusterComServers(int computerId)
        {
            var rnd = new Random();
            
            var imagingServers = ctx.GetCompImagingServers.Run(computerId,true);
            if (imagingServers == null) return "false";

            var randomDpList = new List<string>();
            try
            {
                randomDpList = imagingServers.OrderBy(x => rnd.Next()).Select(x => x.Url).ToList();
            }
            catch (Exception ex)
            {
                ctx.Log.Error("Could Not Select Random Com Server");
                ctx.Log.Error(ex.Message);
                return "false";
            }

            var result = "";
            foreach (var url in randomDpList)
            {
                result += url + " ";
            }

            return result;
        }

        public string GetCustomPartitionScript(int profileId)
        {
            return ctx.ImageProfile.ReadProfile(profileId).CustomPartitionScript;
        }

        public string GetCustomScript(int scriptId)
        {
            var script = ctx.ScriptModule.GetModule(scriptId);
            return script.ScriptContents;
        }

        public string GetFileCopySchema(int profileId)
        {
            var fileFolderSchema = new FileFolderCopySchema { FilesAndFolders = new List<FileFolderCopy>() };
            var counter = 0;
            foreach (var profileFileFolder in ctx.ImageProfile.GetImageProfileFileCopy(profileId))
            {
               
                var fileFolder = ctx.FileCopyModule.GetModule(profileFileFolder.FileCopyModuleId);

                var moduleFiles = ctx.Module.GetModuleFiles(fileFolder.Guid);
                foreach (var file in moduleFiles.OrderBy(x => x.FileName))
                {
                    var clientFileFolder = new FileFolderCopy();
                    clientFileFolder.ModuleGuid = fileFolder.Guid;
                    clientFileFolder.FileName = file.FileName;
                    clientFileFolder.DestinationFolder = fileFolder.Destination;
                    clientFileFolder.DestinationFolder = clientFileFolder.DestinationFolder.Split(':').Last();
                    clientFileFolder.DestinationFolder = clientFileFolder.DestinationFolder.Replace("\\", "/");
                    clientFileFolder.DestinationPartition = profileFileFolder.DestinationPartition;
                    clientFileFolder.IsDriver = fileFolder.IsDriver;
                    if (fileFolder.DecompressAfterCopy)
                        clientFileFolder.Unzip = "true";
                    else
                        clientFileFolder.Unzip = "false";

                    fileFolderSchema.FilesAndFolders.Add(clientFileFolder);
                    counter++;
                }

              
            }
            fileFolderSchema.Count = counter.ToString();
            return JsonConvert.SerializeObject(fileFolderSchema);
        }



        public string GetOriginalLvm(int profileId, string clientHd, string hdToGet, string partitionPrefix)
        {
            string result = null;

            var imageProfile = ctx.ImageProfile.ReadProfile(profileId);
            var hdNumberToGet = Convert.ToInt32(hdToGet);
            ctx.ClientPartition.SetImageSchema(imageProfile);
            
            var imageSchema = ctx.ClientPartition.GetImageSchema();
            foreach (var part in from part in imageSchema.HardDrives[hdNumberToGet].Partitions
                                 where part.Active
                                 where part.VolumeGroup != null
                                 where part.VolumeGroup.LogicalVolumes != null
                                 select part)
            {
                result = "pvcreate -u " + part.Uuid + " --norestorefile -yf " +
                         clientHd + partitionPrefix +
                         part.VolumeGroup.PhysicalVolume[part.VolumeGroup.PhysicalVolume.Length - 1] + "\r\n";
                result += "vgcreate " + part.VolumeGroup.Name + " " + clientHd + partitionPrefix +
                          part.VolumeGroup.PhysicalVolume[part.VolumeGroup.PhysicalVolume.Length - 1] + " -yf" + "\r\n";
                result += "echo \"" + part.VolumeGroup.Uuid + "\" >>/tmp/vg-" + part.VolumeGroup.Name +
                          "\r\n";
                foreach (var lv in part.VolumeGroup.LogicalVolumes.Where(lv => lv.Active))
                {
                    result += "lvcreate --yes -L " + lv.Size + "s -n " + lv.Name + " " +
                              lv.VolumeGroup + "\r\n";
                    var uuid = lv.FsType == "swap" ? lv.Uuid.Split('#')[0] : lv.Uuid;
                    result += "echo \"" + uuid + "\" >>/tmp/" + lv.VolumeGroup + "-" +
                              lv.Name + "\r\n";
                }
                result += "vgcfgbackup -f /tmp/lvm-" + part.VolumeGroup.Name + "\r\n";
            }

            return result;
        }

        public string GetSysprepTag(int tagId, string imageEnvironment)
        {
            var tag = ctx.SysprepModule.GetModule(tagId);
            tag.OpeningTag = StringManipulationServices.EscapeCharacter(tag.OpeningTag, new[] { ">", "<" });
            tag.ClosingTag = StringManipulationServices.EscapeCharacter(tag.ClosingTag, new[] { ">", "<", "/" });
            tag.Contents = StringManipulationServices.EscapeCharacter(tag.Contents, new[] { ">", "<", "/", "\"" });

            var a = tag.Contents.Replace("\r", imageEnvironment == "win" ? "" : "\\r");
            var b = a.Replace("\n", "\\n");

            if (b.Length >= 5)
            {
                if (b.Substring(b.Length - 5) == "\\r\\n'")
                    b = b.Substring(0, b.Length - 1) + "\\r\\n'";
            }
            tag.Contents = b;
            return JsonConvert.SerializeObject(tag);
        }

        public string ImageList(string environment, string computerId, string task, int userId = 0)
        {
            var images = ctx.Image.GetOnDemandImageList(task, userId);
            
            if (environment == "winpe")
            {
                images = images.Where(x => x.Environment == "winpe").ToList();
                var imageList = new List<WinPEImageList>();
                foreach (var image in images)
                {
                    var winpeImage = new WinPEImageList();
                    winpeImage.ImageId = image.Id.ToString();
                    winpeImage.ImageName = image.Name;
                    imageList.Add(winpeImage);
                }
                if(!imageList.Any())
                {
                    imageList.Add(new WinPEImageList() { ImageId = "-1", ImageName = "No Images Found"});
                }
                return JsonConvert.SerializeObject(imageList);
            }
            else
            {
                var imageList = new ImageList { Images = new List<string>() };
                if (images.Count == 0)
                {
                    imageList.Images.Add(-1 + " " + "No_Images_Found");
                    return JsonConvert.SerializeObject(imageList);
                }


                if (environment == "linux")
                    images =
                        images.Where(x => x.Environment != "winpe").ToList();
                foreach (var image in images)
                    imageList.Images.Add(image.Id + " " + image.Name.Replace(" ", "_"));

                if (imageList.Images.Count == 0)
                    imageList.Images.Add(-1 + " " + "No_Images_Found");
                return JsonConvert.SerializeObject(imageList);
            }
        }

        public string ImageProfileList(int imageId)
        {
            var selectedImage = ctx.Image.GetImage(imageId);
            if (selectedImage.Environment == "winpe")
            {
                var imageProfileList = new WinPEProfileList { ImageProfiles = new List<WinPEProfile>() };
                var profileCounter = 0;
                foreach (var imageProfile in ctx.Image.SearchProfiles(Convert.ToInt32(imageId)).OrderBy(x => x.Name)
                    )
                {
                    profileCounter++;
                    var winpeProfile = new WinPEProfile();
                    winpeProfile.ProfileId = imageProfile.Id.ToString();
                    winpeProfile.ProfileName = imageProfile.Name;
                    imageProfileList.ImageProfiles.Add(winpeProfile);

                    if (profileCounter == 1)
                        imageProfileList.FirstProfileId = imageProfile.Id.ToString();
                }
                imageProfileList.Count = profileCounter.ToString();
                return JsonConvert.SerializeObject(imageProfileList);
            }
            else
            {
                var imageProfileList = new ImageProfileList { ImageProfiles = new List<string>() };

                var profileCounter = 0;
                foreach (var imageProfile in ctx.Image.SearchProfiles(Convert.ToInt32(imageId)))
                {
                    profileCounter++;
                    imageProfileList.ImageProfiles.Add(imageProfile.Id + " " + imageProfile.Name.Replace(" ","_"));
                    if (profileCounter == 1)
                        imageProfileList.FirstProfileId = imageProfile.Id.ToString();
                }

                imageProfileList.Count = profileCounter.ToString();
                return JsonConvert.SerializeObject(imageProfileList);
            }
        }

        public string IsLoginRequired(string task)
        {
            switch (task)
            {
                case "register_modelmatch":
                case "register":
                case "debug":
                case "ond":
                    return ctx.Setting.GetSettingValue(SettingStrings.ConsoleTasksRequireLogin);
                case "deploy":
                case "upload":
                case "multicast":
                case "modelmatchdeploy":
                    return ctx.Setting.GetSettingValue(SettingStrings.WebTasksRequireLogin);


                default:
                    return "True";
            }
        }

        public string MulicastSessionList(string environment)
        {
            if (environment == "winpe")
            {
                var multicastList = new List<WinPEMulticastList>();
                foreach (var multicast in ctx.ActiveMulticastSession.GetOnDemandList())
                {
                    var multicastSession = new WinPEMulticastList();
                    multicastSession.Port = multicast.Id.ToString();
                    multicastSession.Name = multicast.Name;
                    multicastList.Add(multicastSession);
                }
                if(!multicastList.Any())
                {
                    multicastList.Add(new WinPEMulticastList() { Port = "-1", Name = "No Multicast Sessions" });
                }
                return JsonConvert.SerializeObject(multicastList);
            }
            else
            {
                var multicastList = new MulticastList { Multicasts = new List<string>() };

                foreach (var multicast in ctx.ActiveMulticastSession.GetOnDemandList())
                {
                    multicastList.Multicasts.Add(multicast.Id + " " + multicast.Name.Replace(" ","_"));
                }

                if (!multicastList.Multicasts.Any())
                {
                    multicastList.Multicasts.Add(-1 + " " + "No_Multicasts_Found");
                }
                
                return JsonConvert.SerializeObject(multicastList);
            }
        }

        public string MulticastCheckout(string portBase,int comServerId)
        {
            string result = null;
            var port = Convert.ToInt32(portBase);
            var mcTask = ctx.ActiveMulticastSession.GetAll().Where(x => x.Port == port && x.ComServerId == comServerId).FirstOrDefault();

            if (mcTask != null)
            {
                var prsRunning = true;

                if (Environment.OSVersion.ToString().Contains("Unix"))
                {
                    try
                    {
                        var prs = Process.GetProcessById(Convert.ToInt32(mcTask.Pid));
                        if (prs.HasExited)
                        {
                            prsRunning = false;
                        }
                    }
                    catch
                    {
                        prsRunning = false;
                    }
                }
                else
                {
                    try
                    {
                        Process.GetProcessById(Convert.ToInt32(mcTask.Pid));
                    }
                    catch
                    {
                        prsRunning = false;
                    }
                }
                if (!prsRunning)
                {
                    if (ctx.ActiveMulticastSession.Delete(mcTask.Id).Success)
                    {
                        result = "Success";
                        ctx.ActiveMulticastSession.SendMulticastCompletedEmail(mcTask);
                    }
                }
                else
                    result = "Cannot Close Session, It Is Still In Progress";
            }
            else
                result = "Session Is Already Closed";

            return result;
        }

        public string OnDemandCheckIn(string mac, int objectId, string task, string userId, string computerId, string comServers)
        {
            var checkIn = new CheckIn();

            if (userId != null) 
            {
                //Check permissions
                if (task.Contains("deploy"))
                {
                    if (!ctx.Authorization.IsAuthorized(Convert.ToInt32(userId),AuthorizationStrings.ImageDeployTask))
                    {
                        checkIn.Result = "false";
                        checkIn.Message = "This User Is Not Authorized To Deploy Images";
                        return JsonConvert.SerializeObject(checkIn);
                    }
                }

                else if (task.Contains("upload"))
                {
                    if (!ctx.Authorization.IsAuthorized(Convert.ToInt32(userId),AuthorizationStrings.ImageUploadTask))
                    {
                        checkIn.Result = "false";
                        checkIn.Message = "This User Is Not Authorized To Upload Images";
                        return JsonConvert.SerializeObject(checkIn);
                    }
                }

                else if (task.Contains("multicast"))
                {
                    if (!ctx.Authorization.IsAuthorized(Convert.ToInt32(userId),AuthorizationStrings.ImageMulticastTask))
                    {
                        checkIn.Result = "false";
                        checkIn.Message = "This User Is Not Authorized To Multicast Images";
                        return JsonConvert.SerializeObject(checkIn);
                    }
                }
            }

            EntityComputer computer = null;
            if (computerId != "false")
                computer = ctx.Computer.GetComputer(Convert.ToInt32(computerId));

            ImageProfileWithImage imageProfile = null;

            var arguments = "";
            if (task == "deploy" || task == "upload" || task == "clobber" || task == "ondupload" || task == "onddeploy" ||
                task == "unregupload" || task == "unregdeploy" || task == "modelmatchdeploy")
            {
                imageProfile = ctx.ImageProfile.ReadProfile(objectId);
                ctx.CreateTaskArguments.InitUnicast(computer, imageProfile, task);
                arguments = ctx.CreateTaskArguments.Execute();

            }
            else //Multicast
            {
                var multicast = ctx.ActiveMulticastSession.Get(objectId);
                imageProfile = ctx.ImageProfile.ReadProfile(multicast.ImageProfileId);
                ctx.CreateTaskArguments.InitMulticast(computer, imageProfile, task,multicast.ComServerId);
                arguments = ctx.CreateTaskArguments.Execute(multicast.Port.ToString());
            }

            int imageDistributionPoint = -1;
            try
            {
                imageDistributionPoint = ctx.GetBestCompImageServer.Run(computer, task,comServers);
            }
            catch
            {
                checkIn.Result = "false";
                checkIn.Message = "Could Not Determine A Client Com Imaging Server";
                return JsonConvert.SerializeObject(checkIn);
            }

            if (imageProfile.Image.Protected && (task == "upload" || task == "ondupload" || task == "unregupload"))
            {
                checkIn.Result = "false";
                checkIn.Message = "This Image Is Protected";
                return JsonConvert.SerializeObject(checkIn);
            }

            if(imageDistributionPoint == -1)
            {
                checkIn.Result = "false";
                checkIn.Message = "Could Not Determine A Client Com Imaging Server";
                return JsonConvert.SerializeObject(checkIn);
            }

            var imageServer = ctx.ClientComServer.GetServer(imageDistributionPoint);

            if (imageProfile.Image.Environment == "")
                imageProfile.Image.Environment = "linux";

            checkIn.ImageEnvironment = imageProfile.Image.Environment;

            if (imageProfile.Image.Environment == "winpe")
            {
                arguments += "image_server=\"" + imageServer.Url + "clientimaging/" + "\"\r\n";
            }
            else
            {
                arguments += " image_server=\"" + imageServer.Url + "clientimaging/" + "\"";
            }

            if (task.Contains("upload"))
            {
                if (!string.IsNullOrEmpty(imageServer.ImagingIp))
                {
                    if (imageProfile.Image.Environment == "winpe")
                        arguments += "upload_server=\"" + imageServer.ImagingIp + "\"\r\n";
                    else
                        arguments += " upload_server=\"" + imageServer.ImagingIp + "\"";
                }

                else
                {
                    //get the ip needed for upload
                    var urlHasIp = Regex.Match(imageServer.Url, @"\b(\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})\b");
                    if (urlHasIp.Success)
                    {
                        if (imageProfile.Image.Environment == "winpe")
                            arguments += "upload_server=\"" + urlHasIp.Captures[0] + "\"\r\n";
                        else
                            arguments += " upload_server=\"" + urlHasIp.Captures[0] + "\"";
                    }
                    else
                    {
                        //get from dns
                        var dnsName = imageServer.Url.Split(new[] { "//" }, StringSplitOptions.None).Last().Split(':').First();
                        var ipaddresses = Dns.GetHostAddresses(dnsName).Where(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToList();
                        if (ipaddresses.Count > 1)
                        {
                            checkIn.Result = "false";
                            checkIn.Message = "More Than 1 Ip Address Has Been Resolved For Com Server.  You Must Add an IP Override.";
                            return JsonConvert.SerializeObject(checkIn);
                        }
                        else
                        {
                            if (imageProfile.Image.Environment == "winpe")
                                arguments += "upload_server=\"" +
                                               ipaddresses.First().ToString() + "\"\r\n";
                            else
                                arguments += " upload_server=\"" +
                                              ipaddresses.First().ToString() + "\"";
                        }
                    }
                }

            }

            var activeTask = new EntityActiveImagingTask();
            activeTask.Direction = task;
            activeTask.UserId = Convert.ToInt32(userId);
            activeTask.Type = task;
            activeTask.ImageProfileId = imageProfile.Id;
            activeTask.ComServerId = imageDistributionPoint;
            activeTask.Status = EnumTaskStatus.ImagingStatus.CheckedIn;

            if (computer == null)
            {
                //Create Task for an unregistered on demand computer
                var rnd = new Random(DateTime.Now.Millisecond);
                var newComputerId = rnd.Next(-200000, -100000);

                if (imageProfile.Image.Environment == "winpe")
                    arguments += "computer_id=" + newComputerId + "\r\n";
                else
                    arguments += " computer_id=" + newComputerId;
                activeTask.ComputerId = newComputerId;
                activeTask.Arguments = mac;
            }
            else
            {
                //Create Task for a registered on demand computer
                activeTask.ComputerId = computer.Id;
                activeTask.Arguments = arguments;
            }
            ctx.ActiveImagingTask.AddActiveImagingTask(activeTask);

            var auditLog = new EntityAuditLog();
            switch (task)
            {
                case "ondupload":
                case "unregupload":
                case "upload":
                    auditLog.AuditType = EnumAuditEntry.AuditType.OndUpload;
                    break;
                default:
                    auditLog.AuditType = EnumAuditEntry.AuditType.OndDeploy;
                    break;
            }

            try
            {
                auditLog.ObjectId = activeTask.ComputerId;
                var user = ctx.User.GetUser(activeTask.UserId);
                if (user != null)
                    auditLog.UserName = user.Name;
                auditLog.ObjectName = computer != null ? computer.Name : mac;
                auditLog.UserId = activeTask.UserId;
                auditLog.ObjectType = "Computer";
                auditLog.ObjectJson = JsonConvert.SerializeObject(activeTask);
                ctx.AuditLog.AddAuditLog(auditLog);

                auditLog.ObjectId = imageProfile.ImageId;
                auditLog.ObjectName = imageProfile.Image.Name;
                auditLog.ObjectType = "Image";
                ctx.AuditLog.AddAuditLog(auditLog);

            }
            catch
            {
                //Do Nothing              
            }
            
            checkIn.Result = "true";
            checkIn.TaskArguments = arguments;
            checkIn.TaskId = activeTask.Id.ToString();
            return JsonConvert.SerializeObject(checkIn);
        }



        public string UpdateGuid(int profileId)
        {
            var comGuid = ctx.Config["ComServerUniqueId"];
            var thisComServer = ctx.ClientComServer.GetServerByGuid(comGuid);

            if (thisComServer == null)
            {
                ctx.Log.Error($"Com Server With Guid {comGuid} Not Found");
                return "false";
            }

            var imageProfile = ctx.ImageProfile.ReadProfile(profileId);
            var image = ctx.Image.GetImage(imageProfile.ImageId);
            var guid = Guid.NewGuid().ToString();
            image.LastUploadGuid = guid;
            ctx.Image.Update(image);

            var basePath = thisComServer.LocalStoragePath;
            if (ctx.Setting.GetSettingValue(SettingStrings.ImageDirectSmb).Equals("True"))
            {
                basePath = ctx.Setting.GetSettingValue(SettingStrings.StoragePath); //if image direct smb, save guid to smb share not local com server
            }
            var path = Path.Combine(basePath, "images", image.Name);

           
                if (ctx.Unc.NetUseWithCredentials() || ctx.Unc.LastError == 1219)
                {
                    try
                    {
                        Directory.CreateDirectory(path);
                        using (var file = new StreamWriter(Path.Combine(path, "guid")))
                        {
                            file.WriteLine(guid);
                        }
                        return "true";
                    }
                    catch (Exception ex)
                    {
                        ctx.Log.Error("Could Not Create Image Guid");
                        ctx.Log.Error(ex.Message);
                        return "false";
                    }
                }
                else
                {
                    return "false";
                }
        }

        public void UpdateProgress(int taskId, string progress, string progressType)
        {
            if (string.IsNullOrEmpty(progress)) return;
            var task = ctx.ActiveImagingTask.GetTask(taskId);
            if (progressType == "wim")
            {
                task.Elapsed = progress;
                task.Remaining = "";
                task.Completed = "";
                task.Rate = "";
            }
            else
            {
                var values = progress.Split('*').ToList();
                task.Elapsed = values[1];
                task.Remaining = values[2];
                task.Completed = values[3];
                task.Rate = values[4];
            }

            ctx.ActiveImagingTask.UpdateActiveImagingTask(task);
        }

        public void UpdateProgressPartition(int taskId, string partition)
        {
            var task = ctx.ActiveImagingTask.GetTask(taskId);
            task.Partition = partition;
            task.Elapsed = "Please Wait...";
            task.Remaining = "";
            task.Completed = "";
            task.Rate = "";
            ctx.ActiveImagingTask.UpdateActiveImagingTask(task);
        }

        public void UploadLog(string computerId, string logContents, string subType, string computerMac)
        {
            if (computerId == "false")
                computerId = "-1";
            var computerLog = new EntityComputerLog
            {
                ComputerId = Convert.ToInt32(computerId),
                Contents = logContents,
                Type = "image",
                SubType = subType,
                Mac = computerMac
            };
            ctx.ComputerLog.AddComputerLog(computerLog);
        }


        public string SaveImageSchema(int profileId, string schema)
        {
            var comGuid = ctx.Config["ComServerUniqueId"];
            var thisComServer = ctx.ClientComServer.GetServerByGuid(comGuid);

            if (thisComServer == null)
            {
                ctx.Log.Error($"Com Server With Guid {comGuid} Not Found");
                return "false";
            }

            var profile = ctx.Uow.ImageProfileRepository.GetImageProfileWithImage(profileId);

            var basePath = thisComServer.LocalStoragePath;
            var path = basePath + "images" + Path.DirectorySeparatorChar +
                             profile.Image.Name + Path.DirectorySeparatorChar;

            try
            {
                Directory.CreateDirectory(path);
                using (var file = new StreamWriter(path + "schema"))
                {
                    file.WriteLine(schema);
                }
                return "true";
            }
            catch (Exception ex)
            {
                ctx.Log.Error("Could Not Create Image Schema");
                ctx.Log.Error(ex.Message);
                return "false";
            }

        }

        public string SaveMbr(IFormFileCollection files, int profileId, string hdNumber)
        {
            var comGuid = ctx.Config["ComServerUniqueId"];
            var thisComServer = ctx.ClientComServer.GetServerByGuid(comGuid);

            if (thisComServer == null)
            {
                ctx.Log.Error($"Com Server With Guid {comGuid} Not Found");
                return "false";
            }
            var profile = ctx.Uow.ImageProfileRepository.GetImageProfileWithImage(profileId);

            var basePath = thisComServer.LocalStoragePath;
            var path = Path.Combine(basePath, "images", profile.Image.Name, $"hd{ hdNumber}");
            
            try
            {
                Directory.CreateDirectory(path);
                foreach (var postedFile in files)
                {
                    var filePath = Path.Combine(path, "table");
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        postedFile.CopyTo(stream); // Save file contents
                    }
                }
                return "true";
            }
            catch (Exception ex)
            {
                ctx.Log.Error("Could Not Save Mbr");
                ctx.Log.Error(ex.Message);
                return "false";
            }
        }

        public string GetSmbShare()
        {
            var smb = new SMB();
            smb.Username = ctx.Setting.GetSettingValue(SettingStrings.StorageUsername);
            smb.Domain = ctx.Setting.GetSettingValue(SettingStrings.StorageDomain);
            smb.SharePath = ctx.Setting.GetSettingValue(SettingStrings.StoragePath);
            smb.Password = ctx.Encryption.DecryptText(ctx.Setting.GetSettingValue(SettingStrings.StoragePassword));

            smb.SharePath = smb.SharePath.Replace(@"\", "/").TrimEnd('/');
            return JsonConvert.SerializeObject(smb);
        }
        public void CloseUpload(int taskId, int port)
        {
            var activeUploadSession = ctx.ActiveMulticastSession.GetAll().Where(x => x.UploadTaskId == taskId && x.Port == port).FirstOrDefault();
            if (activeUploadSession == null)
                return;
            var pid = activeUploadSession.Pid;
            ctx.ActiveMulticastSession.DeleteUpload(activeUploadSession.Id);

            //shouldn't be needed, by try to end task just in case it didn't close
            try
            {
                var prs = Process.GetProcessById(Convert.ToInt32(pid));
                var processName = prs.ProcessName;
            
                if (processName == "cmd")
                        KillProcess(Convert.ToInt32(pid));            
            }
            catch
            {
                //ignored

            }

        }

        private static void KillProcess(int pid)
        {
            var searcher =
                new ManagementObjectSearcher("Select * From Win32_Process Where ParentProcessID=" + pid);
            var moc = searcher.Get();
            foreach (var o in moc)
            {
                var mo = (ManagementObject)o;
                KillProcess(Convert.ToInt32(mo["ProcessID"]));
            }
            try
            {
                var proc = Process.GetProcessById(Convert.ToInt32(pid));
                proc.Kill();
            }
            catch
            {
                //ignored
            }
        }


    }
}
