using System;
using Toems_Common;
using Toems_Common.Entity;

namespace Toems_FrontEnd.BasePages
{
    public class Images : PageBaseMaster
    {
        public EntityImage ImageEntity { get; set; }
        public ImageProfileWithImage ImageProfile { get; set; }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ImageEntity = !string.IsNullOrEmpty(Request.QueryString["imageId"])
               ? Call.ImageApi.Get(Convert.ToInt32(Request.QueryString["imageId"]))
               : null;
            ImageProfile = !string.IsNullOrEmpty(Request["profileId"])
             ? Call.ImageProfileApi.Get(Convert.ToInt32(Request.QueryString["profileId"]))
             : null;
        }
    }
}