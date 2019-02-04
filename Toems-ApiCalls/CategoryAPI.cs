using System.Collections.Generic;
using RestSharp;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{

    public class CategoryAPI : BaseAPI<EntityCategory>
    {
        public CategoryAPI(string resource) : base(resource)
        {

        }

      
    }
}