<%@ Page Title="" Language="C#" MasterPageFile="~/views/groups/groups.master" AutoEventWireup="true" CodeBehind="general.aspx.cs" Inherits="Toems_FrontEnd.views.groups.general" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li><%= GroupEntity.Name %></li>
    <li>General</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
<%= GroupEntity.Name %>
</asp:Content>
<asp:Content ID="Breadcrumb" ContentPlaceHolderID="DropDownActionsSub" Runat="Server">
    <li><asp:LinkButton ID="buttonUpdate" runat="server" OnClick="buttonUpdate_OnClick" Text="Update Group" CssClass="main-action"></asp:LinkButton></li>
</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="SubContent" Runat="Server">
      <script type="text/javascript">
          $(document).ready(function () {
              $('#general').addClass("nav-current");
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
        <asp:TextBox ID="txtDescription" runat="server" CssClass="descbox" TextMode="MultiLine" ></asp:TextBox>
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
        Wakeup Schedule
    </div>

    <div class="size-5 column">
        <asp:DropDownList ID="ddlWakeup" runat="server" CssClass="ddlist"/>
    </div>
    
       <br class="clear"/>
      <div class="size-4 column">
        Shutdown Schedule
    </div>

    <div class="size-5 column">
        <asp:DropDownList ID="ddlShutdown" runat="server" CssClass="ddlist"/>
    </div>
    <br class="clear"/>
    
    <div class="size-4 column">
        Prevent Shutdown
    </div>
    <div class="size-setting column hidden-check">
        <asp:CheckBox ID="chkPreventShutdown" runat="server" ClientIDMode="Static"></asp:CheckBox>
        <label for="chkPreventShutdown">Toggle</label>
    </div>
    <br class="clear" />
        <div class="size-4 column">
        Endpoint Priority
    </div>

    <div class="size-5 column">
        <asp:TextBox ID="txtPriority" runat="server" CssClass="textbox"></asp:TextBox>
    </div>

</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">
    <h1><strong>Actions Menu</strong></h1>
<p>The actions menu has a few options that can be used to run computer specific tasks on all computers in the group.</p>
<h5><span style="color: #ff9900;">Force Checkin:</span></h5>
<p>Forces all computers in the group to checkin immediately, instead of waiting for the next checkin interval.</p>
    <h5><span style="color: #ff9900;">Start Multicast Imaging:</span></h5>
    <p>Starts a multicast session for all members of the group using the image defined in the group's Image Settings</p>
     <h5><span style="color: #ff9900;">Start Unicast Imaging:</span></h5>
    <p>Starts a unicast session for all members of the group using the image defined in the group's Image Settings</p>
<h5><span style="color: #ff9900;">Reboot:</span></h5>
<p>Reboots all computers in the group.</p>
<h5><span style="color: #ff9900;">Shutdown:</span></h5>
<p>Powers off all computers in the group.</p>
<h5><span style="color: #ff9900;">Wake Up:</span></h5>
<p>Powers on all computers in the group, using WOL.</p>
<h5><span style="color: #ff9900;">Collect Inventory:</span></h5>
<p>Immediately runs an inventory collection on all computers in the group without needing to wait for an inventory policy to run.</p>
<h5><span style="color: #ff9900;">Pin / Unpin:</span></h5>
<p>Adds or removes the group from the current users' dashboard.</p>
    <h5><span style="color: #ff9900;">Clear Imaging Ids:</span></h5>
<p>Resets the ImagingId for all computers in a group.  An imaging id is based on a computers physical hardware.  If imaging a computer does not correctly identify itself
, it probably needs it's id reset.
</p>
    <br />
     <h1><strong>General</strong></h1>
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
<h5><span style="color: #ff9900;">Endpoint Priority</span></h5>
    <p>Used to help determine which Communication Server Cluster an endpoint will use if it is a member of multiple groups that use different clusters.  The lower number priority takes precedence.</p>
</asp:Content>

