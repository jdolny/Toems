using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Service.Entity;

namespace Toems_Service
{
   
    public class ServiceOnlineKernel
    {
        private readonly ILog Logger = LogManager.GetLogger(typeof(ServiceOnlineKernel));

        public List<DtoOnlineKernel> GetAllOnlineKernels()
        {
            var wc = new WebClient();
            try
            {
                var data = wc.DownloadData("http://files.clonedeploy.org/kernels/kernels.json");
                var text = Encoding.UTF8.GetString(data);
                return JsonConvert.DeserializeObject<List<DtoOnlineKernel>>(text);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return null;
            }
        }

        private bool WebDownload(DtoOnlineKernel onlineKernel)
        {
            var baseUrl = "http://files.clonedeploy.org/kernels/";
            using (var wc = new WebClient())
            {
                /*try
                {
                    wc.DownloadFile(new Uri(baseUrl + onlineKernel.BaseVersion + "/" + onlineKernel.FileName),
                        ServiceSetting.GetSettingValue(SettingStrings.TftpPath) + "kernels" +
                        Path.DirectorySeparatorChar + onlineKernel.FileName);
                    return true;
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message);
                    return false;
                }*/
            }
            return true;
        }
    }
}
