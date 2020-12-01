<%@ Page Title="" Language="C#" MasterPageFile="~/views/images/profiles/profiles.master" AutoEventWireup="true" CodeBehind="create.aspx.cs" Inherits="Toems_FrontEnd.views.images.profiles.create" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub2">
    
 <li>New Profile</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub2">
New Profile
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub2" Runat="Server">
    <li><asp:LinkButton ID="btnCreate" runat="server" Text="Create Profile" OnClick="buttonCreateProfile_OnClick" CssClass="main-action"/></li>
</asp:Content>




<asp:Content ID="Content2" ContentPlaceHolderID="SubContent2" Runat="Server">
    <script type="text/javascript">
        $(document).ready(function() {
            $('#create').addClass("nav-current");
          

        });
    </script>

        <div class="size-4 column">
        Profile Name:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtProfileName" runat="server" CssClass="textbox"></asp:TextBox>
    </div>
    <br class="clear"/>
    <div class="size-4 column">
        Profile Description:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtProfileDesc" runat="server" CssClass="descbox" TextMode="MultiLine"></asp:TextBox>
    </div>

  
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subsubHelp">
    <h5><span style="color: #ff9900;">Profile Name:</span></h5>
<p>A display name to identify the profile.</p>
<h5><span style="color: #ff9900;">Profile Description:</span></h5>
<p>An optional description for the profile.</p>

</asp:Content>
