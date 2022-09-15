using System.Collections.Generic;
using Newtonsoft.Json;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{

    public class AssetAPI : BaseAPI<EntityAsset>
    {
        public AssetAPI(string resource) : base(resource)
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

        public IEnumerable<EntityAssetAttribute> GetAttributes(int id)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetAttributes/{1}", Resource, id);
            return new ApiRequest().Execute<List<EntityAssetAttribute>>(Request);
        }

        new public IEnumerable<DtoAssetWithType> Search(DtoSearchFilterCategories filter)
        {
            Request.Method = Method.POST;
            Request.AddParameter("application/json", JsonConvert.SerializeObject(filter), ParameterType.RequestBody);
            Request.Resource = string.Format("{0}/Search", Resource);
            return new ApiRequest().Execute<List<DtoAssetWithType>>(Request);
        }

        public string GetArchivedCount()
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetArchivedCount", Resource);
            var responseData = new ApiRequest().Execute<DtoApiStringResponse>(Request);
            return responseData != null ? responseData.Value : string.Empty;

        }

        public IEnumerable<DtoAssetWithType> SearchArchived(DtoSearchFilterCategories filter)
        {
            Request.Method = Method.POST;
            Request.AddParameter("application/json", JsonConvert.SerializeObject(filter), ParameterType.RequestBody);
            Request.Resource = string.Format("{0}/SearchArchived", Resource);
            return new ApiRequest().Execute<List<DtoAssetWithType>>(Request);
        }

        public IEnumerable<EntityAttachment> GetAttachments(int id)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetAttachments/{1}", Resource, id);
            return new ApiRequest().Execute<List<EntityAttachment>>(Request);
        }

        public IEnumerable<EntityComputer> GetAssetSoftwareComputers(int id)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetAssetSoftwareComputers/{1}", Resource, id);
            return new ApiRequest().Execute<List<EntityComputer>>(Request);
        }

        public IEnumerable<DtoAssetComment> GetComments(int id)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetComments/{1}", Resource, id);
            return new ApiRequest().Execute<List<DtoAssetComment>>(Request);
        }

        public IEnumerable<DtoAssetSoftware> GetSoftware(int id)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetSoftware/{1}", Resource, id);
            return new ApiRequest().Execute<List<DtoAssetSoftware>>(Request);
        }

        public DtoActionResult AddComment(DtoAssetComment comment)
        {
            Request.Method = Method.POST;
            Request.Resource = string.Format("{0}/AddComment/", Resource);
            Request.AddParameter("application/json", JsonConvert.SerializeObject(comment), ParameterType.RequestBody);
            return new ApiRequest().Execute<DtoActionResult>(Request);
        }

        public IEnumerable<EntityAssetCategory> GetAssetCategories(int id)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetAssetCategories/{1}", Resource, id);
            return new ApiRequest().Execute<List<EntityAssetCategory>>(Request);
        }
    }
}