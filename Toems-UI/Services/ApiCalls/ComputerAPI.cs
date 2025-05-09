using Blazored.LocalStorage;
using Newtonsoft.Json;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;

namespace Toems_ApiCalls
{
    public class ComputerAPI : BaseAPI<EntityComputer>
    {
        public ComputerAPI(string resource, ILocalStorageService protectedSessionStorage) : base(resource, protectedSessionStorage)
        {
            
        }

        public async Task<DtoActionResult> Restore(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/Restore/{1}", Resource, id);
            var response = await _apiRequest.ExecuteAsync<DtoActionResult>(Request);
            if (response != null)
            {
                if (response.Id == 0)
                    response.Success = false;
            }
            else
            {
                return new DtoActionResult()
                {
                    ErrorMessage = "Unknown Exception.  Check The Exception Logs For More Info.",
                    Success = false
                };
            }
            return response;
        }

        public async Task <DtoActionResult> Archive(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/Archive/{1}", Resource, id);
            var response = await _apiRequest.ExecuteAsync<DtoActionResult>(Request);
            if (response != null)
            {
                if (response.Id == 0)
                    response.Success = false;
            }
            else
            {
                return new DtoActionResult()
                {
                    ErrorMessage = "Unknown Exception.  Check The Exception Logs For More Info.",
                    Success = false
                };
            }
            return response;
        }

        public async Task<DtoActionResult> ClearImagingId(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/ClearImagingId/{1}", Resource, id);
            var response = await _apiRequest.ExecuteAsync<DtoActionResult>(Request);
            if (response != null)
            {
                if (response.Id == 0)
                    response.Success = false;
            }
            else
            {
                return new DtoActionResult()
                {
                    ErrorMessage = "Unknown Exception.  Check The Exception Logs For More Info.",
                    Success = false
                };
            }
            return response;
        }

        public async Task<List<EntitySoftwareInventory>> GetComputerSoftware(int id, string searchstring)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetSoftware", Resource);
            Request.AddParameter("id", id);
            Request.AddParameter("searchstring", searchstring);
            return await _apiRequest.ExecuteAsync<List<EntitySoftwareInventory>>(Request);
        }

        public async Task<IEnumerable<EntityComputerLog>> GetComputerImagingLogs(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetComputerImagingLogs/{1}", Resource, id);
            return await _apiRequest.ExecuteAsync<List<EntityComputerLog>>(Request);
        }

        public async Task<List<EntityClientComServer>> GetComputerEmServers(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetComputerEmServers/{1}", Resource,id);
            return await _apiRequest.ExecuteAsync<List<EntityClientComServer>>(Request);
        }

        public async Task<List<EntityClientComServer>> GetComputerTftpServers(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetComputerTftpServers/{1}", Resource, id);
            return await _apiRequest.ExecuteAsync<List<EntityClientComServer>>(Request);
        }

        public async Task<List<EntityClientComServer>> GetComputerImageServers(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetComputerImageServers/{1}", Resource, id);
            return await _apiRequest.ExecuteAsync<List<EntityClientComServer>>(Request);
        }

