
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Dto.clientimaging;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_DataModel;
using Toems_Service.Entity;
using Toems_Service.Workflows;

namespace Toems_Service
{
    public class ClientImagingServices
    {
        private readonly ILog log = LogManager.GetLogger(typeof(ClientImagingServices));

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

            if(name.Length > 15)
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

            var existingComputer = new ServiceComputer().GetComputerFromClientIdentifier(clientIdentifier);
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
            var result = new ServiceComputer().AddComputer(computer);
            return JsonConvert.SerializeObject(result);
        }

        public string AddImage(string imageName)
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
            var result = new ServiceImage().Add(image);
            if (result.Success)
                result.Id = image.Id;

            return JsonConvert.SerializeObject(result);
        }



        public string AddImageWinPEEnv(string imageName)
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
            var result = new ServiceImage().Add(image);
            if (result.Success)
                result.Id = image.Id;

            return JsonConvert.SerializeObject(result);
        }

        public string GetRegistrationSettings()
        {
            var regDto = new RegistrationDTO();
            regDto.registrationEnabled = ServiceSetting.GetSettingValue(SettingStrings.RegistrationEnabled);
            regDto.keepNamePrompt = ServiceSetting.GetSettingValue(SettingStrings.DisabledRegNamePrompt);
            return JsonConvert.SerializeObject(regDto);
        }

        public bool AuthorizeApiCall(string token)
        {
            if (string.IsNullOrEmpty(token)) return false;
            var webRequiresLogin = ServiceSetting.GetSettingValue(SettingStrings.WebTasksRequireLogin);
            var consoleRequiresLogin = ServiceSetting.GetSettingValue(SettingStrings.ConsoleTasksRequireLogin);
            var globalToken = ServiceSetting.GetSettingValue(SettingStrings.GlobalImagingToken);

            if(webRequiresLogin.Equals("True") && consoleRequiresLogin.Equals("True"))
            {
                 var user = new ServiceUser().GetUserFromToken(token);
                if (user != null)
                    return true;
            }
            else
            {
                //global token is valid

                if (token.Equals(globalToken) && !string.IsNullOrEmpty(globalToken))
                    return true;
            }

            return false;
        }

        public void ChangeStatusInProgress(int taskId)
        {
            var task = new ServiceActiveImagingTask().GetTask(taskId);
            task.Status = EnumTaskStatus.ImagingStatus.Imaging;
            new ServiceActiveImagingTask().UpdateActiveImagingTask(task);
        }

        public string CheckForCancelledTask(int taskId)
        {
            var task = new ServiceActiveImagingTask().GetTask(taskId);
            if (task == null)
                return "true";
            return "false";
        }

        public string CheckHdRequirements(int profileId, int clientHdNumber, string newHdSize, string imageSchemaDrives,
            int clientLbs)
        {
            var result = new HardDriveSchema();

            var imageProfile = new ServiceImageProfile().ReadProfile(profileId);
            var partitionHelper = new ServiceClientPartition(imageProfile);
            var imageSchema = partitionHelper.GetImageSchema();

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
            result.SchemaHdNumber = partitionHelper.NextActiveHardDrive(listSchemaDrives, clientHdNumber);

            if (result.SchemaHdNumber == -1)
            {
                result.IsValid = "false";
                result.Message = "No Active Hard Drive Images Were Found To Deploy.";
                return JsonConvert.SerializeObject(result);
            }

            var newHdBytes = Convert.ToInt64(newHdSize);
            var minimumSize = partitionHelper.HardDrive(result.SchemaHdNumber, newHdBytes);

            if (clientLbs != 0) //if zero should be from the winpe imaging environment
            {
                if (imageProfile.Image.Type != "File")
                {
                    if (clientLbs != imageSchema.HardDrives[result.SchemaHdNumber].Lbs)
                    {
                        log.Error("Error: The Logical Block Size Of This Hard Drive " + clientLbs +
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
                log.Error("Error:  " + newHdBytes / 1024 / 1024 +
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
                result.PhysicalPartitions = partitionHelper.GetActivePartitions(result.SchemaHdNumber, imageProfile);
                result.PhysicalPartitionCount = partitionHelper.GetActivePartitionCount(result.SchemaHdNumber);
                result.PartitionType = imageSchema.HardDrives[result.SchemaHdNumber].Table;
                result.BootPartition = imageSchema.HardDrives[result.SchemaHdNumber].Boot;
                result.UsesLvm = partitionHelper.CheckForLvm(result.SchemaHdNumber);
                result.Guid = imageSchema.HardDrives[result.SchemaHdNumber].Guid;
                return JsonConvert.SerializeObject(result);
            }

            result.IsValid = "true";
            result.PhysicalPartitions = partitionHelper.GetActivePartitions(result.SchemaHdNumber, imageProfile);
            result.PhysicalPartitionCount = partitionHelper.GetActivePartitionCount(result.SchemaHdNumber);
            result.PartitionType = imageSchema.HardDrives[result.SchemaHdNumber].Table;
            result.BootPartition = imageSchema.HardDrives[result.SchemaHdNumber].Boot;
            result.UsesLvm = partitionHelper.CheckForLvm(result.SchemaHdNumber);
            result.Guid = imageSchema.HardDrives[result.SchemaHdNumber].Guid;
            return JsonConvert.SerializeObject(result);
        }

        public string CheckIn(string taskId)
        {
            var checkIn = new CheckIn();
            var computerServices = new ServiceComputer();

            var task = new ServiceActiveImagingTask().GetTask(Convert.ToInt32(taskId));

            if (task == null)
            {
                checkIn.Result = "false";
                checkIn.Message = "Could Not Find Task With Id" + taskId;
                return JsonConvert.SerializeObject(checkIn);
            }

            var computer = computerServices.GetComputer(task.ComputerId);
            if (computer == null)
            {
                checkIn.Result = "false";
                checkIn.Message = "The Computer Assigned To This Task Was Not Found";
                return JsonConvert.SerializeObject(checkIn);
            }

            var comServerId = new GetBestCompImageServer(computer, task.Type).Run();

            task.Status = EnumTaskStatus.ImagingStatus.CheckedIn;
            task.ComServerId = comServerId;

            var imageServer = new ServiceClientComServer().GetServer(comServerId);

            ImageProfileWithImage imageProfile = null;
            if (task.Type == "multicast")
            {
                var mcTask = new ServiceActiveMulticastSession().Get(task.MulticastId);
                var group = new ServiceGroup().GetGroupByName(mcTask.Name);
                imageProfile = new ServiceImageProfile().ReadProfile(group.ImageProfileId);
            }
            else
            {
               imageProfile = new ServiceImageProfile().ReadProfile(Convert.ToInt32(task.ImageProfileId));
            }
            if (imageProfile.Image.Protected && task.Type.Contains("upload"))
            {
                checkIn.Result = "false";
                checkIn.Message = "This Image Is Protected";
                return JsonConvert.SerializeObject(checkIn);
            }

            if (new ServiceActiveImagingTask().UpdateActiveImagingTask(task))
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
                    checkIn.TaskArguments += " image_server=\"" +
                                            imageServer.Url + "clientimaging/" + "\"\r\n";
                }
                else
                {
                    checkIn.TaskArguments += " image_server=\"" +
                                            imageServer.Url + "clientimaging/" + "\"";
                }

                if(task.Type.Contains("upload"))
                {
                    if (!string.IsNullOrEmpty(imageServer.ImagingIp))
                    {
                        if (imageProfile.Image.Environment == "winpe")
                            checkIn.TaskArguments += " upload_server=\"" +
                                           imageServer.ImagingIp + "\"\r\n";
                        else
                            checkIn.TaskArguments += " upload_server=\"" +
                                          imageServer.ImagingIp + "\"";
                    }

                    else
                    {
                        //get the ip needed for upload
                        var urlHasIp = Regex.Match(imageServer.Url, @"\b(\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})\b");
                        if (urlHasIp.Success)
                        {
                            if (imageProfile.Image.Environment == "winpe")
                                checkIn.TaskArguments +=  " upload_server=\"" +
                                               urlHasIp.Captures[0] + "\"\r\n";
                            else
                                checkIn.TaskArguments += " upload_server=\"" +
                                              urlHasIp.Captures[0] + "\"";
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
                                    checkIn.TaskArguments +=  " upload_server=\"" +
                                                   ipaddresses.First().ToString() + "\"\r\n";
                                else
                                    checkIn.TaskArguments += " upload_server=\"" +
                                                  ipaddresses.First().ToString() + "\"";
                            }
                        }
                    }
                    
                }

                return JsonConvert.SerializeObject(checkIn);
            }
            checkIn.Result = "false";
            checkIn.Message = "Could Not Update Task Status";
            return JsonConvert.SerializeObject(checkIn);
        }

        public void CheckOut(int taskId)
        {
            var activeImagingTaskServices = new ServiceActiveImagingTask();
            var task = activeImagingTaskServices.GetTask(taskId);
            if (task.Type.Contains("upload"))
            {
                //protect image by default
                var imageProfile = new ServiceImageProfile().ReadProfile(Convert.ToInt32(task.ImageProfileId));
                imageProfile.Image.Protected = true;
                new ServiceImage().Update(imageProfile.Image);
                new Workflows.ImageSync().RunAllServers();

            }

            if (task.Type.Contains("unreg"))
                activeImagingTaskServices.DeleteUnregisteredOndTask(task.Id);
            else
                activeImagingTaskServices.DeleteActiveImagingTask(task.Id);

            if (task.Type != "multicast" && task.Type != "ondmulticast")
                activeImagingTaskServices.SendTaskCompletedEmail(task);
        }

        public string CheckQueue(int taskId)
        {
            var queueStatus = new QueueStatus();
            var activeImagingTaskServices = new ServiceActiveImagingTask();
            var thisComputerTask = activeImagingTaskServices.GetTask(taskId);
            //var computer = new ServiceComputer().GetComputer(thisComputerTask.ComputerId);
            //Check if already part of the queue
            activeImagingTaskServices.CancelTimedOutTasks();
            if (thisComputerTask.Status == EnumTaskStatus.ImagingStatus.InImagingQueue)
            {
                //Delete Any tasks that have passed the timeout value

                //Check if the queue is open yet
                var inUse = activeImagingTaskServices.GetCurrentQueue(thisComputerTask);
                var totalCapacity = 0;
                var comServer = new ServiceClientComServer().GetServer(thisComputerTask.ComServerId);
                totalCapacity = comServer.ImagingMaxClients;
                if (inUse < totalCapacity)
                {
                    //queue is open, is this computer next
                    var firstTaskInQueue = activeImagingTaskServices.GetNextComputerInQueue(thisComputerTask);
                    if (firstTaskInQueue.ComputerId == thisComputerTask.ComputerId)
                    {
                        ChangeStatusInProgress(taskId);
                        queueStatus.Result = "true";
                        queueStatus.Position = "0";
                        return JsonConvert.SerializeObject(queueStatus);
                    }
                    //not time for this computer yet
                    queueStatus.Result = "false";
                    queueStatus.Position = activeImagingTaskServices.GetQueuePosition(thisComputerTask);
                    return JsonConvert.SerializeObject(queueStatus);
                }
                //queue not open yet
                queueStatus.Result = "false";
                queueStatus.Position = activeImagingTaskServices.GetQueuePosition(thisComputerTask);
                return JsonConvert.SerializeObject(queueStatus);
            }
            else
            {
                //New computer checking queue for the first time

                var inUse = activeImagingTaskServices.GetCurrentQueue(thisComputerTask);
                var totalCapacity = 0;
                var comServer = new ServiceClientComServer().GetServer(thisComputerTask.ComServerId);
                totalCapacity = comServer.ImagingMaxClients;
                if (inUse < totalCapacity)
                {
                    ChangeStatusInProgress(taskId);

                    queueStatus.Result = "true";
                    queueStatus.Position = "0";
                    return JsonConvert.SerializeObject(queueStatus);
                }
                //place into queue
                var lastQueuedTask = activeImagingTaskServices.GetLastQueuedTask(thisComputerTask);
                if (lastQueuedTask == null)
                    thisComputerTask.QueuePosition = 1;
                else
                    thisComputerTask.QueuePosition = lastQueuedTask.QueuePosition + 1;
                thisComputerTask.Status = EnumTaskStatus.ImagingStatus.InImagingQueue;
                activeImagingTaskServices.UpdateActiveImagingTask(thisComputerTask);

                queueStatus.Result = "false";
                queueStatus.Position = activeImagingTaskServices.GetQueuePosition(thisComputerTask);
                return JsonConvert.SerializeObject(queueStatus);
            }
        }

        public string CheckTaskAuth(string task, string token)
        {
            //only check console tasks, 
            var consoleRequiresLogin = ServiceSetting.GetSettingValue(SettingStrings.ConsoleTasksRequireLogin);

            if (consoleRequiresLogin.Equals("False"))
            {
                return "true";
            }
            else
            {
                var globalToken = ServiceSetting.GetSettingValue(SettingStrings.GlobalImagingToken);
                if (token.Equals(globalToken) && !string.IsNullOrEmpty(globalToken))
                    return "true";
                else
                {
                    var user = new ServiceUser().GetUserFromToken(token);
                    if (user != null)
                        return "true";
                    else
                        return "false";
                }
            }
          
        }

        public void DeleteImage(int profileId)
        {
            var profile = new ServiceImageProfile().ReadProfile(profileId);
            if (string.IsNullOrEmpty(profile.Image.Name)) return;
            //Remove existing custom deploy schema, it may not match newly updated image
            profile.CustomSchema = string.Empty;
            new ServiceImageProfile().Update(profile);

            profile.Image.LastUploadGuid = string.Empty;
            new ServiceImage().Update(profile.Image);

            var delResult = new FilesystemServices().DeleteImageFolders(profile.Image.Name);

        }

        public string CheckModelMatch(string environment, string systemModel)
        {
            var modelTask = new ModelTaskDTO();
            //Check for model match
            var modelMatchProfile = new ServiceImageProfile().GetModelMatch(systemModel, environment);
            if (modelMatchProfile != null)
            {
                var image = new ServiceImage().GetImage(modelMatchProfile.ImageId);
                if (image != null)
                    modelTask.imageName = image.Name;
                modelTask.imageProfileId = modelMatchProfile.Id.ToString();
                modelTask.imageProfileName = modelMatchProfile.Name;
                return JsonConvert.SerializeObject(modelTask);
            }
            return JsonConvert.SerializeObject(new ModelTaskDTO());
        }

        public string DetermineTask(string id)
        {
            var determineTaskDto = new DetermineTaskDTO();
            var computerServices = new ServiceComputer();
            EntityComputer computer;


            computer = computerServices.GetComputerFromClientIdentifier(id);
            

            if (computer == null)
            {
                determineTaskDto.task = "ond";
                determineTaskDto.computerId = "false";
                return JsonConvert.SerializeObject(determineTaskDto);
            }

            var computerTask = computerServices.GetTaskForComputerCheckin(computer.Id);
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
            var activeImagingTaskServices = new ServiceActiveImagingTask();
            var task = activeImagingTaskServices.GetTask(taskId);
            activeImagingTaskServices.SendTaskErrorEmail(task, error);
        }

        public string GetAllClusterComServers(int computerId)
        {
            var rnd = new Random();
            var computerServices = new ServiceComputer();
            var computer = computerServices.GetComputer(computerId);
            var uow = new UnitOfWork();

            var imagingServers = new Workflows.GetCompImagingServers().Run(computerId);
            if (imagingServers == null) return "false";

            var randomDpList = new List<string>();
            try
            {
                randomDpList = imagingServers.OrderBy(x => rnd.Next()).Select(x => x.Url).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Could Not Select Random Com Server");
                log.Error(ex.Message);
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
            return new ServiceImageProfile().ReadProfile(profileId).CustomPartitionScript;
        }

        public string GetCustomScript(int scriptId)
        {
            var script = new ServiceScriptModule().GetModule(scriptId);
            return script.ScriptContents;
        }

        public string GetFileCopySchema(int profileId)
        {
            var fileFolderSchema = new FileFolderCopySchema { FilesAndFolders = new List<FileFolderCopy>() };
            var counter = 0;
            foreach (var profileFileFolder in new ServiceImageProfile().GetImageProfileFileCopy(profileId))
            {
               
                var fileFolder = new ServiceFileCopyModule().GetModule(profileFileFolder.FileCopyModuleId);

                var moduleFiles = new ServiceModule().GetModuleFiles(fileFolder.Guid);
                foreach (var file in moduleFiles.OrderBy(x => x.FileName))
                {
                    var clientFileFolder = new FileFolderCopy();
                    clientFileFolder.ModuleGuid = fileFolder.Guid;
                    clientFileFolder.FileName = file.FileName;
                    clientFileFolder.DestinationFolder = fileFolder.Destination;
                    clientFileFolder.DestinationFolder = clientFileFolder.DestinationFolder.Split(':').Last();
                    clientFileFolder.DestinationFolder = clientFileFolder.DestinationFolder.Replace("\\", "/");
                    clientFileFolder.DestinationPartition = profileFileFolder.DestinationPartition;
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

            var imageProfile = new ServiceImageProfile().ReadProfile(profileId);
            var hdNumberToGet = Convert.ToInt32(hdToGet);
            var partitionHelper = new ServiceClientPartition(imageProfile);
            var imageSchema = partitionHelper.GetImageSchema();
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
            var tag = new ServiceSysprepModule().GetModule(tagId);
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
            var images = new ServiceImage().GetOnDemandImageList(task, userId);

            if (images.Count == 0)
            {
                var imageList = new ImageList { Images = new List<string>() };
                imageList.Images.Add(-1 + " " + "No_Images_Found");
                return JsonConvert.SerializeObject(imageList);
            }
            if (computerId == "false")
                computerId = "0";
            
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
                    imageList.Images.Add(image.Id + " " + image.Name);

                if (imageList.Images.Count == 0)
                    imageList.Images.Add(-1 + " " + "No_Images_Found");
                return JsonConvert.SerializeObject(imageList);
            }
        }

        public string ImageProfileList(int imageId)
        {
            var imageServices = new ServiceImage();
            var selectedImage = imageServices.GetImage(imageId);
            if (selectedImage.Environment == "winpe")
            {
                var imageProfileList = new WinPEProfileList { ImageProfiles = new List<WinPEProfile>() };
                var profileCounter = 0;
                foreach (var imageProfile in imageServices.SearchProfiles(Convert.ToInt32(imageId)).OrderBy(x => x.Name)
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
                foreach (var imageProfile in imageServices.SearchProfiles(Convert.ToInt32(imageId)))
                {
                    profileCounter++;
                    imageProfileList.ImageProfiles.Add(imageProfile.Id + " " + imageProfile.Name);
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
                    return ServiceSetting.GetSettingValue(SettingStrings.ConsoleTasksRequireLogin);
                case "deploy":
                case "upload":
                case "multicast":
                case "modelmatchdeploy":
                    return ServiceSetting.GetSettingValue(SettingStrings.WebTasksRequireLogin);


                default:
                    return "True";
            }
        }

        public string MulicastSessionList(string environment)
        {
            if (environment == "winpe")
            {
                var multicastList = new List<WinPEMulticastList>();
                foreach (var multicast in new ServiceActiveMulticastSession().GetOnDemandList())
                {
                    var multicastSession = new WinPEMulticastList();
                    multicastSession.Port = multicast.Id.ToString();
                    multicastSession.Name = multicast.Name;
                    multicastList.Add(multicastSession);
                }
                return JsonConvert.SerializeObject(multicastList);
            }
            else
            {
                var multicastList = new MulticastList { Multicasts = new List<string>() };

                foreach (var multicast in new ServiceActiveMulticastSession().GetOnDemandList())
                {
                    multicastList.Multicasts.Add(multicast.Id + " " + multicast.Name);
                }

                return JsonConvert.SerializeObject(multicastList);
            }
        }

        public string MulticastCheckout(string portBase,int comServerId)
        {
            string result = null;
            var port = Convert.ToInt32(portBase);
            var activeMulticastSessionServices = new ServiceActiveMulticastSession();
            var mcTask = activeMulticastSessionServices.GetAll().Where(x => x.Port == port && x.ComServerId == comServerId).FirstOrDefault();

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
                    if (activeMulticastSessionServices.Delete(mcTask.Id).Success)
                    {
                        result = "Success";
                        activeMulticastSessionServices.SendMulticastCompletedEmail(mcTask);
                    }
                }
                else
                    result = "Cannot Close Session, It Is Still In Progress";
            }
            else
                result = "Session Is Already Closed";

            return result;
        }

        public string OnDemandCheckIn(string mac, int objectId, string task, string userId, string computerId)
        {
            var checkIn = new CheckIn();
            var computerServices = new ServiceComputer();

            if (userId != null) 
            {
                //Check permissions
                if (task.Contains("deploy"))
                {
                    if (
                        !new AuthorizationServices(Convert.ToInt32(userId), AuthorizationStrings.ImageDeployTask)
                            .IsAuthorized())
                    {
                        checkIn.Result = "false";
                        checkIn.Message = "This User Is Not Authorized To Deploy Images";
                        return JsonConvert.SerializeObject(checkIn);
                    }
                }

                else if (task.Contains("upload"))
                {
                    if (
                        !new AuthorizationServices(Convert.ToInt32(userId), AuthorizationStrings.ImageUploadTask)
                            .IsAuthorized())
                    {
                        checkIn.Result = "false";
                        checkIn.Message = "This User Is Not Authorized To Upload Images";
                        return JsonConvert.SerializeObject(checkIn);
                    }
                }

                else if (task.Contains("multicast"))
                {
                    if (
                        !new AuthorizationServices(Convert.ToInt32(userId), AuthorizationStrings.ImageMulticastTask)
                            .IsAuthorized())
                    {
                        checkIn.Result = "false";
                        checkIn.Message = "This User Is Not Authorized To Multicast Images";
                        return JsonConvert.SerializeObject(checkIn);
                    }
                }
            }

            EntityComputer computer = null;
            if (computerId != "false")
                computer = computerServices.GetComputer(Convert.ToInt32(computerId));

            ImageProfileWithImage imageProfile = null;

            var arguments = "";
            if (task == "deploy" || task == "upload" || task == "clobber" || task == "ondupload" || task == "onddeploy" ||
                task == "unregupload" || task == "unregdeploy" || task == "modelmatchdeploy")
            {
                imageProfile = new ServiceImageProfile().ReadProfile(objectId);
                arguments = new CreateTaskArguments(computer, imageProfile, task).Execute();

            }
            else //Multicast
            {
                var multicast = new ServiceActiveMulticastSession().Get(objectId);
                imageProfile = new ServiceImageProfile().ReadProfile(multicast.ImageProfileId);
                arguments = new CreateTaskArguments(computer, imageProfile, task,multicast.ComServerId).Execute(multicast.Port.ToString());
            }

            int imageDistributionPoint = -1;
            try
            {
                imageDistributionPoint = new GetBestCompImageServer(computer, task).Run();
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

            var imageServer = new ServiceClientComServer().GetServer(imageDistributionPoint);

            if (imageProfile.Image.Environment == "")
                imageProfile.Image.Environment = "linux";

            checkIn.ImageEnvironment = imageProfile.Image.Environment;

            if (imageProfile.Image.Environment == "winpe")
            {
                arguments += " image_server=\"" + imageServer.Url + "clientimaging/" + "\"\r\n";
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
                        arguments += " upload_server=\"" + imageServer.ImagingIp + "\"\r\n";
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
                            arguments += " upload_server=\"" + urlHasIp.Captures[0] + "\"\r\n";
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
                                arguments += " upload_server=\"" +
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
            new ServiceActiveImagingTask().AddActiveImagingTask(activeTask);

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
                var user = new ServiceUser().GetUser(activeTask.UserId);
                if (user != null)
                    auditLog.UserName = user.Name;
                auditLog.ObjectName = computer != null ? computer.Name : mac;
                auditLog.UserId = activeTask.UserId;
                auditLog.ObjectType = "Computer";
                auditLog.ObjectJson = JsonConvert.SerializeObject(activeTask);
                new ServiceAuditLog().AddAuditLog(auditLog);

                auditLog.ObjectId = imageProfile.ImageId;
                auditLog.ObjectName = imageProfile.Image.Name;
                auditLog.ObjectType = "Image";
                new ServiceAuditLog().AddAuditLog(auditLog);

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
            var comGuid = ConfigurationManager.AppSettings["ComServerUniqueId"];
            var thisComServer = new ServiceClientComServer().GetServerByGuid(comGuid);

            if (thisComServer == null)
            {
                log.Error($"Com Server With Guid {comGuid} Not Found");
                return "false";
            }

            var imageProfile = new ServiceImageProfile().ReadProfile(profileId);
            var imageServices = new ServiceImage();
            var image = imageServices.GetImage(imageProfile.ImageId);
            var guid = Guid.NewGuid().ToString();
            image.LastUploadGuid = guid;
            imageServices.Update(image);

            var basePath = thisComServer.LocalStoragePath;
            var path = Path.Combine(basePath, "images", image.Name);


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
                log.Error("Could Not Create Image Guid");
                log.Error(ex.Message);
                return "false";
            }
        }

        public void UpdateProgress(int taskId, string progress, string progressType)
        {
            var activeImagingTaskServices = new ServiceActiveImagingTask();
            if (string.IsNullOrEmpty(progress)) return;
            var task = activeImagingTaskServices.GetTask(taskId);
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

            activeImagingTaskServices.UpdateActiveImagingTask(task);
        }

        public void UpdateProgressPartition(int taskId, string partition)
        {
            var activeImagingTaskServices = new ServiceActiveImagingTask();
            var task = activeImagingTaskServices.GetTask(taskId);
            task.Partition = partition;
            task.Elapsed = "Please Wait...";
            task.Remaining = "";
            task.Completed = "";
            task.Rate = "";
            activeImagingTaskServices.UpdateActiveImagingTask(task);
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
            new ServiceComputerLog().AddComputerLog(computerLog);
        }


        public string SaveImageSchema(int profileId, string schema)
        {
            var comGuid = ConfigurationManager.AppSettings["ComServerUniqueId"];
            var thisComServer = new ServiceClientComServer().GetServerByGuid(comGuid);

            if (thisComServer == null)
            {
                log.Error($"Com Server With Guid {comGuid} Not Found");
                return "false";
            }

            var profile = new UnitOfWork().ImageProfileRepository.GetImageProfileWithImage(profileId);

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
                log.Error("Could Not Create Image Schema");
                log.Error(ex.Message);
                return "false";
            }

        }

        public string SaveMbr(HttpFileCollection files, int profileId, string hdNumber)
        {
            var comGuid = ConfigurationManager.AppSettings["ComServerUniqueId"];
            var thisComServer = new ServiceClientComServer().GetServerByGuid(comGuid);

            if (thisComServer == null)
            {
                log.Error($"Com Server With Guid {comGuid} Not Found");
                return "false";
            }
            var profile = new UnitOfWork().ImageProfileRepository.GetImageProfileWithImage(profileId);

            var basePath = thisComServer.LocalStoragePath;
            var path = Path.Combine(basePath, "images", profile.Image.Name, $"hd{ hdNumber}");


            try
            {
                Directory.CreateDirectory(path);
                foreach (string file in files)
                {
                    var postedFile = files[file];
                    var filePath = Path.Combine(path, "table");
                    postedFile.SaveAs(filePath);

                }
                return "true";
            }
            catch (Exception ex)
            {
                log.Error("Could Not Save Mbr");
                log.Error(ex.Message);
                return "false";
            }

        }

        public void CloseUpload(int taskId, int port)
        {
            var activeUploadSession = new ServiceActiveMulticastSession().GetAll().Where(x => x.UploadTaskId == taskId && x.Port == port).FirstOrDefault();
            if (activeUploadSession == null)
                return;
            var pid = activeUploadSession.Pid;
            new ServiceActiveMulticastSession().DeleteUpload(activeUploadSession.Id);

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
