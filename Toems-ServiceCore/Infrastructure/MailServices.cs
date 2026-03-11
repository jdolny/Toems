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
    public class MailServices(ServiceContext ctx)
    {
        public async Task SendMailAsync(string body, string mailTo, string subject)
        {
            try
            {
                var message = new MailMessage(ctx.Setting.GetSettingValue(SettingStrings.SmtpMailFrom), mailTo)
                {
                    Subject = "Toems " + "(" + subject + ")",
                    Body = body
                };

                using var client = new SmtpClient(
                    ctx.Setting.GetSettingValue(SettingStrings.SmtpServer),
                    Convert.ToInt32(ctx.Setting.GetSettingValue(SettingStrings.SmtpPort)))
                {
                    Credentials = new NetworkCredential(
                        ctx.Setting.GetSettingValue(SettingStrings.SmtpUsername),
                        ctx.Encryption.DecryptText(ctx.Setting.GetSettingValue(SettingStrings.SmtpPassword))),
                    EnableSsl = ctx.Setting.GetSettingValue(SettingStrings.SmtpSsl) == "Yes"
                };

                await client.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                
                ctx.Log.Error(ex.Message);
            }
        }
    }
}