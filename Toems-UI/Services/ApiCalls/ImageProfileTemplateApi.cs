using System.Collections.Generic;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;
using RestSharp;
using Toems_Common.Entity;
using Toems_Common.Enum;

namespace Toems_ApiCalls
{

    public class ImageProfileTemplateAPI : BaseAPI<EntityImageProfileTemplate>
    {
        public ImageProfileTemplateAPI(string resource, ProtectedLocalStorage protectedLocalStorage) : base(resource, protectedLocalStorage)
        {

        }

        public EntityImageProfileTemplate Get(EnumProfileTemplate.TemplateType templateType)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/Get/", Resource);
            Request.AddParameter("templateType", templateType);
            return _apiRequest.Execute<EntityImageProfileTemplate>(Request);
        }

    }
}