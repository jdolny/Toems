<%@ Page Title="" Language="C#" MasterPageFile="~/views/groups/groups.master" AutoEventWireup="true" CodeBehind="create.aspx.cs" Inherits="Toems_FrontEnd.views.groups.create" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li>New</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
Groups
</asp:Content>
<asp:Content ID="Breadcrumb" ContentPlaceHolderID="DropDownActionsSub" Runat="Server">
    <li><asp:LinkButton ID="btnSubmit" runat="server" OnClick="btnAdd_OnClick" Text="Add Group" CssClass="main-action"/></li>
</asp:Content>




<asp:Content ID="Content" ContentPlaceHolderID="SubContent" runat="Server">
    <script type="text/javascript">
        $(document).ready(function () {
            $('#create').addClass("nav-current");
        });
    </script>
     <div class="size-4 column">
        Name
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtName" runat="server" CssClass="textbox"></asp:TextBox>
    </div>
    <br class="clear"/>
     <div class="size-4 column">
        Group Type
    </div>
    <div class="size-5 column">
        <asp:DropDownList ID="ddlGroupType" runat="server" CssClass="ddlist">
            <asp:ListItem>Static</asp:ListItem>
            <asp:ListItem>Dynamic</asp:ListItem>
        </asp:DropDownList>
    </div>
    <br class="clear"/>

    <div class="size-4 column">
        Description
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtDescription" runat="server"  TextMode="MultiLine" CssClass="descbox"></asp:TextBox>
    </div>
    <br class="clear"/>
     <div class="size-4 column">
        Communication Server Cluster
    </div>
    <div class="size-5 column">
        <asp:DropDownList ID="ddlCluster" runat="server" CssClass="ddlist"/>
    </div>
    <br class="clear"/>
    
    <div class="size-4 column">
        Prevent Shutdown
    </div>
    <div class="size-setting column hidden-check">
        <asp:CheckBox ID="chkPreventShutdown" runat="server" ClientIDMode="Static"></asp:CheckBox>
        <label for="chkPreventShutdown">Toggle</label>
    </div>
    <br class="clear"/>

   
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">
    <h5><span style="color: #ff9900;">Name:</span></h5>
<p>The name of the group, group names must be unique and contain only alphanumeric characters, space, underscore, or dash.</p>
<h5><span style="color: #ff9900;">Group Type:</span></h5>
<p>Specifies a static or dynamic group.  Static groups are manually assigned computers while dynamic group automatically update members based on criteria set on the criteria page.</p>
<h5><span style="color: #ff9900;">Description:</span></h5>
<p>The description field is optional for you to give a short description for what the group is used for.</p>
<h5><span style="color: #ff9900;">Communication Server Cluster:</span></h5>
<p>Groups can be used to specify which client com servers a computer should communicate with.  The default value is to use the default com server cluster.  Clusters can be configured in admin settings-&gt;Client Com Servers.  If a computer is a member of multiple groups that each have different default com server cluster values, a random cluster is selected.</p>
<h5><span style="color: #ff9900;">Wake Up Schedule:<br />
</span></h5>
<p>Groups can be used automatically power on computers based on a schedule you define.  Schedules can be created in Global Properties-&gt;Schedules.</p>
<h5><span style="color: #ff9900;">Shutdown Schedule:</span></h5>
<p>Groups can be used to automatically power off computers based on a schedule you define.  Schedules can be created in Global Properties-&gt;Schedules.</p>
<h5><span style="color: #ff9900;">Prevent Shutdown:</span></h5>
<p>If this option is enabled, any computers in this group cannot be shutdown or rebooted from Theopenem.  This is helpful to prevent accidentally turning off an important computer.  If a computer is in a group with this option enabled, and someone accidentally adds the computer to a group that has a shutdown schedule, it will not be powered off.  This also applies to the Actions menu to shutdown or reboot.</p>
</asp:Content>

