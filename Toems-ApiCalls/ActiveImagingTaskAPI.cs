using System.Collections.Generic;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{

    public class ActiveImagingTaskAPI : BaseAPI<EntityActiveImagingTask>
    {
        private readonly ApiRequest _apiRequest;

        public ActiveImagingTaskAPI(string resource) : base(resource)
        {
            _apiRequest = new ApiRequest();
        }

        public DtoActionResult DeleteOnDemand(int id)
        {
            Request.Method = Method.DELETE;
            Request.Resource = string.Format("{0}/DeleteOnDemand/{1}", Resource, id);
            var response = _apiRequest.Execute<DtoActionResult>(Request);
            if (response.Id == 0)
                response.Success = false;
            return response;
        }

        public string GetActiveNotOwned()
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetActiveNotOwned/", Resource);
            var response = _apiRequest.Execute<DtoApiStringResponse>(Request);
            return response != null ? response.Value : string.Empty;
        }

        public bool CancelAllImagingTasks()
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/CancelAllImagingTasks/", Resource);
            var response = _apiRequest.Execute<DtoApiBoolResponse>(Request);
            return response != null ? response.Value : false;
        }

        public IEnumerable<TaskWithComputer> GetActiveTasks()
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetActiveTasks/", Resource);
            var result = _apiRequest.Execute<List<TaskWithComputer>>(Request);
            if (result == null)
                return new List<TaskWithComputer>();
            else
                return result;
        }

        public string GetActiveUnicastCount(string taskType = "")
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetActiveUnicastCount/", Resource);
            Request.AddParameter("tasktype", taskType);
            var response = _apiRequest.Execute<DtoApiStringResponse>(Request);
            return response != null ? response.Value : string.Empty;
        }

        public string GetAllActiveCount()
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetAllActiveCount/", Resource);
            var response = _apiRequest.Execute<DtoApiStringResponse>(Request);
            return response != null ? response.Value : string.Empty;
        }

        public IEnumerable<EntityActiveImagingTask> GetAllOnDemandUnregistered()
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetAllOnDemandUnregistered/", Resource);
            var result = _apiRequest.Execute<List<EntityActiveImagingTask>>(Request);
            if (result == null)
                return new List<EntityActiveImagingTask>();
            else
                return result;
        }


        public IEnumerable<TaskWithComputer> GetUnicasts()
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetUnicasts/", Resource);
            var result = _apiRequest.Execute<List<TaskWithComputer>>(Request);
            if (result == null)
                return new List<TaskWithComputer>();
            else
                return result;
        }

        public int OnDemandCount()
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/OnDemandCount/", Resource);
            var response = _apiRequest.Execute<DtoApiIntResponse>(Request);
            if (response != null) return response.Value;
            return 0;
        }
    }
}