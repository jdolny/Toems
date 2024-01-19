﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common.Enum;

namespace Toems_FrontEnd.views.global.attributes
{
    public partial class edit : BasePages.Global
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PopulateTextMode(ddlTextMode);
                PopulateForm();
            }
        }

        private void PopulateForm()
        {
            txtName.Text = CustomAttribute.Name;
            txtDescription.Text = CustomAttribute.Description;
            ddlTextMode.SelectedValue = CustomAttribute.TextMode.ToString();
            chkImaging.Checked = CustomAttribute.ClientImagingAvailable;
        }

        protected void buttonUpdate_OnClick(object sender, EventArgs e)
        {
            CustomAttribute.Name = txtName.Text;
            CustomAttribute.Description = txtDescription.Text;
            CustomAttribute.TextMode = (EnumCustomAttribute.TextMode)Enum.Parse(typeof(EnumCustomAttribute.TextMode), ddlTextMode.SelectedValue);
            CustomAttribute.ClientImagingAvailable = chkImaging.Checked;


            var result = Call.CustomAttributeApi.Put(CustomAttribute.Id, CustomAttribute);
            EndUserMessage = result.Success ? "Successfully Updated Custom Attribute" : result.ErrorMessage;
        }
    }
}