using System.Collections.Generic;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{

    public class SetupCompleteFileAPI : BaseAPI<EntitySetupCompleteFile>
    {
        public SetupCompleteFileAPI(string resource) : base(resource)
        {

        }

       
    }
}