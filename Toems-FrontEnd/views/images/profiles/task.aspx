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
    <h5><span style="color: #ff9900;">Enable Web Cancel:</span></h5>
<p>If a web task is cancelled from the web interface, the task will disappear, but will continue to run on the client itself. Checking this box will stop the task on the client and perform the 
    task completed action. This would mainly be used for debugging. Sometimes an image process will appear to have frozen. If web cancel is enabled it should stop the client and upload any 
    part of the log it currently has. This must be enabled before a task is started to work.</p>
      <h5><span style="color: #ff9900;">Task Completed Action:</span></h5>
    <p>Specifies what should happen when a client task finishes or errors out.</p>
</asp:Content>
