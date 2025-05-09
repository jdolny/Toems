using System.Collections.Generic;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{

    public class ImageAPI : BaseAPI<EntityImage>
    {
        public ImageAPI(string resource, ProtectedLocalStorage protectedLocalStorage) : base(resource, protectedLocalStorage)
        {

        }

        public IEnumerable<EntityImageProfile> GetImageProfiles(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetImageProfiles/{1}", Resource, id);
            var result = _apiRequest.Execute<List<EntityImageProfile>>(Request);
            if (result == null)
                return new List<EntityImageProfile>();
            else
                return result;
        }

        public string GetImageSizeOnServer(string imageName, string hdNumber)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetImageSizeOnServer/", Resource);
            Request.AddParameter("imageName", imageName);
            Request.AddParameter("hdNumber", hdNumber);
            var response = _apiRequest.Execute<DtoApiStringResponse>(Request);
            return response != null ? response.Value : string.Empty;
        }

        public IEnumerable<DtoImageFileInfo> GetPartitionFileInfo(int id, string selectedHd, string selectedPartition)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetPartitionFileInfo/{1}", Resource, id);
            Request.AddParameter("selectedHd", selectedHd);
            Request.AddParameter("selectedPartition", selectedPartition);
            var result = _apiRequest.Execute<List<DtoImageFileInfo>>(Request);
            if (result == null)
                return new List<DtoImageFileInfo>();
            else
                return result;
        }

        public EntityImageProfile SeedDefaultProfile(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/SeedDefaultProfile/{1}", Resource, id);
            return _apiRequest.Execute<EntityImageProfile>(Request);
        }

        new public List<ImageWithDate> Search(DtoSearchFilterCategories filter)
        {
            Request.Method = Method.Post;
            Request.Resource = string.Format("{0}/Search/",Resource);
            Request.AddParameter("application/json", JsonConvert.SerializeObject(filter), ParameterType.RequestBody);
            return _apiRequest.Execute<List<ImageWithDate>>(Request);
        }

        public List<DtoServerImageRepStatus> GetReplicationStatus(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetReplicationStatus/{1}", Resource,id);
            return _apiRequest.Execute<List<DtoServerImageRepStatus>>(Request);
        }

        public IEnumerable<EntityImageCategory> GetImageCategories(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetImageCategories/{1}", Resource, id);
            return _apiRequest.Execute<List<EntityImageCategory>>(Request);
        }

        public IEnumerable<EntityAuditLog> GetImageAuditLogs(int id, int limit)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetImageAuditLogs/{1}", Resource, id);
            Request.AddParameter("limit", limit);
            var result = _apiRequest.Execute<List<EntityAuditLog>>(Request);
            if (result == null)
                return new List<EntityAuditLog>();
            else
                return result;
        }

        public IEnumerable<EntityImageReplicationServer> GetImageReplicationComServers(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetImageReplicationComServers/{1}", Resource, id);
            return _apiRequest.Execute<List<EntityImageReplicationServer>>(Request);
        }

        public DtoActionResult UpdateReplicationServers(List<EntityImageReplicationServer> imageReplicationServers)
        {
            Request.Method = Method.Post;
            Request.Resource = string.Format("{0}/UpdateReplicationServers/", Resource);
            Request.AddParameter("application/json", JsonConvert.SerializeObject(imageReplicationServers), ParameterType.RequestBody);
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

    }
}