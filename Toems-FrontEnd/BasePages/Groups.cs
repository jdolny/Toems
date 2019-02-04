using System;
using Toems_Common;
using Toems_Common.Entity;

namespace Toems_FrontEnd.BasePages
{
    public class Groups : PageBaseMaster
    {
        public EntityGroup GroupEntity { get; set; }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            RequiresAuthorization(AuthorizationStrings.GroupRead);
            GroupEntity = !string.IsNullOrEmpty(Request.QueryString["groupId"])
               ? Call.GroupApi.Get(Convert.ToInt32(Request.QueryString["groupId"]))
               : null;
        }
    }
}