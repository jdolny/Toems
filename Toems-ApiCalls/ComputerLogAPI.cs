using System.Collections.Generic;
using RestSharp;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{

    public class ComputerLogAPI : BaseAPI<EntityComputerLog>
    {
        public ComputerLogAPI(string resource) : base(resource)
        {

        }

      
    }
}