using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common.Dto;
using Toems_Common.Enum;

namespace Toems_FrontEnd.views.admin.toec
{
    public partial class createtargetlist : BasePages.Admin
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ddlListType.DataSource = Enum.GetNames(typeof(EnumToecDeployTargetList.ListType));
                ddlListType.DataBind();


               
            }
        }
    
        protected void PopulateOus()
        {
            gvOus.DataSource = Call.GroupApi.GetOuGroups();
            gvOus.DataBind(); 
        }

        protected void PopulateAdGroups()
        {
            gvAdGroups.DataSource = Call.GroupApi.GetAdSecurityGroups();
            gvAdGroups.DataBind();
        }

        protected void buttonUpdate_Click(object sender, EventArgs e)
        {

        }

        protected void ddlListType_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selected = (EnumToecDeployTargetList.ListType)Enum.Parse(typeof(EnumToecDeployTargetList.ListType), ddlListType.SelectedValue);
            if (selected == EnumToecDeployTargetList.ListType.AdOU)
            {
                ous.Visible = true;
                adGroups.Visible = false;
                computers.Visible = false;
                PopulateOus();
            }
            else if (selected == EnumToecDeployTargetList.ListType.AdGroup)
            {
                ous.Visible = false;
                adGroups.Visible = true;
                computers.Visible = false;
                PopulateAdGroups();
            }
            else
            {
                ous.Visible = false;
                adGroups.Visible = false;
                computers.Visible = true;
            }
        }
    }
}