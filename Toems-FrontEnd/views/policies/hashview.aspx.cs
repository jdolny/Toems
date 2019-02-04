using System;

namespace Toems_FrontEnd.views.policies
{
    public partial class hashview : BasePages.Policies
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var hash = Request.QueryString["hash"];
            if (string.IsNullOrEmpty(hash)) return;

            var hashJson = Call.PolicyApi.GetHashDetail(Policy.Id, hash);
            scriptEditor.Value = hashJson;
        }
    }
}