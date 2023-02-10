using System;
using Toems_Common;
using Toems_FrontEnd.BasePages;

namespace Toems_FrontEnd.views.users
{
    public partial class views_users_editgroup : Users
    {
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            ToemsUserGroup.Name = txtGroupName.Text;
            ToemsUserGroup.GroupLdapName = txtLdapGroupName.Text;
            ToemsUserGroup.Membership = ddlGroupMembership.Text;
            var result = Call.UserGroupApi.Put(ToemsUserGroup.Id, ToemsUserGroup);
            EndUserMessage = !result.Success ? result.ErrorMessage : "Successfully Updated User Group";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            RequiresAuthorization(AuthorizationStrings.Administrator);
            if (!IsPostBack) PopulateForm();
        }

        protected void PopulateForm()
        {
         
            if (ToemsUserGroup.IsLdapGroup == 1)
            {
                lblLdap.Text = "TRUE";
                divldapgroup.Visible = true;
                txtLdapGroupName.Text = ToemsUserGroup.GroupLdapName;
            }
            else
            {
                lblLdap.Text = "FALSE";
            }
            
            txtGroupName.Text = ToemsUserGroup.Name;
            ddlGroupMembership.Text = ToemsUserGroup.Membership;
        }
    }
}