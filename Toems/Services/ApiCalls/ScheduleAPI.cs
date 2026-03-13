using System.Collections.Generic;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;
using RestSharp;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{

    public class ScheduleAPI : BaseAPI<EntitySchedule>
    {
        public ScheduleAPI(string resource, ProtectedLocalStorage protectedLocalStorage) : base(resource, protectedLocalStorage)
        {

        }

        public List<EntityPolicy> GetSchedulePolicies(int id, string type)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetSchedulePolicies", Resource);
            Request.AddParameter("id", id);
            Request.AddParameter("type", type);
            return _apiRequest.Execute<List<EntityPolicy>>(Request);
        }

        public List<EntityGroup> GetScheduleGroups(int id, string type)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetScheduleGroups", Resource);
            Request.AddParameter("id", id);
            Request.AddParameter("type", type);
            return _apiRequest.Execute<List<EntityGroup>>(Request);
        }

        public List<EntityComputer> GetScheduleComputers(int id, string type)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetScheduleComputers", Resource);
            Request.AddParameter("id", id);
            Request.AddParameter("type", type);
            return _apiRequest.Execute<List<EntityComputer>>(Request);
        }
    }
}