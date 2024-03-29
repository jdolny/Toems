﻿using System.Collections.Generic;
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
    public class ToecDeployJobController : ApiController
    {
        private readonly ServiceToecDeployJob _toecDeployJob;

        public ToecDeployJobController()
        {
            _toecDeployJob = new ServiceToecDeployJob();
          
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public DtoActionResult Delete(int id)
        {
            var result = _toecDeployJob.Delete(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public EntityToecDeployJob Get(int id)
        {
            var result = _toecDeployJob.GetToecDeployJob(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        [HttpPost]
        public DtoApiBoolResponse RunToecDeploySingle(DtoSingleToecDeploy job)
        {
            new Toems_Service.Workflows.ToecRemoteInstaller().RunSingle(job);
            return new DtoApiBoolResponse() { Value = true };
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public IEnumerable<EntityToecTargetListComputer> GetTargetComputers(int id)
        {
            return _toecDeployJob.GetTargetComputers(id);
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public IEnumerable<EntityToecDeployJob> Get()
        {
            return _toecDeployJob.Search(new DtoSearchFilter());
        }

         [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        [HttpPost]
        public IEnumerable<EntityToecDeployJob> Search(DtoSearchFilter filter)
        {
            return _toecDeployJob.Search(filter);
        }

         [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public DtoApiStringResponse GetCount()
        {
            return new DtoApiStringResponse {Value = _toecDeployJob.TotalCount()};
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        [HttpGet]
        public DtoApiBoolResponse RestartDeployJobService()
        {
            return new DtoApiBoolResponse { Value = _toecDeployJob.RestartDeployJobService() };
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        [HttpGet]
        public DtoApiBoolResponse ResetComputerStatus(int id)
        {
            return new DtoApiBoolResponse { Value = _toecDeployJob.ResetComputerStatus(id) };
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public DtoActionResult Post(EntityToecDeployJob toecDeployJob)
        {
            return _toecDeployJob.Add(toecDeployJob);
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public DtoActionResult Put(int id, EntityToecDeployJob toecDeployJob)
        {
            toecDeployJob.Id = id;
            var result = _toecDeployJob.Update(toecDeployJob);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }
    }
}