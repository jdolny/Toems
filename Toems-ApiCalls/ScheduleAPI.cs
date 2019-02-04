using System.Collections.Generic;
using RestSharp;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{

    public class ScheduleAPI : BaseAPI<EntitySchedule>
    {
        public ScheduleAPI(string resource) : base(resource)
        {

        }

        public List<EntityPolicy> GetSchedulePolicies(int id, string type)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetSchedulePolicies", Resource);
            Request.AddParameter("id", id);
            Request.AddParameter("type", type);
            return new ApiRequest().Execute<List<EntityPolicy>>(Request);
        }

        public List<EntityGroup> GetScheduleGroups(int id, string type)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetScheduleGroups", Resource);
            Request.AddParameter("id", id);
            Request.AddParameter("type", type);
            return new ApiRequest().Execute<List<EntityGroup>>(Request);
        }

        public List<EntityComputer> GetScheduleComputers(int id, string type)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetScheduleComputers", Resource);
            Request.AddParameter("id", id);
            Request.AddParameter("type", type);
            return new ApiRequest().Execute<List<EntityComputer>>(Request);
        }
    }
}