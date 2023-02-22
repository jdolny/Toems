<%@ Page Title="" Language="C#" MasterPageFile="~/views/images/images.master" AutoEventWireup="true" CodeBehind="create.aspx.cs" Inherits="Toems_FrontEnd.views.images.create" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li>New Image</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
    Images
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub" runat="Server">
    <li><asp:LinkButton ID="btnDelete" runat="server" Text="Add Image" OnClick="btnSubmit_Click" CssClass="main-action"/></li>
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="SubContent" runat="Server">
  <script type="text/javascript">
        $(document).ready(function() {
            $('#create').addClass("nav-current");

        });

    </script>
   
         <div class="size-4 column">
        Image Name:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtImageName" runat="server" CssClass="textbox"></asp:TextBox>
    </div>
    <br class="clear"/>

    <div class="size-4 column">
        Imaging Client Environment:
    </div>
    <div class="size-5 column">
        <asp:DropDownList ID="ddlEnvironment" runat="server" CssClass="ddlist" AutoPostBack="True" OnSelectedIndexChanged="ddlEnvironment_OnSelectedIndexChanged">
            <asp:ListItem>linux</asp:ListItem>
            <asp:ListItem>winpe</asp:ListItem>
        </asp:DropDownList>
    </div>
    <br class="clear"/>

    <div id="imageTypeWinPe" runat="server">
        <div class="size-4 column">
            Image Type:
        </div>
        <div class="size-5 column">
            <asp:DropDownList ID="ddlImageTypeWinPe" runat="server" CssClass="ddlist">
                <asp:ListItem>Block</asp:ListItem>
                <asp:ListItem>File</asp:ListItem>
                <asp:ListItem>Both</asp:ListItem>
            </asp:DropDownList>
        </div>
        <br class="clear"/>
    </div>

     <div id="imageTypeLinux" runat="server">
        <div class="size-4 column">
            Image Type:
        </div>
        <div class="size-5 column">
            <asp:DropDownList ID="ddlImageTypeLinux" runat="server" CssClass="ddlist">
                <asp:ListItem>Block</asp:ListItem>
                <asp:ListItem>File</asp:ListItem>
            </asp:DropDownList>
        </div>
        <br class="clear"/>
    </div>


    <div class="size-4 column">
        Image Description:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtImageDesc" runat="server" TextMode="MultiLine" CssClass="descbox"></asp:TextBox>
    </div>
    <br class="clear"/>

  

    <div class="size-4 column">
        Protected:
    </div>
     <div class="size-setting column hidden-check">
            <asp:CheckBox ID="chkProtected" runat="server" ClientIDMode="Static"></asp:CheckBox>
         <label for="chkProtected">Toggle</label>
        </div>
    <br class="clear"/>
    <br/>
    <div class="size-4 column">
        Available On Demand:
    </div>
      <div class="size-setting column hidden-check">
            <asp:CheckBox ID="chkVisible" runat="server" ClientIDMode="Static"></asp:CheckBox>
         <label for="chkVisible">Toggle</label>
        </div>

</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">
 
<h5><span style="color: #ff9900;">Image Name:</span></h5>
<p>The name of the image.</p>
<h5><span style="color: #ff9900;">Image Client Environment:</span></h5>
<p>The Client Environment that will be used to deploy or upload an image.  This should stay as linux for most images, only switching to WinPE when the Linux environment has issues.</p>
<h5><span style="color: #ff9900;">Image Type</span></h5>
<p>Specifies if the image will be captured at the Block or File level.  Most cases should be Block unless you have a need for a file based image.</p>
<h5><span style="color: #ff9900;">Image Description:</span></h5>
<p>Optional, for your own use.</p>
    <h5><span style="color: #ff9900;">Protected:</span></h5>
<p>A protected image cannot be deleted or uploaded.  Helpful to avoid accidentally overwriting an image if someone accidently selects upload instead of deploy.
    After an image has uploaded, this setting automatically enables itself.
</p>
    <h5><span style="color: #ff9900;">Available On Demand:</span></h5>
<p>Specifies if the image can be used directly from the Client Imaging OS menu, instead of a web based task.</p>
</asp:Content>
