using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Toems_ApplicationApi.Controllers.Authorization;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Service.Entity;

namespace Toems_ApplicationApi.Controllers
{

    // The restsharp deserializer does not work when calling these methods.
    // Must have something to do with when LVM is used.
    // Workaround is to just return the string and deserialize using newtonsoft
    public class ImageSchemaController : ApiController
    {
        [Authorize]
        [HttpPost]
        public DtoApiStringResponse GetHardDrives(DtoImageSchemaRequest schemaRequest)
        {
            var hardDrives = new ServiceImageSchemaFE(schemaRequest).GetHardDrivesForGridView();
            return new DtoApiStringResponse() { Value = JsonConvert.SerializeObject(hardDrives) };
        }

        [Authorize]
        [HttpPost]
        public DtoApiStringResponse GetLogicalVolumes(DtoImageSchemaRequest schemaRequest)
        {
            var logicalVolumes = new ServiceImageSchemaFE(schemaRequest).GetLogicalVolumesForGridView(schemaRequest.selectedHd);
            return new DtoApiStringResponse() { Value = JsonConvert.SerializeObject(logicalVolumes) };
        }

        [Authorize]
        [HttpPost]
        public DtoApiStringResponse GetPartitions(DtoImageSchemaRequest schemaRequest)
        {
            var partitions = new ServiceImageSchemaFE(schemaRequest).GetPartitionsForGridView(schemaRequest.selectedHd);
            return new DtoApiStringResponse() { Value = JsonConvert.SerializeObject(partitions) };
        }

        [Authorize]
        [HttpPost]
        public DtoApiStringResponse GetSchema(DtoImageSchemaRequest schemaRequest)
        {
            var schema = new ServiceImageSchemaFE(schemaRequest).GetImageSchema();
            return new DtoApiStringResponse() { Value = JsonConvert.SerializeObject(schema) };
        }
    }
}