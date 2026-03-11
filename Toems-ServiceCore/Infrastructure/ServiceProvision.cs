using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_ServiceCore.EntityServices;

namespace Toems_ServiceCore.Infrastructure
{
    public class ServiceProvision(ServiceContext ctx)
    {

        public DtoProvisionResponse ProvisionClient(DtoProvisionRequest request)
        {
            ctx.Log.Debug($"Received Provision Request For {request.Name}");
            var provisionResponse = new DtoProvisionResponse();
            var currentIp = ctx.Ip.GetIPAddress();

            //Check if any computers with this installation id already exist
            ctx.Log.Debug($"Installation ID: {request.InstallationId}");
            var existingComputer = ctx.Computer.GetByInstallationId(request.InstallationId);



            if (existingComputer != null)
            {
                ctx.Log.Debug($"A Computer With This Installation Id Already Exists: {request.InstallationId}");
                var originalName = existingComputer.Name;
                ctx.Log.Debug($"Existing Name {originalName} Request Name {request.Name}");

                existingComputer.Name = request.Name;
                existingComputer.Guid = Guid.NewGuid().ToString();
                existingComputer.AdGuid = request.AdGuid;
                existingComputer.ProvisionedTime = DateTime.Now;
                existingComputer.InstallationId = request.InstallationId;
                existingComputer.LastIp = currentIp;

                //even though id is a match, an existing computer may be using the request name
                var doesExist = ctx.Computer.GetByName(request.Name);
                if (doesExist != null)
                {
                    if (doesExist.Id != existingComputer.Id)
                    {
                        //id's don't match, a different computer with this name already exists

                        //Check if we should reset matching installation id's.  Should not be used unless computers have the same install id, due to improper imaging
                        var forceInstallIdReset = ctx.Config["ForceInstallationIdReset"];
                        if (forceInstallIdReset.ToLower().Equals("true"))
                        {
                            ctx.Log.Debug("Installation ID Reset Has Been Enforced From Web.config");
                            return new DtoProvisionResponse() { ProvisionStatus = EnumProvisionStatus.Status.FullReset };
                        }

                        if (existingComputer.ProvisionStatus == EnumProvisionStatus.Status.Reset)
                        {
                            ctx.Log.Debug("Reset Approved.  Archiving Existing Computer And Changing Name Of This Request.");
                            var result = ctx.Computer.ArchiveComputerKeepGroups(doesExist.Id);
                        }

                        else if (doesExist.ProvisionStatus == EnumProvisionStatus.Status.Provisioned)
                        {
                            var requireResetRequests = ctx.Setting.GetSettingValue(SettingStrings.RequireResetRequests);
                            if (requireResetRequests == "1")
                            {
                                //computer has not yet been reset and reset approval is required, return pending reset
                                ctx.Log.Debug($"Computer Has Not Had Reset Approved {originalName}");

                                var resetRequest = new EntityResetRequest();
                                resetRequest.ComputerName = originalName;
                                resetRequest.InstallationId = request.InstallationId;
                                resetRequest.IpAddress = currentIp;

                                var requestResult = ctx.ResetRequest.CreateRequest(resetRequest);
                                if (!requestResult.Success)
                                    provisionResponse.ProvisionStatus = EnumProvisionStatus.Status.Error;

                                provisionResponse.ProvisionStatus = EnumProvisionStatus.Status.PendingReset;
                                return provisionResponse;
                            }
                            else
                            {
                                ctx.Log.Debug("Reset Approvals Are Not Required, Archiving Existing Computer.");
                                var result = ctx.Computer.ArchiveComputerKeepGroups(doesExist.Id);
                            }
                        }
                        else if (doesExist.ProvisionStatus == EnumProvisionStatus.Status.PreProvisioned)
                        {
                            ctx.Log.Debug("Restoring Computer With This ID, Removing The Existing Not Provisioned Computer With This Name");
                            //current id matches computer, but a new computer has preprovisioned with this name, allow provison to occur
                            //delete the doesExist entity, allowing the archived computer to be restored
                            var result = ctx.Computer.DeleteComputer(doesExist.Id); //requires a new instance or delete will fail
                            ctx.Log.Debug("Remove Existing Result: " + JsonConvert.SerializeObject(result));
                        }
                        else if (doesExist.ProvisionStatus == EnumProvisionStatus.Status.ProvisionApproved ||
                                    doesExist.ProvisionStatus == EnumProvisionStatus.Status.Reset)
                        {
                            ctx.Log.Debug("Restoring Computer With This ID, Archiving The Existing Not Provisioned Computer With This Name");
                            //current id matches computer, but a new computer has preprovisioned with this name, allow provison to occur
                            //delete the doesExist entity, allowing the archived computer to be restored
                            var result = ctx.Computer.ArchiveComputerKeepGroups(doesExist.Id); //requires a new instance or delete will fail
                            ctx.Log.Debug("Archive Existing Result: " + JsonConvert.SerializeObject(result));
                        }
                    }
                }

                var cert = GenerateDeviceCert(request.SymmKey, existingComputer);
                ctx.Log.Debug($"Provisioning Computer {request.Name}");
                var addResult = ctx.Computer.UpdateComputer(existingComputer);
                if (!addResult.Success)
                {
                    ctx.Certificate.DeleteCertificate(existingComputer.CertificateId);
                    provisionResponse.ProvisionStatus = EnumProvisionStatus.Status.Error;
                    return provisionResponse;
                }

                provisionResponse.ProvisionStatus = EnumProvisionStatus.Status.PendingConfirmation;
                provisionResponse.Certificate = Convert.ToBase64String(cert);
                provisionResponse.ComputerIdentifier = existingComputer.Guid;

                return provisionResponse;

            }

            var computerEntity = new EntityComputer();
            computerEntity.LastIp = currentIp;

            //By this point we know it's a new installation id, continue on with other checks
            //Check if this computer name is already in use
            var nameExistsEntity = ctx.Computer.GetByName(request.Name);
            if (nameExistsEntity != null)
            {
                ctx.Log.Debug($"A Computer With This Name Exists, Continuing Checks {request.Name}");
                var tmpEntity = JsonConvert.SerializeObject(nameExistsEntity); //done this way to release the entity and not make changes on updates
                computerEntity = JsonConvert.DeserializeObject<EntityComputer>(tmpEntity);

                //if the computer is not approved or preprovisioned check if reset request is required
                if (nameExistsEntity.ProvisionStatus != EnumProvisionStatus.Status.PreProvisioned &&
                    nameExistsEntity.ProvisionStatus != EnumProvisionStatus.Status.ProvisionApproved &&
                    nameExistsEntity.ProvisionStatus != EnumProvisionStatus.Status.Reset)
                {

                    var requireResetRequests = ctx.Setting.GetSettingValue(SettingStrings.RequireResetRequests);
                    if (requireResetRequests == "1")
                    {
                        //computer has not yet been reset and reset approval is required, return pending reset
                        ctx.Log.Debug($"Computer Has Not Had Reset Approved {request.Name}");
                        var resetRequest = new EntityResetRequest();
                        resetRequest.ComputerName = request.Name;
                        resetRequest.InstallationId = request.InstallationId;
                        resetRequest.IpAddress = currentIp;

                        var requestResult = ctx.ResetRequest.CreateRequest(resetRequest);
                        if (!requestResult.Success)
                            provisionResponse.ProvisionStatus = EnumProvisionStatus.Status.Error;

                        provisionResponse.ProvisionStatus = EnumProvisionStatus.Status.PendingReset;
                        return provisionResponse;
                    }
                    else
                    {
                        //computer has not yet been reset and no approval is required, start the reset process
                        ctx.Log.Debug($"Reset Approval Not Required.  Starting Reset Process. {request.Name}");
                        computerEntity.ProvisionStatus = EnumProvisionStatus.Status.Reset;
                        var updateResult = ctx.Computer.UpdateComputer(computerEntity);
                        if (!updateResult.Success)
                            provisionResponse.ProvisionStatus = EnumProvisionStatus.Status.Error;

                        ctx.Certificate.DeleteCertificate(computerEntity.CertificateId);
                        provisionResponse.ProvisionStatus = EnumProvisionStatus.Status.Reset;
                        return provisionResponse;
                    }


                }

            }

            //Check if new provisions require pre provisioning and / or approvals
            var preProvisionRequired = ctx.Setting.GetSettingValue(SettingStrings.RequirePreProvision);
            var provisionApprovalRequired = ctx.Setting.GetSettingValue(SettingStrings.RequireProvisionApproval);
            var preProvisionApprovalRequired = ctx.Setting.GetSettingValue(SettingStrings.PreProvisionRequiresApproval);

            if (preProvisionRequired == "1")
            {
                ctx.Log.Debug($"Pre-Provision Is Required {request.Name}");
                if (computerEntity.ProvisionStatus != EnumProvisionStatus.Status.Reset &&
                    computerEntity.ProvisionStatus != EnumProvisionStatus.Status.PreProvisioned && computerEntity.ProvisionStatus != EnumProvisionStatus.Status.ProvisionApproved)
                {
                    ctx.Log.Debug($"Setting Status To Pending Pre-Provision {request.Name}");
                    provisionResponse.ProvisionStatus = EnumProvisionStatus.Status.PendingPreProvision;
                    return provisionResponse;
                }
                else
                {
                    ctx.Log.Debug($"PreProvision Check Passed {request.Name}");
                }
                if (preProvisionApprovalRequired == "1" && computerEntity.ProvisionStatus == EnumProvisionStatus.Status.PreProvisioned)
                {
                    ctx.Log.Debug($"PreProvision Check Passed. PreProvision Approval Is Required.  Setting Status To Pending Provision Approval {request.Name}");
                    //submit provision approval request
                    var approvalRequest = new EntityApprovalRequest();
                    approvalRequest.ComputerName = request.Name;
                    approvalRequest.InstallationId = request.InstallationId;
                    approvalRequest.IpAddress = currentIp;

                    var requestResult = ctx.ApprovalRequest.CreateRequest(approvalRequest);
                    if (!requestResult.Success)
                        provisionResponse.ProvisionStatus = EnumProvisionStatus.Status.Error;

                    provisionResponse.ProvisionStatus = EnumProvisionStatus.Status.PendingProvisionApproval;
                    return provisionResponse;
                }
            }
            else if (provisionApprovalRequired == "1" && computerEntity.ProvisionStatus != EnumProvisionStatus.Status.ProvisionApproved)
            {
                ctx.Log.Debug($"Provision Approval Is Required.  Setting Status To Pending Provision Approval {request.Name}");
                //submit provision approval request
                var approvalRequest = new EntityApprovalRequest();
                approvalRequest.ComputerName = request.Name;
                approvalRequest.InstallationId = request.InstallationId;
                approvalRequest.IpAddress = currentIp;

                var requestResult = ctx.ApprovalRequest.CreateRequest(approvalRequest);
                if (!requestResult.Success)
                    provisionResponse.ProvisionStatus = EnumProvisionStatus.Status.Error;

                provisionResponse.ProvisionStatus = EnumProvisionStatus.Status.PendingProvisionApproval;
                return provisionResponse;

            }

            //all pre checks complete and successful, move forward with provision
            //Provisioning computer
            computerEntity.Name = request.Name;
            computerEntity.Guid = Guid.NewGuid().ToString();
            computerEntity.AdGuid = request.AdGuid;
            computerEntity.ProvisionedTime = DateTime.Now;
            computerEntity.InstallationId = request.InstallationId;
            computerEntity.LastIp = currentIp;

            byte[] encryptedCert;
            string computerIdentifier;

            if (computerEntity.Id == 0) // this will never be true when preprovision is required
            {
                //this computer does not at exist at all, add it
                ctx.Log.Debug($"Computer Is New, Provisioning Computer {request.Name}");
                encryptedCert = GenerateDeviceCert(request.SymmKey, computerEntity);
                var addResult = ctx.Computer.AddComputer(computerEntity);
                computerIdentifier = computerEntity.Guid;
                if (!addResult.Success)
                {
                    ctx.Certificate.DeleteCertificate(computerEntity.CertificateId);
                    provisionResponse.ProvisionStatus = EnumProvisionStatus.Status.Error;
                    return provisionResponse;
                }
            }
            else
            {
                //computer exists in some way
                //overwriting an existing computer, should we overwrite or archive and create a new one
                //if serial number, memory, processor name, and computer model are the same, assume it's the same computer
                ctx.Log.Debug($"Checking If Computer With Matching Hardware Info Already Exists {request.Name}");
                var existingHardware = ctx.Computer.GetProvisionHardware(computerEntity.Id);

                var macsVerified = true;
                if(existingHardware != null)
                {
                    foreach(var requestMac in request.Macs)
                    {
                        ctx.Log.Debug(requestMac);
                        if(!existingHardware.Macs.Contains(requestMac))
                        {
                            macsVerified = false;
                            break;
                        }
                    }
                }
                ctx.Log.Debug("Macs Verified: " + macsVerified);
                ctx.Log.Debug("Existing Hardware: " + JsonConvert.SerializeObject(existingHardware));
                ctx.Log.Debug("Request Hardware: " + JsonConvert.SerializeObject(request));
                if (existingHardware != null)
                {
                    if (existingHardware.Memory == request.Memory && existingHardware.Model.Equals(request.Model) &&
                        existingHardware.Processor.Equals(request.Processor) &&
                        existingHardware.SerialNumber.Equals(request.SerialNumber) && macsVerified)
                    {
                        ctx.Log.Debug($"Hardware Match Found, Linking To Existing Computer {request.Name}");
                        //same computer, update it
                        encryptedCert = GenerateDeviceCert(request.SymmKey, computerEntity);
                        var updateResult = ctx.Computer.UpdateComputer(computerEntity);
                        computerIdentifier = computerEntity.Guid;
                        if (!updateResult.Success)
                        {
                            provisionResponse.ProvisionStatus = EnumProvisionStatus.Status.Error;
                            return provisionResponse;
                        }
                    }
                    else
                    {
                        //different computer, archive and add new
                        ctx.Log.Debug($"No Hardware Match Found, Archiving Existing Computer{request.Name}");
                        var newComputer = new EntityComputer();
                        newComputer.Name = computerEntity.Name;
                        newComputer.Guid = Guid.NewGuid().ToString();
                        newComputer.AdGuid = computerEntity.AdGuid;
                        newComputer.ProvisionedTime = computerEntity.ProvisionedTime;
                        newComputer.InstallationId = computerEntity.InstallationId;
                        newComputer.LastIp = computerEntity.LastIp;
                        encryptedCert = GenerateDeviceCert(request.SymmKey, newComputer);

                        ctx.Log.Debug("Archiving Computer");
                        ctx.Computer.ArchiveComputerKeepGroups(computerEntity.Id);
                        ctx.Log.Debug("Adding New Computer");
                        var addResult = ctx.Computer.AddComputer(newComputer);
                        computerIdentifier = newComputer.Guid;
                        if (!addResult.Success)
                        {
                            ctx.Certificate.DeleteCertificate(newComputer.CertificateId);
                            provisionResponse.ProvisionStatus = EnumProvisionStatus.Status.Error;
                            return provisionResponse;
                        }
                    }
                }
                else
                {
                    //cannot determine if it's the same computer, when provisioning a new computer with preprovsion or provisional approval required, we should end up here
                    ctx.Log.Debug($"Could Not Determine Hardware Match {request.Name}");
                    if (computerEntity.ProvisionStatus == EnumProvisionStatus.Status.PreProvisioned ||
                        computerEntity.ProvisionStatus == EnumProvisionStatus.Status.ProvisionApproved)
                    {
                        ctx.Log.Debug($"Computer Is PreProvisioned Or Approved.  Provisioning Computer. {request.Name}");
                        //if status is pre-provisioned or approved, assume it's a new computer and finish provision
                        encryptedCert = GenerateDeviceCert(request.SymmKey, computerEntity);
                        var updateResult = ctx.Computer.UpdateComputer(computerEntity);
                        computerIdentifier = computerEntity.Guid;
                        if (!updateResult.Success)
                        {
                            provisionResponse.ProvisionStatus = EnumProvisionStatus.Status.Error;
                            return provisionResponse;
                        }
                    }
                    else
                    {
                        //not sure why we ended up here, no choice but to archive existing and create new
                        //must create a new entity
                        ctx.Log.Debug($"Archiving Existing.  Provisioning Computer. {request.Name}");
                        var newComputer = new EntityComputer();
                        newComputer.Name = computerEntity.Name;
                        newComputer.Guid = Guid.NewGuid().ToString();
                        newComputer.AdGuid = computerEntity.AdGuid;
                        newComputer.ProvisionedTime = computerEntity.ProvisionedTime;
                        newComputer.InstallationId = computerEntity.InstallationId;
                        newComputer.LastIp = computerEntity.LastIp;
                        encryptedCert = GenerateDeviceCert(request.SymmKey, newComputer);

                        ctx.Computer.ArchiveComputerKeepGroups(computerEntity.Id);
                        var addResult = ctx.Computer.AddComputer(newComputer);
                        computerIdentifier = newComputer.Guid;
                        if (!addResult.Success)
                        {
                            ctx.Certificate.DeleteCertificate(newComputer.CertificateId);
                            provisionResponse.ProvisionStatus = EnumProvisionStatus.Status.Error;
                            return provisionResponse;
                        }
                    }
                }
                
            }

            provisionResponse.ProvisionStatus = EnumProvisionStatus.Status.PendingConfirmation;
            provisionResponse.Certificate = Convert.ToBase64String(encryptedCert);
            provisionResponse.ComputerIdentifier = computerIdentifier;

            return provisionResponse;
        }

