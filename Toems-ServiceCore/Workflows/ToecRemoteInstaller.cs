using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Toems_DataModel;
using Toems_Common.Enum;
using Toems_Common.Entity;
using Toems_Service.Entity;
using Newtonsoft.Json;
using Toems_Common.Dto;
using System.IO;
using Toems_Common;
using System.Threading;
using System.Web;
using Toems_ServiceCore.Infrastructure;

namespace Toems_Service.Workflows
{
    public class ToecRemoteInstaller
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly string ExpectedClientVersion = "";
        private string BaseSourcePath = "";

        public ToecRemoteInstaller()
        {

            var expectedClient = new ServiceVersion().Get(1).LatestClientVersion;
            var newVersion = expectedClient.Split('.');
            ExpectedClientVersion = string.Join(".", newVersion.Take(newVersion.Length - 1));
           
        }

        private void CreateThread(EntityToecDeployJob job, string threadId, UnitOfWork uow)
        {
            var deployThread = new EntityToecDeployThread() { JobId = job.Id, TaskId = threadId, DateTimeUpdated = DateTime.Now };
            uow.ToecDeployThreadRepository.Insert(deployThread);
            uow.Save();
        }

        private bool UpdateThreadStatusTime(EntityToecDeployJob job, string threadId, UnitOfWork uow)
        {
            var currentThread = uow.ToecDeployThreadRepository.Get(x => x.JobId == job.Id && x.TaskId == threadId).FirstOrDefault();
            if (currentThread == null)
            {
                return false;
            }
            else
            {
                if (DateTime.Now - currentThread.DateTimeUpdated > TimeSpan.FromMinutes(15))
                {
                    uow.ToecDeployThreadRepository.DeleteRange(x => x.TaskId == threadId);
                    uow.Save();
                    return false; //if thread hasn't check in for more than 15 minutes and then decides to check, return false to stop this thread, and let a new one start
                }

                currentThread.DateTimeUpdated = DateTime.Now;
                uow.ToecDeployThreadRepository.Update(currentThread, currentThread.Id);
                
            }
            uow.Save();
            return true;
        }

        public void Run(string applicationPath)
        {
            var uow = new UnitOfWork();
            BaseSourcePath = Path.Combine(applicationPath, "private", "agent");
            foreach (var job in uow.ToecDeployJobRepository.Get(x => x.Enabled))
            {
                //reset any computers that might be hung up
                var computers = uow.ToecTargetListComputerRepository.Get(x => x.TargetListId == job.TargetListId);
                foreach (var computer in computers)
                {
                    if((computer.Status == EnumToecDeployTargetComputer.TargetStatus.Queued
                        || computer.Status == EnumToecDeployTargetComputer.TargetStatus.Installing
                        || computer.Status == EnumToecDeployTargetComputer.TargetStatus.Reinstalling
                        || computer.Status == EnumToecDeployTargetComputer.TargetStatus.Uninstalling)
                        && DateTime.Now - computer.LastStatusDate >= TimeSpan.FromHours(12))
                    {
                        computer.Status = EnumToecDeployTargetComputer.TargetStatus.AwaitingAction;
                        uow.ToecTargetListComputerRepository.Update(computer, computer.Id);
                    }

                }
                uow.Save();

                //check if thread for this job is already running
                var threads = uow.ToecDeployThreadRepository.Get(x => x.JobId == job.Id);

                var thread = uow.ToecDeployThreadRepository.Get(x => x.JobId == job.Id).OrderByDescending(x => x.DateTimeUpdated).FirstOrDefault();
                if (thread != null)
                {
                    if (DateTime.Now - thread.DateTimeUpdated <= TimeSpan.FromMinutes(15))
                    {
                        Logger.Debug($"An active thread for {job.Name} is already running .  Skipping.");

                        //remove any older threads that could somehow still be running
                        uow.ToecDeployThreadRepository.DeleteRange(x => x.Id != thread.Id);
                        uow.Save();
                        continue;
                    }
                }

                Task.Run(() => RunInstallThread(job));
            }
        }

