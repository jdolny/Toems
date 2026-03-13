using System.Collections.Generic;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{

    public class ImageAPI(string resource, ApiRequest apiRequest)
        : BaseAPI<EntityImage>(resource,apiRequest)
    {

        public async Task<IEnumerable<EntityImageProfile>> GetImageProfiles(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = $"{Resource}/GetImageProfiles/{id}";
            var result = await _apiRequest.ExecuteAsync<List<EntityImageProfile>>(Request);
            if (result == null)
                return new List<EntityImageProfile>();
            else
                return result;
        }

        public async Task<string> GetImageSizeOnServer(string imageName, string hdNumber)
        {
            Request.Method = Method.Get;
            Request.Resource = $"{Resource}/GetImageSizeOnServer/";
            Request.AddParameter("imageName", imageName);
            Request.AddParameter("hdNumber", hdNumber);
            var response = await _apiRequest.ExecuteAsync<DtoApiStringResponse>(Request);
            return response != null ? response.Value : string.Empty;
        }

        public async Task<IEnumerable<DtoImageFileInfo>> GetPartitionFileInfo(int id, string selectedHd, string selectedPartition)
        {
            Request.Method = Method.Get;
            Request.Resource = $"{Resource}/GetPartitionFileInfo/{id}";
            Request.AddParameter("selectedHd", selectedHd);
            Request.AddParameter("selectedPartition", selectedPartition);
            var result = await _apiRequest.ExecuteAsync<List<DtoImageFileInfo>>(Request);
            if (result == null)
                return new List<DtoImageFileInfo>();
            else
                return result;
        }

        public async Task<EntityImageProfile> SeedDefaultProfile(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = $"{Resource}/SeedDefaultProfile/{id}";
            return await _apiRequest.ExecuteAsync<EntityImageProfile>(Request);
        }

        new public async Task<List<ImageWithDate>> Search(DtoSearchFilterCategories filter)
        {
            Request.Method = Method.Post;
            Request.Resource = $"{Resource}/Search/";
            Request.AddParameter("application/json", JsonConvert.SerializeObject(filter), ParameterType.RequestBody);
            return await _apiRequest.ExecuteAsync<List<ImageWithDate>>(Request);
        }

        public async Task<List<DtoServerImageRepStatus>> GetReplicationStatus(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = $"{Resource}/GetReplicationStatus/{id}";
            return await _apiRequest.ExecuteAsync<List<DtoServerImageRepStatus>>(Request);
        }

        public async Task<IEnumerable<EntityImageCategory>> GetImageCategories(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = $"{Resource}/GetImageCategories/{id}";
            return await _apiRequest.ExecuteAsync<List<EntityImageCategory>>(Request);
        }

        public async Task<IEnumerable<EntityAuditLog>> GetImageAuditLogs(int id, int limit)
        {
            Request.Method = Method.Get;
            Request.Resource = $"{Resource}/GetImageAuditLogs/{id}";
            Request.AddParameter("limit", limit);
            var result = await _apiRequest.ExecuteAsync<List<EntityAuditLog>>(Request);
            if (result == null)
                return new List<EntityAuditLog>();
            else
                return result;
        }

        public async Task<IEnumerable<EntityImageReplicationServer>> GetImageReplicationComServers(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = $"{Resource}/GetImageReplicationComServers/{id}";
            return await _apiRequest.ExecuteAsync<List<EntityImageReplicationServer>>(Request);
        }

        public async Task<DtoActionResult> UpdateReplicationServers(List<EntityImageReplicationServer> imageReplicationServers)
        {
            Request.Method = Method.Post;
            Request.Resource = $"{Resource}/UpdateReplicationServers/";
            Request.AddParameter("application/json", JsonConvert.SerializeObject(imageReplicationServers), ParameterType.RequestBody);
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

    }
}