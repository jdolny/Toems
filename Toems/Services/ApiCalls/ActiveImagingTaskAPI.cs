using System.Collections.Generic;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{
    public class ActiveImagingTaskAPI : BaseAPI<EntityActiveImagingTask>
    {

        public ActiveImagingTaskAPI(string resource, ProtectedSessionStorage protectedSessionStorage) : base(resource, protectedSessionStorage)
        {

        }

        public DtoActionResult DeleteOnDemand(int id)
        {
            Request.Method = Method.Delete;
            Request.Resource = $"{Resource}/DeleteOnDemand/{id}";
            var response = _apiRequest.Execute<DtoActionResult>(Request);
            response.Success = response.Id != 0;
            return response;
        }

        public string GetActiveNotOwned()
        {
            Request.Method = Method.Get;
            Request.Resource = $"{Resource}/GetActiveNotOwned/";
            var response = _apiRequest.Execute<DtoApiStringResponse>(Request);
            return response?.Value ?? string.Empty;
        }

        public bool CancelAllImagingTasks()
        {
            Request.Method = Method.Get;
            Request.Resource = $"{Resource}/CancelAllImagingTasks/";
            var response = _apiRequest.Execute<DtoApiBoolResponse>(Request);
            return response?.Value ?? false;
        }

        public IEnumerable<TaskWithComputer> GetActiveTasks()
        {
            Request.Method = Method.Get;
            Request.Resource = $"{Resource}/GetActiveTasks/";
            return _apiRequest.Execute<List<TaskWithComputer>>(Request) ?? new List<TaskWithComputer>();
        }

        public string GetActiveUnicastCount(string taskType = "")
        {
            Request.Method = Method.Get;
            Request.Resource = $"{Resource}/GetActiveUnicastCount/";
            Request.AddParameter("tasktype", taskType);
            var response = _apiRequest.Execute<DtoApiStringResponse>(Request);
            return response?.Value ?? string.Empty;
        }

        public string GetAllActiveCount()
        {
            Request.Method = Method.Get;
            Request.Resource = $"{Resource}/GetAllActiveCount/";
            var response = _apiRequest.Execute<DtoApiStringResponse>(Request);
            return response?.Value ?? string.Empty;
        }

        public IEnumerable<EntityActiveImagingTask> GetAllOnDemandUnregistered()
        {
            Request.Method = Method.Get;
            Request.Resource = $"{Resource}/GetAllOnDemandUnregistered/";
            return _apiRequest.Execute<List<EntityActiveImagingTask>>(Request) ?? new List<EntityActiveImagingTask>();
        }

        public IEnumerable<TaskWithComputer> GetUnicasts()
        {
            Request.Method = Method.Get;
            Request.Resource = $"{Resource}/GetUnicasts/";
            return _apiRequest.Execute<List<TaskWithComputer>>(Request) ?? new List<TaskWithComputer>();
        }

        public int OnDemandCount()
        {
            Request.Method = Method.Get;
            Request.Resource = $"{Resource}/OnDemandCount/";
            var response = _apiRequest.Execute<DtoApiIntResponse>(Request);
            return response?.Value ?? 0;
        }
    }
}
