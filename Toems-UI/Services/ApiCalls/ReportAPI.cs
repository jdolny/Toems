using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{
    public class ReportAPI : BaseAPI<DtoPlaceHolder>
    {

        public ReportAPI(string resource, ProtectedLocalStorage protectedLocalStorage) : base(resource, protectedLocalStorage)
        {

        }

        public List<DtoComputerUserLogins> GetUserLogins(string searchstring)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetUserLogins", Resource);
            Request.AddParameter("searchstring", searchstring);
            return _apiRequest.Execute<List<DtoComputerUserLogins>>(Request);
        }

        public List<DtoProcessWithTime> TopProcessTimes(DateTime dateCutoff, int limit)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/TopProcessTimes", Resource);
            Request.AddParameter("dateCutoff", dateCutoff);
            Request.AddParameter("limit", limit);
            return _apiRequest.Execute<List<DtoProcessWithTime>>(Request);
        }

        public List<DtoProcessWithCount> TopProcessCounts(DateTime dateCutoff, int limit)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/TopProcessCounts", Resource);
            Request.AddParameter("dateCutoff", dateCutoff);
            Request.AddParameter("limit", limit);
            return _apiRequest.Execute<List<DtoProcessWithCount>>(Request);
        }


        public DtoApiStringResponse GetCheckinCounts()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetCheckinCounts", Resource);
            return _apiRequest.Execute<DtoApiStringResponse>(Request);
        }

        public DataSet GetReportSqlQuery(string sql)
        {
            var dtoString = new DtoApiStringResponse();
            dtoString.Value = sql;
            Request.Method = Method.Post;
            Request.AddParameter("application/json", JsonConvert.SerializeObject(dtoString), ParameterType.RequestBody);
            Request.Resource = string.Format("{0}/GetReportSqlQuery/", Resource);
            var response = _apiRequest.Execute<DtoApiStringResponse>(Request);
            if (response != null)
                return JsonConvert.DeserializeObject<DataSet>(response.Value);
            return null;
        }

        public DataSet GetCustomComputer(List<DtoCustomComputerQuery> queries)
        {
            Request.Method = Method.Post;
            Request.AddParameter("application/json", JsonConvert.SerializeObject(queries), ParameterType.RequestBody);
            Request.Resource = string.Format("{0}/GetCustomComputer/", Resource);
            var response = _apiRequest.Execute<DtoApiStringResponse>(Request);
            if (response != null)
                return JsonConvert.DeserializeObject<DataSet>(response.Value);
            return null;
        }

        public DataSet GetCustomAsset(List<DtoCustomComputerQuery> queries)
        {
            Request.Method = Method.Post;
            Request.AddParameter("application/json", JsonConvert.SerializeObject(queries), ParameterType.RequestBody);
            Request.Resource = string.Format("{0}/GetCustomAsset/", Resource);
            var response = _apiRequest.Execute<DtoApiStringResponse>(Request);
            if (response != null)
                return JsonConvert.DeserializeObject<DataSet>(response.Value);
            return null;
        }







    }
}