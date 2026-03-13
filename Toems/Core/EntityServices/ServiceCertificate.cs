using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using log4net;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_DataModel;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceCertificate(ServiceContext ctx)
    {
        public DtoActionResult AddCertificate(EntityCertificate certificate)
        {
            var validationResult = ValidateCertificateEntity(certificate, true);
            var actionResult = new DtoActionResult();
            if (validationResult.Success)
            {
                ctx.Uow.CertificateRepository.Insert(certificate);
                ctx.Uow.Save();
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
          
            ctx.Uow.CertificateRepository.Delete(certificateId);
            ctx.Uow.Save();
            var actionResult = new DtoActionResult();
            actionResult.Success = true;
            actionResult.Id = u.Id;
            return actionResult;
        }

        public EntityCertificate GetCertificate(int certificateId)
        {
            return ctx.Uow.CertificateRepository.GetById(certificateId);
        }

        public X509Certificate2 GetCertX509Public(int certificateId)
        {
            var certEntity = ctx.Uow.CertificateRepository.GetById(certificateId);
            if (certEntity == null) return null;
            var pfx = new X509Certificate2(certEntity.PfxBlob, ctx.Encryption.DecryptText(certEntity.Password), X509KeyStorageFlags.Exportable);
            return new X509Certificate2(pfx.RawData);
        }

        public EntityCertificate GetIntermediateEntity()
        {
            return ctx.Uow.CertificateRepository.GetFirstOrDefault(x => x.Type == EnumCertificate.CertificateType.Intermediate);
        }

        public List<EntityCertificate> GetCAIntPair()
        {
            var ca = ctx.Uow.CertificateRepository.GetFirstOrDefault(x => x.Type == EnumCertificate.CertificateType.Authority);
            var intermediate = ctx.Uow.CertificateRepository.GetFirstOrDefault(x => x.Type == EnumCertificate.CertificateType.Intermediate);

            // Certificates weren't generated yet.
            if (ca == null || intermediate == null) {
                return null;
            }

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
            var deviceCertEntity = ctx.Uow.CertificateRepository.GetById(computer.CertificateId);
            var deviceCert = new X509Certificate2(deviceCertEntity.PfxBlob, ctx.Encryption.DecryptText(deviceCertEntity.Password), X509KeyStorageFlags.Exportable);

            var csp = (RSACryptoServiceProvider)deviceCert.PrivateKey;

            SHA1Managed sha1 = new SHA1Managed();
            UnicodeEncoding encoding = new UnicodeEncoding();
            byte[] data = encoding.GetBytes(message);
            byte[] hash = sha1.ComputeHash(data);

            return csp.SignHash(hash, CryptoConfig.MapNameToOID("SHA1"));

        }

        public X509Certificate2 GetIntermediate()
        {
            var certEntity = ctx.Uow.CertificateRepository.GetFirstOrDefault(x => x.Type == EnumCertificate.CertificateType.Intermediate);
            return new X509Certificate2(certEntity.PfxBlob, ctx.Encryption.DecryptText(certEntity.Password), X509KeyStorageFlags.Exportable);
        }

        public byte[] GetIntermediatePublic()
        {
            var certEntity = ctx.Uow.CertificateRepository.GetFirstOrDefault(x => x.Type == EnumCertificate.CertificateType.Intermediate);
            var pfx =new X509Certificate2(certEntity.PfxBlob,ctx.Encryption.DecryptText(certEntity.Password) , X509KeyStorageFlags.Exportable);
            return pfx.RawData;
        }

        public byte[] GetCAPublic()
        {
            var certEntity = ctx.Uow.CertificateRepository.GetFirstOrDefault(x => x.Type == EnumCertificate.CertificateType.Authority);
            var pfx = new X509Certificate2(certEntity.PfxBlob, ctx.Encryption.DecryptText(certEntity.Password), X509KeyStorageFlags.Exportable);
            return pfx.RawData;
        }

        public byte[] GetCertRawPublic(int certificateId)
        {
            var certEntity = ctx.Uow.CertificateRepository.GetById(certificateId);
            if (certEntity == null) return null;
            var pfx = new X509Certificate2(certEntity.PfxBlob, ctx.Encryption.DecryptText(certEntity.Password), X509KeyStorageFlags.Exportable);
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
                ctx.Uow.CertificateRepository.Update(certificate, certificate.Id);
                ctx.Uow.Save();
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
            var isAllowed = ctx.Config["AllowCAGen"];
            if (!isAllowed.ToLower().Equals("true"))
            {
                ctx.Log.Debug("Certificates cannot be generated without updating the web.config key AllowCAGen");
                return false;
            }

            var certRequest = new DtoCertificateRequest();
            var organization = ctx.Setting.GetSetting(SettingStrings.CertificateOrganization);
            if (organization == null) return false;
            if (string.IsNullOrEmpty(organization.Value)) return false;
            certRequest.SubjectName = string.Format("O={0},CN=Toems CA", organization.Value);
            certRequest.NotBefore = DateTime.UtcNow;
            certRequest.NotAfter = certRequest.NotBefore.AddYears(20);
            ctx.GenerateCertificate.SetRequest(certRequest);
            var authCertificate = ctx.GenerateCertificate.CreateCertificateAuthorityCertificate();
            
            var c = new EntityCertificate();
            c.NotAfter = authCertificate.NotAfter;
            c.NotBefore = authCertificate.NotBefore;
            c.Serial = authCertificate.SerialNumber;
            var pfxPass = GeneratePassword(12);
            c.Password = ctx.Encryption.EncryptText(pfxPass);
            c.PfxBlob = authCertificate.Export(X509ContentType.Pfx, pfxPass);
            c.SubjectName = authCertificate.Subject;
            c.Type = EnumCertificate.CertificateType.Authority;

            var existingCA =
                ctx.Uow.CertificateRepository.GetFirstOrDefault(x => x.Type == EnumCertificate.CertificateType.Authority);
            if(existingCA != null)
                ctx.Uow.CertificateRepository.Delete(existingCA.Id);

            ctx.Uow.CertificateRepository.Insert(c);
          
            //intermediate
            var intRequest = new DtoCertificateRequest();
            intRequest.SubjectName = string.Format("O={0},CN=Toems Intermediate", organization.Value);
            intRequest.NotBefore = DateTime.UtcNow;
            intRequest.NotAfter = intRequest.NotBefore.AddYears(20);
            ctx.GenerateCertificate.SetRequest(intRequest);
            var intCertificate = ctx.GenerateCertificate.IssueCertificate(authCertificate, true, false);

            var ce = new EntityCertificate();
            ce.NotAfter = intCertificate.NotAfter;
            ce.NotBefore = intCertificate.NotBefore;
            ce.Serial = intCertificate.SerialNumber;
            var pfxPassInt = GeneratePassword(12);
            ce.Password = ctx.Encryption.EncryptText(pfxPassInt);
            ce.PfxBlob = intCertificate.Export(X509ContentType.Pfx, pfxPassInt);
            ce.SubjectName = intCertificate.Subject;
            ce.Type = EnumCertificate.CertificateType.Intermediate;


            var existingInt =
                ctx.Uow.CertificateRepository.GetFirstOrDefault(x => x.Type == EnumCertificate.CertificateType.Intermediate);
            if (existingInt != null)
                ctx.Uow.CertificateRepository.Delete(existingInt.Id);

            ctx.Uow.CertificateRepository.Insert(ce);
        
            ctx.Uow.Save();



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
                        ctx.Log.Debug("Certificate chain mismatch");
                        return false;
                    }
                    return true;
                }

                ctx.Log.Error("Could Not Validate Certificate: " + cert.Subject);

                foreach (X509ChainElement chainElement in chain.ChainElements)
                {
                    foreach (X509ChainStatus chainStatus in chainElement.ChainElementStatus)
                    {
                        ctx.Log.Error(chainStatus.StatusInformation);
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                ctx.Log.Error("Could Not Validate Certificate: " + cert.Subject);
                ctx.Log.Error(ex.Message);
                return false;
            }
        }
        public string GeneratePassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()";
            char[] chars = new char[length];
            byte[] bytes = new byte[length];

            RandomNumberGenerator.Fill(bytes);

            for (int i = 0; i < length; i++)
            {
                chars[i] = valid[bytes[i] % valid.Length];
            }

            return new string(chars);
        }
        
    }
    
  
    
    
}