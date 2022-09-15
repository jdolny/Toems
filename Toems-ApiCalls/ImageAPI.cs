using System.Collections.Generic;
using Newtonsoft.Json;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{

    public class ImageAPI : BaseAPI<EntityImage>
    {
        public ImageAPI(string resource) : base(resource)
        {

        }

        public IEnumerable<EntityImageProfile> GetImageProfiles(int id)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetImageProfiles/{1}", Resource, id);
            var result = new ApiRequest().Execute<List<EntityImageProfile>>(Request);
            if (result == null)
                return new List<EntityImageProfile>();
            else
                return result;
        }

        public string GetImageSizeOnServer(string imageName, string hdNumber)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetImageSizeOnServer/", Resource);
            Request.AddParameter("imageName", imageName);
            Request.AddParameter("hdNumber", hdNumber);
            var response = new ApiRequest().Execute<DtoApiStringResponse>(Request);
            return response != null ? response.Value : string.Empty;
        }

        public IEnumerable<DtoImageFileInfo> GetPartitionFileInfo(int id, string selectedHd, string selectedPartition)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetPartitionFileInfo/{1}", Resource, id);
            Request.AddParameter("selectedHd", selectedHd);
            Request.AddParameter("selectedPartition", selectedPartition);
            var result = new ApiRequest().Execute<List<DtoImageFileInfo>>(Request);
            if (result == null)
                return new List<DtoImageFileInfo>();
            else
                return result;
        }

        public EntityImageProfile SeedDefaultProfile(int id)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/SeedDefaultProfile/{1}", Resource, id);
            return new ApiRequest().Execute<EntityImageProfile>(Request);
        }

        new public List<ImageWithDate> Search(DtoSearchFilterCategories filter)
        {
            Request.Method = Method.POST;
            Request.Resource = string.Format("{0}/Search/",Resource);
            Request.AddParameter("application/json", JsonConvert.SerializeObject(filter), ParameterType.RequestBody);
            return new ApiRequest().Execute<List<ImageWithDate>>(Request);
        }

        public List<DtoServerImageRepStatus> GetReplicationStatus(int id)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetReplicationStatus/{1}", Resource,id);
            return new ApiRequest().Execute<List<DtoServerImageRepStatus>>(Request);
        }

        public IEnumerable<EntityImageCategory> GetImageCategories(int id)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetImageCategories/{1}", Resource, id);
            return new ApiRequest().Execute<List<EntityImageCategory>>(Request);
        }

        public IEnumerable<EntityAuditLog> GetImageAuditLogs(int id, int limit)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetImageAuditLogs/{1}", Resource, id);
            Request.AddParameter("limit", limit);
            var result = new ApiRequest().Execute<List<EntityAuditLog>>(Request);
            if (result == null)
                return new List<EntityAuditLog>();
            else
                return result;
        }

    }
}