        public void RunSingle(DtoSingleToecDeploy singleJob)
        {
            BaseSourcePath = Path.Combine(HttpContext.Current.Server.MapPath("~"), "private", "agent");
            Task.Run(() => RunSingleInstallThread(singleJob));
         
        }
        private async void RunSingleInstallThread(DtoSingleToecDeploy singleJob)
        {

            Logger.Info("Deploying Toec To Computer: " + singleJob.ComputerName);
            using (var unc = new UncServices())
            {
                var deployId = Guid.NewGuid().ToString();
                if (unc.ConnectWithCredentials($"\\\\{singleJob.ComputerName}\\admin$", singleJob.Username, singleJob.Domain, singleJob.Password) || unc.LastError == 1219)
                {
                    if (!CopyFilesToAdminShare(singleJob.JobType, singleJob.ComputerName, deployId))
                    {
                        Logger.Error("Could Not Copy Files To Admin Share ");
                        return;
                    }

                    var deployServiceResult = InstallDeployService(singleJob.Username,singleJob.Password,singleJob.Domain, singleJob.ComputerName);
                    if (deployServiceResult != 0)
                    {
                        Logger.Error("Could Not Connect To Service Control Manager: " + deployServiceResult.ToString());
                        Cleanup(singleJob.Username,singleJob.Password,singleJob.Domain, singleJob.ComputerName);
                        return;
                    }

                    var actionComplete = false;
                    var actionCompleteCounter = 1;
                    while (true)
                    {
                        if (File.Exists($"\\\\{singleJob.ComputerName}\\admin$\\Magaeric Solutions\\{deployId}.success"))
                        {
                            actionComplete = true;
                            Logger.Info("Successfully deployed Toec to: " + singleJob.ComputerName);
                        }
                        else if (File.Exists($"\\\\{singleJob.ComputerName}\\admin$\\Magaeric Solutions\\{deployId}.failed"))
                        {
                            var errorMessage = "";
                            try
                            {
                                errorMessage = File.ReadAllText($"\\\\{singleJob.ComputerName}\\admin$\\Magaeric Solutions\\{deployId}.failed");
                            }
                            catch { }//ignored
                            actionComplete = true;
                            Logger.Error("Could Not Complete Toec Action on Computer: " + singleJob.ComputerName + " " + errorMessage);
                        }

                        if (actionComplete) break;

                        if (actionCompleteCounter > 30)
                        {
                            Logger.Error("Toec Installation Result Could Not Be Determined For Computer " + singleJob.ComputerName + " .  5 Minute Timeout exceeded.");
                            break;
                        }

                        actionCompleteCounter++;
                        await Task.Delay(10000);
                    }
                }
                else
                {
                    Logger.Debug($"Could Not Connect To {singleJob.ComputerName} Admin Share");
                    return;
                }

                Cleanup(singleJob.Username, singleJob.Password, singleJob.Domain, singleJob.ComputerName);
            }
        }

