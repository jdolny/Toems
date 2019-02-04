<%@ Page Title="" Language="C#" MasterPageFile="~/views/global/schedules/schedules.master" AutoEventWireup="true" CodeBehind="usages.aspx.cs" Inherits="Toems_FrontEnd.views.global.schedules.usages" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub2">
    <li><% =Schedule.Name %></li>
     <li>General</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub2">
    <% =Schedule.Name %>
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="SubContent2" Runat="Server">
    <script type="text/javascript">
        $(document).ready(function() {
            $('#usages').addClass("nav-current");
        });
    </script>
  
    
    <div class="size-5 column">
        <div class="custom-select">
         <asp:DropDownList runat="server" ID="ddlObjectType" CssClass="ddlist" AutoPostBack="True" OnSelectedIndexChanged="ddlObjectType_OnSelectedIndexChanged">
             <asp:ListItem></asp:ListItem>
        <asp:ListItem Selected="True">shutdown</asp:ListItem>
        <asp:ListItem>wakeup</asp:ListItem>
             <asp:ListItem>window start</asp:ListItem>
             <asp:ListItem>window end</asp:ListItem>
        </asp:DropDownList>
            </div>
    </div>
    

    <div class="size-5 column">
        <div class="custom-select">
          <asp:DropDownList runat="server" ID="ddlType" CssClass="ddlist" AutoPostBack="True" OnSelectedIndexChanged="ddlObjectType_OnSelectedIndexChanged">
              <asp:ListItem></asp:ListItem>
        <asp:ListItem Selected="True">Groups</asp:ListItem>
        <asp:ListItem>Computers</asp:ListItem>
              <asp:ListItem>Policies</asp:ListItem>
        </asp:DropDownList>
            </div>
    </div>
   
    <br class="clear"/>
     
    <asp:GridView ID="gvGroups" runat="server"  DataKeyNames="Id"  AutoGenerateColumns="False" CssClass="Gridview extraPad" AlternatingRowStyle-CssClass="alt">
        <Columns>
            <asp:BoundField DataField="Id" HeaderText="Id"  Visible="False"/>
             <asp:HyperLinkField DataNavigateUrlFields="Id" DataNavigateUrlFormatString="~/views/groups/general.aspx?groupId={0}" Text="View" ItemStyle-CssClass="chkboxwidth" Target="_blank"/>
            <asp:BoundField DataField="Name" HeaderText="Name" ItemStyle-CssClass="width_200" ></asp:BoundField>
            <asp:BoundField DataField="Dn" HeaderText="Dn" ></asp:BoundField>
        </Columns>
        <EmptyDataTemplate>
            No Groups Found
        </EmptyDataTemplate>
    </asp:GridView>
    
     <asp:GridView ID="gvComputers" runat="server"  DataKeyNames="Id"  AutoGenerateColumns="False" CssClass="Gridview extraPad" AlternatingRowStyle-CssClass="alt">
        <Columns>
            <asp:BoundField DataField="Id" HeaderText="Id"  Visible="False"/>
             <asp:HyperLinkField DataNavigateUrlFields="Id" DataNavigateUrlFormatString="~/views/computers/general.aspx?computerId={0}" Text="View" ItemStyle-CssClass="chkboxwidth" Target="_blank"/>
            <asp:BoundField DataField="Name" HeaderText="Name"  ></asp:BoundField>
        </Columns>
        <EmptyDataTemplate>
            No Computers Found
        </EmptyDataTemplate>
    </asp:GridView>
    
    
    <asp:GridView ID="gvPolicies" runat="server"  DataKeyNames="Id"  AutoGenerateColumns="False" CssClass="Gridview extraPad" AlternatingRowStyle-CssClass="alt">
        <Columns>
            <asp:BoundField DataField="Id" HeaderText="Id"  Visible="False"/>
            <asp:HyperLinkField DataNavigateUrlFields="Id" DataNavigateUrlFormatString="~/views/policies/general.aspx?policyId={0}" Text="View" ItemStyle-CssClass="chkboxwidth" Target="_blank"/>
            <asp:BoundField DataField="Name" HeaderText="Name"  ></asp:BoundField>
        </Columns>
        <EmptyDataTemplate>
            No Policies Found
        </EmptyDataTemplate>
    </asp:GridView>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subsubHelp">
    <p>The usages page allows you to easily determine which computers, groups, or policies the selected schedule applies to.</p>
</asp:Content>

