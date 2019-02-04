<%@ Page Title="" Language="C#" MasterPageFile="~/views/policies/policies.master" AutoEventWireup="true" CodeBehind="assignedmodules.aspx.cs" Inherits="Toems_FrontEnd.views.policies.assignedmodules" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li><%= Policy.Name %></li>
    <li>Assigned Modules</li>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
        <%= Policy.Name %>
</asp:Content>

<asp:Content ID="Content" ContentPlaceHolderID="SubContent" runat="Server">
    <script type="text/javascript">
        $(document).ready(function() {
            $('#assignedmodules').addClass("nav-current");

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
    
  
    <br class="clear"/>
    <asp:GridView ID="gvModules" runat="server" AllowSorting="True" DataKeyNames="Id" OnSorting="gridView_Sorting" AutoGenerateColumns="False" CssClass="Gridview" AlternatingRowStyle-CssClass="alt">
        <Columns>
                       
              <asp:TemplateField ItemStyle-CssClass="width_100" HeaderText="Order" SortExpression="Order">
                <ItemTemplate>
                    <asp:TextBox ID="txtOrder" runat="server" CssClass="textbox height_18 order" Text='<%# Eval("Order") %>' />
                </ItemTemplate>
            </asp:TemplateField>
           
            <asp:BoundField DataField="Id" Visible="False"/>
           
            <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" ItemStyle-CssClass="width_200"></asp:BoundField>
            <asp:BoundField DataField="ModuleType" HeaderText="Type" SortExpression="ModuleType" ItemStyle-CssClass="width_200"/>
              <asp:BoundField DataField="Guid" HeaderText="Guid"/>
            <asp:BoundField DataField="ModuleId" Visible="False"/> 
            <asp:HyperLinkField DataNavigateUrlFields="ModuleType,ModuleId" DataNavigateUrlFormatString="~/views/policies/moduleredirect.aspx?type={0}&id={1}" Text="View" ItemStyle-CssClass="chkboxwidth" Target="_blank"/>
             <asp:TemplateField ItemStyle-CssClass="chkboxwidth">
                <ItemTemplate>
                    <asp:LinkButton ID="btnUpdate" runat="server" OnClick="btnUpdate_OnClick" Text="Update Order"/>
                </ItemTemplate>
            </asp:TemplateField>
                        <asp:TemplateField ItemStyle-CssClass="chkboxwidth">
                <ItemTemplate>
                    <asp:LinkButton ID="btnRemove" runat="server" OnClick="btnRemove_OnClick" Text="Remove"/>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <EmptyDataTemplate>
            No Modules Found
        </EmptyDataTemplate>
    </asp:GridView>

 
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">
    <p>This page displays all modules that are currently assigned to the policy.  The results can be filtered by module name, and module type.  This page also allows you to set the order in which the modules run.  Both negative and positive values can be used.  Modules can also be removed from the policy from this page.</p>
</asp:Content>