        private async void RunInstallThread(EntityToecDeployJob job)
        {
            Random rnd = new Random();
            var stopThread = false;
            var threadId = Guid.NewGuid().ToString();
            var uow = new UnitOfWork();
            CreateThread(job, threadId, uow);
            while (true)
            {
                uow = new UnitOfWork();
                var listTargetComputers = new List<EntityToecTargetListComputer>();

                if (job.JobType == EnumToecDeployJob.JobType.Install && job.RunMode == EnumToecDeployJob.RunMode.Continuous)
                    listTargetComputers = uow.ToecTargetListComputerRepository.Get(x => x.TargetListId == job.TargetListId);
                else if (job.JobType == EnumToecDeployJob.JobType.Install)
                    listTargetComputers = uow.ToecTargetListComputerRepository.Get(x => x.TargetListId == job.TargetListId && x.Status != EnumToecDeployTargetComputer.TargetStatus.InstallComplete);
                else if (job.JobType == EnumToecDeployJob.JobType.Reinstall)
                    listTargetComputers = uow.ToecTargetListComputerRepository.Get(x => x.TargetListId == job.TargetListId && x.Status != EnumToecDeployTargetComputer.TargetStatus.ReinstallComplete);
                else if (job.JobType == EnumToecDeployJob.JobType.Uninstall)
                    listTargetComputers = uow.ToecTargetListComputerRepository.Get(x => x.TargetListId == job.TargetListId && x.Status != EnumToecDeployTargetComputer.TargetStatus.UninstallComplete);

                if (!listTargetComputers.Any())
                    break;

                var excludedComputers = uow.ToecTargetListComputerRepository.Get(x => x.TargetListId == job.ExclusionListId).Select(x => x.Name);

                foreach (var ex in excludedComputers)
                {
                    listTargetComputers.RemoveAll(x => x.Name == ex);
                }

                int activeCount = 0;
                activeCount = listTargetComputers.Where(x => (x.Status == EnumToecDeployTargetComputer.TargetStatus.Installing
                || x.Status == EnumToecDeployTargetComputer.TargetStatus.Reinstalling
                || x.Status == EnumToecDeployTargetComputer.TargetStatus.Uninstalling 
                || x.Status == EnumToecDeployTargetComputer.TargetStatus.Queued) && DateTime.Now - x.LastStatusDate < TimeSpan.FromMinutes(5)).Count();
            
                var activeThreadComputers = new List<EntityToecTargetListComputer>();
                var maxWorkers = uow.SettingRepository.Get(x => x.Name == SettingStrings.ToecRemoteInstallMaxWorkers).FirstOrDefault().Value;
                while (activeCount < Convert.ToInt16(maxWorkers))
                {
                    EntityToecTargetListComputer target = null;
                    if (job.JobType == EnumToecDeployJob.JobType.Install && job.RunMode == EnumToecDeployJob.RunMode.Continuous)
                    {
                        var newTargetComputers = listTargetComputers.Where(x => DateTime.Now - x.LastStatusDate > TimeSpan.FromHours(4)).ToList(); //don't keep retrying same computers if list is very small
                        if(newTargetComputers.Any())
                            target = newTargetComputers[rnd.Next(newTargetComputers.Count)];
                    }
                    else
                        target = listTargetComputers.FirstOrDefault(x => x.Status == EnumToecDeployTargetComputer.TargetStatus.AwaitingAction || (x.Status == EnumToecDeployTargetComputer.TargetStatus.Failed && DateTime.Now - x.LastStatusDate > TimeSpan.FromHours(4)));

                    if (target == null && job.RunMode == EnumToecDeployJob.RunMode.Continuous)
                    {
                        break;
                    }
                    else if (target == null)
                    {
                        stopThread = true;
                        break;
                    }

                    target.Status = EnumToecDeployTargetComputer.TargetStatus.Queued;
                    target.LastStatusDate = DateTime.Now;
                    uow.ToecTargetListComputerRepository.Update(target, target.Id);
                    uow.Save();
                    activeThreadComputers.Add(target);
                    activeCount++;
                }

                foreach(var c in activeThreadComputers)
                {
                    if (job.JobType == EnumToecDeployJob.JobType.Install)
                        c.Status = EnumToecDeployTargetComputer.TargetStatus.Installing;
                    else if (job.JobType == EnumToecDeployJob.JobType.Reinstall)
                        c.Status = EnumToecDeployTargetComputer.TargetStatus.Reinstalling;
                    else if (job.JobType == EnumToecDeployJob.JobType.Uninstall)
                        c.Status = EnumToecDeployTargetComputer.TargetStatus.Uninstalling;
                    c.LastStatusDate = DateTime.Now;
                    uow.ToecTargetListComputerRepository.Update(c, c.Id);
                    uow.Save();


                    using (var unc = new UncServices())
                    {
                        var deployId = Guid.NewGuid().ToString();
                        if (unc.ConnectWithCredentials($"\\\\{c.Name}\\admin$", job.Username, job.Domain, new EncryptionServices().DecryptText(job.PasswordEncrypted)) || unc.LastError == 1219)
                        {
                            if (!CopyFilesToAdminShare(job.JobType, c.Name, deployId))
                            {
                                SetError(job, c, "Could Not Copy Files To Admin Share ", uow);
                                continue;
                            }

                            var deployServiceResult = InstallDeployService(job.Username, new EncryptionServices().DecryptText(job.PasswordEncrypted),job.Domain, c.Name);
                            if (deployServiceResult != 0)
                            {
                                SetError(job, c, "Could Not Connect To Service Control Manager: " + deployServiceResult.ToString(), uow);
                                Cleanup(job.Username, new EncryptionServices().DecryptText(job.PasswordEncrypted), job.Domain, c.Name);
                                continue;
                            }

                            var actionComplete = false;
                            var actionCompleteCounter = 1;
                            while (true)
                            {
                                if (File.Exists($"\\\\{c.Name}\\admin$\\Magaeric Solutions\\{deployId}.success"))
                                {
                                    actionComplete = true;
                                    SetComplete(job, c, uow);
                                }
                                else if (File.Exists($"\\\\{c.Name}\\admin$\\Magaeric Solutions\\{deployId}.failed"))
                                {
                                    var errorMessage = "";
                                    try
                                    {
                                        errorMessage = File.ReadAllText($"\\\\{c.Name}\\admin$\\Magaeric Solutions\\{deployId}.failed");
                                    }
                                    catch { }//ignored
                                    actionComplete = true;
                                    SetError(job, c, "Could Not Complete Toec Action: " + errorMessage, uow);
                                }

                                if (actionComplete) break;
                                
                                if(actionCompleteCounter > 30)
                                {
                                    SetError(job, c, "Toec Installation Result Could Not Be Determined.  5 Minute Timeout exceeded.",uow);
                                    break;
                                }

                                actionCompleteCounter++;
                                await Task.Delay(10000);
                                UpdateThreadStatusTime(job, threadId, uow);
                            }
                        }
                        else
                        {
                            Logger.Debug($"Could Not Connect To {c.Name} Admin Share");
                            SetError(job, c, "Could Not Connect To Admin Share: " + unc.LastError.ToString(),uow);
                            continue;
                        }

                        Cleanup(job.Username,new EncryptionServices().DecryptText(job.PasswordEncrypted),job.Domain,c.Name);
                    }
                }

                await Task.Delay(10000);

                if (!UpdateThreadStatusTime(job, threadId, uow)) break;
                if (stopThread) break;
            }

        }

