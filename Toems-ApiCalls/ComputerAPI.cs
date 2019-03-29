using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;

namespace Toems_ApiCalls
{
    public class ComputerAPI : BaseAPI<EntityComputer>
    {

        public ComputerAPI(string resource) : base(resource)
        {
            
        }

        public DtoActionResult Restore(int id)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/Restore/{1}", Resource, id);
            var response = new ApiRequest().Execute<DtoActionResult>(Request);
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

        public DtoActionResult Archive(int id)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/Archive/{1}", Resource, id);
            var response = new ApiRequest().Execute<DtoActionResult>(Request);
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

        public List<EntitySoftwareInventory> GetComputerSoftware(int id, string searchstring)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetSoftware", Resource);
            Request.AddParameter("id", id);
            Request.AddParameter("searchstring", searchstring);
            return new ApiRequest().Execute<List<EntitySoftwareInventory>>(Request);
        }

        public List<EntityCertificateInventory> GetComputerCertificates(int id, string searchstring)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetCertificates", Resource);
            Request.AddParameter("id", id);
            Request.AddParameter("searchstring", searchstring);
            return new ApiRequest().Execute<List<EntityCertificateInventory>>(Request);
        }

        public List<DtoCustomComputerInventory> GetCustomInventory(int id)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetCustomInventory", Resource);
            Request.AddParameter("id", id);
            return new ApiRequest().Execute<List<DtoCustomComputerInventory>>(Request);
        }

        public List<DtoComputerUpdates> GetUpdates(int id, string searchstring)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetUpdates", Resource);
            Request.AddParameter("id", id);
            Request.AddParameter("searchstring", searchstring);
            return new ApiRequest().Execute<List<DtoComputerUpdates>>(Request);
        }

        public List<EntityUserLogin> GetUserLogins(int id, string searchstring)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetUserLogins", Resource);
            Request.AddParameter("id", id);
            Request.AddParameter("searchstring", searchstring);
            return new ApiRequest().Execute<List<EntityUserLogin>>(Request);
        }

        public DtoInventoryCollection GetSystemInfo(int id)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetSystemInfo", Resource);
            Request.AddParameter("id", id);
            return new ApiRequest().Execute<DtoInventoryCollection>(Request);
        }

        public IEnumerable<EntityGroup> GetComputerGroups(int id)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetComputerGroups/{1}", Resource,id);
            return new ApiRequest().Execute<List<EntityGroup>>(Request);
        }

        public IEnumerable<EntityCustomComputerAttribute> GetCustomAttributes(int id)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetCustomAttributes/{1}", Resource, id);
            return new ApiRequest().Execute<List<EntityCustomComputerAttribute>>(Request);
        }

        public bool SendMessage(int id, DtoMessage message)
        {
            Request.Method = Method.POST;
            Request.Resource = string.Format("{0}/SendMessage/{1}", Resource,id);
            Request.AddJsonBody(message);
            var responseData = new ApiRequest().Execute<DtoApiBoolResponse>(Request);
            if (responseData != null)
            {
                return responseData.Value;
            }
            return false;

        }

        public bool ForceCheckin(int id)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/ForceCheckin/{1}", Resource, id);
            var responseData = new ApiRequest().Execute<DtoApiBoolResponse>(Request);
            if (responseData != null)
            {
                return responseData.Value;
            }
            return false;
        }

        public bool CollectInventory(int id)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/CollectInventory/{1}", Resource, id);
            var responseData = new ApiRequest().Execute<DtoApiBoolResponse>(Request);
            if (responseData != null)
            {
                return responseData.Value;
            }
            return false;
        }

        public bool GetStatus(int id)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetStatus/{1}", Resource, id);
            var responseData = new ApiRequest().Execute<DtoApiBoolResponse>(Request);
            if (responseData != null)
            {
                return responseData.Value;
            }
            return false;
        }

        public bool Reboot(int id)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/Reboot/{1}", Resource, id);
            var responseData = new ApiRequest().Execute<DtoApiBoolResponse>(Request);
            if (responseData != null)
            {
                return responseData.Value;
            }
            return false;
        }

        public bool Shutdown(int id)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/Shutdown/{1}", Resource, id);
            var responseData = new ApiRequest().Execute<DtoApiBoolResponse>(Request);
            if (responseData != null)
            {
                return responseData.Value;
            }
            return false;
        }

        public bool Wakeup(int id)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/Wakeup/{1}", Resource, id);
            var responseData = new ApiRequest().Execute<DtoApiBoolResponse>(Request);
            if (responseData != null)
            {
                return responseData.Value;
            }
            return false;
        }

        public string GetLoggedInUsers(int id)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetLoggedInUsers/{1}", Resource, id);
            var responseData = new ApiRequest().Execute<DtoApiStringResponse>(Request);
            if (responseData != null)
            {
                return responseData.Value;
            }
            return string.Empty;
        }

        public IEnumerable<DtoComputerPolicyHistory> GetPolicyHistory(int id)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetPolicyHistory/{1}", Resource, id);
            return new ApiRequest().Execute<List<DtoComputerPolicyHistory>>(Request);
        }

        public IEnumerable<EntityPolicy> GetComputerPolicies(int id)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetComputerPolicies/{1}", Resource, id);
            return new ApiRequest().Execute<List<EntityPolicy>>(Request);
        }

        public IEnumerable<DtoModule> GetComputerModules(int id)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetComputerModules/{1}", Resource, id);
            return new ApiRequest().Execute<List<DtoModule>>(Request);
        }

        public string GetEffectivePolicy(int id, EnumPolicy.Trigger trigger, string comServerUrl)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetEffectivePolicy", Resource);
            Request.AddParameter("id", id);
            Request.AddParameter("trigger", trigger);
            Request.AddParameter("comServerUrl", comServerUrl);
            var result = new ApiRequest().Execute<DtoApiStringResponse>(Request);
            if (result == null) return string.Empty;
            return string.IsNullOrEmpty(result.Value) ? string.Empty : result.Value;
        }


        public List<EntityComputer> SearchAllComputers(DtoSearchFilterAllComputers filter)
        {
            Request.Method = Method.POST;
            Request.Resource = $"{Resource}/SearchAllComputers";
            Request.AddJsonBody(filter);

            return new ApiRequest().Execute<List<EntityComputer>>(Request);
        }

        public Task<List<EntityComputer>> SearchAsync(DtoSearchFilterAllComputers filter)
        {
            Request.Method = Method.POST;
            Request.Resource = $"{Resource}/SearchAllComputers";
            Request.AddJsonBody(filter);

            var res = new ApiRequest().ExecuteAsync<List<EntityComputer>>(Request);
            return res;
        }

        public List<EntityComputer> SearchForGroup(DtoSearchFilter filter)
        {
            Request.Method = Method.POST;
            Request.Resource = string.Format("{0}/SearchForGroup", Resource);
            Request.AddJsonBody(filter);
            return new ApiRequest().Execute<List<EntityComputer>>(Request);
        }


        public string GetActiveCount()
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetActiveCount", Resource);
            var responseData = new ApiRequest().Execute<DtoApiStringResponse>(Request);
            return responseData != null ? responseData.Value : string.Empty;

        }
        public string GetAllCount()
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetAllCount", Resource);
            var responseData = new ApiRequest().Execute<DtoApiStringResponse>(Request);
            return responseData != null ? responseData.Value : string.Empty;

        }

        public IEnumerable<DtoComputerComment> GetComments(int id)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetComments/{1}", Resource, id);
            return new ApiRequest().Execute<List<DtoComputerComment>>(Request);
        }

        public DtoActionResult AddComment(DtoComputerComment comment)
        {
            Request.Method = Method.POST;
            Request.Resource = string.Format("{0}/AddComment/", Resource);
            Request.AddJsonBody(comment);
            return new ApiRequest().Execute<DtoActionResult>(Request);
        }

        public IEnumerable<EntityAttachment> GetAttachments(int id)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetAttachments/{1}", Resource, id);
            return new ApiRequest().Execute<List<EntityAttachment>>(Request);
        }

        public List<DtoProcessWithTime> ComputerProcessTimes(DateTime dateCutoff, int limit, int computerId)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/ComputerProcessTimes", Resource);
            Request.AddParameter("dateCutoff", dateCutoff);
            Request.AddParameter("limit", limit);
            Request.AddParameter("computerId", computerId);
            return new ApiRequest().Execute<List<DtoProcessWithTime>>(Request);
        }

        public List<DtoProcessWithCount> ComputerProcessCounts(DateTime dateCutoff, int limit, int computerId)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/ComputerProcessCounts", Resource);
            Request.AddParameter("dateCutoff", dateCutoff);
            Request.AddParameter("limit", limit);
            Request.AddParameter("computerId", computerId);
            return new ApiRequest().Execute<List<DtoProcessWithCount>>(Request);
        }

        public List<DtoProcessWithUser> ComputerProcess(DateTime dateCutoff, int limit, int computerId)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/ComputerProcess", Resource);
            Request.AddParameter("dateCutoff", dateCutoff);
            Request.AddParameter("limit", limit);
            Request.AddParameter("computerId", computerId);
            return new ApiRequest().Execute<List<DtoProcessWithUser>>(Request);
        }


    }
}