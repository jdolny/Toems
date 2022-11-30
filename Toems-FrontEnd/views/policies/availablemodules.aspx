<%@ Page Title="" Language="C#" MasterPageFile="~/views/policies/policies.master" AutoEventWireup="true" CodeBehind="availablemodules.aspx.cs" Inherits="Toems_FrontEnd.views.policies.availablemodules" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li><%= Policy.Name %></li>
    <li>Available Modules</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
        <%= Policy.Name %>
</asp:Content>
<asp:Content ID="Breadcrumb" ContentPlaceHolderID="DropDownActionsSub" Runat="Server">
    <li><asp:LinkButton ID="btnSubmit" runat="server" OnClick="btnSubmit_OnClick" Text="Add Selected To Policy" CssClass="main-action"/></li>
</asp:Content>






<asp:Content ID="Content" ContentPlaceHolderID="SubContent" runat="Server">
    <script type="text/javascript">
        $(document).ready(function() {
            $('#availablemodules').addClass("nav-current");

            $("[id*=gvModules] td").hover(function() {
                $("td", $(this).closest("tr")).addClass("hover_row");
            }, function() {
                $("td", $(this).closest("tr")).removeClass("hover_row");
            });
        });

    </script>

    <p class="total">
        <asp:Label ID="lblTotal" runat="server"></asp:Label>
    </p>
    <div class="size-7 column">

        <asp:TextBox ID="txtSearch" runat="server" CssClass="rounded-search" OnTextChanged="search_Changed"></asp:TextBox>
    </div>
    
    <div class="size-11 column">
        <div class="custom-select">
            <asp:DropDownList runat="server" ID="ddlLimit" AutoPostBack="True" OnSelectedIndexChanged="ddl_OnSelectedIndexChanged" CssClass="ddlist">
                <asp:ListItem></asp:ListItem>
                <asp:ListItem>25</asp:ListItem>
                <asp:ListItem>100</asp:ListItem>
                <asp:ListItem Selected="True">250</asp:ListItem>
                <asp:ListItem >500</asp:ListItem>
                <asp:ListItem>1000</asp:ListItem>
                <asp:ListItem>5000</asp:ListItem>
                <asp:ListItem>All</asp:ListItem>
            </asp:DropDownList>
        </div>
    </div>

    <br class="clear"/>
    
     <div class="size-10 column hidden-check">
         Printer
        <asp:CheckBox runat="server" ID="chkPrinter" Text="Printer" AutoPostBack="True" OnCheckedChanged="chkFilter_OnCheckedChanged"/>
    </div>
    <div class="size-10 column hidden-check">
        Software
        <asp:CheckBox runat="server" Id="chkSoftware" Text="Software" AutoPostBack="True" OnCheckedChanged="chkFilter_OnCheckedChanged"/>
    </div>
    <div class="size-10 column hidden-check">
        Script
        <asp:CheckBox runat="server" ID="chkScript" Text="Script" AutoPostBack="True" OnCheckedChanged="chkFilter_OnCheckedChanged"/>
    </div>
     <div class="size-10 column hidden-check">
         File Copy
        <asp:CheckBox runat="server" ID="chkFile" Text="File Copy" AutoPostBack="True" OnCheckedChanged="chkFilter_OnCheckedChanged"/>
    </div>
     <div class="size-10 column hidden-check">
         Command
        <asp:CheckBox runat="server" ID="chkCommand" Text="Command" AutoPostBack="True" OnCheckedChanged="chkFilter_OnCheckedChanged"/>
    </div>
    <div class="size-10 column hidden-check">
        Windows Update
        <asp:CheckBox runat="server" ID="chkWu" Text="Command" AutoPostBack="True" OnCheckedChanged="chkFilter_OnCheckedChanged"/>
    </div>
      <div class="size-10 column hidden-check">
        Message
        <asp:CheckBox runat="server" ID="chkMessage" Text="Command" AutoPostBack="True" OnCheckedChanged="chkFilter_OnCheckedChanged"/>
    </div>
    <div class="size-10 column hidden-check">
        WinPE
        <asp:CheckBox runat="server" ID="chkWinPe" Text="Command" AutoPostBack="True" OnCheckedChanged="chkFilter_OnCheckedChanged"/>
    </div>
      <div class="size-10 column hidden-check">
          Unassigned
        <asp:CheckBox runat="server" ID="chkUnassigned" Text="Unassigned" AutoPostBack="True" OnCheckedChanged="chkFilter_OnCheckedChanged"/>
    </div>

  
    <br class="clear"/>
    <asp:GridView ID="gvModules" runat="server" AllowSorting="True" DataKeyNames="Id" OnSorting="gridView_Sorting" AutoGenerateColumns="False" CssClass="Gridview" AlternatingRowStyle-CssClass="alt">
        <Columns>

            <asp:TemplateField>

                <ItemStyle CssClass="chkboxwidth"></ItemStyle>
                <HeaderTemplate>
                    <asp:CheckBox ID="chkSelectAll" runat="server" AutoPostBack="True" OnCheckedChanged="chkSelectAll_CheckedChanged"/>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:CheckBox ID="chkSelector" runat="server"/>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:HyperLinkField DataNavigateUrlFields="ModuleType,Id" DataNavigateUrlFormatString="~/views/policies/moduleredirect.aspx?type={0}&id={1}" Text="View" ItemStyle-CssClass="chkboxwidth" Target="_blank"/>
            <asp:BoundField DataField="Id" HeaderText="moduleID" SortExpression="moduleID" Visible="False"/>
            <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" ItemStyle-CssClass="width_200"></asp:BoundField>
            <asp:BoundField DataField="ModuleType" HeaderText="Type" SortExpression="ModuleType" ItemStyle-CssClass="width_200"/>
               <asp:BoundField DataField="Guid" HeaderText="Guid"/>
              <asp:BoundField DataField="Description" HeaderText="Description"/>
        </Columns>
        <EmptyDataTemplate>
            No Modules Found
        </EmptyDataTemplate>
    </asp:GridView>

 
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">
    <p>This page is used to add modules to the policy.  Modules can be filtered by module name and module type.  There is an additional filter to show modules that are not assigned to any policies.  Modules can be added to policies more than once.</p>
</asp:Content>