        private void Cleanup(string username, string password, string domain, string computerName)
        {
            using (var imp = new ServiceImpersonation(domain, username, password))
            {
                using (var scManager = new ServiceScManager(computerName))
                {
                    if (scManager.ConnectionSuccessful)
                    {
                        var service = "Toec-Remote-Installer";
                        scManager.OpenService(service);
                        scManager.Stop();
                        scManager.Uninstall(service);
                    }
                }
            }
         
        }

        private void SetComplete(EntityToecDeployJob job, EntityToecTargetListComputer c, UnitOfWork uow)
        {
            c.Status = EnumToecDeployTargetComputer.TargetStatus.InstallComplete;
            c.LastStatusDate = DateTime.Now;
            if (job.JobType == EnumToecDeployJob.JobType.Install)
                c.LastUpdateDetails = "Successfully Installed Toec";
            else if (job.JobType == EnumToecDeployJob.JobType.Reinstall)
                c.LastUpdateDetails = "Successfully Re-Installed Toec";
            else if (job.JobType == EnumToecDeployJob.JobType.Uninstall)
                c.LastUpdateDetails = "Successfully UnInstalled Toec";
            uow.ToecTargetListComputerRepository.Update(c, c.Id);
            uow.Save();
        }

        private void SetError(EntityToecDeployJob job, EntityToecTargetListComputer c, string details, UnitOfWork uow)
        {
            c.Status = EnumToecDeployTargetComputer.TargetStatus.Failed;
            c.LastStatusDate = DateTime.Now;
            c.LastUpdateDetails = details;
            uow.ToecTargetListComputerRepository.Update(c, c.Id);
            uow.Save();
        }

        private bool CopyFilesToAdminShare(EnumToecDeployJob.JobType jobType, string computerName, string deployId)
        {
            var configFileContents = new DtoToecDeployConfig();
            configFileContents.Version = ExpectedClientVersion;
            configFileContents.DeployId = deployId;
            if (jobType == EnumToecDeployJob.JobType.Install)
                configFileContents.Install = true;
            else if (jobType == EnumToecDeployJob.JobType.Reinstall)
                configFileContents.Reinstall = true;
            else if (jobType == EnumToecDeployJob.JobType.Uninstall)
                configFileContents.Uninstall = true;

            var destinationFolder = $"\\\\{computerName}\\admin$\\Magaeric Solutions\\";
            try
            {
                Directory.CreateDirectory(destinationFolder);
                File.WriteAllText(destinationFolder + "RemoteInstall.json", JsonConvert.SerializeObject(configFileContents));


                foreach (var file in Directory.GetFiles(Path.Combine(BaseSourcePath, "RemoteInstaller")))
                {
                    var fileInfo = new FileInfo(file);
                    File.Copy(file, Path.Combine(destinationFolder, fileInfo.Name), true);
                }

                File.Copy(Path.Combine(BaseSourcePath, $"Toec-{ExpectedClientVersion}-x64.msi"), Path.Combine(destinationFolder, $"Toec-{ExpectedClientVersion}-x64.msi"),true);
                File.Copy(Path.Combine(BaseSourcePath, $"Toec-{ExpectedClientVersion}-x86.msi"), Path.Combine(destinationFolder, $"Toec-{ExpectedClientVersion}-x86.msi"),true);
            }
            catch(Exception ex) 
            {
                Logger.Error($"Could Not Copy Files To Admin Share on {computerName}: {ex.Message}");
                return false; 
            }
            return true;

        }

        private int InstallDeployService(string username, string password, string domain, string computerName)
        {
            using (var imp = new ServiceImpersonation(domain, username, password))
            {
                if (imp.LastError != 0)
                {
                    Logger.Debug("Could Not Impersonate User For Service Installation");
                    return imp.LastError;
                   
                }

                using (var scManager = new ServiceScManager(computerName))
                {
                    if (scManager.ConnectionSuccessful)
                    {

                        var service = "Toec-Remote-Installer";
                        scManager.OpenService(service);
                        //todo:  how to determine location of c:\windows on a remote computer? instead of hardcoding?
                        scManager.Install(service, "c:\\windows\\magaeric solutions\\Toec-Remote-Installer.exe");
                        if(scManager.Start())
                        {
                            Logger.Debug("Successfully Installed Remote Installer Service");
                            return 0;
                        }
                    }
                    else
                    {
                        return scManager.LastError;
                    }

                }
            }

            return 0;
        }
    }
}