        public async Task<List<EntityCertificateInventory>> GetComputerCertificates(int id, string searchstring)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetCertificates", Resource);
            Request.AddParameter("id", id);
            Request.AddParameter("searchstring", searchstring);
            return await _apiRequest.ExecuteAsync<List<EntityCertificateInventory>>(Request);
        }

        public async Task<List<DtoCustomComputerInventory>> GetCustomInventory(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetCustomInventory", Resource);
            Request.AddParameter("id", id);
            return await _apiRequest.ExecuteAsync<List<DtoCustomComputerInventory>>(Request);
        }

        public async Task<List<DtoComputerUpdates>> GetUpdates(int id, string searchstring)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetUpdates", Resource);
            Request.AddParameter("id", id);
            Request.AddParameter("searchstring", searchstring);
            return await _apiRequest.ExecuteAsync<List<DtoComputerUpdates>>(Request);
        }

        public async Task<List<EntityUserLogin>> GetUserLogins(int id, string searchstring)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetUserLogins", Resource);
            Request.AddParameter("id", id);
            Request.AddParameter("searchstring", searchstring);
            return await _apiRequest.ExecuteAsync<List<EntityUserLogin>>(Request);
        }

        public async Task<DtoInventoryCollection> GetSystemInfo(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetSystemInfo", Resource);
            Request.AddParameter("id", id);
            return await _apiRequest.ExecuteAsync<DtoInventoryCollection>(Request);
        }

        public async Task<ImageProfileWithImage> GetEffectiveImage(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetEffectiveImage", Resource);
            Request.AddParameter("id", id);
            return await _apiRequest.ExecuteAsync<ImageProfileWithImage>(Request);
        }

        public async Task<EntityWinPeModule> GetEffectiveWinPe(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetEffectiveWinPe", Resource);
            Request.AddParameter("id", id);
            return await _apiRequest.ExecuteAsync<EntityWinPeModule>(Request);
        }

        public async Task<IEnumerable<EntityGroup>> GetComputerGroups(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetComputerGroups/{1}", Resource,id);
            return await _apiRequest.ExecuteAsync<List<EntityGroup>>(Request);
        }

        public async Task<IEnumerable<DtoGroupImage>> GetComputerGroupsWithImage(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetComputerGroupsWithImage/{1}", Resource, id);
            return await _apiRequest.ExecuteAsync<List<DtoGroupImage>>(Request);
        }

        public async Task<IEnumerable<EntityCustomComputerAttribute>> GetCustomAttributes(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetCustomAttributes/{1}", Resource, id);
            return await _apiRequest.ExecuteAsync<List<EntityCustomComputerAttribute>>(Request);
        }

        public async Task<bool> SendMessage(int id, DtoMessage message)
        {
            Request.Method = Method.Post;
            Request.Resource = string.Format("{0}/SendMessage/{1}", Resource,id);
            Request.AddParameter("application/json", JsonConvert.SerializeObject(message), ParameterType.RequestBody);
            var responseData = await _apiRequest.ExecuteAsync<DtoApiBoolResponse>(Request);
            if (responseData != null)
            {
                return responseData.Value;
            }
            return false;

        }

        public async Task<bool> ForceCheckin(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/ForceCheckin/{1}", Resource, id);
            var responseData = await _apiRequest.ExecuteAsync<DtoApiBoolResponse>(Request);
            if (responseData != null)
            {
                return responseData.Value;
            }
            return false;
        }

        public async Task<bool> RunModule(int id, string moduleGuid)
        {
            Request.Method = Method.Get;
            Request.AddParameter("computerId", id);
            Request.AddParameter("moduleGuid", moduleGuid);
            Request.Resource = string.Format("{0}/RunModule/", Resource);
            var responseData = await _apiRequest.ExecuteAsync<DtoApiBoolResponse>(Request);
            if (responseData != null)
            {
                return responseData.Value;
            }
            return false;
        }

        public async Task<bool> CollectInventory(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/CollectInventory/{1}", Resource, id);
            var responseData = await _apiRequest.ExecuteAsync<DtoApiBoolResponse>(Request);
            if (responseData != null)
            {
                return responseData.Value;
            }
            return false;
        }

        public async Task<bool> GetStatus(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetStatus/{1}", Resource, id);
            var responseData = await _apiRequest.ExecuteAsync<DtoApiBoolResponse>(Request);
            if (responseData != null)
            {
                return responseData.Value;
            }
            return false;
        }

        public async Task<bool> GetUptime(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetUptime/{1}", Resource, id);
            var responseData = await _apiRequest.ExecuteAsync<DtoApiBoolResponse>(Request);
            if (responseData != null)
            {
                return responseData.Value;
            }
            return false;
        }

        public async Task<bool> GetServiceLog(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetServiceLog/{1}", Resource, id);
            var responseData = await _apiRequest.ExecuteAsync<DtoApiBoolResponse>(Request);
            if (responseData != null)
            {
                return responseData.Value;
            }
            return false;
        }

        public async Task<bool> Reboot(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/Reboot/{1}", Resource, id);
            var responseData = await _apiRequest.ExecuteAsync<DtoApiBoolResponse>(Request);
            if (responseData != null)
            {
                return responseData.Value;
            }
            return false;
        }

        public async Task<bool> StartRemoteControl(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/StartRemoteControl/{1}", Resource, id);
            var responseData = await _apiRequest.ExecuteAsync<DtoApiBoolResponse>(Request);
            if (responseData != null)
            {
                return responseData.Value;
            }
            return false;
        }

        public async Task<bool> Shutdown(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/Shutdown/{1}", Resource, id);
            var responseData = await _apiRequest.ExecuteAsync<DtoApiBoolResponse>(Request);
            if (responseData != null)
            {
                return responseData.Value;
            }
            return false;
        }

        public async Task<bool> Wakeup(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/Wakeup/{1}", Resource, id);
            var responseData = await _apiRequest.ExecuteAsync<DtoApiBoolResponse>(Request);
            if (responseData != null)
            {
                return responseData.Value;
            }
            return false;
        }

        public async Task<bool> GetLoggedInUsers(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetLoggedInUsers/{1}", Resource, id);
            var responseData = await _apiRequest.ExecuteAsync<DtoApiBoolResponse>(Request);
            if (responseData != null)
            {
                return responseData.Value;
            }
            return false;
        }

        public async Task<IEnumerable<DtoComputerPolicyHistory>> GetPolicyHistory(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetPolicyHistory/{1}", Resource, id);
            return await _apiRequest.ExecuteAsync<List<DtoComputerPolicyHistory>>(Request);
        }

        public async Task<IEnumerable<EntityPolicy>> GetComputerPolicies(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetComputerPolicies/{1}", Resource, id);
            return await _apiRequest.ExecuteAsync<List<EntityPolicy>>(Request);
        }

        public async Task<IEnumerable<DtoModule>> GetComputerModules(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetComputerModules/{1}", Resource, id);
            return await _apiRequest.ExecuteAsync<List<DtoModule>>(Request);
        }

        public async Task<string> GetEffectivePolicy(int id, EnumPolicy.Trigger trigger, string comServerUrl)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetEffectivePolicy", Resource);
            Request.AddParameter("id", id);
            Request.AddParameter("trigger", trigger);
            Request.AddParameter("comServerUrl", comServerUrl);
            var result = await _apiRequest.ExecuteAsync<DtoApiStringResponse>(Request);
            if (result == null) return string.Empty;
            return string.IsNullOrEmpty(result.Value) ? string.Empty : result.Value;
        }


        public async Task<List<EntityComputer>> SearchComputers(DtoComputerFilter filter)
        {
            Request.Method = Method.Post;
            Request.Resource = $"{Resource}/SearchComputers";
            Request.AddParameter("application/json", JsonConvert.SerializeObject(filter), ParameterType.RequestBody);

            return await _apiRequest.ExecuteAsync<List<EntityComputer>>(Request);
        }
        
    

      

        public async Task<List<EntityComputer>> SearchForGroup(DtoSearchFilter filter)
        {
            Request.Method = Method.Post;
            Request.Resource = string.Format("{0}/SearchForGroup", Resource);
            Request.AddParameter("application/json", JsonConvert.SerializeObject(filter), ParameterType.RequestBody);
            return await _apiRequest.ExecuteAsync<List<EntityComputer>>(Request);
        }

        public async Task<string> ClearLastSocketResult(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/ClearLastSocketResult/{1}", Resource, id);
            var responseData = await _apiRequest.ExecuteAsync<DtoApiStringResponse>(Request);
            return responseData != null ? responseData.Value : string.Empty;

        }

        public async Task<string> GetLastSocketResult(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetLastSocketResult/{1}", Resource,id);
            var responseData = await _apiRequest.ExecuteAsync<DtoApiStringResponse>(Request);
            return responseData != null ? responseData.Value : string.Empty;

        }

        public async Task<string> GetActiveCount()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetActiveCount", Resource);
            var responseData = await _apiRequest.ExecuteAsync<DtoApiStringResponse>(Request);
            return responseData != null ? responseData.Value : string.Empty;

        }

        public async Task<string> GetImageOnlyCount()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetImageOnlyCount", Resource);
            var responseData = await _apiRequest.ExecuteAsync<DtoApiStringResponse>(Request);
            return responseData != null ? responseData.Value : string.Empty;

        }
        public async Task<string> GetAllCount()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetAllCount", Resource);
            var responseData = await _apiRequest.ExecuteAsync<DtoApiStringResponse>(Request);
            return responseData != null ? responseData.Value : string.Empty;

        }

        public async Task<IEnumerable<DtoComputerComment>> GetComments(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetComments/{1}", Resource, id);
            return await _apiRequest.ExecuteAsync<List<DtoComputerComment>>(Request);
        }

        public async Task<DtoActionResult> AddComment(DtoComputerComment comment)
        {
            Request.Method = Method.Post;
            Request.Resource = string.Format("{0}/AddComment/", Resource);
            Request.AddParameter("application/json", JsonConvert.SerializeObject(comment), ParameterType.RequestBody);
            return await _apiRequest.ExecuteAsync<DtoActionResult>(Request);
        }

        public async Task<IEnumerable<EntityAttachment>> GetAttachments(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetAttachments/{1}", Resource, id);
            return await _apiRequest.ExecuteAsync<List<EntityAttachment>>(Request);
        }

        public async Task<List<DtoProcessWithTime>> ComputerProcessTimes(DateTime dateCutoff, int limit, int computerId)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/ComputerProcessTimes", Resource);
            Request.AddParameter("dateCutoff", dateCutoff);
            Request.AddParameter("limit", limit);
            Request.AddParameter("computerId", computerId);
            return await _apiRequest.ExecuteAsync<List<DtoProcessWithTime>>(Request);
        }

        public async Task<List<DtoProcessWithCount>> ComputerProcessCounts(DateTime dateCutoff, int limit, int computerId)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/ComputerProcessCounts", Resource);
            Request.AddParameter("dateCutoff", dateCutoff);
            Request.AddParameter("limit", limit);
            Request.AddParameter("computerId", computerId);
            return await _apiRequest.ExecuteAsync<List<DtoProcessWithCount>>(Request);
        }

        public async Task<List<DtoProcessWithUser>> ComputerProcess(DateTime dateCutoff, int limit, int computerId)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/ComputerProcess", Resource);
            Request.AddParameter("dateCutoff", dateCutoff);
            Request.AddParameter("limit", limit);
            Request.AddParameter("computerId", computerId);
            return await _apiRequest.ExecuteAsync<List<DtoProcessWithUser>>(Request);
        }

        public async Task<string> StartDeploy(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/StartDeploy/{1}", Resource, id);
            var response = await _apiRequest.ExecuteAsync<DtoApiStringResponse>(Request);
            return response != null ? response.Value : string.Empty;
        }

        public async Task<string> StartDeployWinPe(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/StartDeployWinPe/{1}", Resource, id);
            var response = await _apiRequest.ExecuteAsync<DtoApiStringResponse>(Request);
            return response != null ? response.Value : string.Empty;
        }

        public async Task<string> StartUpload(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/StartUpload/{1}", Resource, id);
            var response = await _apiRequest.ExecuteAsync<DtoApiStringResponse>(Request);
            return response != null ? response.Value : string.Empty;
        }
    }
}