<%@ Page Title="" Language="C#" MasterPageFile="~/views/assets/customassets/customassets.master" AutoEventWireup="true" CodeBehind="archived.aspx.cs" Inherits="Toems_FrontEnd.views.assets.customassets.archived" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub2">
    <li>Archived</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub2">
   Custom Assets
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub2" Runat="Server">
    <li><asp:LinkButton runat="server" ID="btnDelete" Text="Delete Selected" OnClick="btnDelete_OnClick"></asp:LinkButton></li>
</asp:Content>




<asp:Content ID="Content2" ContentPlaceHolderID="SubContent2" Runat="Server">
    <script type="text/javascript">
        $(document).ready(function () {

            $('#archived').addClass("nav-current");
            $("[id*=gvAssets] td").hover(function() {
                $("td", $(this).closest("tr")).addClass("hover_row");
            }, function() {
                $("td", $(this).closest("tr")).removeClass("hover_row");
            });
        });
    </script>
  
    <div class="size-7 column">
        <asp:TextBox ID="txtSearch" runat="server" CssClass="rounded-search" OnTextChanged="search_Changed"></asp:TextBox>
    </div>
    <div class="size-11 column">

        <div class="custom-select">
            <asp:DropDownList runat="server" ID="ddlLimit" AutoPostBack="True" OnSelectedIndexChanged="ddl_OnSelectedIndexChanged" >
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
    
    <div class="size-13 column">
        <div class="custom-select">
            <asp:DropDownList ID="ddlAssetType" runat="server" CssClass="ddlist" AutoPostBack="True" OnSelectedIndexChanged="ddlAssetType_OnSelectedIndexChanged">
            </asp:DropDownList>
                
        </div>
    </div>
    <br class="clear"/>
  
    <br />
      <p class="total">
        <asp:Label ID="lblTotal" runat="server"></asp:Label>
    </p>

    <asp:GridView ID="gvAssets" runat="server"  DataKeyNames="AssetId"  AutoGenerateColumns="False" CssClass="Gridview" AlternatingRowStyle-CssClass="alt">
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
            <asp:HyperLinkField DataNavigateUrlFields="AssetId" DataNavigateUrlFormatString="~/views/assets/customassets/edit.aspx?level=2&assetId={0}" Text="View" ItemStyle-CssClass="chkboxwidth"/>
            <asp:BoundField DataField="AssetId" HeaderText="Id" SortExpression="Id" Visible="False"/>
            <asp:BoundField DataField="Name" HeaderText="Display Name" ItemStyle-CssClass="width_200"></asp:BoundField>
            <asp:BoundField DataField="AssetType" HeaderText="Asset Type" ItemStyle-CssClass="width_200" ></asp:BoundField>
            <asp:TemplateField>                 
                <ItemTemplate>
                    <asp:LinkButton runat="server" ID="btnRestore" Text="Restore" OnClick="btnRestore_OnClick"></asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>

        </Columns>
        <EmptyDataTemplate>
            No Assets Found
        </EmptyDataTemplate>
    </asp:GridView>

    <div id="confirmbox" class="confirm-box-outer">
        <div class="confirm-box-inner">
            <h4>
                <asp:Label ID="lblTitle" runat="server"></asp:Label>
            </h4>
            <div class="confirm-box-btns">
                <asp:LinkButton ID="ConfirmButton" OnClick="ButtonConfirmDelete_Click" runat="server" Text="Yes" CssClass="confirm_yes"/>
                <asp:LinkButton ID="CancelButton" runat="server" Text="No" CssClass="confirm_no"/>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subsubHelp">
    <p>Displays all archived Custom Assets.  Custom Assets can be permanently deleted from this page or restored back to an active status. Filtering options include:</p>
<ul>
	<li>By Asset Type</li>
	<li>By Limit</li>
	<li>By Asset Display Name</li>
</ul>
</asp:Content>
