using System.Collections.Generic;
using RestSharp;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{

    public class CustomAssetTypeAPI : BaseAPI<EntityCustomAssetType>
    {
        public CustomAssetTypeAPI(string resource) : base(resource)
        {

        }

      
    }
}