using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Dto.exports;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{
    public class PolicyAPI : BaseAPI<EntityPolicy>
    {

        public PolicyAPI(string resource, ProtectedLocalStorage protectedLocalStorage) : base(resource, protectedLocalStorage)
        {

        }

        public string GetArchivedCount()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetArchivedCount", Resource);
            var responseData = _apiRequest.Execute<DtoApiStringResponse>(Request);
            return responseData != null ? responseData.Value : string.Empty;

        }

        public List<EntityPolicy> GetArchived(DtoSearchFilterCategories filter)
        {
            Request.Method = Method.Post;
            Request.Resource = string.Format("{0}/GetArchived", Resource);
            Request.AddParameter("application/json", JsonConvert.SerializeObject(filter), ParameterType.RequestBody);
            return _apiRequest.Execute<List<EntityPolicy>>(Request);
        }

        public DtoActionResult Archive(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/Archive/{1}", Resource, id);
            var response = _apiRequest.Execute<DtoActionResult>(Request);
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

        public DtoActionResult Restore(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/Restore/{1}", Resource, id);
            var response = _apiRequest.Execute<DtoActionResult>(Request);
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

        public DtoActionResult Clone(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/Clone/{1}", Resource, id);
            var response = _apiRequest.Execute<DtoActionResult>(Request);
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

        public IEnumerable<EntityPolicyComServer> GetPolicyComServers(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetPolicyComServers/{1}", Resource, id);
            return _apiRequest.Execute<List<EntityPolicyComServer>>(Request);
        }

        public IEnumerable<EntityPolicyCategory> GetPolicyCategories(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetPolicyCategories/{1}", Resource, id);
            return _apiRequest.Execute<List<EntityPolicyCategory>>(Request);
        }

        public IEnumerable<EntityGroup> GetPolicyGroups(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetPolicyGroups/{1}", Resource, id);
            return _apiRequest.Execute<List<EntityGroup>>(Request);
        }

        public IEnumerable<EntityComputer> GetPolicyComputers(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetPolicyComputers/{1}", Resource, id);
            return _apiRequest.Execute<List<EntityComputer>>(Request);
        }

        public EntityActiveClientPolicy GetActiveStatus(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetActiveStatus/{1}", Resource, id);
            return _apiRequest.Execute<EntityActiveClientPolicy>(Request);
        }

        public DtoPolicyExport ExportPolicy(DtoPolicyExportGeneral exportInfo)
        {
            Request.Method = Method.Post;
            Request.AddParameter("application/json", JsonConvert.SerializeObject(exportInfo), ParameterType.RequestBody);
            Request.Resource = string.Format("{0}/ExportPolicy/", Resource);
            return _apiRequest.Execute<DtoPolicyExport>(Request);
        }

        public DtoImportResult ImportPolicy(DtoPolicyExport export)
        {
            Request.Method = Method.Post;
            Request.AddParameter("application/json", JsonConvert.SerializeObject(export), ParameterType.RequestBody);
            Request.Resource = string.Format("{0}/ImportPolicy/", Resource);
            return _apiRequest.Execute<DtoImportResult>(Request);
        }

        public DtoActionResult ValidatePolicyExport(DtoPolicyExportGeneral exportInfo)
        {
            Request.Method = Method.Post;
            Request.AddParameter("application/json", JsonConvert.SerializeObject(exportInfo), ParameterType.RequestBody);
            Request.Resource = string.Format("{0}/ValidatePolicyExport/", Resource);
            return _apiRequest.Execute<DtoActionResult>(Request);
        }

        public IEnumerable<DtoModule> GetAllModules(DtoModuleSearchFilter filter)
        {
            Request.Method = Method.Post;
            Request.AddParameter("application/json", JsonConvert.SerializeObject(filter), ParameterType.RequestBody);
            Request.Resource = string.Format("{0}/GetAllModules/", Resource);
            return _apiRequest.Execute<List<DtoModule>>(Request);
        }

        public IEnumerable<EntityPolicyModules> GetAssignedModules(int id, DtoModuleSearchFilter filter)
        {
            Request.Method = Method.Post;
            Request.Resource = string.Format("{0}/GetAssignedModules/{1}", Resource,id);
            Request.AddParameter("application/json", JsonConvert.SerializeObject(filter), ParameterType.RequestBody);
            return _apiRequest.Execute<List<EntityPolicyModules>>(Request);
        }

        public IEnumerable<EntityPolicyHistory> GetHistoryWithComputer(int id,DtoSearchFilter filter)
        {
            Request.Method = Method.Post;
            Request.AddParameter("application/json", JsonConvert.SerializeObject(filter), ParameterType.RequestBody);
            Request.Resource = string.Format("{0}/GetHistoryWithComputer/{1}", Resource, id);
            return _apiRequest.Execute<List<EntityPolicyHistory>>(Request);
        }

        public IEnumerable<EntityPolicyHashHistory> GetPolicyHashHistory(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetPolicyHashHistory/{1}", Resource, id);
            return _apiRequest.Execute<List<EntityPolicyHashHistory>>(Request);
        }

        public string GetHashDetail(int id, string hash)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetHashDetail/", Resource);
            Request.AddParameter("id", id);
            Request.AddParameter("hash", hash);
            var result = _apiRequest.Execute<DtoApiStringResponse>(Request);
            if (result != null)
                return result.Value ?? string.Empty;
            return string.Empty;
        }

        public string PolicyChangedSinceActivation(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/PolicyChangedSinceActivation/{1}", Resource, id);
            var result = _apiRequest.Execute<DtoApiStringResponse>(Request);
            if (result != null)
                return result.Value;
            else
            {
                throw new NullReferenceException();
            }
        }
        public DtoApiStringResponse GetAssignedModuleCount(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetAssignedModuleCount/{1}", Resource,id);
            return _apiRequest.Execute<DtoApiStringResponse>(Request);
        }

        public DtoActionResult ActivatePolicy(int id, bool reRunExisting)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/ActivatePolicy", Resource);
            Request.AddParameter("id", id);
            Request.AddParameter("reRunExisting", reRunExisting);
            var response = _apiRequest.Execute<DtoActionResult>(Request);
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

        public DtoActionResult DeactivatePolicy(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/DeactivatePolicy/{1}", Resource, id);
            var response = _apiRequest.Execute<DtoActionResult>(Request);
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

        public IEnumerable<DtoPinnedPolicy> GetAllActiveStatus()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetAllActiveStatus/", Resource);
            return _apiRequest.Execute<List<DtoPinnedPolicy>>(Request);
        }
    }
}