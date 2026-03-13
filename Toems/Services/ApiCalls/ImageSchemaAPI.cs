using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;
using Toems_Common.Dto;
using Toems_Common.Dto.imageschemafe;

namespace Toems_ApiCalls
{
    public class ImageSchemaAPI (string resource, ApiRequest apiRequest)
        : BaseAPI<DtoImageSchemaGridView>(resource,apiRequest)
    {

        //The restsharp deserializer does not work properly with the image schema.
        //Workaround is to deserialize the string manually with newtonsoft
        public async Task<IEnumerable<DtoHardDrive>> GetHardDrives(DtoImageSchemaRequest schemaRequest)
        {
            Request.Method = Method.Post;
            Request.Resource = $"{Resource}/GetHardDrives";
            Request.AddParameter("application/json", JsonConvert.SerializeObject(schemaRequest), ParameterType.RequestBody);
            var response = await _apiRequest.ExecuteAsync<DtoApiStringResponse>(Request);
            if(response == null) return new List<DtoHardDrive>();
            var result = JsonConvert.DeserializeObject<List<DtoHardDrive>>(response.Value);
            if (result == null)
                return new List<DtoHardDrive>();
            else
                return result;
        }

        public async Task<List<DtoLogicalVolume>> GetLogicalVolumes(DtoImageSchemaRequest schemaRequest)
        {
            Request.Method = Method.Post;
            Request.Resource = $"{Resource}/GetLogicalVolumes";
            Request.AddParameter("application/json", JsonConvert.SerializeObject(schemaRequest), ParameterType.RequestBody);
            var response = await _apiRequest.ExecuteAsync<DtoApiStringResponse>(Request);
            if (response == null) return new List<DtoLogicalVolume>();
            var result = JsonConvert.DeserializeObject<List<DtoLogicalVolume>>(response.Value);
            if (result == null)
                return new List<DtoLogicalVolume>();
            else
                return result;
        }

        public async Task<List<DtoPartition>> GetPartitions(DtoImageSchemaRequest schemaRequest)
        {
            Request.Method = Method.Post;
            Request.Resource = $"{Resource}/GetPartitions";
            Request.AddParameter("application/json", JsonConvert.SerializeObject(schemaRequest), ParameterType.RequestBody);
            var response = await _apiRequest.ExecuteAsync<DtoApiStringResponse>(Request);
            if (response == null) return new List<DtoPartition>();
            var result = JsonConvert.DeserializeObject<List<DtoPartition>>(response.Value);
            if (result == null)
                return new List<DtoPartition>();
            else
                return result;
        }

        public async Task<DtoImageSchemaGridView> GetSchema(DtoImageSchemaRequest schemaRequest)
        {
            Request.Method = Method.Post;
            Request.Resource = $"{Resource}/GetSchema";
            Request.AddParameter("application/json", JsonConvert.SerializeObject(schemaRequest), ParameterType.RequestBody);
            var response = await _apiRequest.ExecuteAsync<DtoApiStringResponse>(Request);
            if (response == null) return new DtoImageSchemaGridView();
            var result = JsonConvert.DeserializeObject<DtoImageSchemaGridView>(response.Value);
            if (result == null)
                return new DtoImageSchemaGridView();
            else
                return result;
        }
    }
}
