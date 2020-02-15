using System.Collections.Generic;
using RestSharp;
using Toems_Common.Entity;
using Toems_Common.Enum;

namespace Toems_ApiCalls
{

    public class ImageProfileTemplateAPI : BaseAPI<EntityImageProfileTemplate>
    {
        public ImageProfileTemplateAPI(string resource) : base(resource)
        {

        }

        public EntityImageProfileTemplate Get(EnumProfileTemplate.TemplateType templateType)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/Get/", Resource);
            Request.AddParameter("templateType", templateType);
            return new ApiRequest().Execute<EntityImageProfileTemplate>(Request);
        }

    }
}