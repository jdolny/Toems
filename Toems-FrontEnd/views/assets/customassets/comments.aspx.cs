using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;

namespace Toems_FrontEnd.views.assets.customassets
{
    public partial class comments : BasePages.Assets
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RequiresAuthorization(AuthorizationStrings.CommentRead);
            if (!IsPostBack)
            PopulateComments();
        }

        protected void buttonUpdate_OnClick(object sender, EventArgs e)
        {
            var dtoComment = new DtoAssetComment();
            dtoComment.AssetId = Asset.Id;
            dtoComment.Comment = txtComment.Text.Replace("\r\n","<br>");
            var result = Call.AssetApi.AddComment(dtoComment);
            if (!result.Success)
            {
                EndUserMessage = result.ErrorMessage;
                return;
            }

            PopulateComments();

        }

        private void PopulateComments()
        {
            var comments = Call.AssetApi.GetComments(Asset.Id);
            foreach (var comment in comments)
            {
                var divName = new HtmlGenericControl("div");
                divName.Attributes["class"] = "comment-title";
                divName.InnerHtml = "On " + comment.CommentTime + " " + comment.Username + " Added";

                var lbl = new Label();
                lbl.Text = comment.Comment;

                var divLabel = new HtmlGenericControl("div");
                divLabel.Attributes["class"] = "comment";
                divLabel.Controls.Add(lbl);

                placeholder.Controls.Add(divName);
                placeholder.Controls.Add(new LiteralControl("<br class=\"clear\" />"));
                placeholder.Controls.Add(divLabel);
                placeholder.Controls.Add(new LiteralControl("<br class=\"clear\" />"));
                placeholder.Controls.Add(new LiteralControl("<br />"));
            }
        }
    }
}