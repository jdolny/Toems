using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using log4net;
using Toems_ClientApi.Controllers.Authorization;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Dto.client;
using Toems_Common.Entity;
using Toems_Service;
using Toems_Service.Entity;
using Toems_Service.Workflows;

namespace Toems_ClientApi.Controllers
{

    public class ImagePrepController : ApiController
    {
        private static readonly ILog Logger =
         LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [CertificateAuth]
        [HttpGet]
        public DtoApiStringResponse GetDriverList()
        {
            var response = new ServiceFileCopyModule().SearchModules(new DtoSearchFilterCategories());
            string ids = "";
            foreach (var module in response)
                ids += module.Id + ":" + module.Name + ",";


            return new DtoApiStringResponse { Value = ids.Trim(',') };

        }

        public List<EntitySysprepAnswerfile> GetSysprepList()
        {
            return new ServiceSysprepAnswerFile().GetAll();
           
        }

        public List<EntitySetupCompleteFile> GetSetupCompleteList()
        {
            return new ServiceSetupCompleteFile().GetAll();
           
        }

        public DtoApiStringResponse GetSetupCompleteFile(int id)
        {
            var file = new ServiceSetupCompleteFile().Get(id);
            return new DtoApiStringResponse { Value = file.Contents };
        }
        public DtoApiStringResponse GetSysprepFile(int id)
        {
            var file = new ServiceSysprepAnswerFile().Get(id);
            return new DtoApiStringResponse { Value = file.Contents };
        }

        public List<DtoClientFileRequest> GetFileCopyModule(int id)
        {
            var module = new ServiceFileCopyModule().GetModule(id);
            var list = new List<DtoClientFileRequest>();

            var moduleFiles = new ServiceModule().GetModuleFiles(module.Guid);
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
