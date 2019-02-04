using System;
using System.Collections.Generic;
using System.Data;
using Newtonsoft.Json;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{
    public class ReportAPI
    {
        protected readonly RestRequest Request;
        protected readonly string Resource;

        public ReportAPI(string resource)
        {
            Request = new RestRequest();
            Resource = resource;
        }

        public List<DtoComputerUserLogins> GetUserLogins(string searchstring)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetUserLogins", Resource);
            Request.AddParameter("searchstring", searchstring);
            return new ApiRequest().Execute<List<DtoComputerUserLogins>>(Request);
        }

        public List<DtoProcessWithTime> TopProcessTimes(DateTime dateCutoff, int limit)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/TopProcessTimes", Resource);
            Request.AddParameter("dateCutoff", dateCutoff);
            Request.AddParameter("limit", limit);
            return new ApiRequest().Execute<List<DtoProcessWithTime>>(Request);
        }

        public List<DtoProcessWithCount> TopProcessCounts(DateTime dateCutoff, int limit)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/TopProcessCounts", Resource);
            Request.AddParameter("dateCutoff", dateCutoff);
            Request.AddParameter("limit", limit);
            return new ApiRequest().Execute<List<DtoProcessWithCount>>(Request);
        }


        public DtoApiStringResponse GetCheckinCounts()
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetCheckinCounts", Resource);
            return new ApiRequest().Execute<DtoApiStringResponse>(Request);
        }

        public DataSet GetCustomComputer(List<DtoCustomComputerQuery> queries)
        {
            Request.Method = Method.POST;
            Request.AddJsonBody(queries);
            Request.Resource = string.Format("{0}/GetCustomComputer/", Resource);
            var response = new ApiRequest().Execute<DtoApiStringResponse>(Request);
            if (response != null)
                return JsonConvert.DeserializeObject<DataSet>(response.Value);
            return null;
        }

        public DataSet GetCustomAsset(List<DtoCustomComputerQuery> queries)
        {
            Request.Method = Method.POST;
            Request.AddJsonBody(queries);
            Request.Resource = string.Format("{0}/GetCustomAsset/", Resource);
            var response = new ApiRequest().Execute<DtoApiStringResponse>(Request);
            if (response != null)
                return JsonConvert.DeserializeObject<DataSet>(response.Value);
            return null;
        }







    }
}