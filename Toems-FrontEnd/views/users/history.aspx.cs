using System;
using System.Linq;
using System.Web.UI.WebControls;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Enum;
using Toems_FrontEnd.BasePages;

namespace Toems_FrontEnd.views.users
{
    public partial class history : Users
    {
        protected void ddl_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateGrid();
        }

        protected void ddlType_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateGrid();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            RequiresAuthorization(AuthorizationStrings.Administrator);
            if (!IsPostBack)
            {
                ddlType.DataSource = Enum.GetNames(typeof(EnumAuditEntry.AuditType));
                ddlType.DataBind();
                ddlType.Items.Insert(0, new ListItem("", ""));
                ddlType.Items.Insert(1, new ListItem("Select Filter", "Select Filter"));
                ddlType.SelectedIndex = 1;
                PopulateGrid();
            }
        }

        protected void PopulateGrid()
        {
            var filter = new DtoSearchFilter();
            filter.Limit = ddlLimit.Text == "All" ? int.MaxValue : Convert.ToInt32(ddlLimit.Text);
            filter.SearchText = ddlType.Text;
        
            gvHistory.DataSource = Call.ToemsUserApi.GetUserAuditLogs(ToemsUser.Id, filter);
            gvHistory.DataBind();
        }
    }
}