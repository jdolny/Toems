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

    <div id="imageType" runat="server">
        <div class="size-4 column">
            Image Type:
        </div>
        <div class="size-5 column">
            <asp:DropDownList ID="ddlImageType" runat="server" CssClass="ddlist">
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
    <p>Specifies if the storage path is local to this server or on a remote SMB share.  The local type can only be used if only a single server is used for all Toec Api's, Toems Api's, and Toems Front Ends.</p>
<h5><span style="color: #ff9900;">Storage Path:</span></h5>
<p>The path to the storage directory.  If the storage type is local, the path should be a local drive, such as c:\toems-local-storage.  If the storage type is SMB, the path must be a UNC path, such as \\server\toems-remote-storage.</p>
<h5><span style="color: #ff9900;">Username:</span></h5>
<p>Only available for a storage type of SMB.  This is the username used to connect to the share.</p>
<h5><span style="color: #ff9900;">Password:</span></h5>
<p>Only available for a storage type of SMB.  This is the password used to connect to the share.</p>
<h5><span style="color: #ff9900;">Domain:</span></h5>
<p>Only available for a storage type of SMB.  This is the domain used to connect to the share.</p>
</asp:Content>
