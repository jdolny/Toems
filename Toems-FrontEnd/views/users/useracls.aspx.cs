using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Toems_Common;
using Toems_Common.Entity;

namespace Toems_FrontEnd.views.users
{
    public partial class useracls : BasePages.Users
    {
        private List<CheckBox> _listCheckBoxes;

        protected void buttonUpdate_OnClick(object sender, EventArgs e)
        {
            if (ToemsUser.UserGroupId != -1)
            {
                EndUserMessage = "Cannot Update. This User's ACL Is Controlled By A Group";
                return;
            }
            var delResult = Call.ToemsUserApi.DeleteRights(ToemsUser.Id);
            if (!delResult)
            {
                EndUserMessage = "Could Not Update User ACLs";
                return;
            }

            var listOfRights =
                _listCheckBoxes.Where(x => x.Checked)
                    .Select(box => new EntityUserRight { UserId = ToemsUser.Id, Right = box.ID })
                    .ToList();
            EndUserMessage = Call.UserRightApi.Post(listOfRights).Success
                ? "Successfully Updated User ACLs"
                : "Could Not Update User ACLs";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            RequiresAuthorization(AuthorizationStrings.Administrator);

            _listCheckBoxes = new List<CheckBox>
            {
              moduleRead,
              policyRead,
              groupRead,
              computerRead,
              moduleUpdate,
              policyUpdate,
              groupUpdate,
              computerUpdate,
              moduleArchive,
              policyArchive,
              computerArchive,
              moduleDelete,
              policyDelete,
              groupDelete,
              computerDelete,
              reportRead,
              approveProvision,
              approveReset,
              emailApproval,
              emailReset,
              emailSmart,
              policyActivate,
              groupPolicyRead,
              groupPolicyUpdate,
              computerSendMessage,
              computerForceCheckin,
              computerReboot,
              computerShutdown,
              groupReboot,
              groupShutdown,
              computerWakeup,
              groupWakeup,
              scheduleRead,
              scheduleUpdate,
              scheduleDelete,
                categoryRead,
                categoryUpdate,
                categoryDelete,
                customAttributeRead,
                customAttributeUpdate,
                customAttributeDelete,
                assetTypeRead,
                assetTypeUpdate,
                assetTypeDelete,
                assetRead,
                assetUpdate,
                assetArchive,
                assetDelete,
                attachmentAdd,
                attachmentRead,
                commentAdd,
                commentRead,
                policyImport,
                policyExport,
                moduleUploadFiles,
                moduleExternalFiles

            };
            if (!IsPostBack) PopulateForm();
        }

        protected void PopulateForm()
        {
            var listOfRights = Call.ToemsUserApi.GetRights(ToemsUser.Id);
            foreach (var right in listOfRights)
            {
                var localright = right;
                foreach (var box in _listCheckBoxes.Where(box => box.ID == localright.Right))
                    box.Checked = true;
            }
        }
    }
}