﻿using System;
using System.Linq;
using System.Web.UI.WebControls;
using Toems_Common;
using Toems_Common.Entity;
using Toems_Common.Enum;

namespace Toems_FrontEnd.BasePages
{
    public class Assets : PageBaseMaster
    {
        public EntityCustomAssetType AssetType { get; set; }
        public EntityAsset Asset { get; set; }
        public EntityAssetGroup AssetGroup { get; set; }
        

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            AssetType = !string.IsNullOrEmpty(Request.QueryString["assetTypeId"])
                ? Call.CustomAssetTypeApi.Get(Convert.ToInt32(Request.QueryString["assetTypeId"]))
                : null;
            Asset = !string.IsNullOrEmpty(Request.QueryString["assetId"])
                ? Call.AssetApi.Get(Convert.ToInt32(Request.QueryString["assetId"]))
                : null;
            AssetGroup = !string.IsNullOrEmpty(Request.QueryString["assetGroupId"])
                ? Call.AssetGroupApi.Get(Convert.ToInt32(Request.QueryString["assetGroupId"]))
                : null;
           


        }

       

        protected void PopulateAssetTypes(DropDownList ddlAssetTypes)
        {
            ddlAssetTypes.DataSource = Call.CustomAssetTypeApi.Get().Select(d => new { d.Id, d.Name });
            ddlAssetTypes.DataValueField = "Id";
            ddlAssetTypes.DataTextField = "Name";
            ddlAssetTypes.DataBind();
        }

        protected void PopulateMatchType(DropDownList ddl)
        {
            ddl.DataSource = Enum.GetNames(typeof(EnumSoftwareAsset.SoftwareMatchType));
            ddl.DataBind();
        }

    }
}