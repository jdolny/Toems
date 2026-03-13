using System.Net;
using System.Text;
using log4net;
using Newtonsoft.Json;
using Toems_Common.Dto;

namespace Toems_ServiceCore.Infrastructure
{
   
    public class ServiceOnlineKernel
    {
        private readonly ILog Logger = LogManager.GetLogger(typeof(ServiceOnlineKernel));

        public List<DtoOnlineKernel> GetAllOnlineKernels()
        {
            var wc = new WebClient();
            try
            {
                var data = wc.DownloadData("http://files.theopenem.com/kernels/kernels.json");
                var text = Encoding.UTF8.GetString(data);
                return JsonConvert.DeserializeObject<List<DtoOnlineKernel>>(text);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return null;
            }
        }

       
    }
}
