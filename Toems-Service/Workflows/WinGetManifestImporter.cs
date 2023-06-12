using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;
using YamlDotNet.Core;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using static System.Net.Mime.MediaTypeNames;
using System.Dynamic;
using System.Collections;

namespace Toems_Service.Workflows
{
    public class Manifest
    {
        public string PackageIdentifier { get; set; }
        public string ManifestType { get; set; }
    }
    public class WinGetManifestImporter
    {
        public bool Run()
        {
            SearchManifests();

            return true;
        }
        private void SearchManifests()
        {


            var path = HttpContext.Current.Server.MapPath("~");
            var BaseSourcePath = Path.Combine(path, "private", "winget_manifests");
            var allFiles = Directory.GetFiles(BaseSourcePath, "*.yaml", SearchOption.AllDirectories);
            foreach(var file in allFiles)
            {
               


                    var deserializer = new DeserializerBuilder().Build();
                dynamic myConfig = deserializer.Deserialize<ExpandoObject>(File.ReadAllText(file));
                IDictionary<string, object> manifestDict = myConfig;
                var packageIdentifier = manifestDict.Where(kvp => kvp.Key.Equals("PackageIdentifier"));


                var z = "p";


                
            }

        }
    }
}
