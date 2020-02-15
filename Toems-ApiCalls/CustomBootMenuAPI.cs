using System.Collections.Generic;
using RestSharp;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{

    public class CustomBootMenuAPI : BaseAPI<EntityCustomBootMenu>
    {
        public CustomBootMenuAPI(string resource) : base(resource)
        {

        }

      
    }
}