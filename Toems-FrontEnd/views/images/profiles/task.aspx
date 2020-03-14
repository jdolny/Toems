<%@ Page Title="" Language="C#" MasterPageFile="~/views/images/profiles/profiles.master" AutoEventWireup="true" CodeBehind="task.aspx.cs" Inherits="Toems_FrontEnd.views.images.profiles.task" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub2">
      <li>
        <a href="<%= ResolveUrl("~/views/images/profiles/general.aspx") %>?imageId=<%= ImageEntity.Id %>&profileId=<%= ImageProfile.Id %>&sub=profiles"><%= ImageProfile.Name %></a>
    </li>
 <li>Task Options</li>
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
            $('#task').addClass("nav-current");
           

        });
    </script>

     <div class="size-4 column">
        Enable Web Cancel
    </div>
    <div class="size-setting column hidden-check">
            <asp:CheckBox ID="chkWebCancel" runat="server" ClientIDMode="Static"></asp:CheckBox>
         <label for="chkWebCancel">Toggle</label>
        </div>

    <br class="clear"/>

    <div class="size-4 column">
        Task Completed Action
    </div>
    <div class="size-5 column">
        <asp:DropDownList ID="ddlTaskComplete" runat="server" CssClass="ddlist">
            <asp:ListItem>Reboot</asp:ListItem>
            <asp:ListItem>Power Off</asp:ListItem>
            <asp:ListItem>Exit To Shell</asp:ListItem>
        </asp:DropDownList>
    </div>

</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subsubHelp">
    <h5><span style="color: #ff9900;">Display Name:</span></h5>
<p>A name used to identify the com server, it has not effect on the functionality.</p>
<h5><span style="color: #ff9900;">URL:</span></h5>
<p>The url used to access the com server, this must match the url that is set in IIS.</p>
<h5><span style="color: #ff9900;">Description:</span></h5>
<p>An optional description for the com server.</p>
<h5><span style="color: #ff9900;">Replicate Storage:</span></h5>
<p>This option is only available after the com server is added.  When multiple com servers are defined, files for your modules must be replicated across all com servers.  If you want to disable replication to a com server, then disable this option.  If this option is disabled, you must manually replicate the files.</p>
</asp:Content>
