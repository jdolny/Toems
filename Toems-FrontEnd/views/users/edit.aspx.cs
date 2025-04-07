using System;
using Toems_Common;
using Toems_Common.Entity;
using Toems_FrontEnd.BasePages;

namespace Toems_FrontEnd.views.users
{
    public partial class EditUser : Users
    {

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (Call.ToemsUserApi.GetAdminCount() == 1 && ddluserMembership.Text != "Administrator" &&
                ToemsUser.Membership == "Administrator")
            {
                EndUserMessage = "There Must Be At Least One Administrator";
                return;
            }

            var updatedUser = ToemsUser;
            if (!string.IsNullOrEmpty(txtUserPwd.Text))
            {
                if (txtUserPwd.Text == txtUserPwdConfirm.Text)
                {
                    updatedUser.Salt = Utility.CreateSalt(64);
                    updatedUser.Password = Utility.CreatePasswordHash(txtUserPwd.Text, updatedUser.Salt);
                }
                else
                {
                    EndUserMessage = "Passwords Did Not Match";
                    return;
                }

                if (txtUserPwd.Text.Length < 8)
                {
                    EndUserMessage = "Passwords Must Be At Least 8 Characters";
                    return;
                }
            }

            updatedUser.Name = txtUserName.Text;
            updatedUser.Membership = ddluserMembership.Text;
            updatedUser.Email = txtEmail.Text;
            updatedUser.Theme = ddlTheme.Text;
            updatedUser.DefaultLoginPage = ddlLoginPage.Text;
            updatedUser.DefaultComputerView = ddlComputerView.Text;
            updatedUser.ComputerSortMode = ddlComputerSort.Text;
            updatedUser.EnableWebMfa = chkWebMfa.Checked;
            updatedUser.EnableImagingMfa = chkImagingMfa.Checked;
            var result = Call.ToemsUserApi.Put(updatedUser.Id, updatedUser);

            if (!result.Success)
            {
                EndUserMessage = result.ErrorMessage;
                return;
            }

            var userComputerOptions = new EntityToemsUserOptions();
            userComputerOptions.DescriptionEnabled = chkComputerDesc.Checked;
            userComputerOptions.DescriptionOrder = Convert.ToInt16(txtOrderDesc.Text);

            userComputerOptions.LastCheckinEnabled = chkLastCheckin.Checked;
            userComputerOptions.LastCheckinOrder = Convert.ToInt16(txtOrderLastCheckin.Text);

            userComputerOptions.LastIpEnabled = chkLastKnownIp.Checked;
            userComputerOptions.LastIpOrder = Convert.ToInt16(txtOrderLastKnownIp.Text);

            userComputerOptions.ClientVersionEnabled = chkClientVersion.Checked;
            userComputerOptions.ClientVersionOrder = Convert.ToInt16(txtOrderClientVersion.Text);

            userComputerOptions.LastUserEnabled = chkLastUser.Checked;
            userComputerOptions.LastUserOrder = Convert.ToInt16(txtOrderLastUser.Text);

            userComputerOptions.ProvisionDateEnabled = chkProvisionDate.Checked;
            userComputerOptions.ProvisionDateOrder = Convert.ToInt16(txtOrderProvisionDate.Text);

            userComputerOptions.StatusEnabled = chkStatus.Checked;
            userComputerOptions.StatusOrder = Convert.ToInt16(txtOrderStatus.Text);

            userComputerOptions.CurrentImageEnabled = chkCurrentImage.Checked;
            userComputerOptions.CurrentImageOrder = Convert.ToInt16(txtOrderCurrentImage.Text);

            userComputerOptions.ManufacturerEnabled = chkManufacturer.Checked;
            userComputerOptions.ManufacturerOrder = Convert.ToInt16(txtOrderManufacturer.Text);

            userComputerOptions.ModelEnabled = chkModel.Checked;
            userComputerOptions.ModelOrder = Convert.ToInt16(txtOrderModel.Text);

            userComputerOptions.OsNameEnabled = chkOs.Checked;
            userComputerOptions.OsNameOrder = Convert.ToInt16(txtOrderOs.Text);

            userComputerOptions.OsVersionEnabled = chkOsVersion.Checked;
            userComputerOptions.OsVersionOrder = Convert.ToInt16(txtOrderOsVersion.Text);

            userComputerOptions.OsBuildEnabled = chkOsBuild.Checked;
            userComputerOptions.OsBuildOrder = Convert.ToInt16(txtOrderOsBuild.Text);

            userComputerOptions.DomainEnabled = chkDomain.Checked;
            userComputerOptions.DomainOrder = Convert.ToInt16(txtOrderDomain.Text);

            userComputerOptions.ForceCheckinEnabled = chkForceCheckin.Checked;
            userComputerOptions.ForceCheckinOrder = Convert.ToInt16(txtOrderForceCheckin.Text);

            userComputerOptions.CollectInventoryEnabled = chkCollectInventory.Checked;
            userComputerOptions.CollectInventoryOrder = Convert.ToInt16(txtOrderCollectInventory.Text);

            userComputerOptions.RemoteControlEnabled = chkRemoteControl.Checked;
            userComputerOptions.RemoteControlOrder = Convert.ToInt16(txtOrderRemoteControl.Text);

