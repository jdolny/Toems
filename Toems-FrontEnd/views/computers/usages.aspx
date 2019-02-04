<%@ Page Title="" Language="C#" MasterPageFile="~/views/computers/computers.master" AutoEventWireup="true" CodeBehind="usages.aspx.cs" Inherits="Toems_FrontEnd.views.computers.usages" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li> <%= ComputerEntity.Name %></li>
    <li>Usages</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
    <%= ComputerEntity.Name %>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="DropDownActionsSub">

</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="SubContent" Runat="Server">
      <script type="text/javascript">
          $(document).ready(function () {
              $('#usages').addClass("nav-current");
              $("[id*=gvComputers] td").hover(function () {
                  $("td", $(this).closest("tr")).addClass("hover_row");
              }, function () {
                  $("td", $(this).closest("tr")).removeClass("hover_row");
              });
              $("[id*=gvGroups] td").hover(function () {
                  $("td", $(this).closest("tr")).addClass("hover_row");
              }, function () {
                  $("td", $(this).closest("tr")).removeClass("hover_row");
              });
            

          });
    </script>
    
    
    <br class="clear" />
    <div class="size-4 column">
        Usage Type:
    </div>

    <div class="size-5 column">
        <div class="custom-select">
           
            <asp:DropDownList ID="ddlUtil" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlUtil_OnSelectedIndexChanged">
                 <asp:ListItem></asp:ListItem>
                <asp:ListItem Selected="True">Groups</asp:ListItem>
                <asp:ListItem>Policies</asp:ListItem>
                <asp:ListItem>Modules</asp:ListItem>

            </asp:DropDownList>
        </div>
    </div>
    <br />
    <br class="clear" />

    <asp:GridView ID="gvModules" runat="server"  DataKeyNames="Id"  AutoGenerateColumns="False" CssClass="Gridview extraPad" AlternatingRowStyle-CssClass="alt">
        <Columns>
            <asp:BoundField DataField="Id" HeaderText="Id"  Visible="False"/>
               <asp:HyperLinkField DataNavigateUrlFields="ModuleType,Id" DataNavigateUrlFormatString="~/views/policies/moduleredirect.aspx?type={0}&id={1}" Text="View" ItemStyle-CssClass="chkboxwidth" Target="_blank"/>
           
            <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" ItemStyle-CssClass="width_200"></asp:BoundField>
            <asp:BoundField DataField="ModuleType" HeaderText="Type" SortExpression="Type" ItemStyle-CssClass="width_200"/>
              <asp:BoundField DataField="Guid" HeaderText="Guid"/>
             

        </Columns>
        <EmptyDataTemplate>
            No Modules Found
        </EmptyDataTemplate>
    </asp:GridView>
    
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
    
      <asp:GridView ID="gvPolicies" runat="server"  DataKeyNames="Id"  AutoGenerateColumns="False" CssClass="Gridview extraPad" AlternatingRowStyle-CssClass="alt">
        <Columns>
            <asp:BoundField DataField="Id" HeaderText="Id"  Visible="False"/>
             <asp:HyperLinkField DataNavigateUrlFields="Id" DataNavigateUrlFormatString="~/views/policies/general.aspx?policyId={0}" Text="View" ItemStyle-CssClass="chkboxwidth" Target="_blank"/>
            <asp:BoundField DataField="Name" HeaderText="Name" ></asp:BoundField>
        </Columns>
        <EmptyDataTemplate>
            No Policies Found
        </EmptyDataTemplate>
    </asp:GridView>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">
    <p>This page makes it easy to determine any groups, policies, or modules the computer is assigned to.</p>
</asp:Content>
