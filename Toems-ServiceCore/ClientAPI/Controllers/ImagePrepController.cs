using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Toems_ClientApi.Controllers.Authorization;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Dto.client;
using Toems_Common.Entity;

using Toems_ServiceCore.EntityServices;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ClientApi.Controllers
{
    [ApiController]
    [Route("api/client/[controller]")]
    public class ImagePrepController(ServiceContext ctx) : ControllerBase
    {
        [CertificateAuth]
        [HttpGet]
        public DtoApiStringResponse GetDriverList()
        {
            var response = ctx.FileCopyModule.SearchModules(new DtoSearchFilterCategories());
            string ids = "";
            foreach (var module in response)
                ids += module.Id + ":" + module.Name + ",";


            return new DtoApiStringResponse { Value = ids.Trim(',') };

        }

        public List<EntitySysprepAnswerfile> GetSysprepList()
        {
            return ctx.SysprepAnswerFile.GetAll();
           
        }

        public List<EntitySetupCompleteFile> GetSetupCompleteList()
        {
            return ctx.SetupCompleteFile.GetAll();
           
        }

        public DtoApiStringResponse GetSetupCompleteFile(int id)
        {
            var file = ctx.SetupCompleteFile.Get(id);
            return new DtoApiStringResponse { Value = file.Contents };
        }
        public DtoApiStringResponse GetSysprepFile(int id)
        {
            var file = ctx.SysprepAnswerFile.Get(id);
            return new DtoApiStringResponse { Value = file.Contents };
        }

        public List<DtoClientFileRequest> GetFileCopyModule(int id)
        {
            var module = ctx.FileCopyModule.GetModule(id);
            var list = new List<DtoClientFileRequest>();

            var moduleFiles = ctx.Module.GetModuleFiles(module.Guid);
            foreach (var file in moduleFiles.OrderBy(x => x.FileName))
            {
                var fr = new DtoClientFileRequest();
                fr.FileName = file.FileName;
                fr.ModuleGuid = module.Guid;
                list.Add(fr);
            }

            return list;

            
        }


    }

    
}
