using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Toems_Common;
using Toems_Common.Dto;

namespace Toems_FrontEnd.views.computers
{
    public partial class comments : BasePages.Computers
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RequiresAuthorization(AuthorizationStrings.CommentRead);
            if (!IsPostBack)
                PopulateComments();
        }

        protected void buttonUpdate_OnClick(object sender, EventArgs e)
        {
            var dtoComment = new DtoComputerComment();
            dtoComment.ComputerId = ComputerEntity.Id;
            dtoComment.Comment = txtComment.Text.Replace("\r\n", "<br>");
            var result = Call.ComputerApi.AddComment(dtoComment);
            if (!result.Success)
            {
                EndUserMessage = result.ErrorMessage;
                return;
            }

            PopulateComments();

        }

        private void PopulateComments()
        {
            var comments = Call.ComputerApi.GetComments(ComputerEntity.Id);
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