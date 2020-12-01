<%@ Page Title="" Language="C#" MasterPageFile="~/views/images/profiles/profiles.master" AutoEventWireup="true" CodeBehind="general.aspx.cs" Inherits="Toems_FrontEnd.views.images.profiles.general" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub2">
      <li>
        <a href="<%= ResolveUrl("~/views/images/profiles/general.aspx") %>?imageId=<%= ImageEntity.Id %>&profileId=<%= ImageProfile.Id %>&sub=profiles"><%= ImageProfile.Name %></a>
    </li>
 <li>General</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub2">
     <%= ImageProfile.Name %>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub2" Runat="Server">
    <li><asp:LinkButton ID="btnUpdate" runat="server" Text="Update Profile" OnClick="btnUpdate_Click" CssClass="main-action"/></li>
</asp:Content>




<asp:Content ID="Content2" ContentPlaceHolderID="SubContent2" Runat="Server">
    <script type="text/javascript">
        $(document).ready(function() {
            $('#general').addClass("nav-current");
           

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
    <br class="clear"/>
     <div class="size-4 column">
        Model Match:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtModelMatch" runat="server" CssClass="textbox"></asp:TextBox>
    </div>
      <br class="clear"/>
     <div class="size-4 column">
        Model Match Criteria:
    </div>
    <div class="size-5 column">
        <asp:DropDownList ID="ddlMatchCriteria" runat="server" CssClass="textbox">
            <asp:ListItem>Disabled</asp:ListItem>
             <asp:ListItem>Equals</asp:ListItem>
             <asp:ListItem>Starts With</asp:ListItem>
             <asp:ListItem>Ends With</asp:ListItem>
             <asp:ListItem>Contains</asp:ListItem>
        </asp:DropDownList>
    </div>
  
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subsubHelp">
      <h5><span style="color: #ff9900;">Profile Name:</span></h5>
<p>A display name to identify the profile.</p>
<h5><span style="color: #ff9900;">Profile Description:</span></h5>
<p>An optional description for the profile.</p>
    <h5><span style="color: #ff9900;">Model Match:</span></h5>
    <p>Model match is used to automatically deploy an image to any machines that match the model defined here. This setting should be used with caution as it is very easy to image a 
        machine even without starting any tasks. You must also be careful to not overwite a computer that you want to upload an image from. To use this setting enter the model name in this field. 
        The model string can be found in the upload and deploy logs.</p>
    <h5><span style="color: #ff9900;">Model Match Criteria:</span></h5>
    <p>Specifies how the Model Match should be evaluated, such as a partial match or an exact match.</p>
</asp:Content>
