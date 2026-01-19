using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices.ComTypes;
using System.Xml;
using Toems_Common;

namespace Toems_ServiceCore.Infrastructure
{
    /// <summary>
    ///     Summary description for Mail
    /// </summary>
    public class MailServices(InfrastructureContext ictx)
    {
        private async Task SendMailAsync(string body, string mailTo, string subject)
        {
            try
            {
                var message = new MailMessage(ictx.Settings.GetSettingValue(SettingStrings.SmtpMailFrom), mailTo)
                {
                    Subject = "Toems " + "(" + subject + ")",
                    Body = body
                };

                using var client = new SmtpClient(
                    ictx.Settings.GetSettingValue(SettingStrings.SmtpServer),
                    Convert.ToInt32(ictx.Settings.GetSettingValue(SettingStrings.SmtpPort)))
                {
                    Credentials = new NetworkCredential(
                        ictx.Settings.GetSettingValue(SettingStrings.SmtpUsername),
                        ictx.Encryption.DecryptText(ictx.Settings.GetSettingValue(SettingStrings.SmtpPassword))),
                    EnableSsl = ictx.Settings.GetSettingValue(SettingStrings.SmtpSsl) == "Yes"
                };

                await client.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                
                ictx.Log.Error(ex.Message);
            }
        }
    }
}