        private byte[] GenerateDeviceCert(string symmKey, EntityComputer computer)
        {
            var iCert = ctx.Certificate.GetIntermediate();

            byte[] symmetricKey;
            using (RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)iCert.PrivateKey)
            {
                var encryptedKey = Convert.FromBase64String(symmKey);
                symmetricKey = rsa.Decrypt(encryptedKey, true);
            }

            var intermediateEntity = ctx.Certificate.GetIntermediateEntity();
            var pass = ctx.Encryption.DecryptText(intermediateEntity.Password);
            var intermediateCert = new X509Certificate2(intermediateEntity.PfxBlob, pass, X509KeyStorageFlags.Exportable);
            var certRequest = new DtoCertificateRequest();
            var organization = ctx.Setting.GetSettingValue(SettingStrings.CertificateOrganization);
            certRequest.SubjectName = string.Format("O={0},CN={1}", organization, computer.Guid);
            certRequest.NotBefore = DateTime.UtcNow;
            certRequest.NotAfter = certRequest.NotBefore.AddYears(10);
            ctx.GenerateCertificate.SetRequest(certRequest);
            var certificate = ctx.GenerateCertificate.IssueCertificate(intermediateCert, false,false);

            var c = new EntityCertificate();
            c.NotAfter = certificate.NotAfter;
            c.NotBefore = certificate.NotBefore;
            c.Serial = certificate.SerialNumber;
            var pfxPass = Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));
            c.Password = ctx.Encryption.EncryptText(pfxPass);
            c.PfxBlob = certificate.Export(X509ContentType.Pfx, pfxPass);
            c.SubjectName = certificate.Subject;
            c.Type = EnumCertificate.CertificateType.Device;

            var base64DeviceCert = Convert.ToBase64String(certificate.RawData);
            var encryptedCert = new ServiceSymmetricEncryption().EncryptData(symmetricKey, base64DeviceCert);

            ctx.Certificate.AddCertificate(c);
            computer.CertificateId = c.Id;
            computer.ProvisionStatus = EnumProvisionStatus.Status.PendingConfirmation;
            computer.SymmKeyEncrypted = ctx.Encryption.EncryptText(Convert.ToBase64String(symmetricKey));

            return encryptedCert;
        }
    }
}
