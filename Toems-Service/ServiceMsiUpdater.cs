using log4net;
using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Toems_Common;
using Toems_Common.Enum;
using Toems_DataModel;
using Toems_Service.Entity;

namespace Toems_Service
{
    public class ServiceMsiUpdater
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private UnitOfWork _uow;
        private string _comServers;
        private string _thumbprint;
        private string _serverKey;

        public ServiceMsiUpdater()
        {
            _uow = new UnitOfWork();
        }

        public string GetNameForExport(bool is64bit)
        {
            var expectedClient = new ServiceVersion().Get(1).LatestClientVersion;
        
            var newVersion = expectedClient.Split('.');
            var v = string.Join(".", newVersion.Take(newVersion.Length - 1));
            var type = is64bit ? "-x64.msi" : "-x86.msi";
            var outputFileName = $"Toec-{v}{type}";
            return outputFileName;
        }
        public byte[] UpdateMsis(bool is64bit)
        {
            var expectedClient = new ServiceVersion().Get(1).LatestClientVersion;

            if (string.IsNullOrEmpty(expectedClient))
            {
                Logger.Error("Cannot Create MSI.  Unknown Expected Toec Version");
                return null;
            }
            

            var type = is64bit ? "-x64.msi" : "-x86.msi";
            var basePath = Path.Combine(HttpContext.Current.Server.MapPath("~"), "private", "agent");
            var stockFileFullPath = Path.Combine(basePath, $"Toec-{expectedClient}{type}");
            if (!File.Exists(stockFileFullPath))
            {
                Logger.Debug("Cannot Create MSI.  Could Not Locate Stock MSI");
                return null;
            }

            var ca = new ServiceCertificate().GetCAPublic();
            if(ca == null)
            {
                Logger.Debug("Cannot Create MSI.  Certificate Chain Must First Be Created. ");
                return null;
            }

            var newVersion = expectedClient.Split('.');
            var v = string.Join(".", newVersion.Take(newVersion.Length - 1));
            var outputFileName = $"Toec-{v}{type}";
            var outputFileFullPath = Path.Combine(basePath, outputFileName);

            try
            {
                File.Delete(outputFileFullPath);
            }
            catch { //ignored
            }

            try
            {
                File.Copy(stockFileFullPath, outputFileFullPath);
            }
            catch(Exception ex)
            {
                Logger.Error("Could Not Create MSI.");
                Logger.Error(ex.Message);
                return null;
            }

            Stream stream = new MemoryStream(ca);
            Database database = null;
            View serverKey = null;
            View thumbprint = null;
            View comServers = null;
            View cert = null;
            Record rec = null;

            GetMsiArgs();
            using (database = new Database(outputFileFullPath, DatabaseOpenMode.Transact))
            {
                try
                {

                    serverKey = database.OpenView(String.Format("INSERT INTO Property (Property, Value) VALUES ('{0}', '{1}')", "SERVER_KEY", _serverKey));
                    serverKey.Execute();
                    serverKey.Close();

                    comServers = database.OpenView(String.Format("INSERT INTO Property (Property, Value) VALUES ('{0}', '{1}')", "COM_SERVERS", _comServers));
                    comServers.Execute();
                    comServers.Close();

                    thumbprint = database.OpenView(String.Format("INSERT INTO Property (Property, Value) VALUES ('{0}', '{1}')", "CA_THUMBPRINT", _thumbprint));
                    thumbprint.Execute();
                    thumbprint.Close();

                    cert = database.OpenView("UPDATE `Binary` SET `Data` = ? WHERE `Name` = 'ToemsCA.Binary'");
                    rec = new Record(1);
                    rec.SetStream(1,stream);
                    cert.Execute(rec);
                    cert.Close();

                    database.Commit();
                }
                catch(Exception ex)
                {
                    Logger.Error("Could Not Create Msi.");
                    Logger.Error(ex.Message);
                    return null;
                }
                finally
                {
                    if (rec != null) rec.Close();
                    if (serverKey != null) serverKey.Close();
                    if (thumbprint != null) thumbprint.Close();
                    if (comServers != null) comServers.Close();
                    if (cert != null) cert.Close();
                    if (database != null) database.Close();
                }
            }

            var file = File.ReadAllBytes(outputFileFullPath);
            return file;
        }


        private bool GetMsiArgs()
        {
            var certEntity = _uow.CertificateRepository.GetFirstOrDefault(x => x.Type == EnumCertificate.CertificateType.Authority);
            if (certEntity == null) return false;
            var pfx = new X509Certificate2(certEntity.PfxBlob, new EncryptionServices().DecryptText(certEntity.Password), X509KeyStorageFlags.Exportable);
            _thumbprint = pfx.Thumbprint;

            var provisionKeyEncrypted = ServiceSetting.GetSettingValue(SettingStrings.ProvisionKeyEncrypted);
            _serverKey = new EncryptionServices().DecryptText(provisionKeyEncrypted);

            var defaultCluster = _uow.ComServerClusterRepository.GetFirstOrDefault(x => x.IsDefault);
            var clusterServers = _uow.ComServerClusterServerRepository.Get(x => x.ComServerClusterId == defaultCluster.Id);
            _comServers = "";
            foreach (var s in clusterServers)
            {
                var comServer = _uow.ClientComServerRepository.GetById(s.ComServerId);
                _comServers += comServer.Url + ",";
            }
            _comServers = _comServers.Trim(',');

            return true;
        }


    }
}
