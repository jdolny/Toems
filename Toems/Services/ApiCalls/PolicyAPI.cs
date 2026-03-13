using Newtonsoft.Json;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Dto.exports;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{
    public class PolicyAPI (string resource, ApiRequest apiRequest)
        : BaseAPI<EntityPolicy>(resource,apiRequest)
    {

        public async Task<string> GetArchivedCount()
        {
            Request.Method = Method.Get;
            Request.Resource = $"{Resource}/GetArchivedCount";
            var responseData = await _apiRequest.ExecuteAsync<DtoApiStringResponse>(Request);
            return responseData != null ? responseData.Value : string.Empty;

        }

        public async Task<List<EntityPolicy>> GetArchived(DtoSearchFilterCategories filter)
        {
            Request.Method = Method.Post;
            Request.Resource = $"{Resource}/GetArchived";
            Request.AddParameter("application/json", JsonConvert.SerializeObject(filter), ParameterType.RequestBody);
            return await _apiRequest.ExecuteAsync<List<EntityPolicy>>(Request);
        }

        public async Task<DtoActionResult> Archive(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = $"{Resource}/Archive/{id}";
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

        public async Task<DtoActionResult> Restore(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = $"{Resource}/Restore/{id}";
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

        public async Task<DtoActionResult> Clone(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = $"{Resource}/Clone/{id}";
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

        public async Task<IEnumerable<EntityPolicyComServer>> GetPolicyComServers(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = $"{Resource}/GetPolicyComServers/{id}";
            return await _apiRequest.ExecuteAsync<List<EntityPolicyComServer>>(Request);
        }

        public async Task<IEnumerable<EntityPolicyCategory>> GetPolicyCategories(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = $"{Resource}/GetPolicyCategories/{id}";
            return await _apiRequest.ExecuteAsync<List<EntityPolicyCategory>>(Request);
        }

        public async Task<IEnumerable<EntityGroup>> GetPolicyGroups(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = $"{Resource}/GetPolicyGroups/{id}";
            return await _apiRequest.ExecuteAsync<List<EntityGroup>>(Request);
        }

        public async Task<IEnumerable<EntityComputer>> GetPolicyComputers(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = $"{Resource}/GetPolicyComputers/{id}";
            return await _apiRequest.ExecuteAsync<List<EntityComputer>>(Request);
        }

        public async Task<EntityActiveClientPolicy> GetActiveStatus(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = $"{Resource}/GetActiveStatus/{id}";
            return await _apiRequest.ExecuteAsync<EntityActiveClientPolicy>(Request);
        }

        public async Task<DtoPolicyExport> ExportPolicy(DtoPolicyExportGeneral exportInfo)
        {
            Request.Method = Method.Post;
            Request.AddParameter("application/json", JsonConvert.SerializeObject(exportInfo), ParameterType.RequestBody);
            Request.Resource = $"{Resource}/ExportPolicy/";
            return await _apiRequest.ExecuteAsync<DtoPolicyExport>(Request);
        }

        public async Task<DtoImportResult> ImportPolicy(DtoPolicyExport export)
        {
            Request.Method = Method.Post;
            Request.AddParameter("application/json", JsonConvert.SerializeObject(export), ParameterType.RequestBody);
            Request.Resource = $"{Resource}/ImportPolicy/";
            return await _apiRequest.ExecuteAsync<DtoImportResult>(Request);
        }

        public async Task<DtoActionResult> ValidatePolicyExport(DtoPolicyExportGeneral exportInfo)
        {
            Request.Method = Method.Post;
            Request.AddParameter("application/json", JsonConvert.SerializeObject(exportInfo), ParameterType.RequestBody);
            Request.Resource = $"{Resource}/ValidatePolicyExport/";
            return await _apiRequest.ExecuteAsync<DtoActionResult>(Request);
        }

        public async Task<IEnumerable<DtoModule>> GetAllModules(DtoModuleSearchFilter filter)
        {
            Request.Method = Method.Post;
            Request.AddParameter("application/json", JsonConvert.SerializeObject(filter), ParameterType.RequestBody);
            Request.Resource = $"{Resource}/GetAllModules/";
            return await _apiRequest.ExecuteAsync<List<DtoModule>>(Request);
        }

        public async Task<IEnumerable<EntityPolicyModules>> GetAssignedModules(int id, DtoModuleSearchFilter filter)
        {
            Request.Method = Method.Post;
            Request.Resource = $"{Resource}/GetAssignedModules/{id}";
            Request.AddParameter("application/json", JsonConvert.SerializeObject(filter), ParameterType.RequestBody);
            return await _apiRequest.ExecuteAsync<List<EntityPolicyModules>>(Request);
        }

        public async Task<IEnumerable<EntityPolicyHistory>> GetHistoryWithComputer(int id,DtoSearchFilter filter)
        {
            Request.Method = Method.Post;
            Request.AddParameter("application/json", JsonConvert.SerializeObject(filter), ParameterType.RequestBody);
            Request.Resource = $"{Resource}/GetHistoryWithComputer/{id}";
            return await _apiRequest.ExecuteAsync<List<EntityPolicyHistory>>(Request);
        }

        public async Task<IEnumerable<EntityPolicyHashHistory>> GetPolicyHashHistory(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = $"{Resource}/GetPolicyHashHistory/{id}";
            return await _apiRequest.ExecuteAsync<List<EntityPolicyHashHistory>>(Request);
        }

        public async Task<string> GetHashDetail(int id, string hash)
        {
            Request.Method = Method.Get;
            Request.Resource = $"{Resource}/GetHashDetail/";
            Request.AddParameter("id", id);
            Request.AddParameter("hash", hash);
            var result = await _apiRequest.ExecuteAsync<DtoApiStringResponse>(Request);
            if (result != null)
                return result.Value ?? string.Empty;
            return string.Empty;
        }

        public async Task<string> PolicyChangedSinceActivation(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = $"{Resource}/PolicyChangedSinceActivation/{id}";
            var result = await _apiRequest.ExecuteAsync<DtoApiStringResponse>(Request);
            if (result != null)
                return result.Value;
            else
            {
                throw new NullReferenceException();
            }
        }
        public async Task<DtoApiStringResponse> GetAssignedModuleCount(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = $"{Resource}/GetAssignedModuleCount/{id}";
            return await _apiRequest.ExecuteAsync<DtoApiStringResponse>(Request);
        }

        public async Task<DtoActionResult> ActivatePolicy(int id, bool reRunExisting)
        {
            Request.Method = Method.Get;
            Request.Resource = $"{Resource}/ActivatePolicy";
            Request.AddParameter("id", id);
            Request.AddParameter("reRunExisting", reRunExisting);
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

        public async Task<DtoActionResult> DeactivatePolicy(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = $"{Resource}/DeactivatePolicy/{id}";
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

        public async Task<IEnumerable<DtoPinnedPolicy>> GetAllActiveStatus()
        {
            Request.Method = Method.Get;
            Request.Resource = $"{Resource}/GetAllActiveStatus/";
            return await _apiRequest.ExecuteAsync<List<DtoPinnedPolicy>>(Request);
        }
    }
}