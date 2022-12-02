<%@ Page Title="" Language="C#" MasterPageFile="~/views/modules/winpemodules/winpemodule.master" AutoEventWireup="true" CodeBehind="archived.aspx.cs" Inherits="Toems_FrontEnd.views.modules.winpemodules.archived" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub2">
    <li>Archived</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub2">
WinPE Modules
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub2" Runat="Server">
    <li><asp:LinkButton ID="btnDelete" runat="server" OnClick="btnDelete_OnClick" Text="Delete Selected" CssClass="main-action"/></li>
</asp:Content>



<asp:Content ID="Content2" ContentPlaceHolderID="SubContent2" Runat="Server">
    <script type="text/javascript">
        $(document).ready(function () {
            $('#archived').addClass("nav-current");
            $("[id*=gvModules] td").hover(function () {
                $("td", $(this).closest("tr")).addClass("hover_row");
            }, function () {
                $("td", $(this).closest("tr")).removeClass("hover_row");
            });

        });
    </script>
    
  
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
    <br/>
      <p class="total">
        <asp:Label ID="lblTotal" runat="server"></asp:Label>
    </p>
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
            <asp:HyperLinkField DataNavigateUrlFields="Id" DataNavigateUrlFormatString="~/views/modules/winpemodules/general.aspx?winPeModuleId={0}" Text="View" ItemStyle-CssClass="chkboxwidth"/>
            <asp:BoundField DataField="Id" HeaderText="Id" SortExpression="Id" Visible="False"/>
            <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" ItemStyle-CssClass="width_200"></asp:BoundField>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:LinkButton runat="server" ID="btnRestore" Text="Restore" OnClick="btnRestore_OnClick"></asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>


           


        </Columns>
        <EmptyDataTemplate>
            No Archived Modules Found
        </EmptyDataTemplate>
    </asp:GridView>

    <div id="confirmbox" class="confirm-box-outer">
        <div class="confirm-box-inner">
            <h4>
                <asp:Label ID="lblTitle" runat="server" Text="Delete The Selected Modules?"></asp:Label>
            </h4>
            <div class="confirm-box-btns">
                <asp:LinkButton ID="ConfirmButton" OnClick="ButtonConfirmDelete_Click" runat="server" Text="Yes" CssClass="confirm_yes"/>
                <asp:LinkButton ID="CancelButton" runat="server" Text="No" CssClass="confirm_no"/>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subsubHelp">
    <p>The archived page allows you to view and restore archived modules. Filtering options include:</p>
<ul>
	<li><strong>Search by module name</strong></li>
	<li><strong>By limit</strong></li>
</ul>
</asp:Content>