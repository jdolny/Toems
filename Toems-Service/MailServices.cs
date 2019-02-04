using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using log4net;
using Toems_Common;
using Toems_Service.Entity;

namespace Toems_Service
{
    /// <summary>
    ///     Summary description for Mail
    /// </summary>
    public class MailServices
    {
        private readonly ILog log = LogManager.GetLogger(typeof(MailServices));
        public string Body { get; set; }
        public string MailTo { get; set; }
        public string Subject { get; set; }

        public void Send()
        {
            Task.Factory.StartNew(SendMailAsync);
        }

        private void SendMailAsync()
        {
            try
            {
                var message = new MailMessage(ServiceSetting.GetSettingValue(SettingStrings.SmtpMailFrom), MailTo)
                {
                    Subject = "Toems " + "(" + Subject + ")",
                    Body = Body
                };

                var client = new SmtpClient(ServiceSetting.GetSettingValue(SettingStrings.SmtpServer),
                    Convert.ToInt32(ServiceSetting.GetSettingValue(SettingStrings.SmtpPort)))
                {
                    Credentials =
                        new NetworkCredential(ServiceSetting.GetSettingValue(SettingStrings.SmtpUsername),
                            new EncryptionServices().DecryptText(
                                ServiceSetting.GetSettingValue(SettingStrings.SmtpPassword))),
                    EnableSsl = ServiceSetting.GetSettingValue(SettingStrings.SmtpSsl) == "Yes"
                };


                client.Send(message);
            }
            catch (Exception ex)
            {
                
                log.Error(ex.Message);
            }
        }
    }
}