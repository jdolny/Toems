<%@ Page Title="" Language="C#" MasterPageFile="~/views/policies/policies.master" AutoEventWireup="true" CodeBehind="conditions.aspx.cs" Inherits="Toems_FrontEnd.views.policies.conditions" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li><%= Policy.Name %></li>
    <li>Conditions</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
        <%= Policy.Name %>
</asp:Content>
<asp:Content ID="Breadcrumb" ContentPlaceHolderID="DropDownActionsSub" Runat="Server">
    <li><asp:LinkButton ID="buttonUpdate" runat="server" OnClick="buttonUpdate_OnClick" Text="Update Policy" CssClass="main-action"></asp:LinkButton></li>
</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="SubContent" Runat="Server">
      <script type="text/javascript">
          $(document).ready(function () {
              $('#conditions').addClass("nav-current");
          });
      </script>
   
     
    <div class="size-4 column">
        Condition:
    </div>
    <div class="size-5 column">
       
        <asp:DropDownList ID="ddlCondition" runat="server" CssClass="ddlist"></asp:DropDownList>
  
            </div>
    <br class="clear"/>

    <div class="size-4 column">
        Condition Failed Action:
    </div>
    <div class="size-5 column">
       
        <asp:DropDownList ID="ddlConditionFailedAction" runat="server" CssClass="ddlist"></asp:DropDownList>
  
            </div>
    <br class="clear"/>
   
<div class="size-4 column">
    Com Server Condition:
</div>

<div class="size-5 column">
    <asp:DropDownList ID="ddlComCondition" runat="server" CssClass="ddlist" AutoPostBack="True" OnSelectedIndexChanged="ddlComCondition_OnSelectedIndexChanged"></asp:DropDownList>
</div>
<br class="clear" />
<div id="divComServers" runat="server">
<asp:GridView ID="gvServers" runat="server"  DataKeyNames="Id"  AutoGenerateColumns="False" CssClass="Gridview" AlternatingRowStyle-CssClass="alt">
    <Columns>
        <asp:TemplateField>
            <HeaderStyle CssClass="chkboxwidth"></HeaderStyle>
            <ItemStyle CssClass="chkboxwidth"></ItemStyle>
            <HeaderTemplate>
                <asp:CheckBox ID="chkSelectAll" runat="server" AutoPostBack="True" OnCheckedChanged="chkSelectAll_CheckedChanged"/>
            </HeaderTemplate>
            <ItemTemplate>
                <asp:CheckBox ID="chkSelector" runat="server"/>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:HyperLinkField DataNavigateUrlFields="Id" DataNavigateUrlFormatString="~/views/admin/comservers/editcomserver.aspx?level=2&serverId={0}" Text="View" ItemStyle-CssClass="chkboxwidth"/>
        <asp:BoundField DataField="Id" HeaderText="Id" SortExpression="Id" Visible="False"/>
        <asp:BoundField DataField="DisplayName" HeaderText="Display Name" ItemStyle-CssClass="width_200"></asp:BoundField>
        <asp:BoundField DataField="Url" HeaderText="URL"></asp:BoundField>
         

    </Columns>
    <EmptyDataTemplate>
        No Client Communication Servers Found
    </EmptyDataTemplate>
</asp:GridView>
</div>
</asp:Content>