            userComputerOptions.ServiceLogEnabled = chkGetServiceLog.Checked;
            userComputerOptions.ServiceLogOrder = Convert.ToInt16(txtOrderGetServiceLog.Text);

            userComputerOptions.ToemsUserId = ToemsUser.Id;
            var resultOptions = Call.ToemsUserApi.UpdateUserComputerOptions(userComputerOptions);


            EndUserMessage = !resultOptions.Success ? resultOptions.ErrorMessage : "Successfully Updated User";


        }

        protected void Page_Load(object sender, EventArgs e)
        {
            RequiresAuthorization(AuthorizationStrings.Administrator);
            if (!IsPostBack) PopulateForm();
        }

        protected void PopulateForm()
        {
            ddlTheme.DataSource = GetThemes();
            ddlTheme.DataBind();
            if (ToemsUser.IsLdapUser == 1)
            {
                lblLdap.Text = "TRUE";
                passwords.Visible = false;
            }
            else
            {
                lblLdap.Text = "FALSE";
            }
            txtUserName.Text = ToemsUser.Name;
            ddluserMembership.Text = ToemsUser.Membership;
            txtEmail.Text = ToemsUser.Email;
            ddlTheme.Text = ToemsUser.Theme;
            ddlComputerView.Text = ToemsUser.DefaultComputerView;
            ddlComputerSort.Text = ToemsUser.ComputerSortMode;
            ddlLoginPage.Text = ToemsUser.DefaultLoginPage;
            chkWebMfa.Checked = ToemsUser.EnableWebMfa;
            chkImagingMfa.Checked = ToemsUser.EnableImagingMfa;

            var computerOptions = Call.ToemsUserApi.GetUserComputerOptions(ToemsUser.Id);
            if (computerOptions == null)
                return;
            chkComputerDesc.Checked = computerOptions.DescriptionEnabled;
            txtOrderDesc.Text = computerOptions.DescriptionOrder.ToString();

            chkLastCheckin.Checked = computerOptions.LastCheckinEnabled;
            txtOrderLastCheckin.Text = computerOptions.LastCheckinOrder.ToString();

            chkLastKnownIp.Checked = computerOptions.LastIpEnabled;
            txtOrderLastKnownIp.Text = computerOptions.LastIpOrder.ToString();

            chkClientVersion.Checked = computerOptions.ClientVersionEnabled;
            txtOrderClientVersion.Text = computerOptions.ClientVersionOrder.ToString();

            chkLastUser.Checked = computerOptions.LastUserEnabled;
            txtOrderLastUser.Text = computerOptions.LastUserOrder.ToString();

            chkProvisionDate.Checked = computerOptions.ProvisionDateEnabled;
            txtOrderProvisionDate.Text = computerOptions.ProvisionDateOrder.ToString();

            chkStatus.Checked = computerOptions.StatusEnabled;
            txtOrderStatus.Text = computerOptions.StatusOrder.ToString();

            chkCurrentImage.Checked = computerOptions.CurrentImageEnabled;
            txtOrderCurrentImage.Text = computerOptions.CurrentImageOrder.ToString();

            chkManufacturer.Checked = computerOptions.ManufacturerEnabled;
            txtOrderManufacturer.Text = computerOptions.ManufacturerOrder.ToString();

            chkModel.Checked = computerOptions.ModelEnabled;
            txtOrderModel.Text = computerOptions.ModelOrder.ToString();

            chkOs.Checked = computerOptions.OsNameEnabled;
            txtOrderOs.Text = computerOptions.OsNameOrder.ToString();

            chkOsVersion.Checked = computerOptions.OsVersionEnabled;
            txtOrderOsVersion.Text = computerOptions.OsVersionOrder.ToString();

            chkOsBuild.Checked = computerOptions.OsBuildEnabled;
            txtOrderOsBuild.Text = computerOptions.OsBuildOrder.ToString();

            chkDomain.Checked = computerOptions.DomainEnabled;
            txtOrderDomain.Text = computerOptions.DomainOrder.ToString();

            chkForceCheckin.Checked = computerOptions.ForceCheckinEnabled;
            txtOrderForceCheckin.Text = computerOptions.ForceCheckinOrder.ToString();

            chkCollectInventory.Checked = computerOptions.CollectInventoryEnabled;
            txtOrderCollectInventory.Text = computerOptions.CollectInventoryOrder.ToString();

            chkRemoteControl.Checked = computerOptions.RemoteControlEnabled;
            txtOrderRemoteControl.Text = computerOptions.RemoteControlOrder.ToString();

            chkGetServiceLog.Checked = computerOptions.ServiceLogEnabled;
            txtOrderGetServiceLog.Text = computerOptions.ServiceLogOrder.ToString();

        }

        protected void btnResetMfa_Click(object sender, EventArgs e)
        {
            var result = Call.ToemsUserApi.ResetUserMfaData(ToemsUser.Id);
            EndUserMessage = result ? "Successfully Reset MFA Data" : "Could Not Reset MFA Data";
        }

        protected void btnRemoveLegacy_Click(object sender, EventArgs e)
        {
            var result = Call.ToemsUserApi.RemoveUserLegacyGroup(ToemsUser.Id);
            EndUserMessage = result ? "Successfully Removed Legacy User Group Data" : "Could Not Remomve Legacy User Group Data";
        }
    }
}