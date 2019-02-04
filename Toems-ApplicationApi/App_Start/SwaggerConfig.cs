using System.Web.Http;
using WebActivatorEx;
using Toems_ApplicationApi;
using Swashbuckle.Application;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace Toems_ApplicationApi
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                    {
                        c.SingleApiVersion("v1", "Toems_ApplicationApi");
                        c.ApiKey("Token")
                        .Description("API Key Authentication")
                        .Name("Authorization")
                        .In("header");
                        c.UseFullTypeNameInSchemaIds();
                    }).EnableSwaggerUi(c =>
                    {
                        c.EnableApiKeySupport("Authorization", "header");
                    });


        }
    }
}
