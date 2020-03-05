using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web.Security;
using log4net;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceCertificate
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly UnitOfWork _uow;

        public ServiceCertificate()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult AddCertificate(EntityCertificate certificate)
        {
            
          
            var validationResult = ValidateCertificateEntity(certificate, true);
            var actionResult = new DtoActionResult();
            if (validationResult.Success)
            {
                _uow.CertificateRepository.Insert(certificate);
                _uow.Save();
                actionResult.Success = true;
                actionResult.Id = certificate.Id;
            }
            else
            {
                actionResult.ErrorMessage = validationResult.ErrorMessage;
            }

            return actionResult;
        }

        public DtoActionResult DeleteCertificate(int certificateId)
        {
            var u = GetCertificate(certificateId);
            if (u == null) return new DtoActionResult {ErrorMessage = "Certificate Not Found", Id = 0};
          
            _uow.CertificateRepository.Delete(certificateId);
            _uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.Id;
            return actionResult;
        }

        public EntityCertificate GetCertificate(int certificateId)
        {
            return _uow.CertificateRepository.GetById(certificateId);
        }

        public X509Certificate2 GetCertX509Public(int certificateId)
        {
            var certEntity = _uow.CertificateRepository.GetById(certificateId);
            if (certEntity == null) return null;
            var pfx = new X509Certificate2(certEntity.PfxBlob, new EncryptionServices().DecryptText(certEntity.Password), X509KeyStorageFlags.Exportable);
            return new X509Certificate2(pfx.RawData);
        }

        public EntityCertificate GetIntermediateEntity()
        {
            return _uow.CertificateRepository.GetFirstOrDefault(x => x.Type == EnumCertificate.CertificateType.Intermediate);
        }

        public List<EntityCertificate> GetCAIntPair()
        {
            var ca = _uow.CertificateRepository.GetFirstOrDefault(x => x.Type == EnumCertificate.CertificateType.Authority);
            var intermediate = _uow.CertificateRepository.GetFirstOrDefault(x => x.Type == EnumCertificate.CertificateType.Intermediate);

            ca.PfxBlob = null;
            intermediate.PfxBlob = null;
            ca.Password = null;
            intermediate.Password = null;
            var list = new List<EntityCertificate>();
            list.Add(ca);
            list.Add(intermediate);
            return list;
        }


        public byte[] SignMessage(string message, EntityComputer computer)
        {
            var deviceCertEntity = _uow.CertificateRepository.GetById(computer.CertificateId);
            var deviceCert = new X509Certificate2(deviceCertEntity.PfxBlob, new EncryptionServices().DecryptText(deviceCertEntity.Password), X509KeyStorageFlags.Exportable);

            var csp = (RSACryptoServiceProvider)deviceCert.PrivateKey;

            SHA1Managed sha1 = new SHA1Managed();
            UnicodeEncoding encoding = new UnicodeEncoding();
            byte[] data = encoding.GetBytes(message);
            byte[] hash = sha1.ComputeHash(data);

            return csp.SignHash(hash, CryptoConfig.MapNameToOID("SHA1"));

        }

        public X509Certificate2 GetIntermediate()
        {
            var certEntity = _uow.CertificateRepository.GetFirstOrDefault(x => x.Type == EnumCertificate.CertificateType.Intermediate);
            return new X509Certificate2(certEntity.PfxBlob, new EncryptionServices().DecryptText(certEntity.Password), X509KeyStorageFlags.Exportable);
        }

        public byte[] GetIntermediatePublic()
        {
            var certEntity = _uow.CertificateRepository.GetFirstOrDefault(x => x.Type == EnumCertificate.CertificateType.Intermediate);
            var pfx =new X509Certificate2(certEntity.PfxBlob,new EncryptionServices().DecryptText(certEntity.Password) , X509KeyStorageFlags.Exportable);
            return pfx.RawData;
        }

        public byte[] GetCAPublic()
        {
            var certEntity = _uow.CertificateRepository.GetFirstOrDefault(x => x.Type == EnumCertificate.CertificateType.Authority);
            var pfx = new X509Certificate2(certEntity.PfxBlob, new EncryptionServices().DecryptText(certEntity.Password), X509KeyStorageFlags.Exportable);
            return pfx.RawData;
        }

        public byte[] GetCertRawPublic(int certificateId)
        {
            var certEntity = _uow.CertificateRepository.GetById(certificateId);
            if (certEntity == null) return null;
            var pfx = new X509Certificate2(certEntity.PfxBlob, new EncryptionServices().DecryptText(certEntity.Password), X509KeyStorageFlags.Exportable);
            return pfx.RawData;

        }
        public DtoActionResult UpdateCertificate(EntityCertificate certificate)
        {
            var u = GetCertificate(certificate.Id);
            if (u == null) return new DtoActionResult {ErrorMessage = "Certificate Not Found", Id = 0};
           
            var validationResult = ValidateCertificateEntity(certificate, false);
            var actionResult = new DtoActionResult();
            if (validationResult.Success)
            {
                _uow.CertificateRepository.Update(certificate, certificate.Id);
                _uow.Save();
                actionResult.Success = true;
                actionResult.Id = certificate.Id;
            }
            else
            {
                actionResult.ErrorMessage = validationResult.ErrorMessage;
            }

            return actionResult;
        }

        private DtoValidationResult ValidateCertificateEntity(EntityCertificate certificate, bool isNew)
        {
            var validationResult = new DtoValidationResult { Success = true };
            return validationResult;
           //todo: add validation
        }

        public bool GenerateCAandInt()
        {
            var isAllowed = ConfigurationManager.AppSettings["AllowCAGen"];
            if (!isAllowed.ToLower().Equals("true"))
            {
                Logger.Debug("Certificates cannot be generated without updating the web.config key AllowCAGen");
                return false;
            }

            var certRequest = new CertificateRequest();
            var organization = new ServiceSetting().GetSetting(SettingStrings.CertificateOrganization);
            if (organization == null) return false;
            if (string.IsNullOrEmpty(organization.Value)) return false;
            certRequest.SubjectName = string.Format("O={0},CN=Toems CA", organization.Value);
            certRequest.NotBefore = DateTime.UtcNow;
            certRequest.NotAfter = certRequest.NotBefore.AddYears(20);
            var authCertificate = new ServiceGenerateCertificate(certRequest).CreateCertificateAuthorityCertificate();
            
            var c = new EntityCertificate();
            c.NotAfter = authCertificate.NotAfter;
            c.NotBefore = authCertificate.NotBefore;
            c.Serial = authCertificate.SerialNumber;
            var pfxPass = Membership.GeneratePassword(10, 0);
            c.Password = new EncryptionServices().EncryptText(pfxPass);
            c.PfxBlob = authCertificate.Export(X509ContentType.Pfx, pfxPass);
            c.SubjectName = authCertificate.Subject;
            c.Type = EnumCertificate.CertificateType.Authority;

            var existingCA =
                _uow.CertificateRepository.GetFirstOrDefault(x => x.Type == EnumCertificate.CertificateType.Authority);
            if(existingCA != null)
                _uow.CertificateRepository.Delete(existingCA.Id);

            _uow.CertificateRepository.Insert(c);
          
            //intermediate
            var intRequest = new CertificateRequest();
            intRequest.SubjectName = string.Format("O={0},CN=Toems Intermediate", organization.Value);
            intRequest.NotBefore = DateTime.UtcNow;
            intRequest.NotAfter = intRequest.NotBefore.AddYears(20);
            var intCertificate = new ServiceGenerateCertificate(intRequest).IssueCertificate(authCertificate, true, false);

            var ce = new EntityCertificate();
            ce.NotAfter = intCertificate.NotAfter;
            ce.NotBefore = intCertificate.NotBefore;
            ce.Serial = intCertificate.SerialNumber;
            var pfxPassInt = Membership.GeneratePassword(10, 0);
            ce.Password = new EncryptionServices().EncryptText(pfxPassInt);
            ce.PfxBlob = intCertificate.Export(X509ContentType.Pfx, pfxPassInt);
            ce.SubjectName = intCertificate.Subject;
            ce.Type = EnumCertificate.CertificateType.Intermediate;


            var existingInt =
                _uow.CertificateRepository.GetFirstOrDefault(x => x.Type == EnumCertificate.CertificateType.Intermediate);
            if (existingInt != null)
                _uow.CertificateRepository.Delete(existingInt.Id);

            _uow.CertificateRepository.Insert(ce);
        
            _uow.Save();



            return true;
        }

        public bool ValidateCert(X509Certificate2 cert)
        {
            X509Chain chain = new X509Chain();
            X509ChainPolicy chainPolicy = new X509ChainPolicy()
            {
                RevocationMode = X509RevocationMode.NoCheck,
                RevocationFlag = X509RevocationFlag.EntireChain
            };
            chain.ChainPolicy = chainPolicy;

            try
            {
                if (chain.Build(cert))
                {
                    var intermediate = GetIntermediate();
                    //todo: revisit this
                    var correctCaInChain = chain.ChainElements.Cast<X509ChainElement>().Any(x => x.Certificate.Thumbprint == intermediate.Thumbprint);
                    if (!correctCaInChain)
                    {
                        Logger.Debug("Certificate chain mismatch");
                        return false;
                    }
                    return true;
                }

                Logger.Error("Could Not Validate Certificate: " + cert.Subject);

                foreach (X509ChainElement chainElement in chain.ChainElements)
                {
                    foreach (X509ChainStatus chainStatus in chainElement.ChainElementStatus)
                    {
                        Logger.Error(chainStatus.StatusInformation);
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.Error("Could Not Validate Certificate: " + cert.Subject);
                Logger.Error(ex.Message);
                return false;
            }
        }
    }
}