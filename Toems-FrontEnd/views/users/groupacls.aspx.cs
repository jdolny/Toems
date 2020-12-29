using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Toems_Common;
using Toems_Common.Entity;

namespace Toems_FrontEnd.views.users
{
    public partial class groupacls : BasePages.Users
    {
        private List<CheckBox> _listCheckBoxes;

        protected void buttonUpdate_OnClick(object sender, EventArgs e)
        {
            Call.UserGroupApi.DeleteRights(ToemsUserGroup.Id);
            var listOfRights =
                _listCheckBoxes.Where(x => x.Checked)
                    .Select(box => new EntityUserGroupRight { UserGroupId = ToemsUserGroup.Id, Right = box.ID })
                    .ToList();
            Call.UserGroupRightApi.Post(listOfRights);
            Call.UserGroupApi.UpdateMemberAcls(ToemsUserGroup.Id);
            EndUserMessage = "Updated ACLs";
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
              computerForceCheckin,
              computerSendMessage,
              computerReboot,
              computerShutdown,
              groupReboot,
              groupShutdown,
              computerWakeup,
              groupWakeup,
              scheduleRead,
              scheduleDelete,
              scheduleUpdate,
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
                moduleExternalFiles,
                  imageRead,
                imageUpdate,
                imageDelete,
                imageUploadTask,
                imageDeployTask,
                imageMulticastTask,
                emailLowDiskSpace,
                emailImagingTaskFailed,
                emailImagingTaskCompleted,
                pxeSettingsUpdate,
                pxeIsoGen,
                allowRemoteControl
            };
            if (!IsPostBack) PopulateForm();
        }

        protected void PopulateForm()
        {
            var listOfRights = Call.UserGroupApi.GetRights(ToemsUserGroup.Id);
            foreach (var right in listOfRights)
            {
                foreach (var box in _listCheckBoxes.Where(box => box.ID == right.Right))
                    box.Checked = true;
            }
        }
    }
}